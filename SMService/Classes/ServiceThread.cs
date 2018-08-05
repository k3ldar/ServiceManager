/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
 *  Service Manager is distributed under the GNU General Public License version 3 and  
 *  is also available under alternative licenses negotiated directly with Simon Carter.  
 *  If you obtained Service Manager under the GPL, then the GPL applies to all loadable 
 *  Service Manager modules used on your system as well. The GPL (version 3) is 
 *  available at https://opensource.org/licenses/GPL-3.0
 *
 *  This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
 *  without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
 *  See the GNU General Public License for more details.
 *
 *  The Original Code was created by Simon Carter (s1cart3r@gmail.com)
 *
 *  Copyright (c) 2010 - 2018 Simon Carter.  All Rights Reserved.
 *
 *  Product:  Service Manager
 *  
 *  File: ServiceThread.cs
 *
 *  Purpose:  Base Service Thread
 *
 *  Date        Name                Reason
 *  23/04/2018  Simon Carter        Initially Created
 *
 * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */
using System;
using System.Collections.Generic;

using ServiceManager.Core;

using Shared.Classes;
using Shared.Communication;

#pragma warning disable IDE0044

namespace ServiceManager
{
    internal class ServiceThread : ThreadManager
    {
        #region Private Members

        private readonly IServerManagerService _serverManagerService;
        private readonly ILogging _logging;
        private readonly IClientMessage _sendClientMessage;
        private List<Message> _serviceMessages;

        private object _lockObject;

        #endregion Private Members

        #region Constructors

        internal ServiceThread(IServerManagerService serverManagerService, ILogging logging, 
            IClientMessage sendClientMessage, TimeSpan runInterval)
            : base (null, runInterval)
        {
            _serverManagerService = serverManagerService ?? throw new ArgumentNullException(nameof(serverManagerService));
            _logging = logging ?? throw new ArgumentNullException(nameof(logging));
            _sendClientMessage = sendClientMessage ?? throw new ArgumentNullException(nameof(sendClientMessage));

            _lockObject = new object();
        }

        #endregion Constructors

        #region Overridden Methods

        protected override bool Run(object parameters)
        {
            // if the thread has been cancelled then exit
            if (HasCancelled())
                return (false);

            List<Message> messages = new List<Message>();
            try
            {

                using (TimedLock.Lock(_lockObject))
                {
                    // copy all messages to the local list and clear it out,
                    // the service item can then process them as it see's fit
                    foreach (Message message in _serviceMessages)
                    {
                        messages.Add(message);
                    }

                    _serviceMessages.Clear();
                }

                if (!_serverManagerService.ServiceExecute(messages))
                    return (false);

                return (!HasCancelled());
            }
            catch(Exception error)
            {
                _logging.AddToLog(error, ToString());

                // add all message back to the list

                foreach (Message message in messages)
                    AddMessage(message);
            }

            return (!HasCancelled());
        }

        #endregion Overridden Methods

        #region Internal Methods

        internal string ServiceName()
        {
            return (_serverManagerService == null ? ToString() : _serverManagerService.ServiceName());
        }

        internal void ServiceStart()
        {
            _serviceMessages = new List<Message>();
            _serverManagerService.ServiceStart();
        }

        internal void ServiceStop()
        {
            _serviceMessages = null;
            _serverManagerService.ServiceStop();
        }

        internal void ServicePause()
        {
            _serverManagerService.ServicePause();
        }

        internal void AddMessage(Message message)
        {
            using (TimedLock.Lock(_lockObject))
            {
                _serviceMessages.Add(message);
            }
        }

        internal void LoadClientDetails()
        {
            _serverManagerService.LoadClientDetails();
        }

        #endregion Internal Methods
    }
}

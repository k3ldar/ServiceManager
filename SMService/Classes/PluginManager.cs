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
 *  File: PluginManager.cs
 *
 *  Purpose:  
 *
 *  Date        Name                Reason
 *  21/04/2018  Simon Carter        Initially Created
 *
 * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */
using System;
using System.Collections.Generic;
using System.Reflection;

using ServiceManager.Core;

using Shared.Classes;
using Shared.Communication;

using Newtonsoft.Json;

#pragma warning disable IDE0044

namespace ServiceManager
{
    internal class PluginManager
    {
        #region Private Members

        private object _lockObject = new object();

        private List<ServiceThread> _registeredServices;

        private readonly ISendReport _sendReport;

        private readonly ILogging _logging;

        private readonly IEventManager _eventManager;

        private readonly IClientMessage _sendClientMessage;

        private Dictionary<string, List<ServiceThread>> _eventList;

        #endregion Private Members

        #region Constructors

        private PluginManager()
        {
            _registeredServices = new List<ServiceThread>();
            _eventList = new Dictionary<string, List<ServiceThread>>();
        }

        internal PluginManager(IEventManager eventManager, ISendReport sendReport, ILogging logging, IClientMessage sendClientMessage)
            :this ()
        {
            _eventManager = eventManager ?? throw new ArgumentNullException(nameof(eventManager));
            _sendReport = sendReport ?? throw new ArgumentNullException(nameof(sendReport));
            _logging = logging ?? throw new ArgumentNullException(nameof(logging));
            _sendClientMessage = sendClientMessage ?? throw new ArgumentNullException(nameof(sendClientMessage));
        }

        #endregion Constructors

        #region Internal Methods

        internal void StartServices()
        {
            foreach (ServiceThread service in _registeredServices)
            {
                try
                {
                    service.ServiceStart();
                    ThreadManager.ThreadStart(service, service.ServiceName(), System.Threading.ThreadPriority.Lowest);
                }
                catch (Exception err)
                {
                    _logging.AddToLog(err, service.ToString());
                }
            }
        }

        internal void StopServices()
        {
            foreach (ServiceThread service in _registeredServices)
            {
                try
                {
                    service.ServiceStop();
                    ThreadManager.Cancel(service.ToString());
                }
                catch (Exception err)
                {
                    _logging.AddToLog(err, service.ToString());
                }
            }
        }

        internal void PauseServices()
        {
            foreach (ServiceThread service in _registeredServices)
            {
                try
                {
                    service.ServicePause();
                }
                catch (Exception err)
                {
                    _logging.AddToLog(err, service.ToString());
                }
            }
        }

        internal void ProcessMessage(Message message)
        {
            if (message == null)
                throw new ArgumentNullException(nameof(message));

            // service message
            switch (message.Title)
            {
                case StringConstants.SERVER_DATA_DYNAMIC:
                    message.Contents = GetDynamicServerData();
                    _sendClientMessage.MessageRespond(message);

                    break;

                case StringConstants.SERVER_DATA_STATIC:
                    message.Contents = GetStaticServerData();
                    _sendClientMessage.MessageRespond(message);

                    break;

                case StringConstants.LOAD_CLIENT_DETAILS:

                    foreach (ServiceThread service in _registeredServices)
                        service.LoadClientDetails();

                    message.Title = StringConstants.LOAD_CLIENT_DETAILS_END;
                    message.Contents = String.Empty;

                    _sendClientMessage.MessageRespond(message);

                    break;

                default:
                    foreach (ServiceThread service in _registeredServices)
                    {
                        try
                        {
                            using (TimedLock.Lock(_lockObject))
                            {
                                if (_eventList.ContainsKey(message.Title.ToUpper()))
                                {
                                    if (_eventList.TryGetValue(message.Title.ToUpper(), out List<ServiceThread> eventListners))
                                    {
                                        foreach (ServiceThread serviceThread in eventListners)
                                        {
                                            serviceThread.AddMessage(message);
                                        }
                                    }
                                }
                            }
                        }
                        catch (Exception err)
                        {
                            _logging.AddToLog(err, service.ToString());
                        }
                    }

                    break;
            }
        }

        internal void LoadServices()
        {
            string[] files = System.IO.Directory.GetFiles(Utils.GetPath(PathType.Plugins), "*.dll");

            foreach (string file in files)
            {
                LoadService(file);
            }
        }

        /// <summary>
        /// Loads and configures all registered services
        /// </summary>
        /// <param name="serviceName"></param>
        internal void LoadService(string serviceName)
        {
            try
            {
                Assembly pluginAssembly = LoadAssembly(serviceName);

                foreach (Type type in pluginAssembly.GetTypes())
                {
                    if (type.GetInterface("IServerManagerService") != null)
                    {
                        IServerManagerService pluginService = (IServerManagerService)Activator.CreateInstance(type);
                        pluginService.ServiceInitialize(_eventManager, _logging, _sendReport, _sendClientMessage);

                        ServiceThread serviceThread = new ServiceThread(pluginService, _logging, _sendClientMessage, pluginService.ServiceRunInterval());
                        _registeredServices.Add(serviceThread);

                        IEventListner listner = pluginService.GetEventListner();

                        if (listner == null)
                            continue;

                        foreach (string eventName in listner.GetEvents())
                        {
                            using (TimedLock.Lock(_lockObject))
                            {
                                if (!_eventList.ContainsKey(eventName.ToUpper()))
                                {
                                    _eventList.Add(eventName.ToUpper(), new List<ServiceThread>());
                                }

                                List<ServiceThread> keyValue = _eventList[eventName.ToUpper()];
                                keyValue.Add(serviceThread);
                            }
                        }
                    }
                }
            }
            catch (Exception err)
            {
                _logging.AddToLog(err, serviceName);
            }
        }

        #endregion Internal Methods

        #region Private Methods

        /// <summary>
        /// Returns dynamic server data
        /// </summary>
        /// <returns></returns>
        private string GetDynamicServerData()
        {
            return (JsonConvert.SerializeObject(LocalServiceDataThread.GetAllItems()));
        }

        /// <summary>
        /// Returns static server data
        /// </summary>
        /// <returns></returns>
        private string GetStaticServerData()
        {
            return (JsonConvert.SerializeObject(LocalServiceDataThread.StaticServerDetails));
        }

        /// <summary>
        /// Dynamically loads an assembly
        /// </summary>
        /// <param name="assemblyName">name of assembly</param>
        /// <returns>Assembly instance</returns>
        private Assembly LoadAssembly(string assemblyName)
        {
            return (Assembly.Load(System.IO.File.ReadAllBytes(assemblyName)));
        }

        #endregion Private Methods
    }
}

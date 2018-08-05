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
 *  File: IServerManagerService.cs
 *
 *  Purpose:  Plugin service interface
 *
 *  Date        Name                Reason
 *  21/04/2018  Simon Carter        Initially Created
 *
 * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */
using System;
using System.Collections.Generic;

using Shared.Communication;

namespace ServiceManager.Core
{
    public interface IServerManagerService
    {
        #region Plugin Methods

        void ServiceInitialize(IEventManager eventManager, ILogging logging, ISendReport sendReport, IClientMessage clientMessage);

        void ServiceFinalize();

        void ServiceStart();

        void ServiceStop();

        void ServicePause();

        bool ServiceExecute(List<Message> messages);

        TimeSpan ServiceRunInterval();

        IEventListner GetEventListner();

        string ServiceName();

        #endregion Plugin Methods

        void LoadClientDetails();

        #region Plugin Properties



        #endregion Plugin Properties
    }
}

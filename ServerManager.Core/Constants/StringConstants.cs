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
 *  File: StringConstants.cs
 *
 *  Purpose:  
 *
 *  Date        Name                Reason
 *  08/05/2018  Simon Carter        Initially Created
 *
 * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */
using System;


namespace ServiceManager.Core
{
    public static class StringConstants
    {
        public const string Settings = "Settings";

        public const string SERVER_DATA_DYNAMIC = "SERVER_DATA_DYNAMIC";
        public const string SERVER_DATA_DYNAMIC_NEW = "SERVER_DATA_DYNAMIC_NEW";
        public const string SERVER_DATA_STATIC = "SERVER_DATA_STATIC";

        public const string THREAD_USAGE = "THREAD_USAGE";
        public const string THREAD_CPU_CHANGED = "THREAD_CPU_CHANGED";

        public const string LOAD_CLIENT_DETAILS = "LOAD_CLIENT_DETAILS";
        public const string LOAD_CLIENT_DETAILS_END = "LOAD_CLIENT_DETAILS_END";

        public const string CREATE_SETTINGS = "CREATE_SETTINGS";
        public const string CREATE_BUTTON = "CREATE_BUTTON";
        public const string CREATE_CHECKBOX = "CREATE_CHECKBOX";
        public const string CREATE_COMBOBOX = "CREATE_COMBOBOX";
        public const string CREATE_LISTVIEW = "CREATE_LISTVIEW";
        public const string CREATE_TEXTBOX = "CREATE_TEXTBOX";

        public const string MESSAGE_INFORMATION = "MESSAGE_SHOW";
        public const string MESSAGE_QUESTION = "MESSAGE_QUESTION";
        public const string MESSAGE_ERROR = "MESSAGE_ERROR";

        public static class Service
        {
            public const string SERVICE_SETTINGS = "SERVICE_SETTINGS";
            public const string SERVICE_SETTINGS_SAVE = "SERVICE_SETTINGS_SAVE";
            public const string SERVICE_EMAIL_SETTINGS_SAVE = "SERVICE_EMAIL_SETTINGS_SAVE";

            public const string DefaultUsername = "SysAdmin";
            public const string DefaultPassword = "masterkey";

            public const string PLUGIN_SERVICE_STOPPED = "{0} Service Stopped";
            public const string PLUGIN_SERVICE_PAUSED = "{0} Service Paused";
            public const string PLUGIN_SERVICE_STARTED = "{0} Service Started";
            public const string PLUGIN_SERVICE_INITIALISED = "{0} Initialised";
            public const string PLUGIN_SERVICE_FINALISED = "{0} Initialised";
        }

    }
}

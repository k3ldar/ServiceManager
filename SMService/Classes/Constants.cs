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
 *  File: Constants.cs
 *
 *  Purpose:  
 *
 *  Date        Name                Reason
 *  21/04/2018  Simon Carter        Initially Created
 *
 * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */
using System;

namespace ServiceManager
{
    internal static class Constants
    {
        internal static class Service
        { 
            internal const string ServiceName = "Server Administration";
            internal const string ServiceDescription = "Server administration plugin.";

            internal const string ThreadLocalDataMonitoring = "Local Service Data Monitorng";
        }

        internal static class Settings
        {
            internal const string ServiceSettingsFileName = "ServiceSettings.json";
            internal const string EmailSettingsFileName = "EmailSettings.json";
            internal const string Service = "Service";
            internal const string UserName = "Username";
            internal const string Password = "Password";
            internal const string MonitorNetworks = "Monitor Network";
            internal const string MonitorDisks = "Monitor Disks";
            internal const string ConnectionIP = "Connection IP";
            internal const string SmtpSettings = "SMTP Settings";
            internal const string SmtpHost = "Host";
            internal const string SmtpUsername = "Username";
            internal const string SmtpPassword = "Password";
            internal const string SmtpPort = "Port";
            internal const string SmtpSenderEmail = "SenderEmail";
            internal const string SmtpSSL = "SSL";
       }

        internal static class Other
        {
            internal const char Asterix = '*';
            internal const char SemiColon = ';';
        }

        internal static class Parameters
        {
            internal const string DelayStart = "ds";
            internal const uint DelayStartDefault = 30000;
            internal const string PluginServiceFile = "plugin";
            internal const string Path = "path";
        }
    }
}

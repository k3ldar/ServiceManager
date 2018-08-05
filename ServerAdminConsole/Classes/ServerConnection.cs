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
 *  File: ServerConnection.cs
 *
 *  Purpose:  
 *
 *  Date        Name                Reason
 *  20/05/2018  Simon Carter        Initially Created
 *
 * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */
using System;

using Shared;

namespace ServiceAdminConsole
{
    public class ServerConnection
    {
        #region Private Consts

        private const string ENCRYPTION_KEY = "ServiceAdminConsole";

        #endregion Private Consts

        #region Constructors

        public ServerConnection()
        {
            ServerPort = 4567;
        }

        public ServerConnection(string serverName, uint serverPort, string userName, string password)
        {
            if (String.IsNullOrEmpty(serverName))
                throw new ArgumentNullException(nameof(serverName));

            if (serverPort < 0 || serverPort > 65535)
                throw new ArgumentOutOfRangeException(nameof(serverPort));

            ServerName = serverName;
            ServerPort = serverPort;
            ServerUsername = userName;
            ServerPassword = Utilities.Encrypt(password, ENCRYPTION_KEY);
            Encrypted = true;
        }

        #endregion Constructors

        #region Internal Methods

        internal string GetDecryptedPassword()
        {
            if (Encrypted)
                return (Utilities.Decrypt(ServerPassword, ENCRYPTION_KEY));
            else
                return (ServerPassword);
        }

        #endregion Internal Methods

        #region Properties

        public string ServerName { get; set; }

        public string ServerUsername { get; set; }

        public string ServerPassword { get; set; }

        public uint ServerPort { get; set; }

        public bool Encrypted { get; set; }

        #endregion Properties
    }
}

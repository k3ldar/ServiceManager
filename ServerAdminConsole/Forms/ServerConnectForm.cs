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
 *  File: ServerConnectForm.cs
 *
 *  Purpose:  
 *
 *  Date        Name                Reason
 *  20/05/2018  Simon Carter        Initially Created
 *
 * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using SharedControls.Forms;

namespace ServiceAdminConsole
{
    public partial class ServerConnectForm : BaseForm
    {
        #region Constructors

        public ServerConnectForm()
        {
            InitializeComponent();

            this.Icon = Icon.ExtractAssociatedIcon(
                System.Reflection.Assembly.GetExecutingAssembly().Location);
        }

        #endregion Constructors

        #region Static Methods

        public static bool GetServerConnection(ref ServerConnection serverConnection)
        {
            ServerConnectForm serverConnectDialog = new ServerConnectForm();
            try
            {
                serverConnectDialog.txtPassword.Text = serverConnection.GetDecryptedPassword();
                serverConnectDialog.txtServer.Text = serverConnection.ServerName;
                serverConnectDialog.txtUserName.Text = serverConnection.ServerUsername;
                serverConnectDialog.udPort.Value = serverConnection.ServerPort;

                bool Result = serverConnectDialog.ShowDialog() == DialogResult.OK;

                if (Result)
                {
                    serverConnection.ServerName = serverConnectDialog.txtServer.Text;
                    serverConnection.ServerPassword = serverConnectDialog.txtPassword.Text;
                    serverConnection.ServerPort = Convert.ToUInt32(serverConnectDialog.udPort.Value);
                    serverConnection.ServerUsername = serverConnectDialog.txtUserName.Text;
                }

                return (Result);
            }
            finally
            {
                serverConnectDialog.Dispose();
                serverConnectDialog = null;
            }
        }

        #endregion Static Methods

        #region Private Methods

        private void btnConnect_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(txtServer.Text))
            {
                ShowError("Server Connection", "Please specify the server name");
                return;
            }

            DialogResult = DialogResult.OK;

        }

        #endregion Private Methods
    }
}

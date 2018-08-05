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
 *  File: About.cs
 *
 *  Purpose:  About Box
 *
 *  Date        Name                Reason
 *  04/08/2018  Simon Carter        Initially Created
 *
 * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ServiceManager.Core.Forms
{
    public partial class AboutForm : Form
    {
        #region Constructors

        public AboutForm()
        {
            InitializeComponent();

            LoadAboutDetails();
        }

        #endregion Constructors

        #region Static Methods

        public static void ShowAboutWindow(Form parent)
        {
            AboutForm aboutForm = new AboutForm();
            try
            {
                aboutForm.ShowDialog(parent);
            }
            finally
            {
                aboutForm.Dispose();
                aboutForm = null;
            }

        }

        #endregion Static Methods

        #region Private Methods

        private void LoadAboutDetails()
        {
            pbIcon.Image = Icon.ExtractAssociatedIcon(
                System.Reflection.Assembly.GetExecutingAssembly().Location).ToBitmap();

            lblName.Text = Application.ProductName;
            lblVersion.Text = $"Version: {Application.ProductVersion}";
            FileVersionInfo versionInfo = FileVersionInfo.GetVersionInfo(Assembly.GetEntryAssembly().Location);
            lblCopyright.Text = versionInfo.LegalCopyright;
        }

        #endregion Private Methods
    }
}

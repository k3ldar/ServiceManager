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
 *  File: ServerListView.cs
 *
 *  Purpose:  
 *
 *  Date        Name                Reason
 *  02/05/2018  Simon Carter        Initially Created
 *
 * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace ServiceManager.Core
{
    public sealed class ServerListView : ServerBaseControl
    {
        #region Constructors

        public ServerListView ()
        {
            Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;
            ColumnNames = new List<string>();
            ItemSeperator = '#';
        }

        #endregion Constructors

        #region Properties

        public bool AutoSize { get; set; }

        public List<string> ColumnNames { get; set; }

        public char ItemSeperator { get; set; }

        #endregion Properties
    }
}

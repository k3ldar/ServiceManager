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
 *  File: ServerBaseControl.cs
 *
 *  Purpose:  
 *
 *  Date        Name                Reason
 *  02/05/2018  Simon Carter        Initially Created
 *
 * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */
using System;
using System.Windows.Forms;

using Newtonsoft.Json;

namespace ServiceManager.Core
{
    public abstract class ServerBaseControl
    {
        #region Constructors

        public ServerBaseControl()
        {
            Anchor = AnchorStyles.Top | AnchorStyles.Left;
        }

        #endregion Constructors

        #region Methods

        public static T ConvertFromJson<T>(string value)
        {
            if (String.IsNullOrEmpty(value))
                throw new ArgumentNullException(nameof(value));

            return (JsonConvert.DeserializeObject<T>(value));
        }

        public static string ConvertToJson<T>(T value)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            return (JsonConvert.SerializeObject(value));
        }

        #endregion Methods

        #region Properties

        public int Top { get; set; }

        public int Left { get; set; }

        public int Width { get; set; }

        public int Height { get; set; }

        public string MessageName { get; set; }

        public string MessageData { get; set; }

        public string Parent { get; set; }

        public AnchorStyles Anchor { get; set; }

        #endregion Properties
    }
}

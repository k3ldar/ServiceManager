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
 *  File: ServerSetting.cs
 *
 *  Purpose:  
 *
 *  Date        Name                Reason
 *  02/06/2018  Simon Carter        Initially Created
 *
 * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */
using System;

namespace ServiceManager.Core
{
    public sealed class ServiceSetting
    {
        #region Constructors

        public ServiceSetting()
        {
            ItemSeperator = '#';
            MinimumValue = 0;
            MaximumValue = 0;
            Items = String.Empty;
            DecimalPlaces = 0;
            MaximumLength = 0;
            AllowedCharacters = String.Empty;
        }

        public ServiceSetting(string name, SettingType settingType, string value)
            : this ()
        {
            if (String.IsNullOrWhiteSpace(name))
                throw new ArgumentNullException(nameof(name));

            Name = name;
            SettingType = settingType;
            Value = value;
        }

        #endregion Constructors

        #region Public Methods

        public T GetValue<T>()
        {
            return (T)Convert.ChangeType(Value, typeof(T));
        }

        #endregion Public Methods

        #region Properties

        public string Name { get; set; }

        public SettingType SettingType { get; set; }

        public string Value { get; set; }

        public decimal MinimumValue { get; set; }

        public decimal MaximumValue { get; set; }

        public string Items { get; set; }

        public char ItemSeperator { get; set; }

        public int DecimalPlaces { get; set; }

        public int MaximumLength { get; set; }

        public string AllowedCharacters { get; set; }
             
        #endregion Properties
    }
}

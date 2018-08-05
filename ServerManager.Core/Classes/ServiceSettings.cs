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
 *  File: ServerSettings.cs
 *
 *  Purpose:  
 *
 *  Date        Name                Reason
 *  02/06/2018  Simon Carter        Initially Created
 *
 * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */
using System;
using System.Collections.Generic;

namespace ServiceManager.Core
{
    public sealed class ServiceSettings : ServerBaseControl
    {
        #region Constructors

        public ServiceSettings()
        {
            Settings = new List<ServiceSetting>();
        }

        public ServiceSettings(string name)
            : this ()
        {
            if (String.IsNullOrWhiteSpace(name))
                throw new ArgumentNullException(nameof(name));

            Name = name;
        }

        #endregion Constructors

        #region Public Methods

        public void Add(ServiceSetting serverSetting)
        {
            if (serverSetting == null)
                throw new ArgumentNullException(nameof(serverSetting));

            foreach (ServiceSetting setting in Settings)
            {
                if (setting.Name.Equals(serverSetting.Name, StringComparison.CurrentCultureIgnoreCase))
                    throw new ArgumentException(nameof(serverSetting));
            }

            Settings.Add(serverSetting);
        }

        public ServiceSetting Find(string name)
        {
            if (String.IsNullOrEmpty(name))
                throw new ArgumentNullException(nameof(name));

            foreach (ServiceSetting setting in Settings)
            {
                if (setting.Name.Equals(name))
                    return (setting);
            }

            return (null);
        }

        public T GetValue<T>(string name, T defaultValue)
        {
            ServiceSetting setting = Find(name);

            if (setting == null)
                return (defaultValue);

            switch (setting.SettingType)
            {
                case SettingType.Password:
                case SettingType.String:
                    if (String.IsNullOrEmpty((string)setting.Value))
                        return (defaultValue);

                    break;
            }

            return (T)Convert.ChangeType(setting.Value, typeof(T));
        }

        #endregion Public Methods

        #region Properties

        public string Name { get; set; }

        public List<ServiceSetting> Settings { get; set; }

        #endregion Properties
    }
}

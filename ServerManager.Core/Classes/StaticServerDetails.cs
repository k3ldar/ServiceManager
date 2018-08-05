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
 *  File: StaticServerDetails.cs
 *
 *  Purpose:  
 *
 *  Date        Name                Reason
 *  28/06/2018  Simon Carter        Initially Created
 *
 * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Management;


namespace ServiceManager.Core.Classes
{
    public sealed class StaticServerDetails
    {
        #region Constructors

        public StaticServerDetails()
        {

        }

        public StaticServerDetails(bool loadData)
            : this()
        {
            Platform = Environment.OSVersion.Platform.ToString();
            Version = Environment.OSVersion.VersionString;
            ServicePack = Environment.OSVersion.ServicePack;
            WindowsFolder = Environment.GetFolderPath(Environment.SpecialFolder.Windows);
            Drives = Environment.GetLogicalDrives();
            Is64BitOperatingSystem = Environment.Is64BitOperatingSystem;
            Is64BitProcess = Environment.Is64BitProcess;
            MachineName = Environment.MachineName;
            ProcessorCount = Environment.ProcessorCount;
            SystemDirectory = Environment.SystemDirectory;
            SystemPageSize = Environment.SystemPageSize;
            UserDomainName = Environment.UserDomainName;
            UserInteractive = Environment.UserInteractive;
            UserName = Environment.UserName;
            WorkingSet = Environment.WorkingSet;

            GetProcessorData();
            ProcessorCoreCount = GetCoreCount();
        }

        #endregion Constructors

        #region Properties

        public string Platform { get; set; }

        public string Version { get; set; }

        public string ServicePack { get; set; }

        public string WindowsFolder { get; set; }

        public string[] Drives { get; set; }

        public bool Is64BitOperatingSystem { get; set; }

        public bool Is64BitProcess { get; set; }

        public string MachineName { get; set; }

        public string ProcessorName { get; set; }

        public int ProcessorCount { get; set; }

        public int ProcessorCoreCount { get; set;}

        public string ProcessorBaseSpeed { get; set; }

        public int ProcessorLogicalProcessors { get; set; }

        public bool ProcessVirtualization { get; set; }

        public int ProcessorL1CasheSize { get; set; }

        public int ProcessorL2CasheSize { get; set; }

        public int ProcessorL3CasheSize { get; set; }

        public string SystemDirectory { get; set; }

        public int SystemPageSize { get; set; }

        public string UserDomainName { get; set; }

        public bool UserInteractive { get; set; }

        public string UserName { get; set; }

        public long WorkingSet { get; set; }


        #endregion Properties

        #region Private Methods

        private int GetCoreCount()
        {
            int Result = 0;

            foreach (var item in new System.Management.ManagementObjectSearcher("Select * from Win32_Processor").Get())
            {
                Result += int.Parse(item["NumberOfCores"].ToString());
            }

            return (Result);
        }

        private void GetProcessorData()
        {
            ManagementScope ms = new ManagementScope("\\\\localhost\\root\\cimv2");
            ObjectQuery oq = new ObjectQuery("SELECT * FROM Win32_Processor");
            ManagementObjectSearcher query = new ManagementObjectSearcher(ms, oq);
            ManagementObjectCollection queryCollection = query.Get();

            foreach (ManagementObject mo in queryCollection)
            {
                if (3 == Convert.ToUInt16(mo["ProcessorType"], System.Globalization.CultureInfo.InvariantCulture))
                {
                    foreach (PropertyData prop in mo.Properties)
                    {
                        switch (prop.Name)
                        {
                            case "Name":
                                ProcessorName = Convert.ToString(prop.Value ?? "Unknown");
                                break;
                            case "MaxClockSpeed":
                                ProcessorBaseSpeed = Math.Round(Convert.ToDouble(prop.Value ?? 0) / 1000, 2).ToString("0.00");
                                break;
                            case "L1CacheSize":
                                ProcessorL1CasheSize = Convert.ToInt32(prop.Value ?? 0);
                                break;
                            case "L2CacheSize":
                                ProcessorL2CasheSize = Convert.ToInt32(prop.Value ?? 0);
                                break;
                            case "L3CacheSize":
                                ProcessorL3CasheSize = Convert.ToInt32(prop.Value ?? 0);
                                break;
                            case "NumberOfLogicalProcessors":
                                ProcessorLogicalProcessors = Convert.ToInt32(prop.Value ?? 0);
                                break;
                            case "VirtualizationFirmwareEnabled":
                                ProcessVirtualization = Convert.ToBoolean(prop.Value ?? false);
                                break;
                        }
                    }
                }
            }
        }

        #endregion Private Methods
    }
}

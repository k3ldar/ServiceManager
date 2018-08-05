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
 *  File: DiskStatistics.cs
 *
 *  Purpose:  Class for reporting on disk statistics
 *
 *  Date        Name                Reason
 *  02/07/2018  Simon Carter        Initially Created
 *
 * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */
using System;

namespace ServiceManager.Core.Classes
{
    public sealed class DiskStatistics
    {
        #region Properties

        public string DiskName { get; set; }

        public UInt64 DiskReadPerSecond { get; set; }

        public UInt64 DiskWritePerSecond { get; set; }

        public UInt64 DiskTransferPerSecond { get; set; }

        public uint DiskQueueLength { get; set; }

        public UInt64 AverageDiskQueueLength { get; set; }

        public UInt64 TotalDiskSize { get; set;}

        public UInt64 FreeDiskSpace { get; set; }

        public UInt64 PercentDiskTime { get; set; }

        public uint IdleTime { get; set; }

        #endregion Properties
    }
}

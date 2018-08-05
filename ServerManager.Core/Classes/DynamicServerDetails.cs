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
 *  File: DynamicServerDetails.cs
 *
 *  Purpose:  
 *
 *  Date        Name                Reason
 *  28/06/2018  Simon Carter        Initially Created
 *
 * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Globalization;

#pragma warning disable IDE0017

namespace ServiceManager.Core.Classes
{
    public sealed class DynamicServerDetails
    {
        #region Constructors

        public DynamicServerDetails()
        {

        }

        public DynamicServerDetails(in bool loadData, 
            in PerformanceCounter cpuPerformanceCounter,
            in PerformanceCounter memoryPerformanceCounter,
            in List<PerformanceCounter> diskPerformanceCounter,
            in List<PerformanceCounter> networkPerformanceCounter,
            in List<PerformanceCounter> networkAdapterCounter)
            : this()
        {
            if (loadData)
            {
                DiskDriveStats = DriveInfo.GetDrives();
                GetMemoryUsage();
                TimeStamp = DateTime.Now.ToUniversalTime();
                ServiceManagedMemory = GC.GetTotalMemory(false);
                ServiceManagedMemoryUsage = Shared.Utilities.FileSize(ServiceManagedMemory, 2);
                
                

                if (cpuPerformanceCounter != null)
                    CPUUsagePercentage = Convert.ToInt32(cpuPerformanceCounter.NextValue());

                if (memoryPerformanceCounter != null)
                    MemoryUsage = Convert.ToDecimal(memoryPerformanceCounter.NextValue());

                if (diskPerformanceCounter != null)
                {
                    DiskStats = new List<DiskStatistics>();
                    GetDiskStatistics(diskPerformanceCounter);
                }

                if (networkPerformanceCounter != null)
                {
                    NetworkStats = new List<NetworkStatistics>();
                    GetNetworkStatistics(networkPerformanceCounter);
                }

                if (networkAdapterCounter != null)
                {
                    //GetNetworkAdapterStatistics(networkAdapterCounter);
                }

                long ticks = Stopwatch.GetTimestamp();
                double uptime = ((double)ticks) / Stopwatch.Frequency;
                UpTime = TimeSpan.FromSeconds(uptime);
            }
        }

        #endregion Constructors

        #region Properties

        public DateTime TimeStamp { get; set; }

        public long ServiceManagedMemory { get; set; }

        public string ServiceManagedMemoryUsage { get; set; }

        public int CPUUsagePercentage { get; set; }

        public decimal CPUUsageFrequency { get; set; }

        public decimal MemoryUsage { get; set; }

        public List<DiskStatistics> DiskStats { get; set; }

        public DriveInfo[] DiskDriveStats { get; set; }

        public List<NetworkStatistics> NetworkStats { get; set; }

        public TimeSpan UpTime { get; set; }

        public uint MemoryLoad { get; set; }

        public string PhysicalMemoryTotal { get; set; }

        public string PhysicalMemoryAvail { get; set; }

        public string PhysicalMemoryInUse { get; set; }

        public string PageFileTotal { get; set; }

        public string PageFileAvail { get; set; }

        public int PageFileLoad { get; set; }

        public string VirtualMemoryTotal { get; set; }

        public string VirtualMemoryAvail { get; set; }

        public string VirtualMemoryAvailExtended { get; set; }

        #endregion Properties

        #region Private Members

        private void GetMemoryUsage()
        {
            MEMORYSTATUSEX statEX = new MEMORYSTATUSEX();
            if (!Windows.GlobalMemoryStatusEx(statEX))
                return;

            MemoryLoad = statEX.dwMemoryLoad;

            PhysicalMemoryTotal = ConvertToGB(statEX.ullTotalPhys);
            PhysicalMemoryAvail = ConvertToGB(statEX.ullAvailPhys);
            PhysicalMemoryInUse = ConvertToGB(statEX.ullTotalPhys - statEX.ullAvailPhys);
            PageFileTotal = ConvertToGB(statEX.ullTotalPageFile);
            PageFileAvail = ConvertToGB(statEX.ullAvailPageFile);
            PageFileLoad = Convert.ToInt32(Shared.Utilities.Percentage(statEX.ullTotalPageFile / 1024.0d, statEX.ullAvailPageFile / 1024.0d));
            VirtualMemoryTotal = ConvertToGB(statEX.ullTotalVirtual);
            VirtualMemoryAvail = ConvertToGB(statEX.ullAvailVirtual);
            VirtualMemoryAvailExtended = ConvertToGB(statEX.ullAvailExtendedVirtual);
        }

        private string ConvertToGB(ulong valueKB)
        {
            if (valueKB == 0)
                return ("0 b");

            string[] suf = { " B", " KB", " MB", " GB", " TB", " PB" };
            int place = Convert.ToInt32(Math.Floor(Math.Log(valueKB, 1024)));
            double Result = Math.Round(valueKB / Math.Pow(1024, place), 3);

            char decimalSeperator = CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator[0];
            string[] number = Result.ToString().Split(decimalSeperator);
            string format = "N1";
            int intValue = 0;

            if (number.Length == 1 || Int32.TryParse(number[1], out intValue))
            {
                if (intValue == 0)
                    format = "N0";
                else if (intValue > 99)
                    format = "N2";
            }

            return (Result.ToString(format) + suf[place]);
        }

        private void GetDiskStatistics(in List<PerformanceCounter> diskPerformanceCounter)
        {
            try
            {
                try
                {
                    foreach (PerformanceCounter counter in diskPerformanceCounter)
                    {
                        DiskStatistics stats = GetDiskStatistics(counter.InstanceName);

                        if (stats == null)
                            return;

                        switch (counter.CounterName)
                        {
                            case "Free Megabytes":
                                stats.FreeDiskSpace = Convert.ToUInt64(counter.NextValue());
                                break;
                            case "Avg. Disk Queue Length":
                                stats.AverageDiskQueueLength = Convert.ToUInt64(counter.NextValue());
                                break;
                            case "Current Disk Queue Length":
                                stats.DiskQueueLength = Convert.ToUInt32(counter.NextValue());
                                break;
                            case "Disk Transfers/sec":
                                stats.DiskTransferPerSecond = Convert.ToUInt64(counter.NextValue());
                                break;
                            case "Disk Read Bytes/sec":
                                stats.DiskReadPerSecond = Convert.ToUInt64(counter.NextValue());
                                break;
                            case "Disk Write Bytes/sec":
                                stats.DiskWritePerSecond = Convert.ToUInt64(counter.NextValue());
                                break;
                            case "% Disk Time":
                                UInt64 value = Convert.ToUInt64(counter.NextValue());
                                stats.PercentDiskTime = value > 100 ? 100 : value;
                                break;
                            case "% Idle Time":
                                stats.IdleTime = Convert.ToUInt16(counter.NextValue());
                                break;
                        }

                        System.Threading.Thread.Sleep(0);
                    }
                }
                catch (Exception)
                {
                }
            }
            catch (Exception) { }
        }

        private DiskStatistics GetDiskStatistics(in string name)
        {
            foreach (DiskStatistics statistics in DiskStats)
            {
                if (statistics.DiskName == name)
                    return (statistics);
            }

            DiskStatistics stats = new DiskStatistics();
            stats.DiskName = name;
            DiskStats.Add(stats);

            return (stats);
        }

        private void GetNetworkStatistics(List<PerformanceCounter> networkPerformanceCounter)
        {
            try
            {
                try
                {
                    foreach (PerformanceCounter counter in networkPerformanceCounter)
                    {
                        NetworkStatistics stats = GetNetworkStatistics(counter.InstanceName, NetworkType.Network);

                        if (stats == null)
                            return;

                        switch (counter.CounterName)
                        {
                            case "Bytes Sent/sec":
                                stats.BytesPerSecondSent = Convert.ToUInt64(counter.NextValue());
                                break;
                            case "Bytes Received/sec":
                                stats.BytesPerSecondReceived = Convert.ToUInt64(counter.NextValue());
                                break;
                            case "Packets Received/sec":
                                stats.PacketsPerSecondReceived = Convert.ToUInt64(counter.NextValue());
                                break;
                            case "Packets Sent/sec":
                                stats.PacketsPerSecondSent = Convert.ToUInt64(counter.NextValue());
                                break;
                            case "Current Bandwidth":
                                stats.CurrentBandwidth = Convert.ToUInt64(counter.NextValue());
                                break;
                            case "Packets Received Errors":
                                stats.PacketsReceivedErrors = Convert.ToUInt64(counter.NextValue());
                                break;
                            case "Packets Outbound Errors":
                                stats.PacketsOutboundErrors = Convert.ToUInt64(counter.NextValue());
                                break;
                            case "Output Queue Length":
                                stats.OutputQueueLength = Convert.ToUInt64(counter.NextValue());
                                break;
                            case "TCP Active RSC Connections":
                                stats.TcpRscConnectionsActive = Convert.ToUInt64(counter.NextValue());
                                break;
                            //case "Bytes Total/sec":
                            //    stats.BytesPerSecondTotal = Convert.ToUInt64(counter.NextValue());
                            //    break;
                            //case "Packets/sec":
                            //    stats.PacketsPerSecondTotal = Convert.ToUInt64(counter.NextValue());
                            //    break;
                            //case "Packets Received Unicast/sec":
                            //    stats.PacketsPerSecondUnicastReceived = Convert.ToUInt64(counter.NextValue());
                            //    break;
                            //case "Packets Received Non-Unicast/sec":
                            //    stats.PacketsPerSecondNonUnicastReceived = Convert.ToUInt64(counter.NextValue());
                            //    break;
                            //case "Packets Received Discarded":
                            //    stats.PacketsReceivedDiscarded = Convert.ToUInt64(counter.NextValue());
                            //    break;
                            //case "Packets Received Unknown":
                            //    stats.PacketsReceivedUnknown = Convert.ToUInt64(counter.NextValue());
                            //    break;
                            //case "Packets Sent Unicast/sec":
                            //    stats.PacketsPerSecondUnicastSent = Convert.ToUInt64(counter.NextValue());
                            //    break;
                            //case "Packets Sent Non-Unicast/sec":
                            //    stats.PacketsPerSecondNonUnicastSent = Convert.ToUInt64(counter.NextValue());
                            //    break;
                            //case "Packets Outbound Discarded":
                            //    stats.PacketsOutboundDiscarded = Convert.ToUInt64(counter.NextValue());
                            //    break;
                            //case "Offloaded Connections":
                            //    stats.OffloadedConnections = Convert.ToUInt64(counter.NextValue());
                            //    break;
                            //case "TCP RSC Coalesced Packets/sec":
                            //    stats.TcpRscPacketsPerSecondCoalesced = Convert.ToUInt64(counter.NextValue());
                            //    break;
                            //case "TCP RSC Exceptions/sec":
                            //    stats.TcpRscExceptionsPerSecond = Convert.ToUInt64(counter.NextValue());
                            //    break;
                            //case "TCP RSC Average Packet Size":
                            //    stats.TcpRscAveragePacketSize = Convert.ToUInt64(counter.NextValue());
                            //    break;
                        }

                        System.Threading.Thread.Sleep(0);
                    }
                }
                catch (Exception)
                {
                }
            }
            catch (Exception)
            {
            }
        }

        private void GetNetworkAdapterStatistics(List<PerformanceCounter> networkPerformanceCounter)
        {
            try
            {
                try
                {
                    foreach (PerformanceCounter counter in networkPerformanceCounter)
                    {
                        NetworkStatistics stats = GetNetworkStatistics(counter.InstanceName, NetworkType.Adapter);

                        if (stats == null)
                            return;

                        switch (counter.CounterName)
                        {
                            case "Bytes Sent/sec":
                                stats.BytesPerSecondSent = Convert.ToUInt64(counter.NextValue());
                                break;
                            case "Bytes Received/sec":
                                stats.BytesPerSecondReceived = Convert.ToUInt64(counter.NextValue());
                                break;
                            case "Packets Received/sec":
                                stats.PacketsPerSecondReceived = Convert.ToUInt64(counter.NextValue());
                                break;
                            case "Packets Sent/sec":
                                stats.PacketsPerSecondSent = Convert.ToUInt64(counter.NextValue());
                                break;
                            case "Current Bandwidth":
                                stats.CurrentBandwidth = Convert.ToUInt64(counter.NextValue());
                                break;
                            case "Packets Outbound Errors":
                                stats.PacketsOutboundErrors = Convert.ToUInt64(counter.NextValue());
                                break;
                            case "Output Queue Length":
                                stats.OutputQueueLength = Convert.ToUInt64(counter.NextValue());
                                break;
                            case "TCP Active RSC Connections":
                                stats.TcpRscConnectionsActive = Convert.ToUInt64(counter.NextValue());
                                break;
                            case "Packets Received Errors":
                                stats.PacketsReceivedErrors = Convert.ToUInt64(counter.NextValue());
                                break;
                            //case "Packets Received Unicast/sec":
                            //    stats.PacketsPerSecondUnicastReceived = Convert.ToUInt64(counter.NextValue());
                            //    break;
                            //case "Packets Received Non-Unicast/sec":
                            //    stats.PacketsPerSecondNonUnicastReceived = Convert.ToUInt64(counter.NextValue());
                            //    break;
                            //case "Packets Received Discarded":
                            //    stats.PacketsReceivedDiscarded = Convert.ToUInt64(counter.NextValue());
                            //    break;
                            //case "Packets Received Unknown":
                            //    stats.PacketsReceivedUnknown = Convert.ToUInt64(counter.NextValue());
                            //    break;
                            //case "Packets Sent Unicast/sec":
                            //    stats.PacketsPerSecondUnicastSent = Convert.ToUInt64(counter.NextValue());
                            //    break;
                            //case "Packets Sent Non-Unicast/sec":
                            //    stats.PacketsPerSecondNonUnicastSent = Convert.ToUInt64(counter.NextValue());
                            //    break;
                            //case "Packets Outbound Discarded":
                            //    stats.PacketsOutboundDiscarded = Convert.ToUInt64(counter.NextValue());
                            //    break;
                            //case "Bytes Total/sec":
                            //    stats.BytesPerSecondTotal = Convert.ToUInt64(counter.NextValue());
                            //    break;
                            //case "Packets/sec":
                            //    stats.PacketsPerSecondTotal = Convert.ToUInt64(counter.NextValue());
                            //    break;
                            //case "Offloaded Connections":
                            //    stats.OffloadedConnections = Convert.ToUInt64(counter.NextValue());
                            //    break;
                            //case "TCP RSC Coalesced Packets/sec":
                            //    stats.TcpRscPacketsPerSecondCoalesced = Convert.ToUInt64(counter.NextValue());
                            //    break;
                            //case "TCP RSC Exceptions/sec":
                            //    stats.TcpRscExceptionsPerSecond = Convert.ToUInt64(counter.NextValue());
                            //    break;
                            //case "TCP RSC Average Packet Size":
                            //    stats.TcpRscAveragePacketSize = Convert.ToUInt64(counter.NextValue());
                            //    break;
                        }

                        System.Threading.Thread.Sleep(0);
                    }
                }
                catch (Exception)
                {
                }
            }
            catch (Exception) { }
        }

        private NetworkStatistics GetNetworkStatistics(in string name, in NetworkType networkType)
        {
            foreach (NetworkStatistics statistics in NetworkStats)
            {
                if (statistics.Name == name)
                    return (statistics);
            }

            NetworkStatistics stats = new NetworkStatistics();
            stats.Name = name;
            stats.NetworkType = networkType;
            NetworkStats.Add(stats);

            return (stats);
        }

        #endregion Private Members
    }
}

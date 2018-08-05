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
 *  File: LocalServerDataThread.cs
 *
 *  Purpose:  
 *
 *  Date        Name                Reason
 *  29/06/2018  Simon Carter        Initially Created
 *
 * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

using ServiceManager.Core.Classes;

#pragma warning disable IDE0044, IDE1005

namespace ServiceManager
{
    internal sealed class LocalServiceDataThread : Shared.Classes.ThreadManager
    {
        #region Private Members

        private static object _lockObject = new object();

        private bool _initialised = false;

        private readonly PerformanceCounter _cpuPerformanceCounter;
        private readonly PerformanceCounter _memoryPerformanceCounter;
        private readonly List<PerformanceCounter> _diskPerformanceCounter;
        private readonly List<PerformanceCounter> _netorkPerformanceCounter;
        private readonly List<PerformanceCounter> _networkAdapterCounter;

        #endregion Private Members

        #region Constructors

        public LocalServiceDataThread()
            : base (null, new TimeSpan(0, 0, 1))
        {
            ContinueIfGlobalException = true;

            DynamicServerDetails = new Queue<DynamicServerDetails>(100);
            _cpuPerformanceCounter = new PerformanceCounter();
            _memoryPerformanceCounter = new PerformanceCounter();
            _diskPerformanceCounter = new List<PerformanceCounter>();
            _netorkPerformanceCounter = new List<PerformanceCounter>();
            _networkAdapterCounter = new List<PerformanceCounter>();
        }

        #endregion Constructors

        #region Overridden Methods

        protected override bool Run(object parameters)
        {

            if (StaticServerDetails == null)
                StaticServerDetails = new StaticServerDetails(true);

            try
            {
                if (!_initialised)
                {
                    InitialiseAllCounters();
                    _initialised = true;
                }

                DynamicServerDetails dynamicServerDetails = new DynamicServerDetails(true, 
                    _cpuPerformanceCounter, 
                    _memoryPerformanceCounter,
                    MonitorDisks ? _diskPerformanceCounter : null,
                    MonitorNetwork ? _netorkPerformanceCounter : null,
                    MonitorNetworkAdapters ? _networkAdapterCounter : null);

                using (Shared.Classes.TimedLock.Lock(_lockObject))
                {
                    if (DynamicServerDetails.Count == 100)
                        DynamicServerDetails.Dequeue();

                    DynamicServerDetails.Enqueue(dynamicServerDetails);
                }

                if (OnNewServerDetails != null)
                    OnNewServerDetails(this, new DynamicServerDetailsEventArgs(dynamicServerDetails));

                System.Threading.Thread.Sleep(0);
            }
            catch (Exception error)
            {
                Shared.EventLog.Add(error);
            }

            return (!HasCancelled());
        }

        #endregion Overridden Methods

        #region Properties

        internal static StaticServerDetails StaticServerDetails { get; private set; }

        private static Queue<DynamicServerDetails> DynamicServerDetails { get; set; }

        #endregion Properties

        #region Internal Methods

        internal static List<DynamicServerDetails> GetAllItems()
        {
            using (Shared.Classes.TimedLock.Lock(_lockObject))
            {
                return (DynamicServerDetails.ToList<DynamicServerDetails>());
            }
        }

        internal bool MonitorNetwork { get; set; }

        internal bool MonitorDisks { get; set; }

        internal bool MonitorNetworkAdapters { get; set; }

        #endregion Internal Methods

        #region Private Methods

        private void InitialiseAllCounters()
        {
            PerformanceCounterCategory diskPerformanceCounter = new PerformanceCounterCategory("PhysicalDisk");

            foreach (string instance in diskPerformanceCounter.GetInstanceNames())
            {
                if (instance == "_Total")
                    continue;

                foreach (PerformanceCounter counter in diskPerformanceCounter.GetCounters(instance))
                    _diskPerformanceCounter.Add(counter);
            }

            PerformanceCounterCategory networkPerformanceCounter = new PerformanceCounterCategory("Network Interface");

            foreach (string instance in networkPerformanceCounter.GetInstanceNames())
            {
                if (instance == "_Total")
                    continue;

                foreach (PerformanceCounter counter in networkPerformanceCounter.GetCounters(instance))
                {

                    switch (counter.CounterName)
                    {
                        case "Bytes Sent/sec":
                        case "Bytes Received/sec":
                        case "Packets Received/sec":
                        case "Packets Sent/sec":
                        case "Current Bandwidth":
                        case "Packets Received Errors":
                        case "Packets Outbound Errors":
                        case "Output Queue Length":
                        case "TCP Active RSC Connections":
                            _netorkPerformanceCounter.Add(counter);
                            break;
                        //case "Bytes Total/sec":
                        //case "Packets/sec":
                        //case "Packets Received Unicast/sec":
                        //case "Packets Received Non-Unicast/sec":
                        //case "Packets Received Discarded":
                        //case "Packets Received Unknown":
                        //case "Packets Sent Unicast/sec":
                        //case "Packets Sent Non-Unicast/sec":
                        //case "Packets Outbound Discarded":
                        //case "Offloaded Connections":
                        //case "TCP RSC Coalesced Packets/sec":
                        //case "TCP RSC Exceptions/sec":
                        //case "TCP RSC Average Packet Size":
                    }
                }
            }

            PerformanceCounterCategory networkAdapterCounter = new PerformanceCounterCategory("Network Adapter");

            foreach (string instance in networkAdapterCounter.GetInstanceNames())
            {
                if (instance == "_Total")
                    continue;

                foreach (PerformanceCounter counter in networkAdapterCounter.GetCounters(instance))
                {
                    switch (counter.CounterName)
                    {
                        case "Bytes Sent/sec":
                        case "Bytes Received/sec":
                        case "Packets Received/sec":
                        case "Packets Sent/sec":
                        case "Current Bandwidth":
                        case "Packets Outbound Errors":
                        case "Output Queue Length":
                        case "TCP Active RSC Connections":
                        case "Packets Received Errors":
                            _networkAdapterCounter.Add(counter);
                            break;
                        //case "Packets Received Unicast/sec":
                        //case "Packets Received Non-Unicast/sec":
                        //case "Packets Received Discarded":
                        //case "Packets Received Unknown":
                        //case "Packets Sent Unicast/sec":
                        //case "Packets Sent Non-Unicast/sec":
                        //case "Packets Outbound Discarded":
                        //case "Bytes Total/sec":
                        //case "Packets/sec":
                        //case "Offloaded Connections":
                        //case "TCP RSC Coalesced Packets/sec":
                        //case "TCP RSC Exceptions/sec":
                        //case "TCP RSC Average Packet Size":
                    }

                }
            }

            InitPerformanceCounters();

        }

        private void InitPerformanceCounters()
        {
            _cpuPerformanceCounter.CategoryName = "Processor";
            _cpuPerformanceCounter.CounterName = "% Processor Time";
            _cpuPerformanceCounter.InstanceName = "_Total";

            _memoryPerformanceCounter.CategoryName = "Memory";
            _memoryPerformanceCounter.CounterName = "Available MBytes";
        }

        #endregion Private Methods

        #region Public Events

        public event DynamicServerDetailsEventHandler OnNewServerDetails;

        #endregion Public Events
    }

    public class DynamicServerDetailsEventArgs
    {
        #region Constructors

        public DynamicServerDetailsEventArgs(DynamicServerDetails dynamicDetails)
        {
            Details = dynamicDetails;
        }

        #endregion Constructors

        #region Properties

        public DynamicServerDetails Details { get; private set; }

        #endregion Properties
    }

    public delegate void DynamicServerDetailsEventHandler(object sender, DynamicServerDetailsEventArgs e);
}

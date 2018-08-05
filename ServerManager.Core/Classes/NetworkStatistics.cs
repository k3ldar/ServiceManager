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
 *  File: NetworkStatistics.cs
 *
 *  Purpose:  
 *
 *  Date        Name                Reason
 *  14/07/2018  Simon Carter        Initially Created
 *
 * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceManager.Core.Classes
{
    public sealed class NetworkStatistics
    {
        #region Properties

        public NetworkType NetworkType { get; set; }

        public string Name { get; set; }

        //public UInt64 BytesPerSecondTotal { get; set; }

        public UInt64 BytesPerSecondSent { get; set; }

        public UInt64 BytesPerSecondReceived { get; set; }

        //public UInt64 PacketsPerSecondTotal { get; set; }

        public UInt64 PacketsPerSecondReceived { get; set; }

        public UInt64 PacketsPerSecondSent { get; set; }

        public UInt64 CurrentBandwidth { get; set; }

        //public UInt64 PacketsPerSecondUnicastReceived { get; set; }

        //public UInt64 PacketsPerSecondNonUnicastReceived { get; set; }

        //public UInt64 PacketsReceivedDiscarded { get; set; }

        public UInt64 PacketsReceivedErrors { get; set; }

        //public UInt64 PacketsReceivedUnknown { get; set; }

        //public UInt64 PacketsPerSecondUnicastSent { get; set; }

        //public UInt64 PacketsPerSecondNonUnicastSent { get; set; }

        //public UInt64 PacketsOutboundDiscarded { get; set; }

        public UInt64 PacketsOutboundErrors { get; set; }

        public UInt64 OutputQueueLength { get; set; }

        //public UInt64 OffloadedConnections { get; set; }

        public UInt64 TcpRscConnectionsActive { get; set; }

        //public UInt64 TcpRscPacketsPerSecondCoalesced { get; set; }

        //public UInt64 TcpRscExceptionsPerSecond { get; set; }

        //public UInt64 TcpRscAveragePacketSize { get; set; }

        #endregion Properties
    }
}

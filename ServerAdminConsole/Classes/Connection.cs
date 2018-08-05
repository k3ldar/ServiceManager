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
 *  File: Connection.cs
 *
 *  Purpose:  
 *
 *  Date        Name                Reason
 *  20/05/2018  Simon Carter        Initially Created
 *
 * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */
using System;
using System.Drawing;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Text;
using System.Threading.Tasks;

using Shared.Communication;

using ServiceManager.Core;
using ServiceManager.Core.Classes;
using ServiceManager.Core.Controls;

namespace ServiceAdminConsole
{
    internal class Connection
    {
        #region Constructors

        internal Connection()
        {
            ChildControls = new Dictionary<string, Control>();
            DiskStatsLoaded = false;
            DriveStatsLoaded = false;
            NetworkStatsLoaded = false;
        }

        internal Connection(IClientConnectionHelper client, ServerConnection serverConnection)
            : this ()
        {
            if (client == null)
                throw new ArgumentNullException(nameof(client));

            SafeServerName = serverConnection.ServerName.Replace(".", "_");
            ServerConnection = serverConnection ?? throw new ArgumentNullException(nameof(serverConnection));
            Client = new MessageClient(serverConnection.ServerName, (int)serverConnection.ServerPort);
            PrimaryTab = new TabPage(serverConnection.ServerName);

            TabControl ServerTab = new TabControl();
            ServerTab.Left = 3;
            ServerTab.Top = 3;
            ServerTab.Width = PrimaryTab.Width - 6;
            ServerTab.Height = PrimaryTab.Height - 6;
            ServerTab.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;

            PrimaryTab.Controls.Add(ServerTab);

            TabPage generalPage = new TabPage("General");
            ServerTab.TabPages.Add(generalPage);
            PrimaryTab.Tag = this;

            PrimaryTabLayout = new FlowLayoutPanel();
            PrimaryTabLayout.Parent = generalPage;
            PrimaryTabLayout.Left = 3;
            PrimaryTabLayout.Top = 3;
            PrimaryTabLayout.AutoScroll = true;
            PrimaryTabLayout.Width = generalPage.Width - 6;
            PrimaryTabLayout.Height = generalPage.Height - 6;
            PrimaryTabLayout.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;

            GroupBox groupBox = new GroupBox();
            groupBox.Width = 350;
            groupBox.Height = 160;
            groupBox.Parent = PrimaryTabLayout;
            groupBox.Text = "System";

            int newTop = 17;

            CreateLabel(groupBox, $"lblOSType{SafeServerName}", "u/k", 3, ref newTop, true);
            CreateLabel(groupBox, $"lblOSName{SafeServerName}", "u/k", 3, ref newTop, true);
            CreateLabel(groupBox, $"lbl64BitOS{SafeServerName}", "u/k", 3, ref newTop, true);
            CreateLabel(groupBox, $"lblWinFolder{SafeServerName}", "u/k", 3, ref newTop, true);
            CreateLabel(groupBox, $"lblSysFolder{SafeServerName}", "u/k", 3, ref newTop, true);
            CreateLabel(groupBox, $"lblUptime{SafeServerName}", "Uptime:", 3, ref newTop, false);
            CreateLabel(groupBox, $"lblUpTimeValue{SafeServerName}", "u/k", 55, ref newTop, true);


            groupBox = new GroupBox();
            groupBox.Width = 350;
            groupBox.Height = 160;
            groupBox.Parent = PrimaryTabLayout;
            groupBox.Text = "Processor";

            newTop = 17;

            // Static Server Details
            CreateLabel(groupBox, $"lblProcessorType{SafeServerName}", "u/k", 3, ref newTop, true);
            CreateLabel(groupBox, $"lblProcessorSpeed{SafeServerName}", "u/k", 3, ref newTop, true);
            CreateLabel(groupBox, $"lblProcessorCores{SafeServerName}", "u/k", 3, ref newTop, true);
            CreateLabel(groupBox, $"lblProcessorProcessors{SafeServerName}", "u/k", 3, ref newTop, true);
            CreateLabel(groupBox, $"lblProcessorVirtualization{SafeServerName}", "u/k", 3, ref newTop, true);
            CreateLabel(groupBox, $"lblProcessorUtilizationDesc{SafeServerName}", "Utilization:", 3, ref newTop, false);
            CreateLabel(groupBox, $"lblProcessorUtilizationValue{SafeServerName}", "u/k", 70, ref newTop, true);
            CreateHeartBeat(groupBox, $"pnlHeartbeatProcessor{SafeServerName}", 270, 15, 70, 40,
                Color.OrangeRed, Color.Orange, "Current processor utilization");


            // memory details
            groupBox = new GroupBox();
            groupBox.Width = 350;
            groupBox.Height = 160;
            groupBox.Parent = PrimaryTabLayout;
            groupBox.Text = "Memory";

            newTop = 17;

            CreateLabel(groupBox, $"lblMemoryPhysTot{SafeServerName}", "u/k", 3, 
                ref newTop, true, "Total physical memory");
            CreateLabel(groupBox, $"lblMemoryPhysAvail{SafeServerName}", "u/k", 3,
                ref newTop, true, "Total available physical memory");
            CreateLabel(groupBox, $"lblMemoryPageTot{SafeServerName}", "u/k", 3, 
                ref newTop, true, "Total page file size");
            CreateLabel(groupBox, $"lblMemoryPageAvail{SafeServerName}", "u/k", 3, 
                ref newTop, true, "Available page file size");
            CreateLabel(groupBox, $"lblMemoryVirtTot{SafeServerName}", "u/k", 3, 
                ref newTop, true, "Total virtual memory");
            CreateLabel(groupBox, $"lblMemoryVirtAvail{SafeServerName}", "u/k:", 3, 
                ref newTop, true, "Available virtual memory");
            CreateLabel(groupBox, $"lblMemoryVirtExtended{SafeServerName}", "u/k", 3, 
                ref newTop, true);
            CreateHeartBeat(groupBox, $"pnlHeartbeatMemory{SafeServerName}", 270, 15, 70, 40,
                Color.OrangeRed, Color.Orange, "Current memory usage");
            CreateHeartBeat(groupBox, $"pnlHeartbeatPage{SafeServerName}", 270, 65, 70, 40,
                Color.DeepSkyBlue, Color.LightBlue, "Current page file usage");


            // Dynamic Server Details
            newTop = groupBox.Top + groupBox.Height + 5;
            CreateLabel(generalPage, $"lblServiceMemory{SafeServerName}", "u/k", 3, 
                ref newTop, true, "Total managed memory");

            TabPage threadPage = new TabPage("Threads");
            ServerTab.TabPages.Add(threadPage);

            ServerListView serverListView = new ServerListView();
            serverListView.AutoSize = true;
            serverListView.Parent = "Threads";
            serverListView.MessageName = $"{serverConnection.ServerName} {StringConstants.THREAD_USAGE}";
            serverListView.ItemSeperator = ';';

            serverListView.ColumnNames.Add("Name");
            serverListView.ColumnNames.Add("Process CPU");
            serverListView.ColumnNames.Add("System CPU");
            serverListView.ColumnNames.Add("ID");
            serverListView.ColumnNames.Add("Cancelled");
            serverListView.ColumnNames.Add("Unresponsive");
            serverListView.ColumnNames.Add("Removing");
            client.AddListView($"{SafeServerName}_{StringConstants.THREAD_USAGE}", threadPage, serverListView);
        }

        #endregion Constructors

        #region Internal Methods

        internal string SafeName(string diskName)
        {
            return (diskName.Replace(" ", "").Replace(":", "").Replace("\\", ""));
        }

        internal void LoadDriveInformation(System.IO.DriveInfo[] driveStatistics)
        {
            foreach (System.IO.DriveInfo statistics in driveStatistics)
            {
                GroupBox groupBox = new GroupBox();
                groupBox.Width = 350;
                groupBox.Height = 160;
                groupBox.Parent = PrimaryTabLayout;
                groupBox.Text = $"Drive - {statistics.Name} ({statistics.VolumeLabel})";
                int newTop = 17;
                int count = 0;


                string safeName = $"{SafeName(statistics.Name)}_{SafeServerName}_{SafeName(statistics.VolumeLabel.Replace(" ", ""))}";
                string systemFlag = statistics.RootDirectory.Attributes.HasFlag(System.IO.FileAttributes.System) ? "Yes" : "No";

                CreateLabel(groupBox, $"lblDrive{safeName}Format", $"Format: {statistics.DriveFormat}", 3, ref newTop, true);
                CreateLabel(groupBox, $"lblDrive{safeName}DriveType", $"Type: {statistics.DriveType}", 3, ref newTop, true);
                CreateLabel(groupBox, $"lblDrive{safeName}System", $"System: {systemFlag}", 3, ref newTop, true);
                CreateLabel(groupBox, $"lblDrive{safeName}Created", 
                    $"Created: {statistics.RootDirectory.CreationTime.ToShortDateString()}", 3, ref newTop, true);
                CreateLabel(groupBox, $"lblDrive{safeName}TotalSize", 
                    $"Total Size: {Shared.Utilities.FileSize(statistics.TotalSize, 2)}", 3, ref newTop, true);
                CreateLabel(groupBox, $"lblDrive{safeName}FreeSpace", "u/k", 3, ref newTop, true);
                CreateLabel(groupBox, $"lblDrive{safeName}UsedSpace", "u/k", 3, ref newTop, true);

                CreateHeartBeat(groupBox, $"pnlHeartDrive{safeName}", 270, 15, 70, 40,
                    Color.OrangeRed, Color.Orange, "Current disk usage");
            }

            DriveStatsLoaded = true;
        }

        internal void LoadDiskInformation(List<DiskStatistics> diskStatistics)
        {
            foreach (DiskStatistics statistics in diskStatistics)
            {
                GroupBox groupBox = new GroupBox();
                groupBox.Width = 350;
                groupBox.Height = 160;
                groupBox.Parent = PrimaryTabLayout;
                groupBox.Text = "Disk " + statistics.DiskName;

                int newTop = 17;
                string safeName = $"{SafeName(statistics.DiskName)}_{SafeServerName}";

                CreateLabel(groupBox, $"lblDiskLength{safeName}", "u/k", 3, ref newTop, true);
                CreateLabel(groupBox, $"lblDiskReads{safeName}", "u/k", 3, ref newTop, true);
                CreateLabel(groupBox, $"lblDiskWrites{safeName}", "u/k", 3, ref newTop, true);
                CreateLabel(groupBox, $"lblDiskTransfer{safeName}", "u/k", 3, ref newTop, true);
                CreateLabel(groupBox, $"lblDiskFreeSpace{safeName}", "u/k", 3, ref newTop, true);
                CreateLabel(groupBox, $"lblDiskIdleTime{safeName}", "u/k", 3, ref newTop, true);
                CreateLabel(groupBox, $"lblDiskPercentTime{safeName}", "u/k", 3, ref newTop, true);
                CreateHeartBeat(groupBox, $"pnlHeartReads{safeName}", 270, 15, 70, 40,
                    Color.OrangeRed, Color.Orange, "Read per second", true);
                CreateHeartBeat(groupBox, $"pnlHeartWrites{safeName}", 270, 65, 70, 40,
                    Color.DeepSkyBlue, Color.LightBlue, "Write per second", true);
                CreateHeartBeat(groupBox, $"pnlHeartDiskUsage{safeName}", 270, 115, 70, 40,
                    Color.Green, Color.GreenYellow, "% Disk time");
            }

            DiskStatsLoaded = true; 
        }

        internal void LoadNetworkInformation(List<NetworkStatistics> networkStatistics)
        {
            foreach (NetworkStatistics statistics in networkStatistics)
            {
                if (statistics.NetworkType == NetworkType.Adapter)
                    continue;

                GroupBox groupBox = new GroupBox();
                groupBox.Width = 350;
                groupBox.Height = 160;
                groupBox.Parent = PrimaryTabLayout;
                groupBox.Text = statistics.Name;

                int newTop = 17;
                string safeName = $"{SafeName(statistics.Name)}_{SafeServerName}";

                CreateLabel(groupBox, $"lblNetworkConnections{safeName}", "u/k", 3, ref newTop, true);
                CreateLabel(groupBox, $"lblNetworkBytesReceived{safeName}", "u/k", 3, ref newTop, true);
                CreateLabel(groupBox, $"lblNetworkBytesSent{safeName}", "u/k", 3, ref newTop, true);
                CreateLabel(groupBox, $"lblNetworkPacketsReceived{safeName}", "u/k", 3, ref newTop, true);
                CreateLabel(groupBox, $"lblNetworkPacketsSent{safeName}", "u/k", 3, ref newTop, true);
                CreateLabel(groupBox, $"lblNetworkQueueLength{safeName}", "u/k", 3, ref newTop, true);
                CreateLabel(groupBox, $"lblNetworkErrors{safeName}", "u/k", 3, ref newTop, true);
                CreateHeartBeat(groupBox, $"pnlHeartNetworkConnections{safeName}", 270, 15, 70, 40,
                    Color.OrangeRed, Color.Orange, "Total Network Connections", true);
                CreateHeartBeat(groupBox, $"pnlHeartNetworkBytesReceived{safeName}", 270, 65, 70, 40,
                    Color.DeepSkyBlue, Color.LightBlue, "Kbps Received", true);
                CreateHeartBeat(groupBox, $"pnlHeartNetworkBytesSent{safeName}", 270, 115, 70, 40,
                    Color.Green, Color.GreenYellow, "kbps sent", true);
            }

            NetworkStatsLoaded = true;
        }

        internal bool Equals(MessageClient obj)
        {
            return (obj.Server == Client.Server);
        }

        #endregion Internal Methods

        #region Private Methods

        private void CreateHeartBeat(in Control parent, in string name, in int left, in int top,
            in int width, in int height, Color primary, Color secondary, string toolTipText, bool autoPoints = false)
        {
            HeartbeatPanel panel = new HeartbeatPanel(Color.White, primary, secondary);
            panel.Parent = parent;
            panel.Left = left;
            panel.Top = top;
            panel.Height = height;
            panel.Width = width;
            panel.Name = name;

            if (autoPoints)
                panel.AutoPoints = true;

            if (!String.IsNullOrEmpty(toolTipText))
            {
                TooltipItem tooltip = new TooltipItem(String.Empty, toolTipText);
                panel.Tag = tooltip;
            }

            ChildControls.Add(panel.Name, panel);
        }

        private void CreateLabel(in Control parent, in string name, in string description, 
            in int left, ref int top, in bool updateTop, in string toolTipText = "",
            in string toolTipTitle = "")
        {
            Label label = new Label();
            label.Name = name;
            label.Text = description;
            label.Top = top;
            label.Left = left;
            label.Parent = parent;
            label.AutoSize = true;

            if (!String.IsNullOrEmpty(toolTipText))
            {
                TooltipItem tooltip = new TooltipItem(toolTipTitle, toolTipText);
                label.Tag = tooltip;
            }

            if (updateTop)
                top = top + label.Height + 3;

            ChildControls.Add(label.Name, label);
        }

        #endregion Private Methods

        #region Properties

        internal MessageClient Client { get; private set; }

        internal ServerConnection ServerConnection { get; private set; }

        internal TabPage PrimaryTab { get; private set; }

        internal Dictionary<string, Control> ChildControls { get; private set; }

        internal string SafeServerName { get; private set; }

        internal FlowLayoutPanel PrimaryTabLayout { get; private set; }

        internal bool DriveStatsLoaded { get; private set; }

        internal bool DiskStatsLoaded { get; private set; }

        internal bool NetworkStatsLoaded { get; private set; }

        #endregion Properties
    }
}

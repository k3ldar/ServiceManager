/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * 
 * 
 * The contents of this file are subject to the GNU General Public License
 * v3.0 (the "License"); you may not use this file except in compliance
 * with the License. You may obtain a copy * of the License at 
 * https://github.com/k3ldar/FbReplicationEngine/blob/master/LICENSE
 *
 * Software distributed under the License is distributed on an
 * "AS IS" basis, WITHOUT WARRANTY OF ANY KIND, either express
 * or implied. See the License for the specific language governing
 * rights and limitations under the License.
 *
 *  The Original Code was created by Simon Carter (s1cart3r@gmail.com)
 *
 *  Copyright (c) 2011 - 2017 Simon Carter.  All Rights Reserved
 *
 *  Purpose:  
 *
 */

namespace ServiceAdminConsole
{
    partial class ServiceManagerClient
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.tabMain = new System.Windows.Forms.TabControl();
            this.tabPageThreads = new System.Windows.Forms.TabPage();
            this.lvThreads = new SharedControls.Classes.ListViewEx();
            this.colThreadsServer = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colThreadsName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colThreadsProcess = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colThreadsSystem = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colThreadsID = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colThreadsCancelled = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colThreadsUnresponsive = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colThreadsRemoving = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.contextMenuThreads = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.contextMenuThreadsRefresh = new System.Windows.Forms.ToolStripMenuItem();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.tsLabelTimeTillRun = new System.Windows.Forms.ToolStripStatusLabel();
            this.tsCPU = new System.Windows.Forms.ToolStripStatusLabel();
            this.tsRunTime = new System.Windows.Forms.ToolStripStatusLabel();
            this.tsMissingCount = new System.Windows.Forms.ToolStripStatusLabel();
            this.tsStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.tmrRuntime = new System.Windows.Forms.Timer(this.components);
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.menuServer = new System.Windows.Forms.ToolStripMenuItem();
            this.menuServerConnect = new System.Windows.Forms.ToolStripMenuItem();
            this.menuServerDisconnect = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.menuServerRecent = new System.Windows.Forms.ToolStripMenuItem();
            this.menuServerRecentOpenAll = new System.Windows.Forms.ToolStripMenuItem();
            this.menuServerRecentSeperator = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripSeparator();
            this.menuServerClose = new System.Windows.Forms.ToolStripMenuItem();
            this.menuHelp = new System.Windows.Forms.ToolStripMenuItem();
            this.controlToolTip = new System.Windows.Forms.ToolTip(this.components);
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tabMain.SuspendLayout();
            this.tabPageThreads.SuspendLayout();
            this.contextMenuThreads.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabMain
            // 
            this.tabMain.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabMain.Controls.Add(this.tabPageThreads);
            this.tabMain.Location = new System.Drawing.Point(12, 41);
            this.tabMain.Name = "tabMain";
            this.tabMain.SelectedIndex = 0;
            this.tabMain.Size = new System.Drawing.Size(760, 493);
            this.tabMain.TabIndex = 0;
            // 
            // tabPageThreads
            // 
            this.tabPageThreads.Controls.Add(this.lvThreads);
            this.tabPageThreads.Location = new System.Drawing.Point(4, 22);
            this.tabPageThreads.Name = "tabPageThreads";
            this.tabPageThreads.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageThreads.Size = new System.Drawing.Size(752, 467);
            this.tabPageThreads.TabIndex = 4;
            this.tabPageThreads.Text = "Threads";
            this.tabPageThreads.UseVisualStyleBackColor = true;
            // 
            // lvThreads
            // 
            this.lvThreads.AllowColumnReorder = true;
            this.lvThreads.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lvThreads.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colThreadsServer,
            this.colThreadsName,
            this.colThreadsProcess,
            this.colThreadsSystem,
            this.colThreadsID,
            this.colThreadsCancelled,
            this.colThreadsUnresponsive,
            this.colThreadsRemoving});
            this.lvThreads.ContextMenuStrip = this.contextMenuThreads;
            this.lvThreads.FullRowSelect = true;
            this.lvThreads.Location = new System.Drawing.Point(6, 6);
            this.lvThreads.MinimumSize = new System.Drawing.Size(740, 200);
            this.lvThreads.Name = "lvThreads";
            this.lvThreads.OwnerDraw = true;
            this.lvThreads.SaveName = "WDThreads";
            this.lvThreads.ShowToolTip = false;
            this.lvThreads.Size = new System.Drawing.Size(740, 455);
            this.lvThreads.TabIndex = 1;
            this.lvThreads.UseCompatibleStateImageBehavior = false;
            this.lvThreads.View = System.Windows.Forms.View.Details;
            // 
            // colThreadsServer
            // 
            this.colThreadsServer.Text = "Server";
            this.colThreadsServer.Width = 117;
            // 
            // colThreadsName
            // 
            this.colThreadsName.Text = "Name";
            this.colThreadsName.Width = 230;
            // 
            // colThreadsProcess
            // 
            this.colThreadsProcess.Text = "Process CPU";
            this.colThreadsProcess.Width = 90;
            // 
            // colThreadsSystem
            // 
            this.colThreadsSystem.Text = "System CPU";
            this.colThreadsSystem.Width = 90;
            // 
            // colThreadsID
            // 
            this.colThreadsID.Text = "ID";
            this.colThreadsID.Width = 50;
            // 
            // colThreadsCancelled
            // 
            this.colThreadsCancelled.Text = "Cancelled";
            this.colThreadsCancelled.Width = 80;
            // 
            // colThreadsUnresponsive
            // 
            this.colThreadsUnresponsive.Text = "Unresponsive";
            this.colThreadsUnresponsive.Width = 80;
            // 
            // colThreadsRemoving
            // 
            this.colThreadsRemoving.Text = "Marked for Removal";
            this.colThreadsRemoving.Width = 120;
            // 
            // contextMenuThreads
            // 
            this.contextMenuThreads.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.contextMenuThreadsRefresh});
            this.contextMenuThreads.Name = "contextMenuStrip1";
            this.contextMenuThreads.Size = new System.Drawing.Size(114, 26);
            // 
            // contextMenuThreadsRefresh
            // 
            this.contextMenuThreadsRefresh.Name = "contextMenuThreadsRefresh";
            this.contextMenuThreadsRefresh.Size = new System.Drawing.Size(113, 22);
            this.contextMenuThreadsRefresh.Text = "Refresh";
            this.contextMenuThreadsRefresh.Click += new System.EventHandler(this.contextMenuThreadsRefresh_Click);
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsLabelTimeTillRun,
            this.tsCPU,
            this.tsRunTime,
            this.tsMissingCount,
            this.tsStatus});
            this.statusStrip1.Location = new System.Drawing.Point(0, 537);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(784, 24);
            this.statusStrip1.TabIndex = 1;
            this.statusStrip1.Text = "statusStrip";
            // 
            // tsLabelTimeTillRun
            // 
            this.tsLabelTimeTillRun.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Right;
            this.tsLabelTimeTillRun.Name = "tsLabelTimeTillRun";
            this.tsLabelTimeTillRun.Size = new System.Drawing.Size(92, 19);
            this.tsLabelTimeTillRun.Text = "Not Connected";
            // 
            // tsCPU
            // 
            this.tsCPU.AutoSize = false;
            this.tsCPU.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Right;
            this.tsCPU.Name = "tsCPU";
            this.tsCPU.Size = new System.Drawing.Size(85, 19);
            this.tsCPU.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // tsRunTime
            // 
            this.tsRunTime.Name = "tsRunTime";
            this.tsRunTime.Size = new System.Drawing.Size(0, 19);
            // 
            // tsMissingCount
            // 
            this.tsMissingCount.Name = "tsMissingCount";
            this.tsMissingCount.Size = new System.Drawing.Size(0, 19);
            // 
            // tsStatus
            // 
            this.tsStatus.Name = "tsStatus";
            this.tsStatus.Size = new System.Drawing.Size(0, 19);
            // 
            // tmrRuntime
            // 
            this.tmrRuntime.Enabled = true;
            this.tmrRuntime.Interval = 500;
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuServer,
            this.menuHelp});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(784, 24);
            this.menuStrip1.TabIndex = 2;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // menuServer
            // 
            this.menuServer.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuServerConnect,
            this.menuServerDisconnect,
            this.toolStripMenuItem1,
            this.menuServerRecent,
            this.toolStripMenuItem2,
            this.menuServerClose});
            this.menuServer.Name = "menuServer";
            this.menuServer.Size = new System.Drawing.Size(51, 20);
            this.menuServer.Text = "Server";
            this.menuServer.DropDownOpening += new System.EventHandler(this.menuServer_DropDownOpening);
            // 
            // menuServerConnect
            // 
            this.menuServerConnect.Name = "menuServerConnect";
            this.menuServerConnect.Size = new System.Drawing.Size(180, 22);
            this.menuServerConnect.Text = "Connect";
            this.menuServerConnect.Click += new System.EventHandler(this.menuServerConnect_Click);
            // 
            // menuServerDisconnect
            // 
            this.menuServerDisconnect.Name = "menuServerDisconnect";
            this.menuServerDisconnect.Size = new System.Drawing.Size(180, 22);
            this.menuServerDisconnect.Text = "Disconnect";
            this.menuServerDisconnect.Click += new System.EventHandler(this.menuServerDisconnect_Click);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(177, 6);
            // 
            // menuServerRecent
            // 
            this.menuServerRecent.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuServerRecentOpenAll,
            this.menuServerRecentSeperator});
            this.menuServerRecent.Name = "menuServerRecent";
            this.menuServerRecent.Size = new System.Drawing.Size(180, 22);
            this.menuServerRecent.Text = "Recent";
            this.menuServerRecent.DropDownOpening += new System.EventHandler(this.menuServerRecent_DropDownOpening);
            // 
            // menuServerRecentOpenAll
            // 
            this.menuServerRecentOpenAll.Name = "menuServerRecentOpenAll";
            this.menuServerRecentOpenAll.Size = new System.Drawing.Size(120, 22);
            this.menuServerRecentOpenAll.Text = "Open All";
            this.menuServerRecentOpenAll.Click += new System.EventHandler(this.menuServerRecentOpenAll_Click);
            // 
            // menuServerRecentSeperator
            // 
            this.menuServerRecentSeperator.Name = "menuServerRecentSeperator";
            this.menuServerRecentSeperator.Size = new System.Drawing.Size(117, 6);
            this.menuServerRecentSeperator.Visible = false;
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(177, 6);
            // 
            // menuServerClose
            // 
            this.menuServerClose.Name = "menuServerClose";
            this.menuServerClose.Size = new System.Drawing.Size(180, 22);
            this.menuServerClose.Text = "Close";
            this.menuServerClose.Click += new System.EventHandler(this.menuServerClose_Click);
            // 
            // menuHelp
            // 
            this.menuHelp.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.aboutToolStripMenuItem});
            this.menuHelp.Name = "menuHelp";
            this.menuHelp.Size = new System.Drawing.Size(44, 20);
            this.menuHelp.Text = "Help";
            // 
            // controlToolTip
            // 
            this.controlToolTip.AutoPopDelay = 2000;
            this.controlToolTip.InitialDelay = 500;
            this.controlToolTip.ReshowDelay = 100;
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.aboutToolStripMenuItem.Text = "About";
            this.aboutToolStripMenuItem.Click += new System.EventHandler(this.aboutToolStripMenuItem_Click);
            // 
            // ServiceManagerClient
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(784, 561);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.menuStrip1);
            this.Controls.Add(this.tabMain);
            this.DoubleBuffered = true;
            this.MinimumSize = new System.Drawing.Size(800, 600);
            this.Name = "ServiceManagerClient";
            this.Text = "Service Manager";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ServerManagerClient_FormClosing);
            this.Load += new System.EventHandler(this.ServiceClient_Load);
            this.tabMain.ResumeLayout(false);
            this.tabPageThreads.ResumeLayout(false);
            this.contextMenuThreads.ResumeLayout(false);
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TabControl tabMain;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel tsLabelTimeTillRun;
        private System.Windows.Forms.ToolStripStatusLabel tsStatus;
        private System.Windows.Forms.ToolStripStatusLabel tsMissingCount;
        private System.Windows.Forms.Timer tmrRuntime;
        private System.Windows.Forms.ToolStripStatusLabel tsRunTime;
        private System.Windows.Forms.TabPage tabPageThreads;
        private System.Windows.Forms.ToolStripStatusLabel tsCPU;
        private SharedControls.Classes.ListViewEx lvThreads;
        private System.Windows.Forms.ColumnHeader colThreadsName;
        private System.Windows.Forms.ColumnHeader colThreadsProcess;
        private System.Windows.Forms.ColumnHeader colThreadsSystem;
        private System.Windows.Forms.ColumnHeader colThreadsID;
        private System.Windows.Forms.ColumnHeader colThreadsCancelled;
        private System.Windows.Forms.ColumnHeader colThreadsUnresponsive;
        private System.Windows.Forms.ColumnHeader colThreadsRemoving;
        private System.Windows.Forms.ContextMenuStrip contextMenuThreads;
        private System.Windows.Forms.ToolStripMenuItem contextMenuThreadsRefresh;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem menuServer;
        private System.Windows.Forms.ToolStripMenuItem menuServerConnect;
        private System.Windows.Forms.ToolStripMenuItem menuServerDisconnect;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem menuServerRecent;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem2;
        private System.Windows.Forms.ToolStripMenuItem menuServerClose;
        private System.Windows.Forms.ColumnHeader colThreadsServer;
        private System.Windows.Forms.ToolStripMenuItem menuServerRecentOpenAll;
        private System.Windows.Forms.ToolStripSeparator menuServerRecentSeperator;
        private System.Windows.Forms.ToolStripMenuItem menuHelp;
        private System.Windows.Forms.ToolTip controlToolTip;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
    }
}
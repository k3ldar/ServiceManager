namespace ServiceManager
{
    partial class PrimaryService
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this._pumNotifyIcon = new System.Windows.Forms.ContextMenuStrip(this.components);
            this._menuRunAsAppAbout = new System.Windows.Forms.ToolStripMenuItem();
            this._menuRunAsAppSepparator = new System.Windows.Forms.ToolStripSeparator();
            this._menuRunAsAppClose = new System.Windows.Forms.ToolStripMenuItem();
            this._notifyRunAsApp = new System.Windows.Forms.NotifyIcon(this.components);
            this._pumNotifyIcon.SuspendLayout();
            // 
            // _pumNotifyIcon
            // 
            this._pumNotifyIcon.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
                this._menuRunAsAppAbout, this._menuRunAsAppSepparator,
            this._menuRunAsAppClose});
            this._pumNotifyIcon.Name = "pumNotifyIcon";
            this._pumNotifyIcon.Size = new System.Drawing.Size(108, 54);

            this._menuRunAsAppAbout.Name = "_menuRunAsAppAbout";
            this._menuRunAsAppAbout.Text = "About";
            this._menuRunAsAppAbout.Click += new System.EventHandler(this.menuRunAsAppAbout_Click);
            // 
            // _menuRunAsAppClose
            // 
            this._menuRunAsAppClose.Name = "_menuRunAsAppClose";
            this._menuRunAsAppClose.Size = new System.Drawing.Size(107, 22);
            this._menuRunAsAppClose.Text = "Close";
            this._menuRunAsAppClose.Click += new System.EventHandler(this.menuRunAsAppClose_Click);
            // 
            // _notifyRunAsApp
            // 
            this._notifyRunAsApp.ContextMenuStrip = this._pumNotifyIcon;
            this._notifyRunAsApp.Text = "Service Manager";
            this._notifyRunAsApp.DoubleClick += new System.EventHandler(this.notifyRunAsApp_DoubleClick);
            // 
            // PrimaryService
            // 
            this.AutoLog = false;
            this.ServiceName = "Firebird Replication Engine";
            this._pumNotifyIcon.ResumeLayout(false);

        }

        #endregion
    }
}

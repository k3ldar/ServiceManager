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
 *  File: PrimaryService.cs
 *
 *  Purpose:  
 *
 *  Date        Name                Reason
 *  21/04/2018  Simon Carter        Initially Created
 *
 * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */
using System;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using System.Drawing;

using Shared;
using Shared.Classes;

using ServiceManager.Classes;
using ServiceManager.Core;

using Shared.Communication;

#pragma warning disable IDE1006, IDE0017

namespace ServiceManager
{
    public partial class PrimaryService : BaseService, ISendReport, ILogging, IEventManager, IClientMessage
    {
        #region Private Members

        private static object _lockObject = new object();

        private ContextMenuStrip _pumNotifyIcon;
        private ToolStripMenuItem _menuRunAsAppClose;
        private ToolStripMenuItem _menuRunAsAppAbout;
        private ToolStripSeparator _menuRunAsAppSepparator;
        private NotifyIcon _notifyRunAsApp;
        private PluginManager _pluginManager;
        private ServiceSettings _serviceSettings;
        private ServiceSettings _SmtpServer;
        private LocalServiceDataThread _serviceDataThread;

        private FileSystemWatcher _watcher = null;

        #endregion Private Members

        #region Constructors

        public PrimaryService()
        {
            MessageServerLogin = true;

            InitializeComponent();
            LoadServiceSettings();
            LoadEmailSettings();

            _serviceDataThread = new LocalServiceDataThread();
            _serviceDataThread.OnNewServerDetails += ServerDataThread_OnNewServerDetails;

            _serviceDataThread.MonitorDisks = _serviceSettings.Find(Constants.Settings.MonitorDisks).GetValue<bool>();
            _serviceDataThread.MonitorNetwork = _serviceSettings.Find(Constants.Settings.MonitorNetworks).GetValue<bool>();

            ThreadManager.ThreadStart(_serviceDataThread, 
                Constants.Service.ThreadLocalDataMonitoring, 
                System.Threading.ThreadPriority.Lowest);

            _notifyRunAsApp.Icon = Icon.ExtractAssociatedIcon(Assembly.GetExecutingAssembly().Location);
            _pluginManager = new PluginManager(this, this, this, this);
            _pluginManager.LoadServices();

            ProcessParams.PostProcessAllParams(_pluginManager);
        }

        #endregion Constructors

        #region Protected Overridden Service Methods

        /// <summary>
        /// Start this service
        /// </summary>
        protected override void OnStart(string[] args)
        {
            Program.Closing = false;

            Configuration.Initialize();

            base.OnStart(args);
            ThreadManager.Initialise();
            Shared.EventLog.Initialise(7);
            Parameters.Initialise(args, new char[] { '-', '/' }, new char[] { ' ', ':' });

            Microsoft.Win32.SystemEvents.PowerModeChanged += SystemEvents_PowerModeChanged;
            ThreadManager.ThreadForcedToClose += ThreadManager_ThreadForcedToClose;
            ThreadManager.ThreadCpuChanged += ThreadManager_ThreadCpuChanged;

            _pluginManager.StartServices();
            Shared.EventLog.Add("OnStart");
            InitializeTCPServer();
        }

        /// <summary>
        /// Stop this service.
        /// </summary>
        protected override void OnStop()
        {
            Program.Closing = true;

            Shared.EventLog.Add("Service Stopping");

            _pluginManager.StopServices();

            base.OnStop();

            ThreadManager.ThreadForcedToClose -= ThreadManager_ThreadForcedToClose;
            ThreadManager.Finalise();
            Shared.EventLog.Add("Service Stop");
        }

        protected override void OnContinue()
        {
            Shared.EventLog.Add("Continue");
        }

        public override void RunAsApplication()
        {
            Configuration.Initialize();

            InitializeTCPServer();
            try
            {
                Microsoft.Win32.SystemEvents.PowerModeChanged += SystemEvents_PowerModeChanged;

                ThreadManager.ThreadCpuChanged += ThreadManager_ThreadCpuChanged;
                Shared.EventLog.Initialise(7);
                _notifyRunAsApp.Visible = true;
                this._notifyRunAsApp.ContextMenuStrip = this._pumNotifyIcon;

                _pluginManager.StartServices();
                try
                {
                    Application.Run();
                }
                finally
                {
                    _pluginManager.StopServices();
                }
            }
            finally
            {
                TCPServerStop();
                _notifyRunAsApp.Visible = false;
            }
        }

        protected override void MessageReceived(object sender, Shared.Communication.Message message)
        {
            base.MessageReceived(sender, message);

            switch (message.Title)
            {
                case StringConstants.LOAD_CLIENT_DETAILS:
                    message.Title = StringConstants.CREATE_SETTINGS;
                    message.Contents = ServerBaseControl.ConvertToJson<ServiceSettings>(_serviceSettings);
                    MessageRespond(message);

                    message.Contents = ServerBaseControl.ConvertToJson<ServiceSettings>(_SmtpServer);
                    MessageRespond(message);

                    message.Title = StringConstants.LOAD_CLIENT_DETAILS;

                    break;

                case StringConstants.Service.SERVICE_SETTINGS_SAVE:
                    SaveServiceSettings(ServerBaseControl.ConvertFromJson<ServiceSettings>(message.Contents));

                    return;

                case StringConstants.Service.SERVICE_EMAIL_SETTINGS_SAVE:
                    SaveEmailSettings(ServerBaseControl.ConvertFromJson<ServiceSettings>(message.Contents));

                    return;
            }

            _pluginManager.ProcessMessage(message);
        }

        protected override bool MessageServerLoginAttempt(string userName, string password, string ipAddress)
        {
            ServiceSetting serverUser = _serviceSettings.Find(Constants.Settings.UserName);
            ServiceSetting serverPassword = _serviceSettings.Find(Constants.Settings.Password);

            if (!MessageServerAllowConnect(ipAddress))
                return (false);

            if (serverUser == null || serverPassword == null)
                return (true);

            return (userName == serverUser.Value && password == serverPassword.Value);
        }

        protected override bool MessageServerAllowConnect(string ipAddress)
        {
            ServiceSetting ipSetting = _serviceSettings.Find(Constants.Settings.ConnectionIP);

            if (ipSetting == null)
                return (true);

            string[] ipAddresses = ipSetting.Value.Split(Constants.Other.SemiColon);

            foreach (string address in ipAddresses)
            {
                string allowedIPAddresses = address;

                int wildCard = allowedIPAddresses.IndexOf(Constants.Other.Asterix);

                if (wildCard >= 0)
                    allowedIPAddresses = allowedIPAddresses.Substring(0, wildCard);

                if (String.IsNullOrEmpty(allowedIPAddresses)
                    || ipAddress.StartsWith(allowedIPAddresses))
                {
                    return (true);
                }
            }

            return (base.MessageServerAllowConnect(ipAddress));
        }

        #endregion Protected Overridden Service Methods

        #region Interface Methods

        #region IClientMessage

        public void BroadCastMessage(string title)
        {
            if (String.IsNullOrEmpty(title))
                throw new ArgumentNullException(nameof(title));

            MessageSend(new Shared.Communication.Message(title, String.Empty, MessageType.Command), true);
        }

        public void BroadCastMessage(string title, string data)
        {
            if (String.IsNullOrEmpty(title))
                throw new ArgumentNullException(nameof(title));

            MessageSend(new Shared.Communication.Message(title, data, MessageType.Command), true);
        }

        public void BroadCastMessage(Shared.Communication.Message message)
        {
            if (message == null)
                throw new ArgumentNullException(nameof(message));

            MessageSend(message, true);
        }

        public void MessageRespond(Shared.Communication.Message message)
        {
            if (message == null)
                throw new ArgumentNullException(nameof(message));

            MessageSend(message, false);
        }

        #endregion IClientMessage

        #region IEventManager Interface

        public void SendEvent(string eventName, string eventData)
        {
            throw new NotImplementedException();
        }

        #endregion IEventManager Interface

        #region ISendReport Interface

        public void SendMessage(string senderEmail, string recipientEmail, string title, string message, bool urgent)
        {
            try
            {
                Email email = new Email(_SmtpServer.Find(Constants.Settings.SmtpSenderEmail).GetValue<string>(),
                    _SmtpServer.Find(Constants.Settings.SmtpHost).GetValue<string>(),
                    _SmtpServer.Find(Constants.Settings.SmtpUsername).GetValue<string>(),
                    _SmtpServer.Find(Constants.Settings.SmtpPassword).GetValue<string>(),
                    _SmtpServer.Find(Constants.Settings.SmtpPort).GetValue<int>(),
                    _SmtpServer.Find(Constants.Settings.SmtpSSL).GetValue<bool>());

                email.SendEmail(senderEmail, recipientEmail, recipientEmail, message, title, message.Contains("<html>"));
            }
            catch (Exception error)
            {
                AddToLog(error, "Send Email");
            }
        }

        #endregion ISendReport Interface

        #region ILogging Interface

        public void AddToLog(string data)
        {
            Shared.EventLog.Add(data);
        }

        public void AddToLog(Exception exception)
        {
            Shared.EventLog.Add(exception);
        }

        public void AddToLog(Exception exception, string data)
        {
            Shared.EventLog.Add(exception, data);
        }

        #endregion ILogging Interface

        #endregion Interface Methods

        #region Private Methods

        #region Service Settings

        private void SaveServiceSettings(ServiceSettings serviceSettings)
        {
            _serviceSettings = serviceSettings;
            string settingFile = Utils.GetPath(PathType.Settings) + Constants.Settings.ServiceSettingsFileName;
            Utilities.FileWrite(settingFile, ServerBaseControl.ConvertToJson<ServiceSettings>(_serviceSettings));

            _serviceDataThread.MonitorDisks = _serviceSettings.Find(Constants.Settings.MonitorDisks).GetValue<bool>();
            _serviceDataThread.MonitorNetwork = _serviceSettings.Find(Constants.Settings.MonitorNetworks).GetValue<bool>();
        }

        private void SaveEmailSettings(ServiceSettings serviceSettings)
        {
            _SmtpServer = serviceSettings;
            string settingFile = Utils.GetPath(PathType.Settings) + Constants.Settings.EmailSettingsFileName;
            Utilities.FileWrite(settingFile, ServerBaseControl.ConvertToJson<ServiceSettings>(_SmtpServer));
        }

        private void LoadEmailSettings()
        {
            string settingFile = Utils.GetPath(PathType.Settings) + Constants.Settings.EmailSettingsFileName;
            string fileContents = Utilities.FileRead(settingFile, false);


            if (String.IsNullOrEmpty(fileContents))
            {
                _SmtpServer = new ServiceSettings(Constants.Settings.SmtpSettings);
                _SmtpServer.MessageName = StringConstants.Service.SERVICE_EMAIL_SETTINGS_SAVE;

                Utilities.FileWrite(settingFile, ServerBaseControl.ConvertToJson<ServiceSettings>(_SmtpServer));
            }
            else
            {
                _SmtpServer = ServerBaseControl.ConvertFromJson<ServiceSettings>(fileContents);
            }

            if (_SmtpServer.Find(Constants.Settings.SmtpUsername) == null)
                _SmtpServer.Add(new ServiceSetting(Constants.Settings.SmtpUsername, SettingType.String,
                    String.Empty)
                { MaximumLength = 50 });

            if (_SmtpServer.Find(Constants.Settings.SmtpPassword) == null)
                _SmtpServer.Add(new ServiceSetting(Constants.Settings.SmtpPassword, SettingType.Password,
                    String.Empty)
                { MaximumLength = 50 });

            if (_SmtpServer.Find(Constants.Settings.SmtpHost) == null)
                _SmtpServer.Add(new ServiceSetting(Constants.Settings.SmtpHost,
                SettingType.String, String.Empty)
                { MaximumLength = 50 });

            if (_SmtpServer.Find(Constants.Settings.SmtpSenderEmail) == null)
                _SmtpServer.Add(new ServiceSetting(Constants.Settings.SmtpSenderEmail,
                    SettingType.String, String.Empty));

            if (_SmtpServer.Find(Constants.Settings.SmtpSSL) == null)
                _SmtpServer.Add(new ServiceSetting(Constants.Settings.SmtpSSL,
                    SettingType.Bool, Boolean.TrueString));
        }

        private void LoadServiceSettings()
        {
            string settingFile = Utils.GetPath(PathType.Settings) + Constants.Settings.ServiceSettingsFileName;
            string fileContents = Utilities.FileRead(settingFile, false);


            if (String.IsNullOrEmpty(fileContents))
            {
                _serviceSettings = new ServiceSettings(Constants.Settings.Service);
                _serviceSettings.MessageName = StringConstants.Service.SERVICE_SETTINGS_SAVE;

                Utilities.FileWrite(settingFile, ServerBaseControl.ConvertToJson<ServiceSettings>(_serviceSettings));
            }
            else
            {
                _serviceSettings = ServerBaseControl.ConvertFromJson<ServiceSettings>(fileContents);
            }

            if (_serviceSettings.Find(Constants.Settings.UserName) == null)
                _serviceSettings.Add(new ServiceSetting(Constants.Settings.UserName, SettingType.String,
                    StringConstants.Service.DefaultUsername) { MaximumLength = 50 });

            if (_serviceSettings.Find(Constants.Settings.Password) == null)
                _serviceSettings.Add(new ServiceSetting(Constants.Settings.Password, SettingType.Password,
                    StringConstants.Service.DefaultPassword) { MaximumLength = 50 });

            if (_serviceSettings.Find(Constants.Settings.ConnectionIP) == null)
                _serviceSettings.Add(new ServiceSetting(Constants.Settings.ConnectionIP,
                SettingType.String, Constants.Other.Asterix.ToString()) { MaximumLength = 50 });

            if (_serviceSettings.Find(Constants.Settings.MonitorNetworks) == null)
                _serviceSettings.Add(new ServiceSetting(Constants.Settings.MonitorNetworks,
                    SettingType.Bool, Boolean.TrueString));

            if (_serviceSettings.Find(Constants.Settings.MonitorDisks) == null)
                _serviceSettings.Add(new ServiceSetting(Constants.Settings.MonitorDisks,
                    SettingType.Bool, Boolean.TrueString));
        }



        #endregion Service Settings

        #region Thread Manager

        private void ThreadManager_ThreadForcedToClose(object sender, ThreadManagerEventArgs e)
        {
            Shared.EventLog.Add(String.Format("Thread forced to close: {0}, Unresponsive: {1}, Marked For Removal: {2}",
                e.Thread.Name, e.Thread.UnResponsive.ToString(), e.Thread.MarkedForRemoval.ToString()));
            Shared.EventLog.Add(String.Format("Start Time: {0}", e.Thread.TimeStart.ToString("g")));
            Shared.EventLog.Add(String.Format("End Time: {0}", e.Thread.TimeFinish.ToString("g")));

            if (!Program.Closing)
            {
                if (e.Thread.Name == Constants.Service.ThreadLocalDataMonitoring)
                {
                    _serviceDataThread = new LocalServiceDataThread();
                    _serviceDataThread.OnNewServerDetails += ServerDataThread_OnNewServerDetails;

                    _serviceDataThread.MonitorDisks = _serviceSettings.Find(Constants.Settings.MonitorDisks).GetValue<bool>();
                    _serviceDataThread.MonitorNetwork = _serviceSettings.Find(Constants.Settings.MonitorNetworks).GetValue<bool>();

                    ThreadManager.ThreadStart(_serviceDataThread,
                        Constants.Service.ThreadLocalDataMonitoring,
                        System.Threading.ThreadPriority.BelowNormal);
                }
            }
        }

        private void ThreadManager_ThreadCpuChanged(object sender, EventArgs e)
        {
            MessageSend(new Shared.Communication.Message("THREAD_CPU_CHANGED",
                ThreadManager.CpuUsage.ToString(),
                Shared.Communication.MessageType.Broadcast), true);

            string Result = String.Empty;

            for (int i = 0; i < ThreadManager.ThreadCount; i++)
            {
                ThreadManager thread = ThreadManager.Get(i);

                Result += String.Format("{0}\r\n", thread.ToString());
            }

            MessageSend(new Shared.Communication.Message("THREAD_USAGE", Result,
                Shared.Communication.MessageType.Broadcast), true);
        }

        #endregion Thread Manager

        #region Watch Config Files

        private void CreateConfigWatch(string folder)
        {
            if (!Directory.Exists(folder))
                return;

            // delete any existing temp files
            string[] tempFiles = Directory.GetFiles(folder, "*.tmp");

            foreach (string file in tempFiles)
            {
                if (File.Exists(file))
                {
                    File.Delete(file);
                }
            }

            _watcher = new FileSystemWatcher(folder);
            _watcher.Changed += watcher_Changed;
            _watcher.Deleted += watcher_Deleted;
            _watcher.Renamed += watcher_Renamed;
            _watcher.Created += watcher_Created;
            _watcher.EnableRaisingEvents = true;
            _watcher.IncludeSubdirectories = false;
        }

        private void RemoveConfigWatch(string folder)
        {

        }

        #region Watch Events

        private void watcher_Created(object sender, FileSystemEventArgs e)
        {
            string fileName = e.FullPath.ToLower();
        }

        private void watcher_Renamed(object sender, RenamedEventArgs e)
        {
            string fileName = e.FullPath.ToLower();

        }

        private void watcher_Deleted(object sender, FileSystemEventArgs e)
        {

        }

        private void watcher_Changed(object sender, FileSystemEventArgs e)
        {
            string fileName = e.FullPath.ToLower();

            if (!File.Exists(fileName))
                return;

            if (Path.GetExtension(fileName).ToLower() == ".tmp")
            {
                FileInfo info = new FileInfo(fileName);

                if (info.Length == 0)
                    return;

                string[] parts = Shared.Utilities.FileRead(fileName, false).Split('@');

                if (parts.Length == 2)
                {
                    switch (parts[0])
                    {
                        case "NEW":
                            break;

                        case "DELETE":
                        case "CHANGED":

                            break;
                    }
                }

                MessageSend(new Shared.Communication.Message("CONFIGURATION_CHANGED", String.Empty, Shared.Communication.MessageType.Info), true);
                File.Delete(fileName);
            }
        }

        #endregion Watch Events

        #endregion Watch Config Files

        #region System Events

        private void SystemEvents_PowerModeChanged(object sender, Microsoft.Win32.PowerModeChangedEventArgs e)
        {
            Shared.EventLog.Add("PowerModeChanged");
            switch (e.Mode)
            {
                case Microsoft.Win32.PowerModes.Suspend:
                    Shared.EventLog.Add("System Suspend, closing replication Threads");

                    break;

                case Microsoft.Win32.PowerModes.Resume:
                    Shared.EventLog.Add("System Resumed, creating replication Threads");

                    break;
            }
        }

        #endregion System Events

        #region TCP Service

        private void InitializeTCPServer()
        {
#if DEBUG
            Shared.EventLog.Debug("ServiceManager " + System.Reflection.MethodBase.GetCurrentMethod().Name);
#endif
            try
            {
                if (Parameters.OptionExists("port"))
                    Configuration.TCP.Port = (ushort)Parameters.GetOption("port", Configuration.TCP.Port);

                MessageServerPort = (int)Configuration.TCP.Port;
                MessageServerActive = true;
                InitializeTCPServer(Configuration.TCP.Port);
            }
            catch (Exception err)
            {
                if (err.Message.Contains("Only one usage of each socket"))
                {
                    ThreadManager.Cancel("MessageServer Connection Thread");
                }
                else
                    throw;
            }
        }

        #endregion TCP Service

        #region Popup Menu

        private void menuRunAsAppClose_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void menuRunAsAppAbout_Click(object sender, EventArgs e)
        {
            ServiceManager.Core.Forms.AboutForm.ShowAboutWindow(null);
        }

        private void notifyRunAsApp_DoubleClick(object sender, EventArgs e)
        {
            
        }

        #endregion Popup Menu

        #region Dynamic Server Details

        private void ServerDataThread_OnNewServerDetails(object sender, DynamicServerDetailsEventArgs e)
        {
            BroadCastMessage(new Shared.Communication.Message(
                StringConstants.SERVER_DATA_DYNAMIC_NEW,
                Newtonsoft.Json.JsonConvert.SerializeObject(e.Details),
                MessageType.Info));
        }

        #endregion Dynamic Server Details

        #endregion Private Methods
    }
}

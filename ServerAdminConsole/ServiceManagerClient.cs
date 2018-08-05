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
 *  File: Console.cs
 *
 *  Purpose:  
 *
 *  Date        Name                Reason
 *  28/04/2018  Simon Carter        Initially Created
 *
 * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

using ServiceManager.Core;
using ServiceManager.Core.Classes;
using ServiceManager.Core.Controls;

using ServiceAdminConsole.Controls;

using Shared.Classes;
using Shared.Communication;

using SharedControls;
using SharedControls.Controls;
using SharedControls.Forms;

using Newtonsoft.Json;

#pragma warning disable IDE1006, IDE0016, IDE0017, IDE0020

namespace ServiceAdminConsole
{
    public partial class ServiceManagerClient : BaseForm, IClientConnectionHelper
    {
        #region Private Members

        private object _lockObject = new object();
        private Dictionary<string, Connection> _clientConnections;
        private Dictionary<string, SharedControls.Classes.ListViewEx> _clientListViews;

        #endregion Private Members

        #region Constructors

        public ServiceManagerClient()
        {
            InitializeComponent();

#if DEBUG
            Shared.EventLog.Debug("SMC " + System.Reflection.MethodBase.GetCurrentMethod().Name);
#endif
            System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-GB");

            _clientConnections = new Dictionary<string, Connection>();
            _clientListViews = new Dictionary<string, SharedControls.Classes.ListViewEx>();

            tsStatus.Text = "";
            tsMissingCount.Text = "";
            tabMain.TabPages.Remove(tabPageThreads);

            this.Icon = Icon.ExtractAssociatedIcon(
                System.Reflection.Assembly.GetExecutingAssembly().Location);
        }

        #endregion Constructors

        #region IClientConnectionHelper Methods

        public void AddSettings(in TabPage tabPage, in ServiceSettings serverSettings)
        {
            if (tabPage == null)
                throw new ArgumentNullException(nameof(tabPage));

            if (serverSettings == null)
                throw new ArgumentNullException(nameof(serverSettings));

            TreeViewEx treeViewEx;

            if (tabPage.Controls.Count == 0 || tabPage.Tag == null)
            {
                treeViewEx = new TreeViewEx();
                treeViewEx.BeforeSelect += settingsTreeView_BeforeSelect;
                treeViewEx.AfterSelect += settingsTreeView_AfterSelect;
                treeViewEx.Parent = tabPage;
                treeViewEx.Left = 3;
                treeViewEx.Top = 3;
                treeViewEx.Width = 150;
                treeViewEx.Height = tabPage.Height - 6;
                treeViewEx.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Top;
                tabPage.Tag = treeViewEx;

                FlowLayoutPanel flowLayoutPanel = new FlowLayoutPanel();
                flowLayoutPanel.Parent = tabPage;
                flowLayoutPanel.Left = treeViewEx.Width + 6;
                flowLayoutPanel.Top = 3;
                flowLayoutPanel.Width = tabPage.Width - (flowLayoutPanel.Left + 3);
                flowLayoutPanel.Height = tabPage.Height - 32;
                flowLayoutPanel.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;
                //flowLayoutPanel.FlowDirection = FlowDirection.LeftToRight;
                //flowLayoutPanel.HorizontalScroll.Enabled = false;
                //flowLayoutPanel.HorizontalScroll.Visible = false;
                flowLayoutPanel.AutoScroll = true;
                //flowLayoutPanel.WrapContents = true;
                flowLayoutPanel.Padding = new Padding(0, 0, 10, 0);
                flowLayoutPanel.ClientSizeChanged += FlowLayoutPanel_ClientSizeChanged;

                treeViewEx.Tag = flowLayoutPanel;

                SaveButton button = new SaveButton();
                button.FlowLayoutPanel = flowLayoutPanel;
                button.ServerSettings = serverSettings;
                button.Parent = tabPage;
                button.Text = "Save";
                Size size = Shared.Utilities.MeasureText(button.Text, button.Font);
                button.Height = Shared.Utilities.MinimumValue(23, size.Height + 4);
                button.Width = Shared.Utilities.MinimumValue(72, size.Width + 10);
                button.Left = tabPage.Width - (button.Width + 3);
                button.Top = flowLayoutPanel.Height + 6;
                button.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
                button.Tag = serverSettings;
                button.Click += SaveSettingsButton_Click;
            }

            treeViewEx = (TreeViewEx)tabPage.Tag;
            TreeNode treeNode = new TreeNode(serverSettings.Name);
            treeNode.Tag = serverSettings;
            treeViewEx.Nodes.Add(treeNode);
            treeViewEx.SelectedNode = treeViewEx.Nodes[0];
        }

        public void AddTextBox(in TabPage tabPage, in ServerTextBox serverTextBox)
        {
            if (tabPage == null)
                throw new ArgumentNullException(nameof(tabPage));

            if (serverTextBox == null)
                throw new ArgumentNullException(nameof(serverTextBox));

            TextBoxEx textBox = new TextBoxEx();
            textBox.Tag = serverTextBox;

            textBox.Parent = tabPage;
            textBox.Left = serverTextBox.Left;
            textBox.Top = serverTextBox.Top;
            textBox.Width = serverTextBox.Width;
            textBox.Height = serverTextBox.Height;

            if (!String.IsNullOrEmpty(serverTextBox.ButtonText))
            {
                Button button = new Button();
                button.Tag = textBox;

                Size size = Shared.Utilities.MeasureText(serverTextBox.ButtonText, button.Font);
                button.Parent = tabPage;
                button.Text = serverTextBox.ButtonText;
                button.Left = textBox.Left + textBox.Width + 5;
                button.Top = textBox.Top;
                button.Width = size.Width + 10;
                button.Height = Shared.Utilities.MinimumValue(23, size.Height + 4);
                button.Text = serverTextBox.ButtonText;

                button.Click += ClientTextBoxButtonClick;
            }
        }

        public void AddButton(in TabPage tabPage, in ServerButton serverButton)
        {
            if (tabPage == null)
                throw new ArgumentNullException(nameof(tabPage));

            if (serverButton == null)
                throw new ArgumentNullException(nameof(serverButton));

            Button button = new Button();
            button.Tag = serverButton;

            button.Parent = tabPage;

            button.Left = serverButton.Left;
            button.Top = serverButton.Top;
            button.Width = serverButton.Width == 0 ? 75 : serverButton.Width;
            button.Height = serverButton.Height == 0 ? 23 : serverButton.Height;
            button.Text = serverButton.Text;

            button.Click += ClientButtonClick;
        }

        public void AddListView(in string Name, in TabPage tabPage, in ServerListView serverListView)
        {
            if (tabPage == null)
                throw new ArgumentNullException(nameof(tabPage));

            if (serverListView == null)
                throw new ArgumentNullException(nameof(serverListView));

            if (_clientListViews.ContainsKey(serverListView.MessageName))
                throw new ArgumentException("Listview already exists");

            SharedControls.Classes.ListViewEx listView = new SharedControls.Classes.ListViewEx();

            listView.Top = serverListView.AutoSize ? 3 : serverListView.Top;
            listView.Left = serverListView.AutoSize ? 3 : serverListView.Left;
            listView.Width = serverListView.AutoSize ? tabPage.Width - 6 : serverListView.Width;
            listView.Height = serverListView.AutoSize ? tabPage.Height - 6 : serverListView.Height;

            if (serverListView.AutoSize)
            {
                listView.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;
            }
            else
            {
                listView.Anchor = serverListView.Anchor;
            }

            listView.BorderStyle = BorderStyle.Fixed3D;
            listView.View = View.Details;
            listView.Tag = serverListView;
            listView.Name = Name;

            foreach (string colName in serverListView.ColumnNames)
                listView.Columns.Add(colName, 120);

            tabPage.Controls.Add(listView);
            _clientListViews.Add(serverListView.MessageName, listView);
        }

        #endregion IClientConnectionHelper Methods

        #region Private Methods

        #region Message Client

        private void MessageClient_ClientLogin(object sender, ClientLoginArgs e)
        {
            MessageClient client = (MessageClient)sender;
            Connection connection = GetServerConnection(client.Server);

            if (connection != null)
            {
                e.Username = connection.ServerConnection.ServerUsername;
                e.Password = connection.ServerConnection.ServerPassword;
            }
        }

        private void MessageClient_ClientLoginSuccess(object sender, EventArgs e)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new EventHandler(MessageClient_ClientLoginSuccess), new object[] { sender, EventArgs.Empty });
            }
            else
            {
                string serverName = ((MessageClient)sender).Server;
                Connection connection = GetServerConnection(serverName);

                connection.Client.SendMessage(new Shared.Communication.Message("CONNECTED", String.Empty, MessageType.Command));
                connection.Client.SendMessage(Shared.Communication.Message.Command(StringConstants.LOAD_CLIENT_DETAILS));

                if (!tabMain.TabPages.Contains(tabPageThreads))
                    tabMain.TabPages.Add(tabPageThreads);
            }
        }

        private void MessageClient_ClientLoginFailed(object sender, EventArgs e)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new EventHandler(MessageClient_ClientLoginFailed), new object[] { sender, EventArgs.Empty });
            }
            else
            {
                ShowError("Login Failed", "Failed to login to remote service");
            }
        }

        private void MessageClient_OnError(object sender, Shared.Communication.ErrorEventArgs e)
        {
#if DEBUG
            Shared.EventLog.Debug("SMC " +System.Reflection.MethodBase.GetCurrentMethod().Name);
            Shared.EventLog.Debug(e.Error.Message);
#endif
            Shared.EventLog.Add(e.Error, "Console");

            e.Continue = true;
            try
            {
                if (this.Disposing)
                    return;

                if (this.InvokeRequired)
                {
                    Shared.Communication.ErrorEventHandler mreh = new Shared.Communication.ErrorEventHandler(MessageClient_OnError);
                    this.Invoke(mreh, new object[] { sender, e });
                }
                else
                {

                }
            }
            catch (Exception err)
            {
                Shared.EventLog.Add(err);
            }
        }

        private void MessageClient_ClientIDChanged(object sender, EventArgs e)
        {
#if DEBUG
            Shared.EventLog.Debug("SMC " +System.Reflection.MethodBase.GetCurrentMethod().Name);
#endif
            if (this.InvokeRequired)
            {
                this.Invoke(new EventHandler(MessageClient_ClientIDChanged), new object[] { sender, EventArgs.Empty });
            }
            else
            {
                string serverName = ((MessageClient)sender).Server;
                Connection connection = GetServerConnection(serverName);

                connection.Client.SendMessage(new Shared.Communication.Message("CONNECTED", String.Empty, MessageType.Command));
                //connection.Client.SendMessage(Shared.Communication.Message.Command(StringConstants.LOAD_CLIENT_DETAILS));

                //if (!tabMain.TabPages.Contains(tabPageThreads))
                //    tabMain.TabPages.Add(tabPageThreads);
            }
        }

        private void MessageClient_MessageReceived(object sender, Shared.Communication.Message message)
        {
#if DEBUG
            Shared.EventLog.Debug("SMC " + System.Reflection.MethodBase.GetCurrentMethod().Name);

            string msgAsString = new string(Shared.Communication.Message.MessageToStringArray(message));
            Shared.EventLog.Debug("SMC " + msgAsString);
#endif
            TabPage tabPage;

            try
            {
                if (this.InvokeRequired)
                {
                    MessageReceivedEventHandler mreh = new MessageReceivedEventHandler(MessageClient_MessageReceived);
                    this.Invoke(mreh, new object[] { sender, message });
                }
                else
                {
                    MessageClient client = (MessageClient)sender;
                    Connection connection = GetServerConnection(client.Server);

                    switch (message.Title)
                    {
                        case StringConstants.SERVER_DATA_DYNAMIC_NEW:
                            DynamicServerDetails dynamicServerDetail = JsonConvert.DeserializeObject<DynamicServerDetails>(message.Contents);
                            UpdateDynamicServiceDetails(client.Server, dynamicServerDetail);

                            return;

                        case StringConstants.SERVER_DATA_DYNAMIC:
                            List<DynamicServerDetails> dynamicServerDetails = JsonConvert.DeserializeObject<List<DynamicServerDetails>>(message.Contents);

                            foreach (DynamicServerDetails serverDetails in dynamicServerDetails)
                                UpdateDynamicServiceDetails(client.Server, serverDetails);

                            return;

                        case StringConstants.SERVER_DATA_STATIC:
                            StaticServerDetails staticServerDetails = JsonConvert.DeserializeObject<StaticServerDetails>(message.Contents);
                            UpdateStaticServiceDetails(client.Server, staticServerDetails);

                            return;

                        case StringConstants.MESSAGE_QUESTION:

                            return;

                        case StringConstants.MESSAGE_INFORMATION:
                            //ShowInformation("")
                            return;

                        case StringConstants.MESSAGE_ERROR:
                            ShowError("Error", message.Contents);

                            return;

                        case StringConstants.THREAD_USAGE:
                            UpdateThreadData(client.Server, message.Contents);

                            foreach (KeyValuePair<string, SharedControls.Classes.ListViewEx> kvp in _clientListViews)
                            {
                                ServerListView serverListView = (ServerListView)kvp.Value.Tag;

                                if (serverListView.MessageName == $"{client.Server} {StringConstants.THREAD_USAGE}")
                                {
                                    // update the list view
                                    UpdateThreadData(kvp.Value, message.Contents);
                                    return;
                                }
                            }

                            return;

                        case StringConstants.THREAD_CPU_CHANGED:
                            return;

                        case StringConstants.LOAD_CLIENT_DETAILS_END:
                            
                            tabMain.TabPages.Insert(0, connection.PrimaryTab);
                            message.Title = StringConstants.SERVER_DATA_STATIC;
                            message.Contents = String.Empty;
                            SendMessage(message);

                            message.Title = StringConstants.SERVER_DATA_DYNAMIC;
                            SendMessage(message);

                            return;

                        case StringConstants.CREATE_SETTINGS:
                            tabPage = GetServiceTab(client.Server, StringConstants.Settings);
                            ServiceSettings serverSettings = ServerBaseControl.ConvertFromJson<ServiceSettings>(message.Contents);
                            AddSettings(tabPage, serverSettings);
                            return;

                        case StringConstants.CREATE_LISTVIEW:
                            ServerListView listView = ServerBaseControl.ConvertFromJson<ServerListView>(message.Contents);
                            tabPage = GetServiceTab(client.Server, listView.Parent);
                            AddListView($"{connection.SafeServerName}_{listView.MessageName}", tabPage, listView);
                            return;

                        case StringConstants.CREATE_BUTTON:
                            ServerButton serverButton = ServerBaseControl.ConvertFromJson<ServerButton>(message.Contents);
                            tabPage = GetServiceTab(client.Server, serverButton.Parent);
                            AddButton(tabPage, serverButton);
                            return;

                        case StringConstants.CREATE_CHECKBOX:
                        case StringConstants.CREATE_COMBOBOX:

                            return;

                        case StringConstants.CREATE_TEXTBOX:
                            ServerTextBox serverTextBox = ServerBaseControl.ConvertFromJson<ServerTextBox>(message.Contents);
                            tabPage = GetServiceTab(client.Server, serverTextBox.Parent);
                            AddTextBox(tabPage, serverTextBox);

                            return;

                        default:
                            // is it a list view?
                            foreach (KeyValuePair<string, SharedControls.Classes.ListViewEx> kvp in _clientListViews)
                            {
                                ServerListView serverListView = (ServerListView)kvp.Value.Tag;

                                if (serverListView.MessageName == message.Title)
                                {
                                    // update the list view
                                    UpdateListItem(serverListView, kvp.Value, message.Contents);
                                    return;
                                }
                            }

                            return;
                    }
                }
            }
            catch (ObjectDisposedException)
            {
                //ignore
            }
            catch (Exception err)
            {
                
            }
        }

        private void MessageClient_Disconnected(object sender, EventArgs e)
        {
#if DEBUG
            Shared.EventLog.Debug("SMC " + System.Reflection.MethodBase.GetCurrentMethod().Name);
#endif
            try
            {
                if (this.InvokeRequired)
                {
                    this.Invoke(new EventHandler(MessageClient_Disconnected),new object[] {sender, EventArgs.Empty});
                }
                else
                {
                    tsCPU.Text = String.Empty;

                    MessageClient messageClient = (MessageClient)sender;
                    Connection connection = GetServerConnection(messageClient.Server);
                    DisconnectFromServer(connection.ServerConnection);

                    if (tabMain.TabPages.Count == 1)
                        tabMain.TabPages.Remove(tabPageThreads);

                }
            }
            catch (Exception err)
            {

            }
        }

        private void MessageClient_Connected(object sender, EventArgs e)
        {
#if DEBUG
            Shared.EventLog.Debug("SMC " + System.Reflection.MethodBase.GetCurrentMethod().Name);
#endif
            if (this.InvokeRequired)
            {
                this.Invoke(new EventHandler(MessageClient_Connected), new object[] { sender, EventArgs.Empty });
            }
            else
            {

            }
        }

        #endregion Message Client

        private void ServerManagerClient_FormClosing(object sender, FormClosingEventArgs e)
        {
#if DEBUG
            Shared.EventLog.Debug("SMC " + System.Reflection.MethodBase.GetCurrentMethod().Name);
#endif
            Shared.Communication.Message msg = new Shared.Communication.Message("ALLOWCONFIRMCOUNTS", Convert.ToString(true), MessageType.Command);
            SendMessage(msg);

            CloseAllConnections();
        }

        private void UpdateThreadData(SharedControls.Classes.ListViewEx listView, string rawData)
        {
            string[] threads = rawData.Split('\r');
            listView.BeginUpdate();
            try
            {
                for (int i = 0; i < listView.Items.Count; i++)
                {
                    listView.Items[i].Tag = 1;
                }

                foreach (string thread in threads)
                {
                    if (String.IsNullOrEmpty(thread))
                        continue;

                    string[] threadParts = thread.Split(';');

                    ListViewItem threadItem = FindThreadListViewItem(listView, SplitText(threadParts[1], ':'));
                    threadItem.Tag = 0;

                    string cpu = SplitText(threadParts[0], ':');
                    threadItem.SubItems[1].Text = cpu.Substring(0, cpu.IndexOf("/"));
                    threadItem.SubItems[2].Text = cpu.Substring(cpu.IndexOf("/") + 1);
                    threadItem.SubItems[3].Text = SplitText(threadParts[2], ':');
                    threadItem.SubItems[4].Text = SplitText(threadParts[3], ':');
                    threadItem.SubItems[5].Text = SplitText(threadParts[4], ':');
                    threadItem.SubItems[6].Text = SplitText(threadParts[5], ':');
                }

                for (int i = listView.Items.Count - 1; i > 0; i--)
                {
                    if (listView.Items[i].Tag == null || (int)listView.Items[i].Tag == 1)
                    {
                        listView.Items.RemoveAt(i);
                    }
                }
            }
            finally
            {
                listView.EndUpdate();
            }

        }

        private ListViewItem FindThreadListViewItem(SharedControls.Classes.ListViewEx listView, in string name)
        {
            if (listView == null)
                throw new ArgumentNullException(nameof(listView));

            foreach (ListViewItem item in listView.Items)
            {
                if (item.SubItems[1].Text == name)
                    return (item);
            }

            ListViewItem Result = new ListViewItem(name);
            Result.SubItems.Add(String.Empty);
            Result.SubItems.Add(String.Empty);
            Result.SubItems.Add(String.Empty);
            Result.SubItems.Add(String.Empty);
            Result.SubItems.Add(String.Empty);
            Result.SubItems.Add(String.Empty);
            listView.Items.Add(Result);

            return (Result);
        }

        private void UpdateThreadData(string serverName, string rawData)
        {
            string[] threads = rawData.Split('\r');
            lvThreads.BeginUpdate();
            try
            {
                for (int i = 0; i < lvThreads.Items.Count; i++)
                {
                    if (lvThreads.Items[0].Text == serverName)
                        lvThreads.Items[i].Tag = 1;
                }

                foreach (string thread in threads)
                {
                    if (String.IsNullOrEmpty(thread))
                        continue;

                    string[] threadParts = thread.Split(';');

                    ListViewItem threadItem = FindThreadListViewItem(serverName, SplitText(threadParts[1], ':'));
                    threadItem.Tag = 0;

                    string cpu = SplitText(threadParts[0], ':');
                    threadItem.SubItems[2].Text = cpu.Substring(0, cpu.IndexOf("/"));
                    threadItem.SubItems[3].Text = cpu.Substring(cpu.IndexOf("/") + 1);
                    threadItem.SubItems[4].Text = SplitText(threadParts[2], ':');
                    threadItem.SubItems[5].Text = SplitText(threadParts[3], ':');
                    threadItem.SubItems[6].Text = SplitText(threadParts[4], ':');
                    threadItem.SubItems[7].Text = SplitText(threadParts[5], ':');
                }

                for (int i = lvThreads.Items.Count - 1; i > 0; i--)
                {
                    if (lvThreads.Items[i].Tag == null || (int)lvThreads.Items[i].Tag == 1)
                    {
                        lvThreads.Items.RemoveAt(i);
                    }
                }
            }
            finally
            {
                lvThreads.EndUpdate();
            }

        }

        private ListViewItem FindThreadListViewItem(in string server, in string name)
        {
            foreach (ListViewItem item in lvThreads.Items)
            {
                if (item.Text == server && item.SubItems[1].Text == name)
                    return (item);
            }

            ListViewItem Result = new ListViewItem(server);
            Result.SubItems.Add(name);
            Result.SubItems.Add(String.Empty);
            Result.SubItems.Add(String.Empty);
            Result.SubItems.Add(String.Empty);
            Result.SubItems.Add(String.Empty);
            Result.SubItems.Add(String.Empty);
            Result.SubItems.Add(String.Empty);
            lvThreads.Items.Add(Result);

            return (Result);
        }

        private string SplitText(string text, char splitText)
        {
            if (text.Contains(splitText.ToString()))
            {
                string result = text.Substring(text.IndexOf(splitText) + 1);
                return (result.Trim());
            }
            else
                return (text);
        }

        private void contextMenuThreadsRefresh_Click(object sender, EventArgs e)
        {
#if DEBUG
            Shared.EventLog.Debug("SMC " + System.Reflection.MethodBase.GetCurrentMethod().Name);
#endif
            SendMessage(new Shared.Communication.Message("THREAD_USAGE", "", MessageType.Command));
        }

        private void ServiceClient_Load(object sender, EventArgs e)
        {
            // load any previously opened servers

            string[] recentFiles = System.IO.Directory.GetFiles(Shared.Utilities.CurrentPath(), "Recent*.rec");

            foreach (string file in recentFiles)
            {
                try
                {
                    ServerConnection serverConnection = Newtonsoft.Json.JsonConvert.DeserializeObject<ServerConnection>(
                        Shared.Utilities.FileRead(file, true));
                    ToolStripMenuItem recentServer = new ToolStripMenuItem(serverConnection.ServerName);
                    menuServerRecent.DropDownItems.Add(recentServer);
                    recentServer.Click += RecentServer_Click;
                    recentServer.Tag = serverConnection;

                    if (!menuServerRecentSeperator.Visible)
                        menuServerRecentSeperator.Visible = true;
                }
                catch
                {
                    System.IO.File.Delete(file);
                }
            }
        }

        #region Build Client Interface Methods

        private TabPage GetServiceTab(in string server, string parentName)
        {
            if (String.IsNullOrEmpty(server))
                throw new ArgumentNullException(nameof(server));

            if (String.IsNullOrEmpty(parentName))
                parentName = "General";

            Connection connection = GetServerConnection(server);

            if (connection == null)
                throw new ArgumentNullException(nameof(server));

            if (connection.PrimaryTab.HasChildren && connection.PrimaryTab.Controls[0] is TabControl)
            {
                TabControl tabControl = connection.PrimaryTab.Controls[0] as TabControl;

                foreach (TabPage page in tabControl.TabPages)
                {
                    if (page.Text == parentName)
                        return (page);
                }

                // if we get here it is a new tab
                TabPage tabPage = new TabPage(parentName);
                tabControl.TabPages.Add(tabPage);
                return (tabPage);
            }

            // should NEVER get here
            throw new InvalidProgramException();
        }

        private void UpdateHeartbeat(in Control control, in int value)
        {
            if (control == null)
                return;

            if (control is HeartbeatPanel)
            {
                HeartbeatPanel panel = (HeartbeatPanel)control;
                panel.AddPoint(value);
            }
        }

        private void UpdateControlText(in Control control, in string text)
        {
            if (control == null)
                return;

            control.Text = text;
        }

        private void UpdateStaticServiceDetails(in string server, in StaticServerDetails staticServerDetails)
        {
            if (staticServerDetails == null)
                throw new ArgumentNullException(nameof(staticServerDetails));

            Connection connection = GetServerConnection(server);

            UpdateControlText(connection.ChildControls[$"lblProcessorType{connection.SafeServerName}"],
                staticServerDetails.ProcessorName);

            UpdateControlText(connection.ChildControls[$"lblProcessorSpeed{connection.SafeServerName}"],
                $"Base Speed: {staticServerDetails.ProcessorBaseSpeed} Ghz");

            UpdateControlText(connection.ChildControls[$"lblProcessorCores{connection.SafeServerName}"],
                $"Cores: {staticServerDetails.ProcessorCoreCount}");

            UpdateControlText(connection.ChildControls[$"lblProcessorProcessors{connection.SafeServerName}"],
                $"Logical Processors: {staticServerDetails.ProcessorLogicalProcessors}");

            UpdateControlText(connection.ChildControls[$"lblProcessorVirtualization{connection.SafeServerName}"],
                $"Virtualization: {staticServerDetails.ProcessVirtualization}");


            UpdateControlText(connection.ChildControls[$"lblOSType{connection.SafeServerName}"],
                $"Platform: {staticServerDetails.Platform}");

            UpdateControlText(connection.ChildControls[$"lblOSName{connection.SafeServerName}"],
                String.Format("Version: {0} {1}", staticServerDetails.Version,
                staticServerDetails.Is64BitOperatingSystem ? "64 Bit" : "32 Bit"));

            UpdateControlText(connection.ChildControls[$"lbl64BitOS{connection.SafeServerName}"],
                String.Format("Name: {0}", staticServerDetails.MachineName));

            UpdateControlText(connection.ChildControls[$"lblWinFolder{connection.SafeServerName}"],
                $"Windows Folder: {staticServerDetails.WindowsFolder}");

            UpdateControlText(connection.ChildControls[$"lblSysFolder{connection.SafeServerName}"],
                $"System Folder: {staticServerDetails.SystemDirectory}");
        }

        private void UpdateDynamicServiceDetails(in string server, in DynamicServerDetails dynamicServerDetails)
        {
            if (dynamicServerDetails == null)
                throw new ArgumentNullException(nameof(dynamicServerDetails));

            Connection connection = GetServerConnection(server);

            UpdateHeartbeat(connection.ChildControls[$"pnlHeartbeatMemory{connection.SafeServerName}"],
                (int)dynamicServerDetails.MemoryLoad);

            UpdateHeartbeat(connection.ChildControls[$"pnlHeartbeatPage{connection.SafeServerName}"],
                100 - dynamicServerDetails.PageFileLoad);

            UpdateHeartbeat(connection.ChildControls[$"pnlHeartbeatProcessor{connection.SafeServerName}"],
                dynamicServerDetails.CPUUsagePercentage);

            UpdateControlText(connection.ChildControls[$"lblServiceMemory{connection.SafeServerName}"],
                $"Managed Memory Usage: {dynamicServerDetails.ServiceManagedMemoryUsage}");

            UpdateControlText(connection.ChildControls[$"lblProcessorUtilizationValue{connection.SafeServerName}"],
                String.Format("{0}%", dynamicServerDetails.CPUUsagePercentage));

            UpdateControlText(connection.ChildControls[$"lblUpTimeValue{connection.SafeServerName}"],
                dynamicServerDetails.UpTime.ToString(@"dd\.hh\:mm\:ss"));

            UpdateControlText(connection.ChildControls[$"lblMemoryPhysTot{connection.SafeServerName}"],
                $"Total Memory: {dynamicServerDetails.PhysicalMemoryTotal} In Use: {dynamicServerDetails.PhysicalMemoryInUse}  ({dynamicServerDetails.MemoryLoad}%)");

            UpdateControlText(connection.ChildControls[$"lblMemoryPhysAvail{connection.SafeServerName}"],
                $"Available Memory: {dynamicServerDetails.PhysicalMemoryAvail} ({100 - dynamicServerDetails.MemoryLoad}%)");

            UpdateControlText(connection.ChildControls[$"lblMemoryPageTot{connection.SafeServerName}"],
                $"Total Page File Size: {dynamicServerDetails.PageFileTotal}");

            UpdateControlText(connection.ChildControls[$"lblMemoryPageAvail{connection.SafeServerName}"],
                $"Available Page File Size: {dynamicServerDetails.PageFileAvail} ({dynamicServerDetails.PageFileLoad}%)");

            UpdateControlText(connection.ChildControls[$"lblMemoryVirtTot{connection.SafeServerName}"],
                $"Total Virtual Memory: {dynamicServerDetails.VirtualMemoryTotal}");

            UpdateControlText(connection.ChildControls[$"lblMemoryVirtAvail{connection.SafeServerName}"],
                $"Available Virtual Memory {dynamicServerDetails.VirtualMemoryAvail}");

            UpdateControlText(connection.ChildControls[$"lblMemoryVirtExtended{connection.SafeServerName}"],
                $"Extended Virtual Memory: {dynamicServerDetails.VirtualMemoryAvailExtended}");

            if (!connection.DriveStatsLoaded)
            {
                connection.LoadDriveInformation(dynamicServerDetails.DiskDriveStats);
                UpdateTooltips(this);
            }

            // check to make sure disks have been added already
            if (!connection.DiskStatsLoaded)
            {
                connection.LoadDiskInformation(dynamicServerDetails.DiskStats);
                UpdateTooltips(this);
            }

            // make sure network stats loaded
            if (!connection.NetworkStatsLoaded)
            {
                connection.LoadNetworkInformation(dynamicServerDetails.NetworkStats);
                UpdateTooltips(this);
            }

            foreach (System.IO.DriveInfo statistics in dynamicServerDetails.DiskDriveStats)
            {
                string safeName = $"{connection.SafeName(statistics.Name)}_{connection.SafeServerName}_{connection.SafeName(statistics.VolumeLabel.Replace(" ", ""))}";

                UpdateControlText(connection.ChildControls[$"lblDrive{safeName}FreeSpace"], 
                    $"Free Space: {Shared.Utilities.FileSize(statistics.AvailableFreeSpace, 2)}");
                UpdateControlText(connection.ChildControls[$"lblDrive{safeName}UsedSpace"], 
                    $"Used Space: {Shared.Utilities.FileSize(statistics.TotalSize - statistics.AvailableFreeSpace, 2)}");

                UpdateHeartbeat(connection.ChildControls[$"pnlHeartDrive{safeName}"], 
                    100 - Convert.ToInt32(Shared.Utilities.Percentage((double)statistics.TotalSize, (double)statistics.AvailableFreeSpace)));
            }

            foreach (NetworkStatistics statistics in dynamicServerDetails.NetworkStats)
            {
                string safeName = $"{connection.SafeName(statistics.Name)}_{connection.SafeServerName}";

                if (statistics.NetworkType == NetworkType.Adapter)
                    continue;

                UpdateControlText(connection.ChildControls[$"lblNetworkConnections{safeName}"], 
                    $"Active Connections: {statistics.TcpRscConnectionsActive}");
                UpdateControlText(connection.ChildControls[$"lblNetworkBytesReceived{safeName}"],
                    $"Bytes Received: {statistics.BytesPerSecondReceived}");
                UpdateControlText(connection.ChildControls[$"lblNetworkBytesSent{safeName}"], 
                    $"Bytes Sent: {statistics.BytesPerSecondSent}");
                UpdateControlText(connection.ChildControls[$"lblNetworkPacketsReceived{safeName}"],
                    $"Packets Received: {statistics.PacketsPerSecondReceived}");
                UpdateControlText(connection.ChildControls[$"lblNetworkPacketsSent{safeName}"],
                    $"PacketsSent: {statistics.PacketsPerSecondSent}");
                UpdateControlText(connection.ChildControls[$"lblNetworkQueueLength{safeName}"],
                    $"Queue Length: {statistics.OutputQueueLength}");
                UpdateControlText(connection.ChildControls[$"lblNetworkErrors{safeName}"],
                    $"Packet Errors; In: {statistics.PacketsReceivedErrors}/Out: {statistics.PacketsOutboundErrors}");

                UpdateHeartbeat(connection.ChildControls[$"pnlHeartNetworkConnections{safeName}"],
                    Convert.ToInt32(statistics.TcpRscConnectionsActive));
                UpdateHeartbeat(connection.ChildControls[$"pnlHeartNetworkBytesReceived{safeName}"],
                    Convert.ToInt32((float)statistics.BytesPerSecondReceived / 0.008f));
                UpdateHeartbeat(connection.ChildControls[$"pnlHeartNetworkBytesSent{safeName}"],
                    Convert.ToInt32((float)statistics.BytesPerSecondSent / 0.008f));
            }


            foreach (DiskStatistics statistics in dynamicServerDetails.DiskStats)
            {
                string safeName = $"{connection.SafeName(statistics.DiskName)}_{connection.SafeServerName}";

                UpdateControlText(connection.ChildControls[$"lblDiskLength{safeName}"], 
                    $"Average Queue Length: {statistics.AverageDiskQueueLength}");
                UpdateControlText(connection.ChildControls[$"lblDiskReads{safeName}"],
                    $"Reads per Second: {statistics.DiskReadPerSecond} ({ConvertSize(statistics.DiskReadPerSecond)})");
                UpdateControlText(connection.ChildControls[$"lblDiskWrites{safeName}"],
                    $"Writes per Second: {statistics.DiskWritePerSecond} ({ConvertSize(statistics.DiskWritePerSecond)})");
                UpdateControlText(connection.ChildControls[$"lblDiskTransfer{safeName}"],
                    $"Transfers per Second: {statistics.DiskTransferPerSecond}");
                UpdateControlText(connection.ChildControls[$"lblDiskFreeSpace{safeName}"],
                    $"Free Space: {statistics.FreeDiskSpace}");
                UpdateControlText(connection.ChildControls[$"lblDiskIdleTime{safeName}"],
                    $"Active Time: {100 - statistics.IdleTime}%");
                UpdateControlText(connection.ChildControls[$"lblDiskPercentTime{safeName}"],
                    $"Percent In Use: {statistics.PercentDiskTime}");

                UpdateHeartbeat(connection.ChildControls[$"pnlHeartDiskUsage{safeName}"], 
                    Convert.ToInt32(statistics.PercentDiskTime));
                UpdateHeartbeat(connection.ChildControls[$"pnlHeartReads{safeName}"],
                    Convert.ToInt32(statistics.DiskReadPerSecond));
                UpdateHeartbeat(connection.ChildControls[$"pnlHeartWrites{safeName}"],
                    Convert.ToInt32(statistics.DiskWritePerSecond));
            }
        }

        private string ConvertSize(ulong size)
        {
            return (Shared.Utilities.FileSize((long)size, 1).ToLower() + "/s");
            //return (((double)size / 1024).ToString("N1") + "kb/s");
        }


        #region List Views

        private ListViewItem FindListViewItem(SharedControls.Classes.ListViewEx listView, in string name)
        {
            foreach (ListViewItem item in listView.Items)
            {
                if (item.Text == name)
                    return (item);
            }

            ListViewItem Result = new ListViewItem(name);

            foreach (ColumnHeader column in listView.Columns)
                Result.SubItems.Add(String.Empty);

            listView.Items.Add(Result);

            return (Result);
        }

        private void UpdateListItem(ServerListView serverListView, SharedControls.Classes.ListViewEx listView, string rawData)
        {
            if (serverListView == null)
                throw new ArgumentNullException(nameof(serverListView));

            if (listView == null)
                throw new ArgumentNullException(nameof(listView));

            string[] rows = rawData.Split('\r');
            listView.BeginUpdate();
            try
            {
                for (int i = 0; i < listView.Items.Count; i++)
                {
                    listView.Items[i].Tag = 1;
                }

                foreach (string row in rows)
                {
                    if (String.IsNullOrEmpty(row))
                        continue;

                    string[] rowParts = row.Split(serverListView.ItemSeperator);

                    ListViewItem item = FindListViewItem(listView, SplitText(rowParts[0], ':'));
                    item.Tag = 0;

                    for (int i = 0; i < rowParts.Length; i++)
                        item.SubItems[i].Text = rowParts[i];
                }

                for (int i = listView.Items.Count - 1; i > 0; i--)
                {
                    if (listView.Items[i].Tag == null || (int)listView.Items[i].Tag == 1)
                    {
                        listView.Items.RemoveAt(i);
                    }
                }
            }
            finally
            {
                listView.EndUpdate();
            }
        }

        #endregion List Views

        #region Client TextBox

        private void ClientTextBoxButtonClick(object sender, EventArgs e)
        {
            if (sender != null && sender is Button && ((Button)sender).Tag != null)
            {
                TextBoxEx textBox = (TextBoxEx)((Button)sender).Tag;
                ServerTextBox serverTextBox = (ServerTextBox)textBox.Tag;

                if (!String.IsNullOrEmpty(serverTextBox.MessageName))
                {
                    // show a message on behalf of the button

                    SendMessage(Shared.Communication.Message.Command(serverTextBox.MessageName, textBox.Text));
                }
            }
        }

        #endregion Client TextBox

        #region Client Buttons

        private void ClientButtonClick(object sender, EventArgs e)
        {
            if (sender != null && sender is Button && ((Button)sender).Tag != null)
            {
                ServerButton serverButton = (ServerButton)((Button)sender).Tag;
                
                if (!String.IsNullOrEmpty(serverButton.MessageText))
                {
                    // show a message on behalf of the button
                    switch (serverButton.MessageButtons)
                    {
                        case MessageBoxButtons.OKCancel:
                        case MessageBoxButtons.YesNo:
                            if (!ShowQuestion(serverButton.MessageTitle, serverButton.MessageText, true))
                                return;

                            break;

                        default:
                            // just an ok dialog
                            ShowInformation(serverButton.MessageTitle, serverButton.MessageText);

                            break;
                    }

                    SendMessage(Shared.Communication.Message.Command(serverButton.MessageName));
                }
            }
        }

        #endregion Client Buttons

        #region Settings

        private void SaveSettingsButton_Click(object sender, EventArgs e)
        {
            SaveButton saveButton = (SaveButton)sender;

            foreach (ServiceSettingControl control in saveButton.FlowLayoutPanel.Controls)
            {
                ServiceSetting setting = saveButton.ServerSettings.Find(control.SettingName);

                if (setting == null)
                    continue;

                saveButton.ServerSettings.Find(control.SettingName).Value = control.SettingValue;
            }

            SendMessage(new Shared.Communication.Message(saveButton.ServerSettings.MessageName, 
                ServerBaseControl.ConvertToJson<ServiceSettings>(saveButton.ServerSettings), 
                MessageType.Command));
        }

        private void settingsTreeView_AfterSelect(object sender, TreeViewEventArgs e)
        {
            ServiceSettings serverSettings = (ServiceSettings)e.Node.Tag;

            FlowLayoutPanel layoutPanel = (FlowLayoutPanel)((TreeViewEx)sender).Tag;
            
            foreach (ServiceSetting setting in serverSettings.Settings)
            {
                ServiceSettingControl control = new ServiceSettingControl(setting);
                control.Width = this.ClientSize.Width;
                layoutPanel.Controls.Add(control);
            }

            foreach (Control control in layoutPanel.Parent.Controls)
            {
                if (control is SaveButton)
                {
                    ((SaveButton)control).ServerSettings = serverSettings;
                    break;
                }
            }

            FlowLayoutPanel_ClientSizeChanged(layoutPanel, EventArgs.Empty);
        }

        private void settingsTreeView_BeforeSelect(object sender, TreeViewCancelEventArgs e)
        {
            if (e.Node == null)
                return;

            FlowLayoutPanel layoutPanel = (FlowLayoutPanel)((TreeViewEx)sender).Tag;

            // get all settings



            layoutPanel.Controls.Clear();
        }

        private void FlowLayoutPanel_ClientSizeChanged(object sender, EventArgs e)
        {
            FlowLayoutPanel flowLayoutPanel = (FlowLayoutPanel)sender;

            int newWidth = flowLayoutPanel.ClientRectangle.Width - 15;

            if (flowLayoutPanel.VerticalScroll.Visible)
                newWidth -= 15;

            foreach (Control control in flowLayoutPanel.Controls)
                control.Width = newWidth;
        }

        #endregion Settings

        #endregion Build Client Interface Methods

        #region Connection Methods

        private void ConnectToServer(ServerConnection serverConnection)
        {
            if (serverConnection == null)
                throw new ArgumentNullException(nameof(serverConnection));

            Connection connection = null;

            using (MutexEx mutex = new MutexEx("ServiceManagerClientList", true))
            {
                mutex.CreateMutex();
                using (TimedLock.Lock(_lockObject))
                {
                    if (_clientConnections.ContainsKey(serverConnection.ServerName))
                    {
                        connection = GetServerConnection(serverConnection.ServerName);
                    }
                    else
                    {
                        connection = new Connection(this, serverConnection);
                        connection.Client.ClientLogin += MessageClient_ClientLogin;
                        connection.Client.ClientIDChanged += MessageClient_ClientIDChanged;
                        connection.Client.MessageReceived += MessageClient_MessageReceived;
                        connection.Client.Connected += MessageClient_Connected;
                        connection.Client.Disconnected += MessageClient_Disconnected;
                        connection.Client.OnError += MessageClient_OnError;
                        connection.Client.ClientLoginFailed += MessageClient_ClientLoginFailed;
                        connection.Client.ClientLoginSuccess += MessageClient_ClientLoginSuccess;

                        Shared.Utilities.FileWrite(Shared.Utilities.CurrentPath(true) + "Recent" + serverConnection.ServerName + ".rec",
                            Newtonsoft.Json.JsonConvert.SerializeObject(serverConnection));

                        _clientConnections.Add(serverConnection.ServerName, connection);

                        UpdateTooltips(connection.PrimaryTab);
                    }
                }
            }

            if (!connection.Client.IsRunning)
                connection.Client.StartListening();
        }

        private void UpdateTooltips(Control control)
        {
            if (control.Tag != null)
            {
                if (control.Tag is TooltipItem)
                {
                    control.MouseEnter += TooltipControl_MouseEnter;
                    control.MouseLeave += TooltipControl_MouseLeave;
                }
            }

            foreach (Control ctl in control.Controls)
                UpdateTooltips(ctl);
        }

        private void TooltipControl_MouseLeave(object sender, EventArgs e)
        {
            controlToolTip.SetToolTip((Control)sender, String.Empty);
        }

        private void TooltipControl_MouseEnter(object sender, EventArgs e)
        {
            if (sender is Control)
            {
                Control thisControl = (Control)sender;

                if (thisControl.Tag == null)
                    return;

                if (thisControl.Tag is TooltipItem)
                {
                    TooltipItem tooltipItem = (TooltipItem)thisControl.Tag;

                    controlToolTip.SetToolTip((Control)sender, tooltipItem.Text);
                }
            }
        }

        private void DisconnectFromServer(ServerConnection serverConnection)
        {
            if (serverConnection == null)
                throw new ArgumentNullException(nameof(serverConnection));

            Connection connection = GetServerConnection(serverConnection.ServerName);

            if (connection == null)
                throw new ArgumentException(nameof(ServerConnectForm));

            using (MutexEx mutex = new MutexEx("ServiceManagerClientList", true))
            {
                mutex.CreateMutex();

                using (Shared.Classes.TimedLock.Lock(_lockObject))
                {
                    _clientConnections.Remove(serverConnection.ServerName);
                }
            }

            if (connection.Client.IsRunning)
            {
                connection.Client.StopListening();
            }

            tabMain.TabPages.Remove(connection.PrimaryTab);

            if (tabMain.TabPages.Count == 1)
                tabMain.TabPages.Remove(tabPageThreads);

            connection.PrimaryTab.Dispose();

            foreach (KeyValuePair<string, Control> ctl in connection.ChildControls)
            {
                ctl.Value.Dispose();
            }

            List<string> removeable = new List<string>();

            foreach (KeyValuePair<string, SharedControls.Classes.ListViewEx> kvp in _clientListViews)
            {
                if (kvp.Key.StartsWith(connection.SafeServerName) ||
                    kvp.Value.Name.StartsWith(connection.SafeServerName))
                {
                    removeable.Add(kvp.Key);
                }
            }
            
            foreach (string keyName in removeable)
            {
                if (_clientListViews.ContainsKey(keyName))
                    _clientListViews.Remove(keyName);
            }
        }

        private Connection GetServerConnection(string serverName)
        {
            using (MutexEx mutex = new MutexEx("ServiceManagerClientList", true))
            {
                mutex.CreateMutex();

                using (Shared.Classes.TimedLock.Lock(_lockObject))
                {
                    if (String.IsNullOrEmpty(serverName))
                        throw new ArgumentNullException(nameof(serverName));

                    if (serverName == "Not Connected")
                    {
                        foreach (KeyValuePair<string, Connection> kvp in _clientConnections)
                        {
                            if (kvp.Value.Client.Server == serverName)
                                return (kvp.Value);
                        }
                    }
                    else
                    {
                        if (!_clientConnections.ContainsKey(serverName))
                            throw new Exception("Connection not found!");
                    }

                    return (_clientConnections[serverName]);
                }
            }
        }

        private void SendMessage(Shared.Communication.Message message)
        {
            using (Shared.Classes.TimedLock.Lock(_lockObject))
            {
                foreach (KeyValuePair<string, Connection> connection in _clientConnections)
                {
                    if (connection.Value.Client != null && connection.Value.Client.IsRunning)
                        connection.Value.Client.SendMessage(message);

                }
            }
        }

        private void CloseAllConnections()
        {
            using (MutexEx mutex = new MutexEx("ServiceManagerClientList", true))
            {
                mutex.CreateMutex();

                using (Shared.Classes.TimedLock.Lock(_lockObject))
                {
                    foreach (KeyValuePair<string, Connection> connection in _clientConnections)
                    {
                        if (connection.Value.Client != null && connection.Value.Client.IsRunning)
                            connection.Value.Client.StopListening();

                    }
                }
            }
        }

        #endregion Connection Methods

        #region Menu Items

        #region Server Menu

        private void menuServer_DropDownOpening(object sender, EventArgs e)
        {
            menuServerDisconnect.Enabled = tabMain.TabPages.Count > 0 && tabMain.SelectedTab.Tag != null;
        }

        private void menuServerConnect_Click(object sender, EventArgs e)
        {
            ServerConnection serverConnection = new ServerConnection();

            if (ServerConnectForm.GetServerConnection(ref serverConnection))
            {
                ConnectToServer(serverConnection);
            }
        }

        private void menuServerDisconnect_Click(object sender, EventArgs e)
        {
            if (tabMain.SelectedTab.Tag != null)
            {
                Connection connection = (Connection)tabMain.SelectedTab.Tag;
                DisconnectFromServer(connection.ServerConnection);
            }
        }

        private void menuServerRecent_DropDownOpening(object sender, EventArgs e)
        {
            menuServerRecentOpenAll.Enabled = menuServerRecent.DropDownItems.Count > 1;
        }

        private void RecentServer_Click(object sender, EventArgs e)
        {
            ServerConnection serverConnection = (ServerConnection)((ToolStripMenuItem)sender).Tag;
            ConnectToServer(serverConnection);
        }

        private void menuServerRecentOpenAll_Click(object sender, EventArgs e)
        {
            foreach (object item in menuServerRecent.DropDownItems)
            {
                if (item is ToolStripMenuItem)
                {
                    ToolStripMenuItem menu = (ToolStripMenuItem)item;

                    if (menu.Tag != null)
                        ConnectToServer((ServerConnection)menu.Tag);
                }
            }
        }

        private void menuServerClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        #endregion Server Menu

        #region Help Menu

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ServiceManager.Core.Forms.AboutForm.ShowAboutWindow(this);
        }

        #endregion Help Menu

        #endregion Menu Items

        #endregion Private Methods
    }
}


// replication code

//                    if (clientHeader[0] == "REPLICATION_RUNNING")
//                    {
//                        _replicationRunning = message.Contents.ToUpper() == "TRUE";

//                        if (_replicationRunning)
//                            tsLabelTimeTillRun.Text = "Replicating   ";

//addMessage = false;
//                    }
//                    else if (clientHeader[0] == "REPLICATION_ENABLED")
//                    {
//                        _replicationEnabled = message.Contents.ToUpper() == "TRUE";
//                        btnCancelReplication.Enabled = _replicationEnabled;
//                        btnForceReplication.Enabled = _replicationEnabled;
//                        btnPreventReplication.Enabled = _replicationEnabled;
//                        btnHardConfirm.Enabled = _replicationEnabled;

//                        if (!_replicationEnabled)
//                            tsLabelTimeTillRun.Text = "Replication Disabled";

//                        addMessage = false;
//                    }
//                    else if (clientHeader[0] == "FORCEHARDCOUNT")
//                    {
//                        addMessage = false;
//                    }
//                    else if (clientHeader[0] == "REPLICATION_RUNTIME")
//                    {
//                        Int64 time = Shared.Utilities.StrToInt64(message.Contents, 0);
//_replicationStartTime = DateTime.FromFileTimeUtc(time);
//                        addMessage = false;
//                    }
//                    if (message.Contents.StartsWith("#STATUS#"))
//                    {
//                        if (message.Contents.Contains(";"))
//                            tsStatus.Text = message.Contents.Substring(message.Contents.IndexOf(";") + 2);
//                        else
//                            tsStatus.Text = message.Contents.Replace("#STATUS#", "");
//                    }
//                    else if (message.Contents.StartsWith("#MISSING#"))
//                    {
//                        tsMissingCount.Text = String.Format("Missing Records: {0}   ", message.Contents.Replace("#MISSING#", ""));
//                    }
//                    else if (message.Contents.StartsWith("Sleeping, time until next run"))
//                    {
//                        if (cmbClient.SelectedIndex == 0)
//                        {
//                            tsLabelTimeTillRun.Text = String.Empty;
//                        }
//                        else if (_replicationEnabled)
//                        {
//                            tsLabelTimeTillRun.Text = String.Format("Next Run: {0}", message.Contents.Substring(30));
//                        }
//                        else
//                        {
//                            tsLabelTimeTillRun.Text = "Replication Disabled";
//                        }

//                        tsLabelTimeTillRun.Invalidate();
//                    }
//                    else if (message.Contents.StartsWith("Run Replication"))
//                    {
//                        _replicationRunning = true;
//                        _replicationStartTime = DateTime.Now;

//                        tsLabelTimeTillRun.Text = "Replicating   ";

//                        int idx = lstReplicationMessages.Items.Add(message.Contents);
//                        //lstReplicationMessages.SelectedIndex = idx;

//                        if (cbAutoScroll.Checked)
//                            lstReplicationMessages.SelectedIndex = idx;
//                    }
//                    else if (message.Contents.Contains("Replication End"))
//                    {
//                        _replicationRunning = false;
//                        tsLabelTimeTillRun.Text = "";
//                        tsMissingCount.Text = "";
//                        tsStatus.Text = "";

//                        int idx = lstReplicationMessages.Items.Add(message.Contents);
//                        //lstReplicationMessages.SelectedIndex = idx;

//                        if (cbAutoScroll.Checked)
//                            lstReplicationMessages.SelectedIndex = idx;
//                    }

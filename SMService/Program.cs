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
 *  File: Program.cs
 *
 *  Purpose:  
 *
 *  Date        Name                Reason
 *  21/04/2018  Simon Carter        Initially Created
 *
 * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */
using System;
using System.Globalization;
using System.Configuration.Install;
using System.Reflection;
using System.Windows.Forms;
using System.Threading;

using Shared;
using Shared.Classes;

using ServiceManager.Classes;
using ServiceManager.Core;

namespace ServiceManager
{
    class Program
    {
        #region Private Members

        private static PrimaryService INSTANCE;

        #endregion Private Members

        #region Internal Static Members

        internal static bool Closing;

        #endregion Internal Static Members

        static void Main(string[] args)
        {
            Closing = false;
            try
            {
                AppDomain.CurrentDomain.AssemblyResolve += CurrentDomainAssemblyResolve;
                Configuration.Initialize();
                Parameters.Initialise(args, new char[] { '-', '/' }, new char[] { ' ', ':' });

                Thread.CurrentThread.CurrentCulture = new CultureInfo("en-GB");
                ProcessParams.PreProcessAllParams();

                if (Environment.UserInteractive)
                {
                    ThreadManager.Initialise();
                    try
                    {
                        INSTANCE = new PrimaryService();

                        EventLog.Add("Initializing UserInteractive");

                        if (Parameters.OptionExists("i")) // install
                        {
                            try
                            {
                                EventLog.Add("Installing");
                                ManagedInstallerClass.InstallHelper(new string[] { Assembly.GetExecutingAssembly().Location });
                            }
                            catch (Exception err)
                            {
                                if (!err.Message.Contains("The installation failed, and the rollback has been performed"))
                                    throw;
                            }
                        }
                        else if (Parameters.OptionExists("u")) // uninstall
                        {
                            EventLog.Add("Uninstalling");
                            ManagedInstallerClass.InstallHelper(new string[] { "/u", Assembly.GetExecutingAssembly().Location });
                        }
                        else
                        {
                            EventLog.Add("Run as Application");
                            ThreadManager.ThreadForcedToClose += ThreadManager_ThreadForcedToClose;
                            INSTANCE.RunAsApplication();
                            Closing = true;
                        }
                    }
                    catch (Exception err)
                    {
                        MessageBox.Show(String.Format("{0}\r\n{1}", err.Message, err.StackTrace.ToString()));
                        EventLog.Add(err.Message);
                        EventLog.Add(err);
                    }
                    finally
                    {
                        ThreadManager.Finalise();
                    }

                }
                else
                {
                    System.ServiceProcess.ServiceBase[] ServicesToRun;
                    INSTANCE = new PrimaryService();
                    ServicesToRun = new System.ServiceProcess.ServiceBase[] { INSTANCE };
                    System.ServiceProcess.ServiceBase.Run(ServicesToRun);
                }
            }
            catch (Exception error)
            {
                EventLog.Add(error);
            }
        }

        private static Assembly CurrentDomainAssemblyResolve(object sender, ResolveEventArgs args)
        {
            // for system assemblies and other 3rd party files,
            // anything else can be ignored here
            AssemblyName assyName = new AssemblyName(args.Name);

            EventLog.Add($"Assembly not found: {args.Name}");
            string filename = args.Name.ToLower().Split(',')[0];

            string assembly = System.IO.Path.Combine(Utils.GetPath(PathType.System), filename);

            if (!assembly.EndsWith(".dll"))
                assembly += ".dll";

            try
            {
                if (System.IO.File.Exists(assembly))
                    return (Assembly.LoadFrom(assembly));

                // has a path been passed in on the parameters?
                if (Parameters.OptionExists(Constants.Parameters.Path))
                {
                    assembly = Shared.Utilities.AddTrailingBackSlash(
                        Parameters.GetOption(Constants.Parameters.Path, String.Empty));

                    assembly += filename;

                    if (System.IO.File.Exists(assembly))
                        return (Assembly.LoadFrom(assembly));
                }
            }
            catch (Exception error)
            {
                Shared.EventLog.Add(error);
            }

            return (null);
        }

        private static void ThreadManager_ThreadForcedToClose(object sender, Shared.ThreadManagerEventArgs e)
        {
            EventLog.Add(String.Format("Thread forced to close: {0}, Unresponsive: {1}, Marked For Removal: {2}",
                e.Thread.Name, e.Thread.UnResponsive.ToString(), e.Thread.MarkedForRemoval.ToString()));
            EventLog.Add(String.Format("Start Time: {0}", e.Thread.TimeStart.ToString("g")));
            EventLog.Add(String.Format("End Time: {0}", e.Thread.TimeFinish.ToString("g")));
        }
    }
}

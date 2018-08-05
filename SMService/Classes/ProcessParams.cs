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
 *  Product:  Server Manager
 *  
 *  File: ProcessParams.cs
 *
 *  Purpose:  
 *
 *  Date        Name                Reason
 *  05/08/2018  Simon Carter        Initially Created
 *
 * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

using Shared.Classes;

namespace ServiceManager.Classes
{
    internal static class ProcessParams
    {
        /// <summary>
        /// Parameters that are processed immediately after loading
        /// </summary>
        internal static void PreProcessAllParams()
        {
            // option to delay start, allow time for debugger to attach to process etc
            if (Parameters.OptionExists(Constants.Parameters.DelayStart))
            {
                uint delay = Parameters.GetOption(Constants.Parameters.DelayStart, Constants.Parameters.DelayStartDefault);

                TimeSpan spanDelay = new TimeSpan(0, 0, (int)delay / 1000);
                DateTime startDelayTime = DateTime.Now;

                Shared.EventLog.Add($"Delaying Application Start for {spanDelay.Seconds} seconds");

                while ((DateTime.Now - startDelayTime).TotalSeconds < spanDelay.TotalSeconds)
                    System.Threading.Thread.Sleep(50);
            }
        }

        /// <summary>
        /// Parameters that are processed after initialization and services loaded
        /// </summary>
        internal static void PostProcessAllParams(PluginManager pluginManager)
        {
            // are we loading a custom plugin?
            string pluginServiceFile = Parameters.GetOption(Constants.Parameters.PluginServiceFile, String.Empty);
            Shared.EventLog.Add($"Attempt to load Plugin: {pluginServiceFile}");

            if (File.Exists(pluginServiceFile))
                pluginManager.LoadService(pluginServiceFile);

        }
    }
}

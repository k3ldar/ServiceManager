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
 *  06/01/2018  Simon Carter        Initially Created
 *
 * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */
using System;
using System.Windows.Forms;


using Shared.Classes;

namespace ServiceAdminConsole
{
    class Program
    {
        //private static string CACHE_HEADER = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>\r\n" +
        //    "<configuration>\r\n" +
        //    "    <system.webServer>\r\n" +
        //    "        <httpProtocol>\r\n" +
        //    "            <customHeaders>\r\n" +
        //    "                <add name = \"Cache-Control\" value=\"max-age=8000\" />\r\n" +
        //    "            </customHeaders>\r\n" +
        //    "        </httpProtocol>\r\n" +
        //    "    </system.webServer>\r\n" +
        //    "</configuration>\r\n";

        static void Main(string[] args)
        {
            Parameters.Initialise(args, new char[] { '/', '-' }, new char[] { ':' }, false);

            Application.Run(new ServiceManagerClient());
        }
    }
}

﻿using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;

namespace AutoConnectWifi
{
    class Program
    {
        [DllImport("wininet.dll", EntryPoint = "InternetGetConnectedState")]

        //if network is okay, return true; otherwise return false;
        public extern static bool InternetGetConnectedState(out int conState, int reder);

        private const string argInterval = "--interval";
        private const string argWifi = "--wifi";
        private const string argNetworkInterface = "--networkInterface";

        static void Main(string[] args)
        {
            #region parse arguments
            int interval = 15;
            string wifiName = string.Empty;
            string networkInterfaceName = string.Empty;

            if (args != null && args.Length > 0)
            {
                for (int i = 0; i < args.Length; i++)
                {
                    if (args[i] == argInterval)
                    {
                        if (i + 1 < args.Length)
                        {
                            int.TryParse(args[i + 1], out interval);
                        }
                    }
                    else if (args[i] == argWifi)
                    {
                        if (i + 1 < args.Length)
                        {
                            wifiName = args[i + 1];
                        }
                    }
                    else if (args[i] == argNetworkInterface)
                    {
                        if (i + 1 < args.Length)
                        {
                            networkInterfaceName = args[i + 1];
                        }
                    }
                }
            }

            #endregion

            #region connect wifi in loop

            while (!string.IsNullOrWhiteSpace(wifiName)
                && !InternetGetConnectedState(out int connectState, 0))
            {
                Console.Write(DateTime.Now.ToString("HH:mm:ss"));

                Console.WriteLine($" connect wifi : {wifiName}");
                ConnectWifi(wifiName, networkInterfaceName);

                Thread.Sleep(1000 * interval);
            }
            #endregion
        }

        /// <summary>
        /// connect wifi
        /// </summary>
        /// <param name="wifiName"></param>
        private static void ConnectWifi(string wifiName, string networkInfaceName)
        {
            var strCmd = "/C ";
            if (!string.IsNullOrWhiteSpace(networkInfaceName))
            {
                strCmd += $" (netsh interface set interface name=\"{networkInfaceName}\" admin=disable) & (netsh interface set interface name=\"{networkInfaceName}\" admin=enable) & ";
            }

            if (!string.IsNullOrWhiteSpace(wifiName))
            {
                strCmd += $" netsh wlan connect {wifiName}";
            }

            Process cmd = new Process();
            cmd.StartInfo.FileName = "cmd.exe";
            cmd.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            cmd.StartInfo.Arguments = strCmd;
            cmd.Start();
        }
    }
}

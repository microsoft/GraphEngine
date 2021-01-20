// Graph Engine
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using System.Threading.Tasks;

using Trinity;
using Trinity.Diagnostics;
using System.Globalization;

namespace Trinity.Utilities
{
    #region Comparers
    /// <summary>
    /// Defines a method for comparing two <see cref="System.Net.IPAddress"/> objects.
    /// </summary>
    public class IPAddressComparer : Comparer<IPAddress>
    {
        private const Int32 V4Loopback = 16777343;
        private const Int64 V6LoopbackLower = 0;
        private const Int64 V6LoopbackHigher = 72057594037927936;
        /// <summary>
        /// Compares two IPAddresses and returns an indication of their relative order.
        /// </summary>
        /// <param name="ipA">An IPAddress to compare to <paramref name="ipB"/>.</param>
        /// <param name="ipB">An IPAddress to compare to <paramref name="ipA"/>.</param>
        /// <returns>
        /// A signed integer that indicates the partial order of <paramref name="ipA" /> and <paramref name="ipB" />. 
        /// A value less than zero is returned if <paramref name="ipA" /> is ordered before <paramref name="ipB" />. 
        /// Zero is returned if <paramref name="ipA" /> equals <paramref name="ipB" />. A value greater than zero is returned if <paramref name="ipA" /> is ordered after <paramref name="ipB" />.
        /// </returns>
        public override int Compare(IPAddress ipA, IPAddress ipB)
        {
            return SameHost(ipA, ipB);
        }

        internal static bool IsLocalhost(string hostname)
        {
            var local_ip = Global.MyIPAddress;
            var host_ips = Dns.GetHostAddresses(hostname);
            return host_ips.Any(_ => 0 == SameHost(_, local_ip));
        }

        internal static int SameHost(IPAddress ipA, IPAddress ipB)
        {
            byte[] x_bytes = ipA.GetAddressBytes();
            byte[] y_bytes = ipB.GetAddressBytes();

            if (x_bytes.Length < y_bytes.Length)
                return -1;
            if (x_bytes.Length > y_bytes.Length)
                return 1;
            if (x_bytes.Length == 4) //ipv4
            {
                int int_x = BitConverter.ToInt32(x_bytes, 0);
                int int_y = BitConverter.ToInt32(y_bytes, 0);

                if (int_x == int_y)
                    return 0;

                if (int_x == V4Loopback)
                {
                    return NetworkUtility.IsMyIPAddress(ipB) ? 0 : -1;
                }

                if (int_y == V4Loopback)
                {
                    return NetworkUtility.IsMyIPAddress(ipA) ? 0 : 1;
                }

                if (int_x < int_y)
                    return -1;
                if (int_x > int_y)
                    return 1;
                return 0;
            }
            else
            {
                if (x_bytes.Length == 16)
                {
                    long long1_x = BitConverter.ToInt64(x_bytes, 0);
                    long long1_y = BitConverter.ToInt64(y_bytes, 0);
                    long long2_x = BitConverter.ToInt64(x_bytes, 8);
                    long long2_y = BitConverter.ToInt64(y_bytes, 8);

                    if (long1_x == long1_y && long2_x == long2_y)
                        return 0;

                    if (long1_x == V6LoopbackLower && long2_x == V6LoopbackHigher)
                    {
                        return NetworkUtility.IsMyIPAddress(ipB) ? 0 : -1;
                    }

                    if (long1_y == V6LoopbackLower && long2_y == V6LoopbackHigher)
                    {
                        return NetworkUtility.IsMyIPAddress(ipA) ? 0 : 1;
                    }

                    if (long1_x < long1_y)
                        return -1;
                    if (long1_x > long1_y)
                        return 1;

                    if (long2_x < long2_y)
                        return -1;
                    if (long2_x > long2_y)
                        return 1;
                    return 0;
                }
                else
                {
                    throw new Exception("Invalid IPAddress Exception.");
                }
            }
        }
    }

    /// <summary>
    /// Defines a method for comparing two <see cref="System.Net.IPEndPoint"/> objects.
    /// </summary>
    public class IPEndPointComparer : Comparer<IPEndPoint>
    {
        /// <summary>
        /// Compares two IPEndPoints and returns an indication of their relative order.
        /// </summary>
        /// <param name="ipeA">An IPEndPoint to compare to <paramref name="ipeB"/>.</param>
        /// <param name="ipeB">An IPEndPoint to compare to <paramref name="ipeA"/>.</param>
        /// <returns>
        /// A signed integer that indicates the partial order of <paramref name="ipeA" /> and <paramref name="ipeB" />. 
        /// A value less than zero is returned if <paramref name="ipeA" /> is ordered before <paramref name="ipeB" />. 
        /// Zero is returned if <paramref name="ipeA" /> equals <paramref name="ipeB" />. A value greater than zero is returned if <paramref name="ipeA" /> is ordered after <paramref name="ipeB" />.
        /// </returns>
        public override int Compare(IPEndPoint ipeA, IPEndPoint ipeB)
        {
            int ip_cmp_result = IPAddressComparer.SameHost(ipeA.Address, ipeB.Address);
            if (ip_cmp_result == 0)
            {
                if (ipeA.Port < ipeB.Port)
                    return -1;
                if (ipeA.Port > ipeB.Port)
                    return 1;
                return 0;
            }
            else
                return ip_cmp_result;
        }
    }
    #endregion
    internal class NetworkUtility
    {
        /// <summary>
        /// Checks whether the specified IPAddress belongs to the current machine.
        /// </summary>
        /// <param name="ipa">An IPAddress.</param>
        /// <returns>true if the specified IPAddress belongs to the current machine; false, otherwise.</returns>
        public static bool IsMyIPAddress(IPAddress ipa)
        {
            IPAddress[] addressList = Dns.GetHostEntry(Environment.MachineName).AddressList;
            foreach (IPAddress ip in addressList)
            {
                if (ip.Equals(ipa))
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Returns the Internet Protocol (IP) addresses for the specified host.
        /// </summary>
        /// <param name="host">The host name or IP address to resolve.</param>
        /// <returns>
        /// The IP address of the host. If multiple IP addresses point to the host, a preferred one is selected.
        /// </returns>
        /// <exception cref="System.Net.Sockets.SocketException">An error occurred when resolving the hostname.</exception>
        /// <exception cref="ArgumentNullException">host is null. </exception>
        /// <exception cref="ArgumentOutOfRangeException">The length of host is greater than 255 characters.</exception>
        /// <exception cref="ArgumentException">host is an invalid IP address.</exception>
        /// <exception cref="FormatException">Invalid host IP address override.</exception>
        public static IPAddress Hostname2IPv4Address(string host)
        {
            if (host.Length == 0)
                return null;
            host = host.Trim().ToLowerInvariant();

            IPAddress _ip = null;
            if (IPAddress.TryParse(host, out _ip))
            {
                return _ip;
            }

            IPAddress[] ips = Dns.GetHostAddresses(host);
            IPAddress selectedIP = null;
            foreach (IPAddress ip in ips)
            {
                if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                {

                    if (selectedIP == null)
                    {
                        selectedIP = ip;
                    }
                    else
                    {
                        selectedIP = ip;
                    }
                }
            }
            return selectedIP;
        }

        /// <summary>
        /// Converts an endpoint string in the form "hostname:port" to the corresponding IPEndPoint instance.
        /// </summary>
        /// <param name="ep_value">The endpoint string.</param>
        /// <returns>Returns the corresponding IPEndPoint instance.</returns>
        /// <exception cref="ArgumentNullException">ep_value is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">The length of host is greater than 255 characters.</exception>
        /// <exception cref="ArgumentException">ep_value, the host or the port extracted from it is invalid</exception>
        /// <exception cref="System.Net.Sockets.SocketException">An error occurred when resolving the hostname.</exception>
        /// <exception cref="FormatException">Invalid host IP address override.</exception>
        public static IPEndPoint Hostname2IPEndPoint(string ep_value)
        {
            if (ep_value == null)
                throw new ArgumentNullException("ep_value");

            if (ep_value.Length == 0)
                throw new ArgumentException("Endpoint string is empty.", "ep_value");

            string[] ep_tuple = ep_value.Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
            if (ep_tuple.Length != 2)
                throw new ArgumentException(String.Format("Invalid endpoint '{0}', should be 'address:port'", ep_value));

            int port = -1;
            if (!int.TryParse(ep_tuple[1].Trim(), System.Globalization.NumberStyles.None, CultureInfo.InvariantCulture, out port))
                throw new ArgumentException(String.Format("Invalid endpoint port '{0}'", ep_tuple[1]));

            IPAddress addr = Hostname2IPv4Address(ep_tuple[0]);
            return new IPEndPoint(addr, port);
        }

        /// <summary>
        /// If host name is available, then return a host:port tuple
        /// Otherwise, return ip:port tuple
        /// </summary>
        /// <param name="ipe"></param>
        /// <returns></returns>

        public static string IPEndPointToString(IPEndPoint ipe)
        {
            return IPAddress2HostName(ipe.Address) + ":" + ipe.Port.ToString();
        }

        public static string IPAddress2HostName(IPAddress ip)
        {
            string hostName = "";

            try
            {
                hostName = Dns.GetHostEntry(ip).HostName.ToLowerInvariant();
                string[] parts = hostName.Split(".".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                IPAddress[] ips = null;
                try
                {
                    ips = Dns.GetHostAddresses(parts[0]);
                }
                catch (Exception)
                {
                }
                if (ips != null)
                {
                    bool match = false;
                    foreach (IPAddress sip in ips)
                    {
                        if (sip.Equals(ip))
                        {
                            match = true;
                            break;
                        }
                    }
                    if (match)
                        return parts[0];
                }
                return hostName;
            }
            catch (Exception)
            {
                return ip.ToString().ToLowerInvariant();
            }
        }

        internal static void MergePartialFileFromServers(string server_list_file_name, string data_dir_on_server, string work_dir, string file_name, string merged_file_name)
        {
            if (Directory.Exists(work_dir))
            {
                Directory.Delete(work_dir, true);
            }
            Directory.CreateDirectory(work_dir);
            string merged_file_path = Trinity.Utilities.FileUtility.CompletePath(work_dir) + merged_file_name;

            string server_list_file = Global.MyAssemblyPath + server_list_file_name;
            string[] server_host_names = File.ReadAllLines(server_list_file);
            List<string> server_list = new List<string>();
            Parallel.ForEach<string>(server_host_names, host =>
            {
                string host_name = host.Trim();
                if (host_name.Length != 0)
                {
                    server_list.Add(host_name);
                    CopyDataFromServer(work_dir, host, data_dir_on_server, file_name);
                }
            });

            using (StreamWriter sw = new StreamWriter(merged_file_path))
            {
                foreach (string server in server_list)
                {
                    using (StreamReader sr = new StreamReader(work_dir + server + Path.DirectorySeparatorChar + file_name))
                    {
                        string line;
                        while ((line = sr.ReadLine()) != null)
                        {
                            sw.WriteLine(line);
                        }
                    }
                }
            }
        }

        internal static string CopyDataFromServer(string work_dir, string server_host_name, string data_dir_on_server, string file_name)
        {
            work_dir = Trinity.Utilities.FileUtility.CompletePath(work_dir);

            string dest_path = work_dir + server_host_name + Path.DirectorySeparatorChar;
            if (!Directory.Exists(dest_path))
            {
                Directory.CreateDirectory(dest_path);
            }

            string source_path = data_dir_on_server.Replace(":", "$");
            source_path = @"\\" + server_host_name + "\\" + Trinity.Utilities.FileUtility.CompletePath(source_path, false);
            try
            {
                File.Copy(source_path + file_name, dest_path + file_name, true);
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e.Message);
            }
            return dest_path;
        }

        internal static void DistributedDataToServers(string local_dir, string file_name, string remote_dir, string server_list_file, bool same_file)
        {
            string[] server_host_names = File.ReadAllLines(server_list_file);
            List<string> server_list = new List<string>();
            Parallel.ForEach<string>(server_host_names, host =>
            {
                string host_name = host.Trim();
                if (host_name.Length != 0)
                {
                    DistributeDataToServer(local_dir, file_name, host_name, remote_dir, same_file);
                }
            });
        }

        internal static void DistributeDataToServer(string local_dir, string file_name, string server_host_name, string remote_dir, bool same_file)
        {

            string dest_path = remote_dir.Replace(":", "$");
            dest_path = @"\\" + server_host_name + "\\" + Trinity.Utilities.FileUtility.CompletePath(dest_path, false);
            if (!Directory.Exists(dest_path))
            {
                Directory.CreateDirectory(dest_path);
            }

            try
            {
                if (same_file)
                {
                    File.Copy(Trinity.Utilities.FileUtility.CompletePath(local_dir, false) + file_name, dest_path + file_name, true);
                }
                else
                {
                    File.Copy(Trinity.Utilities.FileUtility.CompletePath(local_dir, false) + server_host_name + "\\" + file_name, dest_path + file_name, true);
                }
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e.Message);
            }
        }
    }
}

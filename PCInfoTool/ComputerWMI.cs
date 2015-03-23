using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Management;

namespace PCInfoTool
{
    class ComputerWMI
    {
        private static List<string> _oIPv4Addresses;
        private static List<string> _oIPv6Addresses;
        private static List<string> _oMACAddresses;

        /// <summary>
        /// Performs a basic property lookup from WMI
        /// </summary>
        /// <param name="wql">WQL query to execute</param>
        /// <param name="property">Property name to return the value of</param>
        /// <returns></returns>
        private static string BasicWMIQuery(string wql, string property)
        {
            using (ManagementObjectSearcher wmi = new ManagementObjectSearcher(wql))
            {
                string value = null;

                foreach (ManagementObject result in wmi.Get()) using (result)
                {
                    var resultProperty = result[property];

                    if (!(resultProperty is string))
                    {
                        resultProperty = resultProperty.ToString();
                    }

                    value = (string) resultProperty;
                }

                return value;
            }
        }

        /// <summary>
        /// Fetches the serial number of the computer from BIOS via WMI.
        /// </summary>
        public static string SerialNumber
        {
            get
            {
                return BasicWMIQuery("SELECT * FROM Win32_BIOS", "SerialNumber");
            }
        }

        /// <summary>
        /// Fetches the model of the computer via WMI.
        /// </summary>
        public static string Model
        {
            get
            {
                return BasicWMIQuery("SELECT * FROM Win32_ComputerSystem", "Model");
            }
        }

        /// <summary>
        /// Fetches the manufacturer of the computer via WMI.
        /// </summary>
        public static string Manufacturer
        {
            get
            {
                return BasicWMIQuery("SELECT * FROM Win32_ComputerSystem", "Manufacturer");
            }
        }

        /// <summary>
        /// Fetches the BIOS revision via WMI.
        /// </summary>
        public static string BIOSRevision
        {
            get
            {
                return BasicWMIQuery("SELECT * FROM Win32_BIOS", "SMBIOSBIOSVersion");
            }
        }

        /// <summary>
        /// Fetches the computer's unique identifier via WMI.
        /// </summary>
        public static string SMBIOSID
        {
            get
            {
                return BasicWMIQuery("SELECT * FROM Win32_ComputerSystemProduct", "UUID");
            }
        }

        /// <summary>
        /// Returns the computer's IPv4 addresses
        /// </summary>
        public static List<string> IPv4Addresses
        {
            get
            {
                PopulateNICInfo();
                return _oIPv4Addresses;
            }
        }

        /// <summary>
        /// Returns the computer's IPv6 addresses
        /// </summary>
        public static List<string> IPv6Addresses
        {
            get
            {
                PopulateNICInfo();
                return _oIPv6Addresses;
            }
        }

        /// <summary>
        /// Returns the computer's MAC addresses
        /// </summary>
        public static List<string> MACAddresses
        {
            get
            {
                PopulateNICInfo();
                return _oMACAddresses;
            }
        }

        /// <summary>
        /// Populates IPv4, IPv6, and MAC addresses from WMI
        /// </summary>
        private static void PopulateNICInfo()
        {
            if ((_oIPv4Addresses != null) && (_oIPv6Addresses != null) && (_oMACAddresses != null))
                return;

            _oIPv4Addresses = new List<string>();
            _oIPv6Addresses = new List<string>();
            _oMACAddresses = new List<string>();

            // Regular expression match for an IPv4 address
            Regex ipv4Address = new Regex("^(?:[0-9]{1,3}\\.){3}[0-9]{1,3}$");

            using (ManagementClass wmi = new ManagementClass("Win32_NetworkAdapterConfiguration"))
            {
                using (ManagementObjectCollection allNICs = wmi.GetInstances())
                {
                    foreach (ManagementObject nic in allNICs) using (nic)
                    {
                        // Skip interfaces that are disabled
                        if ((bool) nic["IPEnabled"] == true)
                        {
                            // Fetch IP addresses
                            if (nic["IPAddress"] != null)
                            {
                                if (nic["IPAddress"] is Array)
                                {
                                    string[] addresses = (string[])nic["IPAddress"];

                                    foreach (string addr in addresses)
                                    {
                                        if (ipv4Address.IsMatch(addr))
                                            _oIPv4Addresses.Add(addr);
                                        else
                                            _oIPv6Addresses.Add(addr);
                                    }
                                }
                                else
                                {
                                    string addr = nic["IPAddress"].ToString();

                                    if (ipv4Address.IsMatch(addr))
                                        _oIPv4Addresses.Add(addr);
                                    else
                                        _oIPv6Addresses.Add(addr);
                                }
                            }

                            // Fetch MAC address
                            if (nic["MACAddress"] != null)
                            {
                                _oMACAddresses.Add(nic["MACAddress"].ToString());
                            }
                        }
                    }
                }
            }
        }
    }
}

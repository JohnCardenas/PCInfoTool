using System;
using System.Configuration;
using System.Collections.Generic;
using System.DirectoryServices;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PCInfoTool
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        const string SETTING_TXT_HEADER      = "HeaderText";
        const string SETTING_TXT_DESCRIPTION = "DescriptionText";
        const string SETTING_SHOW_IPV4_ADDR  = "ShowIPv4Addresses";
        const string SETTING_SHOW_IPV6_ADDR  = "ShowIPv6Addresses";
        const string SETTING_SHOW_MAC_ADDR   = "ShowMACAddresses";
        const string SETTING_CHECK_DOMAIN    = "CheckDomain";

        /// <summary>
        /// Constructor
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Reads the specified AppConfig setting and returns a string
        /// </summary>
        /// <param name="key">Key to read</param>
        /// <returns></returns>
        private string ReadStringSetting(string key)
        {
            try
            {
                var appSettings = ConfigurationManager.AppSettings;
                return appSettings[key];
            }
            catch (ConfigurationErrorsException)
            {
                Console.WriteLine("Error reading app setting " + key);
                return null;
            }
        }

        /// <summary>
        /// Reads the specified AppConfig setting and returns a boolean
        /// </summary>
        /// <param name="key">Key to read</param>
        /// <returns></returns>
        private bool ReadBooleanSetting(string key)
        {
            try
            {
                var appSettings = ConfigurationManager.AppSettings;
                string result = appSettings[key] ?? null;
                return Convert.ToBoolean(result);
            }
            catch (ConfigurationErrorsException)
            {
                Console.WriteLine("Error reading app setting " + key);
                return false;
            }
        }

        /// <summary>
        /// Checks if the specified domain exists, returns false if not found or an exception occurs
        /// </summary>
        /// <param name="domain">Domain name to check</param>
        /// <returns></returns>
        private bool DomainExists(string domain)
        {
            try
            {
                if (DirectoryEntry.Exists("LDAP://" + domain))
                    return true;
                else
                    return false;
            }
            catch (System.DirectoryServices.DirectoryServicesCOMException)
            {
                return false;
            }

        }

        /// <summary>
        /// Hides the grid row that the specified control exists in
        /// </summary>
        /// <param name="control">Control to check for row</param>
        /// <param name="gridControl">Parent Grid object</param>
        private void HideGridControlRow(Control control, Grid gridControl)
        {
            int row = Grid.GetRow(control);
            gridControl.RowDefinitions[row].Height = new GridLength(0);
        }

        /// <summary>
        /// Processes the list values to the specified TextBox control, or hides the grid row if showRow is false.
        /// </summary>
        /// <param name="list">List of strings to add to textBox</param>
        /// <param name="textBox">TextBox control to add strings to</param>
        /// <param name="gridControl">Grid that contains the specified textBox</param>
        /// <param name="showRow">Show or hide the row?</param>
        private void ProcessListValues(List<string> list, ref TextBox textBox, ref Grid gridControl, bool showRow)
        {
            if (showRow)
            {
                foreach (string str in list)
                {
                    textBox.Text += str + "\n";
                }
            }
            else
            {
                HideGridControlRow(textBox, gridControl);
            }
        }

        /// <summary>
        /// Window load event handler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            txtHostName.Text = System.Net.Dns.GetHostName();

            txtManufacturer.Text = ComputerWMI.Manufacturer;
            txtModel.Text        = ComputerWMI.Model;
            txtSerialNumber.Text = ComputerWMI.SerialNumber;
            txtBIOSRev.Text      = ComputerWMI.BIOSRevision;
            txtSMBIOS.Text       = ComputerWMI.SMBIOSID;

            ProcessListValues(ComputerWMI.IPv4Addresses, ref txtIPv4Addresses, ref NetworkLayout, ReadBooleanSetting(SETTING_SHOW_IPV4_ADDR));
            ProcessListValues(ComputerWMI.IPv6Addresses, ref txtIPv6Addresses, ref NetworkLayout, ReadBooleanSetting(SETTING_SHOW_IPV6_ADDR));
            ProcessListValues(ComputerWMI.MACAddresses,  ref txtMACAddresses,  ref NetworkLayout, ReadBooleanSetting(SETTING_SHOW_MAC_ADDR));

            txtLoggedOnUser.Text = System.Security.Principal.WindowsIdentity.GetCurrent().Name;

            txtHeader.Content = ReadStringSetting(SETTING_TXT_HEADER);
            txbDescription.Text = ReadStringSetting(SETTING_TXT_DESCRIPTION);

            // Check domain
            string checkDomain = ReadStringSetting(SETTING_CHECK_DOMAIN);
            if (!(string.IsNullOrEmpty(checkDomain)))
            {
                txtCompanyNetwork.Text = DomainExists(checkDomain) ? "Connected" : "Not Connected";
            }
            else
            {
                HideGridControlRow(txtCompanyNetwork, NetworkLayout);
            }
        }
    }
}

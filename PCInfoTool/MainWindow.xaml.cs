using System;
using System.Collections.Generic;
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
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            txtHostName.Text = System.Net.Dns.GetHostName();

            txtManufacturer.Text = ComputerWMI.Manufacturer;
            txtModel.Text        = ComputerWMI.Model;
            txtSerialNumber.Text = ComputerWMI.SerialNumber;
            txtBIOSRev.Text      = ComputerWMI.BIOSRevision;
            txtSMBIOS.Text       = ComputerWMI.SMBIOSID;

            List<string> ipv4 = ComputerWMI.IPv4Addresses;
            foreach (string addr in ipv4)
            {
                txtIPv4Addresses.Text += addr + "\n";
            }

            List<string> ipv6 = ComputerWMI.IPv6Addresses;
            foreach (string addr in ipv6)
            {
                txtIPv6Addresses.Text += addr + "\n";
            }

            List<string> mac = ComputerWMI.MACAddresses;
            foreach (string addr in mac)
            {
                txtMACAddresses.Text += addr + "\n";
            }

            txtLoggedOnUser.Text = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
        }
    }
}


using MetroFramework;
using MetroFramework.Forms;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using Microsoft.Win32;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium;

namespace proxy_changer

{
    public partial class Form1 : MetroFramework.Forms.MetroForm
    {
        RegistryKey reg_key;

        public Form1()
        {
            InitializeComponent();
        }


        [DllImport("wininet.dll")]
        public static extern bool InternetSetOption(IntPtr hInternet, int dwOption, IntPtr lpBuffer, int dwBufferLength);
        public const int INTERNET_OPTION_SETTINGS_CHANGED = 39;
        public const int INTERNET_OPTION_REFRESH = 37;
        static bool settingsReturn, refreshReturn;

        int counter = 0;
        string file;
        string filebase = Directory.GetCurrentDirectory();

        private void btnChangeProxy_Click(object sender, EventArgs e)
        {
            string proxy = "http://" + txtIp.Text + ":" + txtPort.Text; //ip:port

            const string userRoot = "HKEY_CURRENT_USER";
            const string subkey = "Software\\Microsoft\\Windows\\CurrentVersion\\Internet Settings";
            const string keyName = userRoot + "\\" + subkey;

            Registry.SetValue(keyName, "ProxyServer", proxy);
            Registry.SetValue(keyName, "ProxyEnable", true ? "1" : "0", RegistryValueKind.DWord);

            // These lines implement the Interface in the beginning of program 
            // They cause the OS to refresh the settings, causing IP to realy update
            InternetSetOption(IntPtr.Zero, INTERNET_OPTION_SETTINGS_CHANGED, IntPtr.Zero, 0);
            InternetSetOption(IntPtr.Zero, INTERNET_OPTION_REFRESH, IntPtr.Zero, 0);



           /* reg_key = Registry.LocalMachine.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Internet Settings", true);

            string proxy = "http://" + txtIp.Text + ":" + txtPort.Text; //ip:port
            reg_key.SetValue("ProxyEnable", 1);
            reg_key.SetValue("ProxySever", proxy.ToString());
            reg_key.Close();
            refresh_vpn();*/

            MessageBox.Show("Proxy change to: ", proxy);
        }

        private void refresh_vpn()
        {
            // These lines implement the Interface in the beginning of program 
            // They cause the OS to refresh the settings, causing IP to realy update
            settingsReturn = InternetSetOption(IntPtr.Zero, INTERNET_OPTION_SETTINGS_CHANGED, IntPtr.Zero, 0);
            refreshReturn = InternetSetOption(IntPtr.Zero, INTERNET_OPTION_REFRESH, IntPtr.Zero, 0);
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            const string userRoot = "HKEY_CURRENT_USER";
            const string subkey = "Software\\Microsoft\\Windows\\CurrentVersion\\Internet Settings";
            const string keyName = userRoot + "\\" + subkey;

            Registry.SetValue(keyName, "ProxyEnable", 0);
        }
    }
}

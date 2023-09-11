
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
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium.DevTools;
using System.Threading;
using System.Security.Cryptography;
using WebDriverManager;
using WebDriverManager.DriverConfigs.Impl;
using WebDriverManager.Helpers;
using DevToolsSessionDomains = OpenQA.Selenium.DevTools.V114.DevToolsSessionDomains;
using DevToolsSessionDomains1 = OpenQA.Selenium.DevTools.V114.DevToolsSessionDomains;
using OpenQA.Selenium.Interactions;

namespace proxy_changer

{
    public partial class Form1 : MetroFramework.Forms.MetroForm
    {
        RegistryKey reg_key;

        protected IDevToolsSession session;
        protected IWebDriver driver;
        protected DevToolsSessionDomains1 devToolsSession;
        List<string> comboBoxBrowserItems = new List<string> { "Google Chrome", "Pirefox" };
        string GoogleChrome = "Google Chrome";
        string Pirefox = "Pirefox";

        public Form1()
        {
            InitializeComponent();
            load();
        }

        public void load()
        {
            comboBoxBrowser.DataSource = comboBoxBrowserItems;
        }


        [DllImport("wininet.dll")]
        public static extern bool InternetSetOption(IntPtr hInternet, int dwOption, IntPtr lpBuffer, int dwBufferLength);
        public const int INTERNET_OPTION_SETTINGS_CHANGED = 39;
        public const int INTERNET_OPTION_REFRESH = 37;
        static bool settingsReturn, refreshReturn;

        [DllImport("urlmon.dll", CharSet = CharSet.Ansi)]
        private static extern int UrlMkSetSessionOption(
        int dwOption, string pBuffer, int dwBufferLength, int dwReserved);

        const int URLMON_OPTION_USERAGENT = 0x10000001;
        const int URLMON_OPTION_USERAGENT_REFRESH = 0x10000002;

        int counter = 0;
        string file;
        string filebase = Directory.GetCurrentDirectory();

        private void button1_Click(object sender, EventArgs e)
        {
            var selectValue = comboBoxBrowser.SelectedValue;
            string userAgent = textUserAgent.Text;

            if (selectValue == GoogleChrome)
            {
                new DriverManager().SetUpDriver(new ChromeConfig(), VersionResolveStrategy.MatchingBrowser);
                var chromeDriverService = ChromeDriverService.CreateDefaultService();
                ChromeOptions options = new ChromeOptions();
                options.AddArgument("--user-agent=" + userAgent);
                WebDriver driver = new ChromeDriver(chromeDriverService, options);
                driver.Url = "https://www.whatismybrowser.com/detect/what-is-my-user-agent";
            }
            else
            {
                new DriverManager().SetUpDriver(new FirefoxConfig());
                var profile = new FirefoxOptions();
                profile.SetPreference("general.useragent.override", userAgent);

                WebDriver driver = new FirefoxDriver(profile);

                driver.Url = "https://www.whatismybrowser.com/detect/what-is-my-user-agent";
            }
        }

        public void SetupFirefoxDriver()
        {
            new DriverManager().SetUpDriver(new FirefoxConfig());
            FirefoxOptions chromeOptions = new FirefoxOptions();
            chromeOptions.AddArguments("--headless");
            //Set ChromeDriver
            FirefoxDriver driver = new FirefoxDriver();
            //Get DevTools
            IDevTools devTools = driver as IDevTools;
            //DevTools Session
            session = devTools.GetDevToolsSession();

            devToolsSession = session.GetVersionSpecificDomains<DevToolsSessionDomains1>();
            devToolsSession.Network.Enable(new OpenQA.Selenium.DevTools.V114.Network.EnableCommandSettings());
        }

        public void SetupChromeDriver()
        {
            new DriverManager().SetUpDriver(new ChromeConfig());
            ChromeOptions chromeOptions = new ChromeOptions();
            chromeOptions.AddArguments("--headless");
            //Set ChromeDriver
            
            ChromeDriver driver = new ChromeDriver();
            //Get DevTools
            IDevTools devTools = driver as IDevTools;
            //DevTools Session
            session = devTools.GetDevToolsSession();

            devToolsSession = session.GetVersionSpecificDomains<DevToolsSessionDomains1>();
            devToolsSession.Network.Enable(new OpenQA.Selenium.DevTools.V114.Network.EnableCommandSettings());
        }

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

        private void button2_Click(object sender, EventArgs e)
        {
            new DriverManager().SetUpDriver(new ChromeConfig());

            ChromeDriverService service1 = ChromeDriverService.CreateDefaultService();
            ChromeOptions options1 = new ChromeOptions();
            options1.AddArguments($"user-data-dir=C:/Users/{Environment.UserName}/AppData/Local/Google/Chrome/User Data");
            options1.AddArgument("profile-directory=Profile 25");
            service1.HideCommandPromptWindow = true;
            ChromeDriver driver1 = new ChromeDriver(service1, options1);
            driver1.Navigate().GoToUrl("https://www.google.com");

            /* new DriverManager().SetUpDriver(new ChromeConfig());
             ChromeOptions chromeOptions = new ChromeOptions();
             chromeOptions.AddArguments("--user-data-directory=C:\\Users\\nguyen\\AppData\\Local\\Google\\Chrome\\User Data\\");
             chromeOptions.AddArguments("--profile-directory=Profile 2");
             //Set ChromeDriver
             IWebDriver driver = new ChromeDriver(chromeOptions);*/
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}

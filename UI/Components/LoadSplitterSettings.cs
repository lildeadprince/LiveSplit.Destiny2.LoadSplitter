using System;
using System.Drawing;
using System.Net;
using System.Net.Sockets;
using System.Windows.Forms;
using System.Xml;

namespace LiveSplit.UI.Components
{
    public partial class LoadSplitterSettings : UserControl
    {
        public bool AutoStart { get; set; }
        public Color StatusColor_WaitingForDestinyProcess { get; set; }
        public Color StatusColor_IdleOn { get; set; }
        public Color StatusColor_WaitingForApiStart { get; set; }
        public Color StatusColor_Off { get; set; }

        public static string GetLocalIP()
        {
            IPAddress[] ipv4Addresses = Array.FindAll(
                Dns.GetHostEntry(string.Empty).AddressList,
                a => a.AddressFamily == AddressFamily.InterNetwork
            );

            return String.Join(",", Array.ConvertAll(ipv4Addresses, x => x.ToString()));
        }

        public LoadSplitterSettings()
        {
            InitializeComponent();

            AutoStart = false;
            StatusColor_Off = Color.FromArgb(128, 128, 128, 128);
            StatusColor_WaitingForDestinyProcess = Color.FromArgb(200, 40, 40);
            StatusColor_WaitingForApiStart = Color.FromArgb(255, 128, 0);
            StatusColor_IdleOn = Color.FromArgb(40, 200, 200);

            btnColor1.DataBindings.Add("BackColor", this, "StatusColor_Off", false, DataSourceUpdateMode.OnPropertyChanged);
            btnColor2.DataBindings.Add("BackColor", this, "StatusColor_WaitingForDestinyProcess", false, DataSourceUpdateMode.OnPropertyChanged);
            btnColor3.DataBindings.Add("BackColor", this, "StatusColor_WaitingForApiStart", false, DataSourceUpdateMode.OnPropertyChanged);
            btnColor4.DataBindings.Add("BackColor", this, "StatusColor_IdleOn", false, DataSourceUpdateMode.OnPropertyChanged);
        }

        public XmlNode GetSettings(XmlDocument document)
        {
            var parent = document.CreateElement("Settings");
            CreateSettingsNode(document, parent);
            return parent;
        }

        public int GetSettingsHashCode()
        {
            return CreateSettingsNode(null, null);
        }

        private int CreateSettingsNode(XmlDocument document, XmlElement parent)
        {
            return SettingsHelper.CreateSetting(document, parent, "StatusColor_Off", StatusColor_Off)
                 ^ SettingsHelper.CreateSetting(document, parent, "StatusColor_WaitingForDestinyProcess", StatusColor_WaitingForDestinyProcess)
                ^ SettingsHelper.CreateSetting(document, parent, "StatusColor_WaitingForApiStart", StatusColor_WaitingForApiStart)
                ^ SettingsHelper.CreateSetting(document, parent, "StatusColor_IdleOn", StatusColor_IdleOn);
        }

        public void SetSettings(XmlNode settings)
        {
            StatusColor_Off = SettingsHelper.ParseColor(settings["StatusColor_Off"]);
            StatusColor_WaitingForDestinyProcess = SettingsHelper.ParseColor(settings["StatusColor_WaitingForDestinyProcess"]);
            StatusColor_WaitingForApiStart = SettingsHelper.ParseColor(settings["StatusColor_WaitingForApiStart"]);
            StatusColor_IdleOn = SettingsHelper.ParseColor(settings["StatusColor_IdleOn"]);
        }

        private void ColorButtonClick(object sender, EventArgs e)
        {
            SettingsHelper.ColorButtonClick((Button)sender, this);
        }

        private void label11_Click(object sender, EventArgs e)
        {

        }

        private void tableLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void tableLayoutPanel1_Paint_1(object sender, PaintEventArgs e)
        {

        }
    }
}

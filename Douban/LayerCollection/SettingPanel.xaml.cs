using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using DoubanFM;
using System.Windows.Media.Animation;
using Microsoft.Win32;
using WF = System.Windows.Forms;
using WFDrawing = System.Drawing;

namespace Douban.LayerCollection
{
    /// <summary>
    /// Interaction logic for SettingPanel.xaml
    /// </summary>
    public partial class SettingPanel : UserControl
    {

        Channel ChangedChannel = null;

        public static readonly RoutedEvent SettingChangedEvent;

        static SettingPanel()
        {
            SettingChangedEvent = EventManager.RegisterRoutedEvent(
                "SettingChange",
                RoutingStrategy.Bubble,
                typeof(RoutedEventHandler),
                typeof(PlayerPanel));
        }

        public event RoutedEventHandler SettingChange
        {
            add { AddHandler(SettingChangedEvent, value); }
            remove { RemoveHandler(SettingChangedEvent, value); }
        }

        public SettingPanel()
        {
            InitializeComponent();
        }

      

        public void SetChannelList(List<Channel> ChannelList, Channel CurrentChannel)
        {
            wpChannels.Children.Clear();
            foreach (Channel channel in ChannelList)
            {
                if (Properties.Settings.Default.CurrentUser == null && channel.ChannelNo == 0)
                    continue;
                Button b = new Button();
                b.Click += new RoutedEventHandler(ChangeButton_Click);
                b.Content = channel.ChannelName;
                b.Tag = channel;
                if (channel.ChannelNo == CurrentChannel.ChannelNo)
                {
                    b.Background = new SolidColorBrush(Color.FromArgb(255, 225, 149, 4));
                }
                else
                {
                    b.Background = new SolidColorBrush(Color.FromArgb(255, 210, 210, 210));
                }
                b.Height = 20;
                b.Width = 90;
                b.Margin = new Thickness(5, 3, 5, 3);
                b.Style = (Style)Application.Current.Resources["ShineButton"];
                wpChannels.Children.Add(b);
            }
            lbCurrentChannelName.Content = "当前频道 - " + CurrentChannel.ChannelName;
        }

        private void ChangeButton_Click(object sender, RoutedEventArgs e)
        {
            foreach (Button b in wpChannels.Children)
            {
                if (b == (Button)sender)
                    b.Background = new SolidColorBrush(Color.FromArgb(255, 225, 149, 4));
                else
                    b.Background = new SolidColorBrush(Color.FromArgb(255, 210, 210, 210));
            }
            ChangedChannel = (Channel)((Button)sender).Tag;
        }

        private void btnChangeChannel_Click(object sender, RoutedEventArgs e)
        {
            if (btnChangeChannel.Content.ToString() == "更改")
            {
                btnChangeChannel.Content = "确定";
                Storyboard sb = (Storyboard)Resources["EntryChannel"];
                sb.Begin();
            }
            else
            {
                btnChangeChannel.Content = "更改";
                Storyboard sb = (Storyboard)Resources["ExitChannel"];
                sb.Begin();
            }
        }

        public void SaveSetting()
        {
            if (!this.IsLoaded)
                return;
            Properties.Settings.Default.EnableAutoPlay = cbAutoplay.IsChecked.Value;
            if (cbAutorun.IsChecked.Value)
            {
                SetAutoRun();
            }
            else
            {
                if (GetAutoRun())
                    ClearAutoRun();
            }
            
            Properties.Settings.Default.MinimizeStyle = rbMinimize.IsChecked.Value;
            Properties.Settings.Default.EnableSpeech = cbEnableSpeech.IsChecked.Value;

            if (ChangedChannel != null)
            {
                Properties.Settings.Default.ChannelName = ChangedChannel.ChannelName;
                Properties.Settings.Default.ChannelNo = ChangedChannel.ChannelNo;
            }

            Properties.Settings.Default.BackgroundColor = ((SolidColorBrush)cvPreviewColor.Background).Color;
            Properties.Settings.Default.BackgroundOpacity = (int)slOpacity.Value;

            Properties.Settings.Default.Save();

            RoutedEventArgs args = new RoutedEventArgs();
            args.RoutedEvent = SettingPanel.SettingChangedEvent;
            args.Source = this;
            RaiseEvent(args);
        }
#region RegeditController
        private void SetAutoRun()
        {
            RegistryKey hkml = Registry.CurrentUser;
            RegistryKey run = hkml.OpenSubKey("SOFTWARE", true)
                .OpenSubKey("Microsoft", true)
                .OpenSubKey("Windows", true)
                .OpenSubKey("CurrentVersion", true)
                .OpenSubKey("Run", true);

            run.SetValue("DoubanRadio", System.Windows.Forms.Application.ExecutablePath);
        }
        private bool GetAutoRun()
        {
            RegistryKey hkml = Registry.CurrentUser;
            RegistryKey run = hkml.OpenSubKey("SOFTWARE", false)
                .OpenSubKey("Microsoft", false)
                .OpenSubKey("Windows", false)
                .OpenSubKey("CurrentVersion", false)
                .OpenSubKey("Run", false);

            string[] Results = run.GetValueNames();
            foreach (string result in Results)
            {
                if (result == "DoubanRadio")
                    return true;
            }
            return false;
        }
        private void ClearAutoRun()
        {
            RegistryKey hkml = Registry.CurrentUser;
            RegistryKey run = hkml.OpenSubKey("SOFTWARE", true)
                .OpenSubKey("Microsoft", true)
                .OpenSubKey("Windows", true)
                .OpenSubKey("CurrentVersion", true)
                .OpenSubKey("Run", true);

            run.DeleteValue("DoubanRadio", false);

        }
#endregion
#region SettingControls
        private void cbAutoplay_Checked(object sender, RoutedEventArgs e)
        {
            SaveSetting();
        }

        private void cbAutorun_Checked(object sender, RoutedEventArgs e)
        {
            SaveSetting();
        }

        private void rbMinimize_Checked(object sender, RoutedEventArgs e)
        {
            SaveSetting();
        }

        private void rbClose_Checked(object sender, RoutedEventArgs e)
        {
            SaveSetting();
        }

        private void silerColor_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            cvPreviewColor.Background = new SolidColorBrush(Color.FromRgb((byte)slRed.Value, (byte)slGreen.Value, (byte)slBlue.Value));
            Properties.Settings.Default.BackgroundPicture = "";
            SaveSetting();
        }

        private void btnMoreColor_Click(object sender, RoutedEventArgs e)
        {
            WF.ColorDialog cd = new WF.ColorDialog();
            if (cd.ShowDialog() == WF.DialogResult.OK)
            {
                slRed.Value = cd.Color.R;
                slBlue.Value = cd.Color.B;
                slGreen.Value = cd.Color.G;
                Properties.Settings.Default.BackgroundPicture = "";
                SaveSetting();
            }
            
        }

        private void btnSelectPicture_Click(object sender, RoutedEventArgs e)
        {
            WF.OpenFileDialog ofd = new WF.OpenFileDialog();
            ofd.Title = "选择文件";
            ofd.Filter = "图片文件(*.BMP;*.JPG;*.JPEG)|*.BMP;*.JPG;*.JPEG";
            ofd.Multiselect = false;
            if (ofd.ShowDialog() == WF.DialogResult.OK)
            {
                Properties.Settings.Default.BackgroundPicture = ofd.FileName;
                Properties.Settings.Default.Save();
                SaveSetting();
            }
            
        }

        private void slOpacity_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            SaveSetting();
        }

        private void cbEnableAero_Checked(object sender, RoutedEventArgs e)
        {
            SaveSetting();
        }

        private void cbEnableSpeech_Checked(object sender, RoutedEventArgs e)
        {
            SaveSetting();
        }
#endregion

        private void userControl_Loaded(object sender, RoutedEventArgs e)
        {
            byte r = Properties.Settings.Default.BackgroundColor.R;
            byte g = Properties.Settings.Default.BackgroundColor.G;
            byte b = Properties.Settings.Default.BackgroundColor.B;
            int Opacity = Properties.Settings.Default.BackgroundOpacity;

            bool AutoPlay = Properties.Settings.Default.EnableAutoPlay;
            bool Minimize = Properties.Settings.Default.MinimizeStyle;
            bool Speech = Properties.Settings.Default.EnableSpeech;

            string PicturePath = Properties.Settings.Default.BackgroundPicture;

            slOpacity.Value = Opacity;

            cbAutoplay.IsChecked = AutoPlay;

            rbMinimize.IsChecked = Minimize;
            rbClose.IsChecked = !Minimize;

            cbEnableSpeech.IsChecked = Speech;

            slRed.Value = r;
            slGreen.Value = g;
            slBlue.Value = b;

            cbAutorun.IsChecked = GetAutoRun();

            Properties.Settings.Default.BackgroundPicture = PicturePath;

            RoutedEventArgs args = new RoutedEventArgs();
            args.RoutedEvent = SettingPanel.SettingChangedEvent;
            args.Source = this;
            RaiseEvent(args);
        }
        
    }
}

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
using System.Windows.Interop;
using Douban.Interop;
using System.Windows.Threading;
using DoubanFM;
using System.Windows.Media.Animation;
using System.Speech.Recognition;
using WF = System.Windows.Forms;
using WFDrawing = System.Drawing;
using System.IO;
using Douban.LayerCollection;
using System.Text.RegularExpressions;

namespace Douban
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private API api;
        private Libraries.MediaPlayer mediaPlayer;
        private DispatcherTimer timer;
        private bool ToggleSetting = false;
        SpeechRecognizer speechRecognizer;
        WF.NotifyIcon notifyIcon;

        public MainWindow()
        {
            InitializeComponent();

            mediaPlayer = new Libraries.MediaPlayer(gMain);
            mediaPlayer.MediaOpened += new EventHandler(mediaPlayer_MediaOpened);
            mediaPlayer.MediaEnded += new EventHandler(mediaPlayer_MediaEnded);
            mediaPlayer.MediaFailed += new EventHandler(mediaPlayer_MediaFailed);

            timer = new DispatcherTimer();
            timer.Interval = new TimeSpan(0, 0, 0, 0, 1000);
            timer.Tick += new EventHandler(timer_Tick);

            this.notifyIcon = new WF.NotifyIcon();
            this.notifyIcon.Text = "豆瓣电台";
            this.notifyIcon.BalloonTipIcon = WF.ToolTipIcon.Info;
            this.notifyIcon.BalloonTipTitle = "音乐将继续播放";
            this.notifyIcon.BalloonTipText = "单击这里恢复豆瓣电台窗口。";
            this.notifyIcon.Icon = Png2Icon("Douban.png");
            this.notifyIcon.Visible = false;
            notifyIcon.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(NotifyIconDoubleClick);
            this.notifyIcon.ShowBalloonTip(500); 


            api = new API();
            api.CurrentChannel = new Channel(Properties.Settings.Default.ChannelNo, Properties.Settings.Default.ChannelName);
            if (Properties.Settings.Default.CurrentUser != null)
            {
                api.CurrentUser = Properties.Settings.Default.CurrentUser;
                pnlControl.Title = "豆瓣电台 - " + api.CurrentUser.user_name;
                pnlControl.UserName = "注销";
                pnlPlayer.AcitveUserButtons();
            }

            // api.EnableExtraMusicQueue = true;
            pnlPlayer.Volume = Properties.Settings.Default.Volume;
            mediaPlayer.Volume = Properties.Settings.Default.Volume;

            api.NextCompleted += new API.NextCompletedEventHandler(API_NextCompleted);
            api.NextError += new API.NextErrorEventHandler(API_NextError);

            if (Properties.Settings.Default.EnableAutoPlay)
                api.Next(false);

            if (Properties.Settings.Default.EnableSpeech)
                speechRecognizer = new SpeechRecognizer();
        }
        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            // HwndSource source = PresentationSource.FromVisual(this) as HwndSource;
            // source.AddHook(WndProc);
        }
        private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            System.Windows.Forms.Message Msg = new System.Windows.Forms.Message();
            Msg.HWnd = hwnd;
            Msg.LParam = lParam;
            Msg.WParam = wParam;
            Msg.Msg = msg;

            return IntPtr.Zero;
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            pnlPlayer.SetMediaPosition(mediaPlayer.Position, mediaPlayer.NaturalDuration);
        }
        private void ApplySetting()
        {
            WindowBackground.Opacity = Properties.Settings.Default.BackgroundOpacity * 0.01;
            if (Properties.Settings.Default.BackgroundPicture == "" || !File.Exists(Properties.Settings.Default.BackgroundPicture))
            {
                WindowBackground.Background = new SolidColorBrush(Properties.Settings.Default.BackgroundColor);
            }
            else
            {

                BitmapImage imageSource = new BitmapImage();
                imageSource.BeginInit();
                imageSource.UriSource = new Uri(Properties.Settings.Default.BackgroundPicture);
                imageSource.EndInit();

                ImageBrush ib = new ImageBrush(imageSource);
                ib.Stretch = Stretch.UniformToFill;
                WindowBackground.Background = ib;
                
            }

            Color c = Properties.Settings.Default.BackgroundColor;
            if (Properties.Settings.Default.BackgroundPicture == "" || !File.Exists(Properties.Settings.Default.BackgroundPicture))
            {
                if ((c.R + c.G + c.B) / 3 >= 127)
                {
                    this.Foreground = new SolidColorBrush(Color.FromRgb(0, 0, 0));
                }
                else
                {
                    this.Foreground = new SolidColorBrush(Color.FromRgb(255, 255, 255));
                }
            }
            else
            {
                WFDrawing.Bitmap image = new WFDrawing.Bitmap(Properties.Settings.Default.BackgroundPicture);
                WFDrawing.Imaging.BitmapData data = image.LockBits(new WFDrawing.Rectangle(0, 0, image.Width, image.Height), WFDrawing.Imaging.ImageLockMode.ReadWrite, WFDrawing.Imaging.PixelFormat.Format24bppRgb);

                unsafe
                {
                    long r = 0;
                    long g = 0;
                    long b = 0;
                    long pixelCount = data.Height * data.Width;

                    byte* ptr = (byte*)(data.Scan0);
                    for (int i = 0; i < data.Height; i++)
                    {
                        for (int j = 0; j < data.Width; j++)
                        {
                            r += *ptr;
                            g += *(ptr + 1);
                            b += *(ptr + 2);
                            ptr += 3;
                        }
                        ptr += data.Stride - data.Width * 3;
                    }

                    double totalRGB = (r / pixelCount + g / pixelCount + b / pixelCount) / 3;
                    if(totalRGB > 127)
                    {
                        this.Foreground = new SolidColorBrush(Color.FromRgb(0, 0, 0));
                    }
                    else
                    {
                        this.Foreground = new SolidColorBrush(Color.FromRgb(255, 255, 255));
                    }
                }
            }

        }
#region 组件的事件
        private void mediaPlayer_MediaOpened(object sender, EventArgs e)
        {
            timer.Start();
        }
        private void mediaPlayer_MediaEnded(object sender, EventArgs e)
        {
            timer.Stop();
            api.Next(false);
            pnlPlayer.SetMediaPosition(new TimeSpan(0), new TimeSpan(0));
        }
        private void mediaPlayer_MediaFailed(object sender, EventArgs e)
        {
            timer.Stop();
            api.Next(false);
            pnlPlayer.SetMediaPosition(new TimeSpan(0), new TimeSpan(0));
        }

        private void API_NextCompleted(object sender, Song CurrentSong)
        {
            mediaPlayer.Play(new Uri(CurrentSong.url));
            pnlPlayer.SetPlayerStatus(true);
            pnlPlayer.SetNextSong(CurrentSong);
            pnlPlayer.ChannelName = api.CurrentChannel.ChannelName;
        }
        private void API_NextError(object sender, string Message)
        {
            if (MessageBox.Show("加载歌曲失败，请检查网络后重试。\n是否现在重试？", "加载失败", MessageBoxButton.YesNo, MessageBoxImage.Error) == MessageBoxResult.Yes)
            {
                api.Next(mediaPlayer.MediaState == MediaState.Play);
            }
        }
#endregion
#region UI的事件
        private void pnlControl_ChangeStyle(object sender, RoutedEventArgs e)
        {
            pnlPlayer.ChangeStyle();
            Storyboard sb = ((Storyboard)Resources["ChangeStyle1"]).Clone();
            sb.Completed += delegate
            {
                this.Tag = new Point(this.Left, this.Top);
                this.Top = 0;
                this.Left = (System.Windows.Forms.Screen.PrimaryScreen.Bounds.Width - this.Width) / 2;
                Storyboard sb2 = (Storyboard)Resources["ChangeStyle2"];
                sb2.Begin(window);
                this.Topmost = true;
                this.ShowInTaskbar = false;
            };
            sb.Begin(window);
        }

        private void pnlControl_RestoreStyle(object sender, RoutedEventArgs e)
        {
            pnlPlayer.RestoreStyle();
            Storyboard sb = ((Storyboard)Resources["RestoreStyle2"]).Clone();
            sb.Completed += delegate
            {
                if (this.Tag == null)
                    this.Tag = new Point(100, 100);
                this.Left = ((Point)this.Tag).X;
                this.Top = ((Point)this.Tag).Y;
                Storyboard sb2 = (Storyboard)Resources["RestoreStyle1"];
                sb2.Begin(window);
                this.Topmost = false;
                this.ShowInTaskbar = true;
            };
            sb.Begin(window);
        }

        private void window_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed && (e.Source.GetType() == this.GetType() || e.Source.GetType() == this.pnlControl.GetType()))
                this.DragMove();
        }
        private void pnlPlayer_Hate(object sender, RoutedEventArgs e)
        {
            api.Bye();
        }
        private void pnlPlayer_Like(object sender, RoutedEventArgs e)
        {
            api.ToggleRate();
            if (api.CurrentMusic.like)
                pnlPlayer.SetLike();
            else
                pnlPlayer.SetUnlike();
        }
        private void pnlPlayer_VolumeChanged(object sender, RoutedEventArgs e)
        {
            if (mediaPlayer == null)
                return;
            mediaPlayer.Volume = pnlPlayer.Volume;
        }
        private void pnlPlayer_PauseResume(object sender, RoutedEventArgs e)
        {
            if (mediaPlayer.MediaState == MediaState.Play)
            {
                mediaPlayer.Pause();
                pnlPlayer.SetPlayerStatus(false);
            }
            else
            {
                if (mediaPlayer.MediaUri != null)
                {
                    mediaPlayer.Play();
                    pnlPlayer.SetPlayerStatus(true);
                }
            }
        }
        private void pnlPlayer_Next(object sender, RoutedEventArgs e)
        {
            api.Next(true);
        }
        
        private void ControlPanel_CloseWindow(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        private void ControlPanel_MinimizeWindow(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }
        private void pnlControl_MoveTitle(object sender, RoutedEventArgs e)
        {
            this.DragMove();
        }
        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            if (ToggleSetting)
            {
                pnlSetting.SaveSetting();
                if (Properties.Settings.Default.ChannelNo != api.CurrentChannel.ChannelNo)
                {
                    api.CurrentChannel = new Channel(Properties.Settings.Default.ChannelNo, Properties.Settings.Default.ChannelName);
                    api.Next(mediaPlayer.MediaState == MediaState.Play);
                }

                ToggleSetting = false;
                pnlPlayer.Width = 300;
                pnlPlayer.Height = 500;
                pnlControl.EnableChangeStyleButton = true;
                Storyboard sb = ((Storyboard)Resources["BackMain"]).Clone();
                sb.Completed += delegate
                {
                    pnlSetting.Width = 0;
                    pnlSetting.Height = 0;
                    label.Text = "设置";
                    btnBack.ToolTip = "更改频道以及设置";
                };
                if (Resources.Contains(btnBack))
                {
                    Resources[btnBack] = sb;
                }
                else
                {
                    Resources.Add(btnBack, sb);
                }
                sb.Begin();
            }
            else
            {
                pnlSetting.SetChannelList(api.ChannelList, api.CurrentChannel);
                ToggleSetting = true;
                pnlSetting.Width = 300;
                pnlSetting.Height = 500;
                Storyboard sb = ((Storyboard)Resources["EntrySetting"]).Clone();
                pnlControl.EnableChangeStyleButton = false;
                sb.Completed += delegate
                {
                    pnlPlayer.Width = 0;
                    pnlPlayer.Height = 0;
                    label.Text = "返回";
                    btnBack.ToolTip = "返回播放器";
                };
                if (Resources.Contains(btnBack))
                {
                    Resources[btnBack] = sb;
                }
                else
                {
                    Resources.Add(btnBack, sb);
                }
                sb.Begin();
            }
        }
        private void pnlControl_Login(object sender, RoutedEventArgs e)
        {
            if (api.CurrentUser == null)
            {
                LoginWindow loginWindow = new LoginWindow(api);
                bool? DialogResult = loginWindow.ShowDialog();
                if (DialogResult.HasValue && DialogResult.Value)
                {
                    pnlControl.Title = "豆瓣电台 - " + api.CurrentUser.user_name;
                    pnlControl.UserName = "注销";
                    pnlPlayer.AcitveUserButtons();
                    api.Next(mediaPlayer.MediaState == MediaState.Play);
                }
            }
            else
            {
                if (MessageBox.Show("是否确定要注销当前用户？", "注销", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    api.CurrentUser = null;
                    Properties.Settings.Default.CurrentUser = null;
                    Properties.Settings.Default.Save();
                    pnlControl.Title = "豆瓣电台 - ";
                    pnlControl.UserName = "登录";
                    pnlPlayer.DeactiveUserButtons();
                }
            }
        }
        private void window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (Properties.Settings.Default.MinimizeStyle)
            {
                e.Cancel = true;
                if (!this.ShowInTaskbar)
                {
                    this.ShowInTaskbar = true;
                }
                this.WindowState = WindowState.Minimized;
                this.ShowInTaskbar = false;
                this.notifyIcon.Visible = true;
                this.notifyIcon.ShowBalloonTip(500);
            }

            Properties.Settings.Default.Volume = pnlPlayer.Volume;
            Properties.Settings.Default.Save();
        }

        private WFDrawing.Icon Png2Icon(string ResourcePath)
        {
            Stream s = Application.GetResourceStream(new Uri(ResourcePath, UriKind.RelativeOrAbsolute)).Stream;
            MemoryStream mStream = new MemoryStream();
            WFDrawing.Bitmap image = new WFDrawing.Bitmap(s);
            image.Save(mStream, System.Drawing.Imaging.ImageFormat.Png);
            WFDrawing.Icon icon = WFDrawing.Icon.FromHandle(new WFDrawing.Bitmap(mStream).GetHicon());
            mStream.Close();
            return icon;
        }

        private void NotifyIconDoubleClick(object sender, EventArgs e)
        {
            this.ShowInTaskbar = true;
            this.WindowState = WindowState.Normal;
            this.notifyIcon.Visible = false;
        }
        private void window_Loaded(object sender, RoutedEventArgs e)
        {
            // this.WindowStyle = WindowStyle.SingleBorderWindow;

            ApplySetting();

            if (Properties.Settings.Default.EnableSpeech)
                VoiceCommand();

        }
#endregion
#region VoiceCommand
        private void VoiceCommand()
        {
            RegisterVoiceCommand(VoiceCommand_Close, "退出");
            RegisterVoiceCommand(VoiceCommand_Next, "下一首", "切歌");
            RegisterVoiceCommand(VoiceCommand_Minimize, "最小化");
            RegisterVoiceCommand(VoiceCommand_Restore, "恢复");

            RegisterVoiceCommand(VoiceCommand_Pause, "暂停");
            RegisterVoiceCommand(VoiceCommand_Resume, "播放");

            RegisterVoiceCommand(VoiceCommand_Like, "我喜欢");
            RegisterVoiceCommand(VoiceCommand_NoLike, "不再喜欢");
            RegisterVoiceCommand(VoiceCommand_Bye, "不再播放");

            for (int i = 0; i <= 100; ++i)
            {
                EventHandler VoiceChangeEvent = delegate(object sender, EventArgs e)
                {
                    if (mediaPlayer == null)
                        return;

                    int Volume;
                    if (int.TryParse(((SpeechRecognizedEventArgs)e).Result.Alternates[0].Text.Replace("音量", ""), out Volume))
                    {
                        mediaPlayer.Volume = Volume * 0.01;
                        pnlPlayer.Volume = Volume * 0.01;
                    }
                };
                RegisterVoiceCommand(VoiceChangeEvent, "音量" + i.ToString());
            }
        }

        private void VoiceCommand_Bye(object sender, EventArgs e)
        {
            api.Bye();
        }

        private void VoiceCommand_Like(object sender, EventArgs e)
        {
            if (!api.CurrentMusic.like)
            {
                api.ToggleRate();
                pnlPlayer.SetLike();
            }
        }

        private void VoiceCommand_NoLike(object sender, EventArgs e)
        {
            if (api.CurrentMusic.like)
            {
                api.ToggleRate();
                pnlPlayer.SetUnlike();
            }
                
        }

        private void VoiceCommand_Resume(object sender, EventArgs e)
        {
            if (mediaPlayer.MediaState != MediaState.Play)
            {
                if (mediaPlayer.MediaUri != null)
                {
                    mediaPlayer.Play();
                    pnlPlayer.SetPlayerStatus(true);
                }
            }
        }

        private void VoiceCommand_Pause(object sender, EventArgs e)
        {
            if (mediaPlayer.MediaState == MediaState.Play)
            {
                mediaPlayer.Pause();
                pnlPlayer.SetPlayerStatus(false);
            }
        }

        private void VoiceCommand_Close(object sender, EventArgs e)
        {
            this.Close();
        }

        private void VoiceCommand_Next(object sender, EventArgs e)
        {
            api.Next(mediaPlayer.MediaState == MediaState.Play);
        }

        private void VoiceCommand_Minimize(object sender, EventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void VoiceCommand_Restore(object sender, EventArgs e)
        {
            this.WindowState = WindowState.Normal;
        }

        private void RegisterVoiceCommand(EventHandler Callback, params string[] Args)
        {
            Choices choices = new Choices();
            foreach (string arg in Args)
            {
                choices.Add(arg);
            }
            Grammar grammar = new Grammar(new GrammarBuilder(choices));

            grammar.SpeechRecognized += delegate(object sender, SpeechRecognizedEventArgs e)
            {
                EventHandler eventHandler = new EventHandler(Callback);
                Dispatcher.BeginInvoke(eventHandler, sender, e);
            };

            speechRecognizer.LoadGrammarAsync(grammar);
        }
#endregion

        private void pnlSetting_SettingChange(object sender, RoutedEventArgs e)
        {
            ApplySetting();
        }

    }
}

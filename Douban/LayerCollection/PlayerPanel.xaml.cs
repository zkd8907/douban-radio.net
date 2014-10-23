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
using System.Windows.Threading;
using System.Windows.Media.Animation;
using DoubanFM;

namespace Douban.LayerCollection
{
    /// <summary>
    /// Interaction logic for PlayerPanel.xaml
    /// </summary>
    public partial class PlayerPanel : UserControl
    {
        public static readonly RoutedEvent LikeEvent;
        public static readonly RoutedEvent HateEvent;
        public static readonly RoutedEvent NextEvent;
        public static readonly RoutedEvent VolumeChangedEvent;
        public static readonly RoutedEvent PauseResumeEvent;

        static PlayerPanel()
        {
            LikeEvent = EventManager.RegisterRoutedEvent(
                "Like",
                RoutingStrategy.Bubble,
                typeof(RoutedEventHandler),
                typeof(PlayerPanel));
            HateEvent = EventManager.RegisterRoutedEvent(
                "Hate",
                RoutingStrategy.Bubble,
                typeof(RoutedEventHandler),
                typeof(PlayerPanel));
            NextEvent = EventManager.RegisterRoutedEvent(
                "Next",
                RoutingStrategy.Bubble,
                typeof(RoutedEventHandler),
                typeof(PlayerPanel));
            VolumeChangedEvent = EventManager.RegisterRoutedEvent(
                "VolumeChanged",
                RoutingStrategy.Bubble,
                typeof(RoutedEventHandler),
                typeof(PlayerPanel));
            PauseResumeEvent = EventManager.RegisterRoutedEvent(
                "PauseResume",
                RoutingStrategy.Bubble,
                typeof(RoutedEventHandler),
                typeof(PlayerPanel));
        }

        public event RoutedEventHandler Like
        {
            add { AddHandler(LikeEvent, value); }
            remove { RemoveHandler(LikeEvent, value); }
        }
        public event RoutedEventHandler Hate
        {
            add { AddHandler(HateEvent, value); }
            remove { RemoveHandler(HateEvent, value); }
        }
        public event RoutedEventHandler Next
        {
            add { AddHandler(NextEvent, value); }
            remove { RemoveHandler(NextEvent, value); }
        }
        public event RoutedEventHandler VolumeChanged
        {
            add { AddHandler(VolumeChangedEvent, value); }
            remove { RemoveHandler(VolumeChangedEvent, value); }
        }
        public event RoutedEventHandler PauseResume
        {
            add { AddHandler(PauseResumeEvent, value); }
            remove { RemoveHandler(PauseResumeEvent, value); }
        }

        public PlayerPanel()
        {
            InitializeComponent();
        }

        public double Volume
        {
            get { return slVolume.Value; }
            set { slVolume.Value = value; }
        }
        public string ChannelName
        {
            get { return lbChannelName.Text; }
            set { lbChannelName.Text = value + "MHz"; }
        }
#region 控件公开的方法
        public void AcitveUserButtons()
        {
            btnHate.IsEnabled = true;
            btnLike.IsEnabled = true;
            mbtnLike.IsEnabled = true;
        }
        public void DeactiveUserButtons()
        {
            btnHate.IsEnabled = false;
            btnLike.IsEnabled = false;
            mbtnLike.IsEnabled = false;
        }
        public void SetNextSong(Song CurrentSong)
        {
            Storyboard sb1 = ((Storyboard)Resources["HideSongInfo"]).Clone();
            sb1.Completed += delegate
            {
                rTitle.Text = CurrentSong.title;
                rArtist.Text = CurrentSong.artist;
                mtbSongInfo.Text = CurrentSong.title + " - " + CurrentSong.artist;
                rAlbum.Text = "《" + CurrentSong.albumtitle + "》";
                rCompany.Text = "唱片公司：" + CurrentSong.company;
                rPublic.Text = "发行时间：" + CurrentSong.public_time;
                Storyboard sb2 = ((Storyboard)Resources["ShowSongInfo"]);
                sb2.Begin(border);
            };
            sb1.Begin(border);

            BitmapImage imageSource = new BitmapImage();
            imageSource.DownloadCompleted += delegate
            {
                imgAlbum.Source = imageSource;
                tbSongInfo.Tag = CurrentSong.album;
                imgAlbum.Tag = CurrentSong.albumtitle;
            };
            imageSource.BeginInit();
            imageSource.UriSource = new Uri(CurrentSong.picture.Replace("mpic", "lpic"));
            imageSource.EndInit();
            
            
            rToolTip.Text = "正在播放：" + CurrentSong.title + " - " + CurrentSong.artist + " 《" + CurrentSong.albumtitle + "》";
            if (CurrentSong.like)
            {
                SetLike();
            }
            else
            {
                SetUnlike();
            }
        }
        public void SetUnlike()
        {
            btnLike.Tag = @"Image\Like.png";
            btnLike.ToolTip = "我喜欢";
            mbtnLike.Tag = @"Image\Like.png";
            mbtnLike.ToolTip = "我喜欢";
        }
        public void SetLike()
        {
            btnLike.Tag = @"Image\Like_On.png";
            btnLike.ToolTip = "不再喜欢";
            mbtnLike.Tag = @"Image\Like_On.png";
            mbtnLike.ToolTip = "不再喜欢";
        }
        public void SetMediaPosition(TimeSpan Position, TimeSpan DurationTimeSpan)
        {
            TimeSpan LeftTimeSpan = DurationTimeSpan - Position;
            lbPositionDisplay.Text = Position.Minutes.ToString("D2") + ":" + Position.Seconds.ToString("D2");
            lbLeftDisplay.Text = "-" + LeftTimeSpan.Minutes.ToString("D2") + ":" + LeftTimeSpan.Seconds.ToString("D2");
            rectProgress.Width = Position.TotalSeconds / DurationTimeSpan.TotalSeconds * 180;
        }
        public void SetPlayerStatus(bool IsPlaying)
        {
            if (IsPlaying)
            {
                btnTogglePlay.Tag = @"Image\Pause.png";
                btnTogglePlay.ToolTip = "暂停";
                mbtnTogglePlay.Tag = @"Image\Pause.png";
                mbtnTogglePlay.ToolTip = "暂停";
            }
            else
            {
                btnTogglePlay.Tag = @"Image\Play.png";
                btnTogglePlay.ToolTip = "播放";
                mbtnTogglePlay.Tag = @"Image\Play.png";
                mbtnTogglePlay.ToolTip = "播放";
            }
        }
        public void ChangeStyle()
        {
            gdMiniPlayer.Visibility = Visibility.Visible;
            Storyboard sb = (Storyboard)Resources["ChangeStyle"];
            sb.Begin();
        }
        public void RestoreStyle()
        {
            gdMiniPlayer.Visibility = Visibility.Visible;
            Storyboard sb = (Storyboard)Resources["RestoreStyle"];
            sb.Begin();
        }
#endregion
#region 事件
        private void imgAlbum_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed && imgAlbum.Tag != null)
            {
                System.Diagnostics.Process.Start("http://www.amazon.cn/s?ie=UTF8&source=shalke&index=music&keywords=" + Uri.EscapeUriString(imgAlbum.Tag.ToString()));
            }
        }

        private void tbSongInfo_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed && tbSongInfo.Tag != null)
            {
                System.Diagnostics.Process.Start("http://music.douban.com" + tbSongInfo.Tag.ToString());
            }
        }
        private void slVolume_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            RoutedEventArgs args = new RoutedEventArgs();
            args.RoutedEvent = PlayerPanel.VolumeChangedEvent;
            args.Source = this;
            RaiseEvent(args);
        }
        private void btnTogglePlay_Click(object sender, RoutedEventArgs e)
        {
            RoutedEventArgs args = new RoutedEventArgs();
            args.RoutedEvent = PlayerPanel.PauseResumeEvent;
            args.Source = this;
            RaiseEvent(args);
        }
        private void btnNext_Click(object sender, RoutedEventArgs e)
        {
            RoutedEventArgs args = new RoutedEventArgs();
            args.RoutedEvent = PlayerPanel.NextEvent;
            args.Source = this;
            RaiseEvent(args);
        }
        private void btnLike_Click(object sender, RoutedEventArgs e)
        {
            RoutedEventArgs args = new RoutedEventArgs();
            args.RoutedEvent = PlayerPanel.LikeEvent;
            args.Source = this;
            RaiseEvent(args);
        }
        private void btnHate_Click(object sender, RoutedEventArgs e)
        {
            RoutedEventArgs args = new RoutedEventArgs();
            args.RoutedEvent = PlayerPanel.HateEvent;
            args.Source = this;
            RaiseEvent(args);
        }
#endregion
    }
}

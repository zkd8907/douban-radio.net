using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Data;

namespace Douban.Libraries
{
    public class MediaPlayer
    {

        private MediaElement pMediaPlayer;

        public MediaPlayer(Grid g)
        {
            pMediaPlayer = new MediaElement();
            g.Children.Add(pMediaPlayer);
            pMediaPlayer.LoadedBehavior = MediaState.Manual;
            pMediaPlayer.UnloadedBehavior = MediaState.Manual;
            pMediaPlayer.Volume = 0.5;

            pMediaPlayer.MediaOpened += new RoutedEventHandler(pMediaPlayer_MediaOpended);
            pMediaPlayer.MediaEnded += new RoutedEventHandler(pMediaPlayer_MediaEnded);
            pMediaPlayer.MediaFailed += new EventHandler<ExceptionRoutedEventArgs>(pMediaPlayer_MediaFailed);
            pMediaPlayer.BufferingStarted += new RoutedEventHandler(pMediaPlayer_BufferingStarted);
            pMediaPlayer.BufferingEnded += new RoutedEventHandler(pMediaPlayer_MediaEnded);
            pMediaPlayer.SourceUpdated += new EventHandler<DataTransferEventArgs>(pMediaPlayer_SourceUpdated);
        }
#region 事件
        public EventHandler MediaOpened;
        public EventHandler MediaEnded;
        public EventHandler MediaFailed;
        public EventHandler BufferingStarted;
        public EventHandler BufferingEnded;
        public EventHandler SourceUpdated;

        private void pMediaPlayer_MediaOpended(object sender, RoutedEventArgs e)
        {
            if (MediaOpened != null)
                MediaOpened(sender, new EventArgs());
        }
        private void pMediaPlayer_MediaEnded(object sender, RoutedEventArgs e)
        {
            if (MediaEnded != null && pMediaPlayer.HasAudio && pMediaPlayer.Position.TotalSeconds >= this.NaturalDuration.TotalSeconds * 0.9)
                MediaEnded(sender, new EventArgs());
        }
        private void pMediaPlayer_MediaFailed(object sender, ExceptionRoutedEventArgs e)
        {
            if (MediaFailed != null)
                MediaFailed(sender, new EventArgs());
        }
        private void pMediaPlayer_BufferingStarted(object sender, RoutedEventArgs e)
        {
            if (BufferingStarted != null)
                BufferingStarted(sender, new EventArgs());
        }
        private void pMediaPlayer_BufferingEnded(object sender, RoutedEventArgs e)
        {
            if (BufferingEnded != null)
                BufferingEnded(sender, new EventArgs());
        }
        private void pMediaPlayer_SourceUpdated(object sender, DataTransferEventArgs e)
        {
            if (SourceUpdated != null)
                SourceUpdated(sender, new EventArgs());
        }
#endregion
#region 属性
        public Uri MediaUri { get; set; }
        public double Volume
        {
            get 
            {
                return pMediaPlayer.Volume;
            }
            set
            {
                if (value > 100) value = 100;
                if (value < 0) value = 0;
                pMediaPlayer.Volume = value;
            }
        }
        public TimeSpan Position
        {
            get
            {
                return pMediaPlayer.Position;
            }
            set
            {
                if (pMediaPlayer.NaturalDuration.HasTimeSpan && value > pMediaPlayer.NaturalDuration.TimeSpan)
                    pMediaPlayer.Position = pMediaPlayer.NaturalDuration.TimeSpan;
                if (value < new TimeSpan(0))
                    pMediaPlayer.Position = new TimeSpan(0);
                pMediaPlayer.Position = value;
            }
        }
        public TimeSpan NaturalDuration
        {
            get
            {
                if (pMediaPlayer.NaturalDuration.HasTimeSpan)
                {
                    return pMediaPlayer.NaturalDuration.TimeSpan;
                }
                return new TimeSpan(0);
            }
        }
        public MediaState MediaState { get; set; }
#endregion
#region 方法
        public void Play()
        {
            if (MediaUri == null)
                return;
            pMediaPlayer.Source = MediaUri;

            pMediaPlayer.Play();

            MediaState = MediaState.Play;
        }
        public void Play(Uri MediaUri)
        {
            this.MediaUri = MediaUri;
            Play();
        }
        public void Pause()
        {
            if (pMediaPlayer.Source == null)
                return;
            pMediaPlayer.Pause();
            MediaState = MediaState.Pause;
        }
        public void Stop()
        {
            if (pMediaPlayer.Source == null)
                return;
            pMediaPlayer.Stop();
            MediaState = MediaState.Stop;
        }
        public void Resume()
        {
            if (pMediaPlayer.Source == null)
                return;
            pMediaPlayer.Play();
            MediaState = MediaState.Play;
        }
#endregion
    }
}

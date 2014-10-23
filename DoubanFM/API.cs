using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DoubanFM
{
    public partial class API
    {
        private Uri BASEURI = new Uri("http://www.douban.com/j/app/radio/people");

        private Channel pCurrentChannel = null;
        private Channel pLastChannel = null;
        private Queue<Song> pMusicQueue = new Queue<Song>();
        private Queue<Song> pHistoryQueue = new Queue<Song>();
        private Song pCurrentMusic = null;
        private Song pLastMusic = null;
        private Queue<string> hStrQueue = new Queue<string>();
        

        public readonly List<Channel> ChannelList = new List<Channel>();

        public User CurrentUser { get; set; }
        public Channel CurrentChannel 
        {
            get 
            {
                return pCurrentChannel.Clone(); 
            }
            set
            {
                pCurrentChannel = value.Clone();
            }
        }
        public Channel LastChannel { get { return pLastChannel.Clone(); } }
        public Queue<Song> MusicQueue { get { return CloneQueue(pMusicQueue); } }
        public Queue<Song> HistoryQueue { get { return CloneQueue(pHistoryQueue); } }
        public Song CurrentMusic { get { return pCurrentMusic.Clone(); } }
        public Song LastMusic { get { return pLastMusic.Clone(); } }

        public bool EnableExtraMusicQueue { get; set; }
        public const int ExtraMusicTimes = 5;
        
        private int pGetMusicTimes = 0;
        private bool pIsGetNewList = false;
        private bool pIsLastOne = false;
        private bool pIsPlaying = false;

        public API()
        {
            ChannelList.Add(new Channel(0, "私人频道"));

            ChannelList.Add(new Channel(1, "华语"));
            ChannelList.Add(new Channel(2, "欧美"));
            ChannelList.Add(new Channel(3, "七零"));
            ChannelList.Add(new Channel(4, "八零"));
            ChannelList.Add(new Channel(5, "九零"));
            ChannelList.Add(new Channel(6, "粤语"));
            ChannelList.Add(new Channel(7, "摇滚"));
            ChannelList.Add(new Channel(8, "民谣"));
            ChannelList.Add(new Channel(9, "轻音乐"));
            ChannelList.Add(new Channel(10, "电影原声"));
            ChannelList.Add(new Channel(13, "爵士"));
            ChannelList.Add(new Channel(14, "电子"));
            ChannelList.Add(new Channel(15, "说唱"));
            ChannelList.Add(new Channel(16, "R&B"));
            ChannelList.Add(new Channel(17, "日语"));
            ChannelList.Add(new Channel(18, "韩语"));
            ChannelList.Add(new Channel(19, "Puma"));
            ChannelList.Add(new Channel(20, "女声"));
            ChannelList.Add(new Channel(22, "法语"));
            ChannelList.Add(new Channel(26, "豆瓣音乐人"));
            ChannelList.Add(new Channel(27, "古典"));
            ChannelList.Add(new Channel(28, "动漫"));
            ChannelList.Add(new Channel(30, "BMW Club3"));
            ChannelList.Add(new Channel(31, "NB 敢动"));
            ChannelList.Add(new Channel(34, "Lee"));

            EnableExtraMusicQueue = false;

            pCurrentChannel = new Channel(1, "华语");
        }

        private Queue<Song> CloneQueue(Queue<Song> CurrentQueue)
        {
            Queue<Song> TargetQueue = new Queue<Song>();
            foreach (Song s in CurrentQueue)
            {
                TargetQueue.Enqueue(s.Clone());
            }
            return TargetQueue;
        }
        private List<Channel> CloneChannel(List<Channel> CurrentChannel)
        {
            List<Channel> TargetList = new List<Channel>();
            foreach (Channel c in CurrentChannel)
            {
                TargetList.Add(c.Clone());
            }
            return TargetList;
        }
    }
}

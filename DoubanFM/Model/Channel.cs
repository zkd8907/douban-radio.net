using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DoubanFM
{
    public class Channel
    {
        public int ChannelNo;
        public string ChannelName;
        public Channel(int ChannelNo, string ChannelName)
        {
            this.ChannelNo = ChannelNo;
            this.ChannelName = ChannelName;
        }
        public Channel Clone()
        {
            return new Channel(this.ChannelNo, this.ChannelName);
        }
    }
}

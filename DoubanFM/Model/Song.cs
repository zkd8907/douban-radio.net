using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DoubanFM
{
    public class Song
    {
        public string picture { get; set; }
        public string albumtitle { get; set; }
        public string company { get; set; }
        public double rating_avg { get; set; }
        public string public_time { get; set; }
        public string ssid { get; set; }
        public string album { get; set; }
        public bool like { get; set; }
        public string artist { get; set; }
        public string url { get; set; }
        public string title { get; set; }
        public string subtype { get; set; }
        public double length { get; set; }
        public string sid { get; set; }
        public string aid { get; set; }

        public Song Clone()
        {
            Song NewSong = new Song();
            NewSong.picture = this.picture;
            NewSong.albumtitle = this.albumtitle;
            NewSong.company = this.company;
            NewSong.rating_avg = this.rating_avg;
            NewSong.public_time = this.public_time;
            NewSong.ssid = this.ssid;
            NewSong.album = this.album;
            NewSong.like = this.like;
            NewSong.artist = this.artist;
            NewSong.url = this.url;
            NewSong.title = this.title;
            NewSong.subtype = this.subtype;
            NewSong.length = this.length;
            NewSong.sid = this.sid;
            NewSong.aid = this.aid;
            return NewSong;
        }
    }
}

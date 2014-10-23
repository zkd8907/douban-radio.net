using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DoubanFM
{
    public class ListResultArgs
    {
        public bool r { get; set; }
        public int version_max { get; set; }
        public Song[] song { get; set; }
    }
}

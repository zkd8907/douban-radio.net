using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DoubanFM
{
    public class ReceivedDataArgs: EventArgs
    {
        public readonly string Result;
        public readonly Uri RedirectedURI;
        public ReceivedDataArgs(string Result, Uri RedirectedURI)
        {
            this.Result = Result;
            this.RedirectedURI = RedirectedURI;
        }
    }
}

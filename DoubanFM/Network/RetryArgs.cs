using System;
using System.Collections.Generic;
using System.Text;

namespace DoubanFM
{
    public class RetryArgs: EventArgs
    {
        public readonly int RetriedTimes;
        public readonly Exception Exception;
        public RetryArgs(int RetriedTimes, Exception Exception)
        {
            this.RetriedTimes = RetriedTimes;
            this.Exception = Exception;
        }
    }
}

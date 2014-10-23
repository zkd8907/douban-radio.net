using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DoubanFM
{
    public class ReceivingErrorArgs : EventArgs
    {
        public readonly string ErrorMessage;
        public readonly Exception Exception;
        public ReceivingErrorArgs(string ErrorMessage, Exception Exception)
        {
            this.ErrorMessage = ErrorMessage;
            this.Exception = Exception;
        }
    }
}

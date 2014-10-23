using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Lyrics
{
    public partial class Lrc
    {
        public delegate void GetLrcErrorEventHandler(object sender, string Message);
        public event GetLrcErrorEventHandler GetLrcError;

        public delegate void GetLrcCompleteEventHandler(object sender, string LrcStr);
        public event GetLrcCompleteEventHandler GetLrcComplete;

        public void GetLrc(string Title, string Artist)
        {
            HttpAgent httpAgent = new HttpAgent();
            httpAgent.ReceivedData += new HttpAgent.ReceivedDataEventHandler(httpAgent_ReceivedData);
            httpAgent.ReceivingError += new HttpAgent.ReceivingErrorEventHandler(httpAgent_ReceivingError);

            httpAgent.AddParameter("op", "12");
            httpAgent.AddParameter("count", "1");
            httpAgent.AddParameter("title", Title + "$$" + Artist + "$$$");

            httpAgent.Get(new Uri("http://box.zhangmen.baidu.com/x"));
        }

        private void httpAgent_ReceivedData(object sender, ReceivedDataArgs e)
        {
            string result = e.Result;
            Regex MSL = new Regex("<lrcid>.*?</lrcid>");
            MatchCollection MMSL = MSL.Matches(result);
            if (MMSL.Count == 0)
            {
                if (GetLrcError != null)
                    GetLrcError(sender, "No Found");
            }
            else
            {
                result = MMSL[0].Value.Replace("<lrcid>", "").Replace("</lrcid>", "");

                int LrcID = 0;
                int.TryParse(result, out LrcID);
                if (LrcID == 0)
                {
                    if (GetLrcError != null)
                        GetLrcError(sender, "No Found");
                }
                else
                {
                    HttpAgent httpAgent = new HttpAgent();
                    httpAgent.ReceivedData += new HttpAgent.ReceivedDataEventHandler(LRC_ReceivedData);
                    httpAgent.ReceivingError += new HttpAgent.ReceivingErrorEventHandler(httpAgent_ReceivingError);
                    httpAgent.ReceivedEncoding = Encoding.GetEncoding(936);
                    httpAgent.Get(new Uri("http://box.zhangmen.baidu.com/bdlrc/" + (LrcID / 100).ToString() + "/" + LrcID.ToString() + ".lrc"));
                }
            }
        }

        private void httpAgent_ReceivingError(object sender, ReceivingErrorArgs e)
        {
            if (GetLrcError != null)
                GetLrcError(sender, e.ErrorMessage);
        }

        private void LRC_ReceivedData(object sender, ReceivedDataArgs e)
        {
            if (GetLrcComplete != null)
                GetLrcComplete(sender, e.Result);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization.Json;
using System.IO;

namespace DoubanFM
{
    public partial class API
    {
        private void GetNewList(char type)
        {
            HttpAgent httpAgent = new HttpAgent();
            httpAgent.ReceivedData += new HttpAgent.ReceivedDataEventHandler(httpAgent_ReceivedData);
            httpAgent.ReceivingError += new HttpAgent.ReceivingErrorEventHandler(httpAgent_ReceivingError);

            httpAgent.AddParameter("app_name", "radio_desktop_win");
            httpAgent.AddParameter("version", "100");

            if (CurrentUser != null)
            {
                httpAgent.AddParameter("user_id", CurrentUser.user_id.ToString());
                httpAgent.AddParameter("expire", CurrentUser.expire);
                httpAgent.AddParameter("token", CurrentUser.token);
            }

            if (pCurrentMusic != null)
            {
                httpAgent.AddParameter("sid", pCurrentMusic.sid);
                hStrQueue.Enqueue("|" + pCurrentMusic.sid + ":" + type);
                while (hStrQueue.Count > 20)
                {
                    hStrQueue.Dequeue();
                }
            }

            StringBuilder hStr = new StringBuilder();
            foreach (string s in hStrQueue)
            {
                hStr.Append(s);
            }

            httpAgent.AddParameter("h", hStr.ToString());
            httpAgent.AddParameter("channel", CurrentChannel.ChannelNo.ToString());
            httpAgent.AddParameter("type", type.ToString());
            httpAgent.Get(BASEURI);
        }

        private void httpAgent_ReceivedData(object sender, ReceivedDataArgs e)
        {
            if (!pIsGetNewList)
                return;
            string Result = e.Result;
            var serializer = new DataContractJsonSerializer(typeof(ListResultArgs));
            var mStream = new MemoryStream(Encoding.UTF8.GetBytes(e.Result));
            ListResultArgs result = (ListResultArgs)serializer.ReadObject(mStream);
            GetNewListCompleted(result);
        }

        private void httpAgent_ReceivingError(object sender, ReceivingErrorArgs e)
        {
            if (NextError != null)
                NextError(sender, e.ErrorMessage);
        }
    }
}

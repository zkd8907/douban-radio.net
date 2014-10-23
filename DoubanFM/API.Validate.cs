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
        public delegate void ValidateCompletedEventHandler(User CurrentUser);
        public event ValidateCompletedEventHandler ValidateCompleted;

        public void Validate(string Username, string Password)
        {
            if (Username == null || Password == null || Username.Length == 0 || Password.Length == 0)
            {
                Exception ex = new Exception("Username or password cannot be empty.");
                throw ex;
            }
            HttpAgent httpAgent = new HttpAgent();

            httpAgent.ReceivedData += new HttpAgent.ReceivedDataEventHandler(Validate_ReceivedData);
            httpAgent.ReceivingError += new HttpAgent.ReceivingErrorEventHandler(Validate_ReceivingError);

            httpAgent.ClearParameters();
            httpAgent.AddParameter("email", Username);
            httpAgent.AddParameter("password", Password);
            httpAgent.AddParameter("app_name", "radio_desktop_win");
            httpAgent.AddParameter("version", "100");
            httpAgent.Post(new Uri("http://www.douban.com/j/app/login"));
        }

        private void Validate_ReceivedData(object sender, ReceivedDataArgs e)
        {
            string Result = e.Result;
            var serializer = new DataContractJsonSerializer(typeof(User));
            var mStream = new MemoryStream(Encoding.UTF8.GetBytes(e.Result));
            User result = (User)serializer.ReadObject(mStream);
            CurrentUser = result;
            this.CurrentUser = CurrentUser;
            pMusicQueue.Clear();
            if (ValidateCompleted != null)
            {
                ValidateCompleted(CurrentUser);
            }
        }

        private void Validate_ReceivingError(object sender, ReceivingErrorArgs e)
        {
            throw e.Exception;
        }
    }
}

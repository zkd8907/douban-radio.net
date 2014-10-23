using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Threading;
using System.IO;

namespace Lyrics
{
    public class HttpAgent
    {
#region events and events handler
        public delegate void ReceivedDataEventHandler(object sender, ReceivedDataArgs e);
        public delegate void ReceivingErrorEventHandler(object sender, ReceivingErrorArgs e);
        public delegate void RetryEventHandler(object sender, RetryArgs e);
        
        /// <summary>
        /// Occurs when received the data.
        /// </summary>
        public event ReceivedDataEventHandler ReceivedData;
        /// <summary>
        /// Occurs when received on error.
        /// </summary>
        public event ReceivingErrorEventHandler ReceivingError;
        /// <summary>
        /// Occurs when retry the operation.
        /// </summary>
        public event RetryEventHandler Retry;
#endregion
#region private members
        private Uri pURI = null;
        private Dictionary<string, string> pParametersContainer = new Dictionary<string, string>();
        private Dictionary<string, string> pHeaders = new Dictionary<string, string>();
        private CookieContainer pCookies = null;
        private string pPostData = string.Empty;
        private bool pIsBusy = false;
        private Thread pThread = null;
        private int pRetriedTimes = 0;
#endregion       
#region public members
        /// <summary>
        /// Gets or sets the object that contains data about the control.
        /// </summary>
        public object Tag { get; set; }

        /// <summary>
        /// Gets or sets the encoding of send data.
        /// </summary>
        public Encoding SendEncoding { get; set; }

        /// <summary>
        /// Gets or sets the encoding of received data.
        /// </summary>
        public Encoding ReceivedEncoding { get; set; }

        /// <summary>
        /// Gets or sets the credential cache.
        /// </summary>
        public CredentialCache CredentialCache { get; set; }

        /// <summary>
        /// Gets or sets the proxy of network operation.
        /// </summary>
        public WebProxy Proxy { get; set; }

        /// <summary>
        /// Gets or sets the timeout of network operation. The timeout must be between 1000 and 60000.
        /// </summary>
        public int Timeout { get; set; }

        /// <summary>
        /// Gets or sets the retry times of network operation. The retry times must be between 0 and 100.
        /// </summary>
        public int RetryTimes { get; set; }

        /// <summary>
        /// Gets whether the operation is busy.
        /// </summary>
        public bool IsBusy
        {
            get { return pIsBusy; }
        }
#endregion
#region constructor list
        public HttpAgent()
        {
            pCookies = new CookieContainer();
            Tag = null;
            Timeout = 100000;
            SendEncoding = Encoding.UTF8;
            ReceivedEncoding = Encoding.UTF8;
        }
        public HttpAgent(CookieContainer cookieContainer)
        {
            pCookies = cookieContainer;
            Tag = null;
            Timeout = 100000;
            SendEncoding = Encoding.UTF8;
            ReceivedEncoding = Encoding.UTF8;
        }
#endregion
#region public methods
        /// <summary>
        /// Add a new uri parameter.
        /// </summary>
        /// <param name="Name">Parameter key</param>
        /// <param name="Value">Parameter value</param>
        public void AddParameter(string Name, string Value)
        {
            if (pParametersContainer.ContainsKey(Name))
                pParametersContainer[Name] = Value;
            else
                pParametersContainer.Add(Name, Value);
        }
        /// <summary>
        /// Remove a uri parameter.
        /// </summary>
        /// <param name="Name">Parameter key</param>
        public void RemoveParameter(string Name)
        {
            if(pParametersContainer.ContainsKey(Name))
                pParametersContainer.Remove(Name);
        }
        /// <summary>
        /// Clear all the uri parameters.
        /// </summary>
        public void ClearParameters()
        {
            pParametersContainer.Clear();
        }

        /// <summary>
        /// Add a new http header.
        /// </summary>
        /// <param name="Name">Header key</param>
        /// <param name="Value">Header value</param>
        public void AddHeader(string Name, string Value)
        {
            if (pHeaders.ContainsKey(Name))
                pHeaders[Name] = Value;
            else
                pHeaders.Add(Name, Value);
        }
        /// <summary>
        /// Remove a http header.
        /// </summary>
        /// <param name="Name">Header key</param>
        public void RemoveHeader(string Name)
        {
            if(pHeaders.ContainsKey(Name))
                pHeaders.Remove(Name);
        }
        /// <summary>
        /// Clear all the http headers.
        /// </summary>
        public void ClearHeaders()
        {
            pHeaders.Clear();
        }
        /// <summary>
        /// Post data to a uri.
        /// </summary>
        /// <param name="URI">Uri address</param>
        public void Post(Uri URI)
        {
            StringBuilder PostData = new StringBuilder();
            pURI = URI;
            foreach (string name in pParametersContainer.Keys)
            {
                PostData.Append(Uri.EscapeUriString(name) + "=" + Uri.EscapeUriString(pParametersContainer[name]) + "&");
            }
            pPostData = PostData.Remove(PostData.Length - 1, 1).ToString();
            pRetriedTimes = 0;

            SynchronizationContext sc = SynchronizationContext.Current;
            pThread = new Thread(new ParameterizedThreadStart(OnRequest));
            pThread.IsBackground = false;
            pThread.Start((object)sc);
        }
        /// <summary>
        /// Get data to a uri.
        /// </summary>
        /// <param name="URI">Uri address</param>
        public void Get(Uri URI)
        {
            StringBuilder PostData = new StringBuilder();
            foreach (string name in pParametersContainer.Keys)
            {
                PostData.Append(Uri.EscapeUriString(name) + "=" + Uri.EscapeUriString(pParametersContainer[name]) + "&");
            }
            if (PostData.Length != 0)
                pURI = new Uri(URI.ToString() + "?" + PostData.Remove(PostData.Length - 1, 1).ToString());
            else
                pURI = URI;
            pPostData = string.Empty;
            pRetriedTimes = 0;

            SynchronizationContext sc = SynchronizationContext.Current;
            pThread = new Thread(new ParameterizedThreadStart(OnRequest));
            pThread.IsBackground = false;
            pThread.Start((object)sc);
        }
#endregion
#region private methods
        private void OnRequest(object obj)
        {
            try
            {
                if (obj == null)
                {
                    obj = (object)(new SynchronizationContext());
                }
                SynchronizationContext sc = (SynchronizationContext)obj;

                if (pIsBusy)
                {
                    sc.Post(new SendOrPostCallback(p =>
                    {
                        OnError((ReceivingErrorArgs)p);
                    }), new ReceivingErrorArgs("Operation is busy now.", null));
                    return;
                }
                pIsBusy = true;

                bool Method = (pPostData.Length > 0);
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(pURI);
                request.CookieContainer = pCookies;
                // request.Timeout = Timeout;
                // request.ReadWriteTimeout = Timeout;
                if (Proxy != null)
                    request.Proxy = Proxy;

                foreach (string name in pHeaders.Keys)
                {
                    request.Headers.Add(name, pHeaders[name]);
                }
                if (Method)
                {
                    request.Method = "POST";
                    request.ContentType = "application/x-www-form-urlencoded";
                    request.ContentLength = pPostData.Length;
                }
                else
                {
                    request.Method = "GET";
                }

                if (Method)
                {
                    using (Stream writeStream = request.GetRequestStream())
                    {
                        UTF8Encoding encodingUTF8 = new UTF8Encoding();
                        byte[] bytes = SendEncoding.GetBytes(pPostData.ToString());
                        writeStream.Write(bytes, 0, bytes.Length);
                    }
                }

                String ResultStr = "";
                Uri RedirectURI;
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    string c = response.StatusDescription;
                    using (Stream responseStream = response.GetResponseStream())
                    {
                        using (StreamReader readStream = new StreamReader(responseStream, ReceivedEncoding))
                        {
                            ResultStr = readStream.ReadToEnd();
                            RedirectURI = response.ResponseUri;
                        }
                    }
                }

                sc.Post(new SendOrPostCallback(p =>
                {
                    OnReceived((ReceivedDataArgs)p);
                }), new ReceivedDataArgs(ResultStr, RedirectURI));
            }
            catch (System.Exception ex)
            {
                if (obj == null)
                {
                    obj = (object)(new SynchronizationContext());
                }
                SynchronizationContext sc = (SynchronizationContext)obj;

                if (pRetriedTimes == RetryTimes)
                {
                    sc.Post(new SendOrPostCallback(p =>
                    {
                        OnError((ReceivingErrorArgs)p);
                    }), new ReceivingErrorArgs("Retry has reached the maximum.", ex));
                    return;
                }
                else
                {
                    pRetriedTimes += 1;
                    sc.Post(new SendOrPostCallback(p =>
                    {
                        OnRetry((RetryArgs)p);
                    }), new RetryArgs(pRetriedTimes, ex));
                    OnRequest(obj);
                }
            }

        }

        private void OnError(ReceivingErrorArgs e)
        {
            if (ReceivingError != null)
                ReceivingError(this, e);
        }
        private void OnRetry(RetryArgs e)
        {
            if (Retry != null)
                Retry(this, e);
        }
        private void OnReceived(ReceivedDataArgs e)
        {
            if (ReceivedData != null)
                ReceivedData(this, e);
        }
#endregion
    }
}

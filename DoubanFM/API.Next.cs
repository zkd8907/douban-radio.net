using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DoubanFM
{
    public partial class API
    {
        public delegate void NextCompletedEventHandler(object sender, Song CurrentMusic);
        public event NextCompletedEventHandler NextCompleted;
        public delegate void NextErrorEventHandler(object sender, string Message);
        public event NextErrorEventHandler NextError;
        public void Next(bool IsPlaying)
        {
            if (pCurrentChannel == null)
                return;
            if (pLastChannel == null)
                pLastChannel = CurrentChannel;
            if (pLastChannel.ChannelNo != pCurrentChannel.ChannelNo)
            {
                pLastChannel = pCurrentChannel;
                pMusicQueue.Clear();
            }
            pIsPlaying = IsPlaying;

            if (pCurrentMusic != null)
            {
                pHistoryQueue.Enqueue(pCurrentMusic);
                while (pHistoryQueue.Count > 20)
                    pHistoryQueue.Dequeue();
                pLastMusic = pCurrentMusic;
            }

            if (pMusicQueue.Count > 0)
            {
                if (pIsLastOne)
                {
                    pIsLastOne = false;
                    return;
                }
                pCurrentMusic = pMusicQueue.Dequeue();
                if (NextCompleted != null)
                {
                    NextCompleted(this, pCurrentMusic.Clone());
                }

                if (pMusicQueue.Count == 0)
                {
                    pIsLastOne = true;
                    pIsGetNewList = true;
                    if (EnableExtraMusicQueue)
                    {
                        pGetMusicTimes = 0;
                        for (int i = 0; i < ExtraMusicTimes; i++)
                        {
                            if (IsPlaying)
                            {
                                GetNewList('p');
                            }
                            else
                            {
                                GetNewList('n');
                            }
                        }
                    }
                    else
                    {
                        if (IsPlaying)
                        {
                            GetNewList('p');
                        }
                        else
                        {
                            GetNewList('n');
                        }
                    }   
                }
                else
                {
                    pIsGetNewList = false;
                    if (IsPlaying)
                    {
                        GetNewList('s');
                    }
                    else
                    {
                        GetNewList('e');
                    }
                }

            }
            else
            {
                pIsLastOne = false;
                pIsGetNewList = true;
                if (EnableExtraMusicQueue)
                {
                    pGetMusicTimes = 0;
                    for (int i = 0; i < ExtraMusicTimes; i++)
                    {
                        GetNewList('n');
                    }
                }
                else
                {
                    GetNewList('n');
                }
            }
        }
        private void GetNewListCompleted(ListResultArgs e)
        {
            if (e.r)
                return;
            foreach (Song s in e.song)
            {
                if (s.subtype != "T")
                    pMusicQueue.Enqueue(s);
            }
            if (EnableExtraMusicQueue)
            {
                pGetMusicTimes += 1;
                if (pGetMusicTimes == ExtraMusicTimes)
                    Next(pIsPlaying);
            }
            else
            {
                Next(pIsPlaying);
            }
            
        }
    }
}

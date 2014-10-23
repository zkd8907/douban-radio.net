using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DoubanFM
{
    public partial class API
    {
        public void ToggleRate()
        {
            if (pCurrentMusic == null)
                return;
            pIsGetNewList = false;
            pCurrentMusic.like = !pCurrentMusic.like;
            if (pCurrentMusic.like)
                GetNewList('r');
            else
                GetNewList('u');
        }
    }
}

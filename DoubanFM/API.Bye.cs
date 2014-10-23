using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DoubanFM
{
	public partial class API
	{
        public void Bye()
        {
            if (pCurrentMusic == null)
                return;
            GetNewList('b');
            Next(true);
        }
	}
}

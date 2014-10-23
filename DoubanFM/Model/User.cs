using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DoubanFM
{
    public class User
    {
        public Int32 user_id { get; set; }
        public string err { get; set; }
        public string token { get; set; }
        public bool r { get; set; }
        public string expire { get; set; }
        public string user_name { get; set; }
        public User Clone()
        {
            User NewUser = new User();
            NewUser.user_id = this.user_id;
            NewUser.user_name = this.user_name;
            NewUser.err = this.err;
            NewUser.token = this.token;
            NewUser.r = this.r;
            NewUser.expire = this.expire;
            return NewUser;
        }
    }
}

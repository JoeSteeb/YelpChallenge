using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YelpApp
{
   
        public class pUser
        {
            public string userName { get; set; }
            public string stars { get; set; }
            public string yelpingSince { get; set; }
            public string tipLikes { get; set; }
        }

        public class pTip
        {
            public string uName { get; set; }
            public string bName { get; set; }
            public string city { get; set; }
            public string text { get; set; }
            public string date { get; set; }
        }
}

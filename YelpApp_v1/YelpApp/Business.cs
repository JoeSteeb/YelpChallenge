using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YelpApp
{
    public class Business
    {
        public string bID { get; set; }
        public string bName { get; set; }
        public string bAddress { get; set; }
        public string longitude { get; set; }
        public string latitude { get; set; }
        public string stars { get; set; }
        public string is_open { get; set; }
        public string tip_count { get; set; }
        public string numCheckins { get; set; }
    }
}

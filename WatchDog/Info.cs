using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WatchDog
{
    public class Info
    {
        public string path { get; set; }
        public string span { get; set; }
        public string stat { get; set; }
        public int count { get; set; }
        public string time { get; set; }

        public Info()
        {
            path = "";
            span = "";
            stat = "";
            count = 0;
            time = "";
        }
    }
}

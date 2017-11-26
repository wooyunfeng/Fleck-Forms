using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Fleck_Forms
{
    class COpenBook
    {
        public Int64 id { get; set; }
        public Int64 vkey { get; set; }
        public Int64 vmove { get; set; }
        public Int64 vscore { get; set; }
        public Int64 vwin { get; set; }
        public Int64 vdraw { get; set; }
        public Int64 vlost { get; set; }
        public Int64 vvalid { get; set; }
   //    public string vmemo { get; set; }
        public Int64 vindex { get; set; }
        public COpenBook()
        {
            id = 0;
            vkey = 0;
            vmove = 0;
            vscore = 0;
            vwin = 0;
            vdraw = 0;
            vlost = 0;
            vvalid = 0;
           // vmemo = "";
            vindex = 0;
        }
    }
}

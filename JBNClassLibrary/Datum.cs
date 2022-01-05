using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JBNClassLibrary
{
    public class Datum
    {
        public string id { get; set; }
        public string customid { get; set; }
        public string customid1 { get; set; }
        public string customid2 { get; set; }
        public string mobile { get; set; }
        public string status { get; set; }
    }
    public class RootData
    {
        public string status { get; set; }
        public List<Datum> data { get; set; }
        public string message { get; set; }
    }
}

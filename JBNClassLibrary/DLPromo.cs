using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JBNClassLibrary
{
    public class DLPromo
    {
        public class Promotion
        {   
            public bool IsEmail { get; set; }
            public bool IsSMS { get; set; }
            public bool IsWhatsApp { get; set; }
            public string Message { get; set; }
            public List<CustomerDetails> customerList { get; set; }
        }
    }
}

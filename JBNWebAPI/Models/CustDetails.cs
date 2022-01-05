using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace JBNWebAPI.Models
{
    public class CustDetails
    {
        public int? CustID { get; set; }
        public int? IsRegistered { get; set; }
        public string UserType { get; set; }
        public string Password { get; set; }
    }
}
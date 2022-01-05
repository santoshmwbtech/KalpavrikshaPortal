using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace JBNAdminPortal.Models
{
    public class SearchEnquiry
    {
        public int? CustID { get; set; }
        public int? BusinessTypeID { get; set; }
        public int? BusinessDemandID { get; set; }
        public int? ItemID { get; set; }
        public string FirmName { get; set; }
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public int? EnquiryState { get; set; }
        public int? EnquiryCity { get; set; }
        public int? CustomerState { get; set; }
        public int? CustomerCity { get; set; }
        public string PurposeOfBusiness { get; set; }
    }
}
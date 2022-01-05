using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JBNClassLibrary
{
    public class Enquiries
    {
        public int? QueryId { get; set; }
        public int? CityId { get; set; }
        public string VillageLocalityname { get; set; }
        public string BusinessDemand { get; set; }
        public string PurposeOfBusiness { get; set; }
        public DateTime? LastUpdatedMsgDate { get; set; }
        public string StrLastUpdatedMsgDate { get; set; }
        public int IsDeleted { get; set; }
        public string ChildCategoryName { get; set; }
        public int ReplyCount { get; set; }
        public DateTime CreatedDate { get; set; }
        public string RequirementName { get; set; }
        public DateTime? EnquiryDate { get; set; }
        public string EnquiryType { get; set; }
    }
}

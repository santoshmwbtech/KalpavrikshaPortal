using System.Collections.Generic;

namespace JBNClassLibrary
{
    public class SearchParameters
    {
        public int CustID { get; set; }
        public int? CityId { get; set; }
        public List<City> CityIdList { get; set; }
        public int? ProductID { get; set; }
        public List<BusinessTypeId> BusinessTypeIds { get; set; }
        public List<tblBusinessDemand> BusinessDemand { get; set; }
        public string TypeOfUse { get; set; }
        public string Requirements { get; set; }
        public string ProductPhoto { get; set; }
        public int IsFavorite { get; set; }
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public int IsSentEnquiry { get; set; }
        public int IsReceivedEnquiry { get; set; }
        public string EnquiryType { get; set; }
    }

    public class BusinessTypeId
    {
        public int BusinessTypeID { get; set; }
    }

    public class SentEnquirySearchParameters
    {
        public int QueryID { get; set; }
        public int CustID { get; set; }
        public int ReceiverID { get; set; }
        public int? CityId { get; set; }
        public List<City> CityIdList { get; set; }
        public List<BusinessTypeId> BusinessTypeIds { get; set; }
        public List<tblBusinessDemand> BusinessDemand { get; set; }
        public string TypeOfUse { get; set; }
        public string Requirements { get; set; }
        public string ProductPhoto { get; set; }
        public int IsFavorite { get; set; }
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public string EnquiryType { get; set; }
    }
}

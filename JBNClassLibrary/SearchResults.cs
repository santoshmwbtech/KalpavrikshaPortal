using System;
using System.Collections.Generic;
using System.Net;

namespace JBNClassLibrary
{
    public class SearchResults
    {
        public int SearchID { get; set; }
        public int CustID { get; set; }
        public string MobileNumber { get; set; }
        public string CustomerName { get; set; }
        public string BusinessType { get; set; }
        public string BusinessTypeID { get; set; }
        public int? ChildCategoryId { get; set; }
        public int? City { get; set; }
        public string VillageLocalityname { get; set; }
        public bool InterestCity { get; set; }
        public bool InterestState { get; set; }
        public bool InterestCountry { get; set; }
        public int? SubCategoryId { get; set; }
    }

    public class SearchDistinctList
    {
        public int CustID { get; set; }
        public string MobileNumber { get; set; }
        public string CustomerName { get; set; }
        public string BusinessType { get; set; }
        public int? City { get; set; }
        public string VillageLocalityname { get; set; }
    }

    public class SubmitQuery
    {
        public int QueryId { get; set; }
        public List<CustIDS> CustIDS { get; set; }
        public int CustID { get; set; }
        public int? CityId { get; set; }
        public int? ProductID { get; set; }
        public List<BusinessTypeId> BusinessTypeIds { get; set; }
        public string BusinessDemand { get; set; }
        public int? BusinessDemandID { get; set; }
        public string TypeOfUse { get; set; }
        public string Requirements { get; set; }
        public string ProductPhoto { get; set; }
        public string ProductPhoto2 { get; set; }
        public int? ProfessionalRequirementID { get; set; }
        public Nullable<bool> IsAdEnquiry { get; set; }
        public string EnquiryType { get; set; }
        public string ResponseMessage { get; set; }
        public HttpStatusCode StatusCode { get; set; }
    }

    public class CustIDS
    {
        public int CustID { get; set; }
        public string FirmName { get; set; }
        public List<BusinessTypes> businessTypes { get; set; }
        public int? CityId { get; set; }
        public int StateID { get; set; }
        public string StateName { get; set; }
    }

    public class CustomerQueries
    {
        public int? QueryId { get; set; }
        public int? CityId { get; set; }
        public string BusinessTID { get; set; }
        public List<tblBusinessTypewithCust> BusinessTypes { get; set; }
        public int CustID { get; set; }
        public string FirmName { get; set; }
        public string MobileNumber { get; set; }
        public string EmailID { get; set; }
        public string VillageLocalityname { get; set; }
        public string BusinessDemand { get; set; }
        public int? BusinessDemandID { get; set; }
        public string PurposeOfBusiness { get; set; }
        public string Requirements { get; set; }
        public string ProductPhoto { get; set; }
        public string ProductPhoto2 { get; set; }
        public DateTime? LastUpdatedMsgDate { get; set; }
        //public string StrLastUpdatedMsgDate { get; set; }
        public int IsRead { get; set; }
        public int SenderRead { get; set; }
        public int SenderID { get; set; }
        public string ChildCategoryName { get; set; }
        public int IsFavorite { get; set; }
        public string FavoriteCustID { get; set; }
        public int IsDeleted { get; set; }
        public int CreatedBy { get; set; }
        public string RequirementName { get; set; }
        public int? ProfessionalRequirementID { get; set; }
        public DateTime? EnquiryDate { get; set; }
        public string EnquiryType { get; set; }
        public string SenderImage { get; set; }
        public byte[] UserPhoto { get; set; }
    }
    public class FavoriteConversations
    {
        public int? QueryId { get; set; }
        public int? CityId { get; set; }
        public string BusinessTID { get; set; }
        public List<tblBusinessTypewithCust> BusinessTypes { get; set; }
        public int CustID { get; set; }
        public string FirmName { get; set; }
        public string MobileNumber { get; set; }
        public string EmailID { get; set; }
        public string VillageLocalityname { get; set; }
        public string BusinessDemand { get; set; }
        public int? BusinessDemandID { get; set; }
        public string PurposeOfBusiness { get; set; }
        public string Requirements { get; set; }
        public string ProductPhoto { get; set; }
        public string ProductPhoto2 { get; set; }
        public DateTime? LastUpdatedMsgDate { get; set; }
        //public string StrLastUpdatedMsgDate { get; set; }
        public int IsRead { get; set; }
        public int SenderRead { get; set; }
        public int SenderID { get; set; }
        public string ChildCategoryName { get; set; }
        public int IsFavorite { get; set; }
        public string FavoriteCustID { get; set; }
        public int IsDeleted { get; set; }
        public int CreatedBy { get; set; }
        public int isSentEnquiry { get; set; }
        public string RequirementName { get; set; }
        public int? ProfessionalRequirementID { get; set; }
        public DateTime? EnquiryDate { get; set; }
        public string EnquiryType { get; set; }
        public string SenderImage { get; set; }
        public byte[] UserPhoto { get; set; }
    }
}

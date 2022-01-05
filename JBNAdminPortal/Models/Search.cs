using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace JBNAdminPortal.Models
{
    public class Search
    {
        public int? CustID { get; set; }
        public int? BusinessTypeID { get; set; }
        public int? BusinessDemandID { get; set; }
        public int? SubCategoryID { get; set; }
        public int? CategoryProductID { get; set; }
        public int? ItemID { get; set; }
        public string FirmName { get; set; }
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public string FromTime { get; set; }
        public string ToTime { get; set; }
        public string MobileNumber { get; set; }
        public int? StateID { get; set; }
        public int? StatewithCityID { get; set; }
        public string VillageLocalityName { get; set; }
        public int? IsActive { get; set; }
        public int[] StateList { get; set; }
        public int[] CityList { get; set; }
        public int[] BusinessTypeList { get; set; }
        public int[] BusinessTypeIDStrList { get; set; }
        public int[] SubCategoryList { get; set; }
        public int[] MainCategoryList { get; set; }
        public int[] ChildCategoryList { get; set; }
        public int[] ItemCategoryList { get; set; }
        public int[] CustomerList { get; set; }
        public string PurposeOfBusiness { get; set; }
        public int? NumberofDays { get; set; }
        public Nullable<bool> InterstCity { get; set; }
        public Nullable<bool> InterstState { get; set; }
        public Nullable<bool> InterstCountry { get; set; }
        public string EnquiryType { get; set; }
    }
}
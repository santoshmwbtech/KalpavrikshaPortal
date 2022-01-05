using System;
using System.Collections.Generic;

namespace JBNClassLibrary
{
    public class CustomerDetails
    {
        public int CustID { get; set; }
        public string DeviceID { get; set; }
        public string FirmName { get; set; }
        public string Password { get; set; }
        public string UserType { get; set; }
        public string CustName { get; set; }
        public string EmailID { get; set; }
        public string MobileNumber { get; set; }
        public string MobileNumber2 { get; set; }
        public string TelephoneNumber { get; set; }
        public string ShopImage { get; set; }
        public string BillingAddress { get; set; }
        public string Area { get; set; }
        public int? City { get; set; }
        public City city { get; set; }
        public string CityCode { get; set; }
        public int? State { get; set; }
        public State state { get; set; }
        public District district { get; set; }
        public string Pincode { get; set; }
        public string Lattitude { get; set; }
        public string Langitude { get; set; }
        public Nullable<bool> InterstCity { get; set; }
        public Nullable<bool> InterstState { get; set; }
        public Nullable<bool> InterstCountry { get; set; }
        public string RegistrationType { get; set; }
        public string TinNumber { get; set; }
        public string PanNumber { get; set; }
        public string Bankname { get; set; }
        public string BankBranchName { get; set; }
        public string Accountnumber { get; set; }
        public string IFSCCode { get; set; }
        public string BankCity { get; set; }
        public Nullable<int> SalesmanID { get; set; }
        public Nullable<int> CreatedByID { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<int> IsRegistered { get; set; }
        public Nullable<int> UpdatedByID { get; set; }
        public string AdditionalPersonName { get; set; }
        public Nullable<System.DateTime> UpdatedByDate { get; set; }
        //public List<BusinessTypeWithCusts> BusinessTypeWithCust { get; set; }
        public List<BusinessTypeWithCusts> BusinessTypeWithCust { get; set; }
        public List<CategoryTypeWithCust> CategoryTypeWithCust { get; set; }
        public List<SubCategoryProducts> SubCategoryTypeWithCust { get; set; }
        public List<ChildCategoryTypeWithCust> ChildCategoryTypeWithCust { get; set; }
        public string SMSOTP { get; set; }
        public Nullable<bool> IsOTPVerified { get; set; }
        public bool IsActive { get; set; }
        public string ReasonForDeactivate { get; set; }
        public string RegisteredDate { get; set; }
        public int[] StateList { get; set; }
        public int[] CityList { get; set; }
        public int[] BusinessTypeList { get; set; }
        public int[] SubCategoryList { get; set; }
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public string FromTime { get; set; }
        public string ToTime { get; set; }
        public int TotalStates { get; set; }
        public int TotalCities { get; set; }
        public int TotalCustomers { get; set; }
        public int BlockedCustomers { get; set; }
        public int TotalSubCategories { get; set; }
        public List<StatusHistory> statusHistories { get; set; }
        public bool IsBGSMember { get; set; }
        public string UserImage { get; set; }
        public byte[] UserPhoto { get; set; }
        public bool IsTAndCAgreed { get; set; }
        public string TAndCVersion { get; set; }
        public string DisplayMessage { get; set; }
        public string CategoriesStr { get; set; }
        public string SubCategoriesStr { get; set; }
    }

    public partial class tblCustomerDetail_Questions
    {
        public int CustID { get; set; }
        public string FirmName { get; set; }
        public string MobileNumber { get; set; }
        public Nullable<bool> IsActive { get; set; }
        public Nullable<bool> IsBlocked { get; set; }
        public Nullable<System.DateTime> BlockedDate { get; set; }
        public tblQuestion tblQuestion { get; set; }
        public Nullable<bool> Blockstatus { get; set; }
        public Nullable<bool> QuestionAnswered { get; set; }
        public bool IsQuestionRequired { get; set; }

    }    
  
    public class State
    {
        public int? StateID { get; set; }
        public string StateName { get; set; }
        public Nullable<double> Matrix { get; set; }
    }
    public class District
    {
        public int? DistrictID { get; set; }
        public string DistrictName { get; set; }
    }
    public class City
    {
        public int? StateWithCityID { get; set; }
        public string VillageLocalityName { get; set; }
    }
    public class BusinessTypeWithCusts
    {
        public int ID { get; set; }
        public int BusinessTypeID { get; set; }
        public Nullable<int> CustID { get; set; }
        public string BusinessTypeName { get; set; }
        public bool Checked { get; set; }
    }
    public class CategoryTypeWithCust
    {
        public int ID { get; set; }
        public Nullable<int> CategoryProductID { get; set; }
        public Nullable<int> CustID { get; set; }
        public string MainCategoryName { get; set; }
    }
    public class SubCategoryTypeWithCust
    {
        public int ID { get; set; }
        public Nullable<int> CategoryProductID { get; set; }
        public Nullable<int> CustID { get; set; }
        public Nullable<int> SubCategoryId { get; set; }
        public string SubCategoryName { get; set; }
    }
    public class ChildCategoryTypeWithCust
    {
        public int ID { get; set; }
        public Nullable<int> SubCategoryId { get; set; }
        public Nullable<int> CustID { get; set; }
        public Nullable<int> ChildCategoryId { get; set; }
        public string ChildCategoryName { get; set; }
    }

    public class CatCityWiseCustListParameters
    {
        public int? CityId { get; set; }
        public int? ProductID { get; set; }
        public int ProductType { get; set; }
    }

    public class CityWiseDetailedRpt
    {
        public int CustID { get; set; }
        public int? CityID { get; set; }
        public int? StateID { get; set; }
        public string CityName { get; set; }
        public string StateName { get; set; }
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public string Mobile1 { get; set; }
        public string EmailID { get; set; }
        public string Address { get; set; }
        public string FirmName { get; set; }
        public string DealerName { get; set; }
        public string ContactPersonName { get; set; }
        public int SubCategoryID { get; set; }
        public int BusinessTypeID { get; set; }
        public List<BusinessTypes> BusinessTypesList { get; set; }
        public List<SubCat> SubCategoryList { get; set; }
        public string TypeOfBusiness { get; set; }
        public bool IsChecked { get; set; }
    }
    public class StatusHistory
    {
        public int ID { get; set; }
        public Nullable<int> CustID { get; set; }
        public string Comments { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public string CreatedByUser { get; set; }
        public string CustomerStatus { get; set; }
    }
    public class AdvertisementHoliday
    {
        public string HolidayName { get; set; }
        public string HolidayType { get; set; }
        public Nullable<double> HolidayMatrix { get; set; }
        public Nullable<System.DateTime> HolidayDate { get; set; }
        public string DayName { get; set; }
        public Nullable<int> StateID { get; set; }
        public string HolidayDefinition { get; set; }
    }
    public class PushNotifications
    {
        public int ID { get; set; }
        public Nullable<int> CustID { get; set; }
        public Nullable<System.DateTime> NotificationDate { get; set; }
        public string NotificationDateStr { get; set; }
        public string PushNotification { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public string Title { get; set; }
        public string CategoryName { get; set; }
        public string ImageURL { get; set; }
    }
}
using System;
using System.Collections.Generic;

namespace JBNClassLibrary
{
    public class CustDetails
    {
        public int? CustID { get; set; }
        //public string DeviceID { get; set; }
        public int? IsRegistered { get; set; }
        public bool IsActive { get; set; }
        public string UserType { get; set; }
        public string Password { get; set; }
        public string MobileNumber { get; set; }
        public string SMSOTP { get; set; }
        public Nullable<bool> IsOTPVerified { get; set; }
        public Nullable<bool> QuestionAnswered { get; set; }
        public string Question { get; set; }
        public string AnswerForQuestion { get; set; }
        public string OTPStatus { get; set; }
        public int CityID { get; set; }
        public string VillageLocalityName { get; set; }
        public int StateID { get; set; }
        public string StateName { get; set; }
        public int ChildCategoryId { get; set; }
        public string ChildCategoryName { get; set; }
        public string FullScreenAdURL { get; set; }
        public string BusinessTypeID { get; set; }
        public List<BusinessTypes> businessTypes { get; set; }
        public int AdUserID { get; set; }
        public string AdFirmName { get; set; }
        public List<State> States { get; set; }
        public List<City> Cities { get; set; }
    }
}
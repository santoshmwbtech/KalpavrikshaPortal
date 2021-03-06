//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace JBNClassLibrary
{
    using System;
    using System.Collections.Generic;
    
    public partial class tblAdvertisement
    {
        public int ID { get; set; }
        public Nullable<int> AdvertisementMainID { get; set; }
        public string AdvertisementName { get; set; }
        public Nullable<int> BrandID { get; set; }
        public Nullable<int> CustID { get; set; }
        public Nullable<int> TypeOfAdvertisementID { get; set; }
        public Nullable<int> AdTimeSlotID { get; set; }
        public Nullable<int> ProductID { get; set; }
        public Nullable<System.DateTime> FromDate { get; set; }
        public Nullable<System.DateTime> ToDate { get; set; }
        public Nullable<int> DaysCount { get; set; }
        public Nullable<System.TimeSpan> AdStartTime { get; set; }
        public Nullable<System.TimeSpan> CurrentAdTime { get; set; }
        public Nullable<System.TimeSpan> NextAdTime { get; set; }
        public Nullable<int> IntervalsPerDay { get; set; }
        public Nullable<int> CompletedIntervals { get; set; }
        public int CreatedBy { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public Nullable<int> ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public Nullable<int> AdvertisementAreaID { get; set; }
        public Nullable<int> IntervalsPerHour { get; set; }
        public bool TemporaryBooked { get; set; }
        public bool IsCancelled { get; set; }
        public bool IsActive { get; set; }
        public bool IsCompleted { get; set; }
    
        public virtual tblAdvertisementTimeSlot tblAdvertisementTimeSlot { get; set; }
        public virtual tblAdvertisementMain tblAdvertisementMain { get; set; }
        public virtual tblUser tblUser { get; set; }
        public virtual tblAdvertisementArea tblAdvertisementArea { get; set; }
        public virtual tblAdvertisementType tblAdvertisementType { get; set; }
        public virtual tblCustomerDetail tblCustomerDetail { get; set; }
    }
}

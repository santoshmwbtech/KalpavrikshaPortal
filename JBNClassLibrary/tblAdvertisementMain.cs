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
    
    public partial class tblAdvertisementMain
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public tblAdvertisementMain()
        {
            this.tblAdvertisements = new HashSet<tblAdvertisement>();
            this.tblAdvertisementCities = new HashSet<tblAdvertisementCity>();
            this.tblAdvertisementDistricts = new HashSet<tblAdvertisementDistrict>();
            this.tbladvertisementPayments = new HashSet<tbladvertisementPayment>();
            this.tblAdvertisementStates = new HashSet<tblAdvertisementState>();
            this.tblPayments = new HashSet<tblPayment>();
        }
    
        public int ID { get; set; }
        public string AdvertisementName { get; set; }
        public Nullable<int> BrandID { get; set; }
        public Nullable<int> CustID { get; set; }
        public Nullable<int> TypeOfAdvertisementID { get; set; }
        public Nullable<int> ProductID { get; set; }
        public int CreatedBy { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public Nullable<int> ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public Nullable<System.DateTime> FromDate { get; set; }
        public Nullable<System.DateTime> ToDate { get; set; }
        public Nullable<double> AdTotalPrice { get; set; }
        public Nullable<double> FinalPrice { get; set; }
        public Nullable<double> TotalDiscount { get; set; }
        public bool TemporaryBooked { get; set; }
        public bool IsApproved { get; set; }
        public bool IsCancelled { get; set; }
        public bool IsActive { get; set; }
        public bool IsCompleted { get; set; }
        public System.DateTime BookingExpiryDate { get; set; }
        public string InvoiceNumber { get; set; }
        public int ProformaInvoiceNumber { get; set; }
        public Nullable<double> DiscountPer { get; set; }
        public Nullable<double> TaxAmount { get; set; }
        public Nullable<double> TaxValue { get; set; }
        public string AdImageURL { get; set; }
        public string Remarks { get; set; }
        public bool PaymentStatus { get; set; }
        public Nullable<double> CGSTAmount { get; set; }
        public Nullable<double> SGSTAmount { get; set; }
        public Nullable<double> IGSTAmount { get; set; }
        public Nullable<double> CGSTPer { get; set; }
        public Nullable<double> SGSTPer { get; set; }
        public Nullable<double> IGSTPer { get; set; }
        public string AdText { get; set; }
        public Nullable<int> AdvertisementAreaID { get; set; }
        public bool IsRejected { get; set; }
        public Nullable<double> PublicHolidayMatrix { get; set; }
        public Nullable<double> FestivalMatrix { get; set; }
        public Nullable<double> WeekendMatrix { get; set; }
        public string BrandName { get; set; }
        public Nullable<System.DateTime> ContentApprovedDate { get; set; }
        public Nullable<System.DateTime> PaymentApprovedDate { get; set; }
        public Nullable<int> TotalRotations { get; set; }
        public Nullable<int> TotalStates { get; set; }
        public Nullable<int> TotalDistricts { get; set; }
        public Nullable<int> TotalCities { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tblAdvertisement> tblAdvertisements { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tblAdvertisementCity> tblAdvertisementCities { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tblAdvertisementDistrict> tblAdvertisementDistricts { get; set; }
        public virtual tblUser tblUser { get; set; }
        public virtual tblCustomerDetail tblCustomerDetail { get; set; }
        public virtual tblUser tblUser1 { get; set; }
        public virtual tblItemCategory tblItemCategory { get; set; }
        public virtual tblAdvertisementType tblAdvertisementType { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tbladvertisementPayment> tbladvertisementPayments { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tblAdvertisementState> tblAdvertisementStates { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tblPayment> tblPayments { get; set; }
        public virtual tblCustomerDetail tblCustomerDetail1 { get; set; }
    }
}

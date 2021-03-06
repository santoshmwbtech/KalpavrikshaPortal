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
    
    public partial class tblCustomerDetail
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public tblCustomerDetail()
        {
            this.tblAdvertisementMains = new HashSet<tblAdvertisementMain>();
            this.tblBusinessTypewithCusts = new HashSet<tblBusinessTypewithCust>();
            this.tblBusinessTypewithCusts1 = new HashSet<tblBusinessTypewithCust>();
            this.tblBusinessTypewithCusts2 = new HashSet<tblBusinessTypewithCust>();
            this.tblCategoryProductWithCusts = new HashSet<tblCategoryProductWithCust>();
            this.tblCategoryProductWithCusts1 = new HashSet<tblCategoryProductWithCust>();
            this.tblCustomerStatusHistories = new HashSet<tblCustomerStatusHistory>();
            this.tblDeleteChats = new HashSet<tblDeleteChat>();
            this.tblDeleteChats1 = new HashSet<tblDeleteChat>();
            this.tblDeleteChats2 = new HashSet<tblDeleteChat>();
            this.tblDeleteConversations = new HashSet<tblDeleteConversation>();
            this.tblDeleteConversations1 = new HashSet<tblDeleteConversation>();
            this.tblDeleteConversations2 = new HashSet<tblDeleteConversation>();
            this.tblDeleteEnquiries = new HashSet<tblDeleteEnquiry>();
            this.tblDeleteEnquiries1 = new HashSet<tblDeleteEnquiry>();
            this.tblFavoriteConversations = new HashSet<tblFavoriteConversation>();
            this.tblFavoriteConversations1 = new HashSet<tblFavoriteConversation>();
            this.tblFavoriteConversations2 = new HashSet<tblFavoriteConversation>();
            this.tblPayments = new HashSet<tblPayment>();
            this.tblPayments1 = new HashSet<tblPayment>();
            this.tblPushNotifications = new HashSet<tblPushNotification>();
            this.tblselectedDealers = new HashSet<tblselectedDealer>();
            this.tblselectedDealers1 = new HashSet<tblselectedDealer>();
            this.tblSelectedDealerBusinessTypes = new HashSet<tblSelectedDealerBusinessType>();
            this.tblSelectedDealerBusinessTypes1 = new HashSet<tblSelectedDealerBusinessType>();
            this.tblselectedDealerDetails = new HashSet<tblselectedDealerDetail>();
            this.tblselectedDealerDetails1 = new HashSet<tblselectedDealerDetail>();
            this.tblSubCategoryProductWithCusts = new HashSet<tblSubCategoryProductWithCust>();
            this.tblSubCategoryProductWithCusts1 = new HashSet<tblSubCategoryProductWithCust>();
            this.tblUserConversations = new HashSet<tblUserConversation>();
            this.tblUserConversations1 = new HashSet<tblUserConversation>();
            this.tblAdvertisements = new HashSet<tblAdvertisement>();
            this.tblAdvertisementMains1 = new HashSet<tblAdvertisementMain>();
            this.tblBusinessDemandwithCusts = new HashSet<tblBusinessDemandwithCust>();
            this.tblBusinessDemandwithCusts1 = new HashSet<tblBusinessDemandwithCust>();
            this.tblBusinessDemandwithCusts2 = new HashSet<tblBusinessDemandwithCust>();
        }
    
        public int ID { get; set; }
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
        public Nullable<int> City { get; set; }
        public string CityCode { get; set; }
        public Nullable<int> State { get; set; }
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
        public int CreatedBy { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public Nullable<int> IsRegistered { get; set; }
        public Nullable<int> UpdatedByID { get; set; }
        public Nullable<System.DateTime> UpdatedByDate { get; set; }
        public string AdditionalPersonName { get; set; }
        public string SMSOTP { get; set; }
        public Nullable<bool> IsOTPVerified { get; set; }
        public Nullable<int> DistrictID { get; set; }
        public Nullable<bool> QuestionAnswered { get; set; }
        public string Question { get; set; }
        public string AnswerForQuestion { get; set; }
        public string DeviceID { get; set; }
        public Nullable<System.DateTime> LastLoginDate { get; set; }
        public bool IsActive { get; set; }
        public string ReasonForDeactivate { get; set; }
        public Nullable<bool> IsBGSMember { get; set; }
        public string UserImage { get; set; }
        public Nullable<bool> IsBlocked { get; set; }
        public Nullable<System.DateTime> BlockedDate { get; set; }
        public byte[] UserPhoto { get; set; }
        public bool IsTAndCAgreed { get; set; }
        public string TAndCVersion { get; set; }
        public Nullable<int> ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public Nullable<int> BusinessDemandID { get; set; }
        public bool InterstDistrict { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tblAdvertisementMain> tblAdvertisementMains { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tblBusinessTypewithCust> tblBusinessTypewithCusts { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tblBusinessTypewithCust> tblBusinessTypewithCusts1 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tblBusinessTypewithCust> tblBusinessTypewithCusts2 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tblCategoryProductWithCust> tblCategoryProductWithCusts { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tblCategoryProductWithCust> tblCategoryProductWithCusts1 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tblCustomerStatusHistory> tblCustomerStatusHistories { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tblDeleteChat> tblDeleteChats { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tblDeleteChat> tblDeleteChats1 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tblDeleteChat> tblDeleteChats2 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tblDeleteConversation> tblDeleteConversations { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tblDeleteConversation> tblDeleteConversations1 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tblDeleteConversation> tblDeleteConversations2 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tblDeleteEnquiry> tblDeleteEnquiries { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tblDeleteEnquiry> tblDeleteEnquiries1 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tblFavoriteConversation> tblFavoriteConversations { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tblFavoriteConversation> tblFavoriteConversations1 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tblFavoriteConversation> tblFavoriteConversations2 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tblPayment> tblPayments { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tblPayment> tblPayments1 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tblPushNotification> tblPushNotifications { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tblselectedDealer> tblselectedDealers { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tblselectedDealer> tblselectedDealers1 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tblSelectedDealerBusinessType> tblSelectedDealerBusinessTypes { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tblSelectedDealerBusinessType> tblSelectedDealerBusinessTypes1 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tblselectedDealerDetail> tblselectedDealerDetails { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tblselectedDealerDetail> tblselectedDealerDetails1 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tblSubCategoryProductWithCust> tblSubCategoryProductWithCusts { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tblSubCategoryProductWithCust> tblSubCategoryProductWithCusts1 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tblUserConversation> tblUserConversations { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tblUserConversation> tblUserConversations1 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tblAdvertisement> tblAdvertisements { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tblAdvertisementMain> tblAdvertisementMains1 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tblBusinessDemandwithCust> tblBusinessDemandwithCusts { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tblBusinessDemandwithCust> tblBusinessDemandwithCusts1 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tblBusinessDemandwithCust> tblBusinessDemandwithCusts2 { get; set; }
    }
}

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
    
    public partial class tblItemCategory
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public tblItemCategory()
        {
            this.tblAdvertisementMains = new HashSet<tblAdvertisementMain>();
            this.tblCategoryHistories = new HashSet<tblCategoryHistory>();
            this.tblselectedDealers = new HashSet<tblselectedDealer>();
        }
    
        public int ID { get; set; }
        public string ItemName { get; set; }
        public Nullable<int> ChildCategoryID { get; set; }
        public bool IsActive { get; set; }
        public string RefferedByOrReason { get; set; }
        public string ApprovedBy { get; set; }
        public bool IsMasterProduct { get; set; }
        public Nullable<bool> IsRejected { get; set; }
        public string ItemImage1 { get; set; }
        public string ItemImage2 { get; set; }
        public Nullable<double> ItemMatrix { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public int CreatedBy { get; set; }
        public Nullable<int> ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tblAdvertisementMain> tblAdvertisementMains { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tblCategoryHistory> tblCategoryHistories { get; set; }
        public virtual tblChildCategory tblChildCategory { get; set; }
        public virtual tblUser tblUser { get; set; }
        public virtual tblUser tblUser1 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tblselectedDealer> tblselectedDealers { get; set; }
    }
}

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
    
    public partial class tblStateWithCity
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public tblStateWithCity()
        {
            this.tblAdvertisementCities = new HashSet<tblAdvertisementCity>();
            this.tblselectedDealers = new HashSet<tblselectedDealer>();
            this.tblselectedDealerDetails = new HashSet<tblselectedDealerDetail>();
        }
    
        public int ID { get; set; }
        public Nullable<int> StatewithCityID { get; set; }
        public int StateID { get; set; }
        public string VillageLocalityName { get; set; }
        public Nullable<int> DistrictID { get; set; }
        public Nullable<int> TairTypeOfCityID { get; set; }
        public string TairTypeOfCity { get; set; }
        public int CreatedBy { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public Nullable<int> ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tblAdvertisementCity> tblAdvertisementCities { get; set; }
        public virtual tblDistrict tblDistrict { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tblselectedDealer> tblselectedDealers { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tblselectedDealerDetail> tblselectedDealerDetails { get; set; }
        public virtual tblUser tblUser { get; set; }
        public virtual tblUser tblUser1 { get; set; }
    }
}

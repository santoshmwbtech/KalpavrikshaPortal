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
    
    public partial class tblAdvertisementState
    {
        public int ID { get; set; }
        public Nullable<int> AdvertisementID { get; set; }
        public Nullable<int> AdvertisementMainID { get; set; }
        public Nullable<int> StateID { get; set; }
        public Nullable<int> TairTypeOfStateID { get; set; }
        public string TairTypeOfState { get; set; }
        public string StateName { get; set; }
        public int CreatedBy { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public Nullable<int> ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
    
        public virtual tblAdvertisementMain tblAdvertisementMain { get; set; }
        public virtual tblUser tblUser { get; set; }
        public virtual tblUser tblUser1 { get; set; }
        public virtual tblState tblState { get; set; }
        public virtual tblTairTypeOfState tblTairTypeOfState { get; set; }
    }
}

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
    
    public partial class tblBusinessTypewithCust
    {
        public int ID { get; set; }
        public int BusinessTypeID { get; set; }
        public int CustID { get; set; }
        public int CreatedBy { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public Nullable<int> ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
    
        public virtual tblBusinessType tblBusinessType { get; set; }
        public virtual tblCustomerDetail tblCustomerDetail { get; set; }
        public virtual tblCustomerDetail tblCustomerDetail1 { get; set; }
        public virtual tblCustomerDetail tblCustomerDetail2 { get; set; }
    }
}

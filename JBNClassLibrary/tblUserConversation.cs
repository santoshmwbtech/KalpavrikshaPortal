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
    
    public partial class tblUserConversation
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public tblUserConversation()
        {
            this.tblDeleteChats = new HashSet<tblDeleteChat>();
        }
    
        public int ID { get; set; }
        public Nullable<int> QueryId { get; set; }
        public Nullable<int> CustID { get; set; }
        public string Message { get; set; }
        public Nullable<int> IsDealer { get; set; }
        public Nullable<int> IsCustomer { get; set; }
        public string Image { get; set; }
        public Nullable<int> IsRead { get; set; }
        public Nullable<int> IsDeleted { get; set; }
        public string DeletedCustID { get; set; }
        public Nullable<int> IsArchived { get; set; }
        public string FileType { get; set; }
        public int CreatedBy { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public Nullable<int> ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
    
        public virtual tblCustomerDetail tblCustomerDetail { get; set; }
        public virtual tblCustomerDetail tblCustomerDetail1 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tblDeleteChat> tblDeleteChats { get; set; }
        public virtual tblselectedDealer tblselectedDealer { get; set; }
    }
}

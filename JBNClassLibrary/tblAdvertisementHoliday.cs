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
    
    public partial class tblAdvertisementHoliday
    {
        public int ID { get; set; }
        public string HolidayYear { get; set; }
        public string HolidayName { get; set; }
        public Nullable<System.DateTime> HolidayDate { get; set; }
        public string HolidayType { get; set; }
        public Nullable<double> HolidayMatrix { get; set; }
        public int CreatedBy { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public Nullable<int> ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public string DayName { get; set; }
        public Nullable<int> StateID { get; set; }
        public string HolidayDefinition { get; set; }
    }
}

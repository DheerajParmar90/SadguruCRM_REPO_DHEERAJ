//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace SadguruCRM.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Service_Estimate_Templates
    {
        public int Service_Estimate_Template_ID { get; set; }
        public int ServiceID { get; set; }
        public string Subject { get; set; }
        public string General_Instruction { get; set; }
        public string General_Disinfestation { get; set; }
        public string Frequency_Of_Services { get; set; }
        public string Charges_Per_Annum { get; set; }
        public string Terms_Of_Payment { get; set; }
        public Nullable<System.DateTime> CreatedOn { get; set; }
        public int CreatedByUserID { get; set; }
        public Nullable<System.DateTime> LastUpdatedOn { get; set; }
        public Nullable<int> LastUpdatedByUserID { get; set; }
        public Nullable<int> ServiceGroupID { get; set; }
    
        public virtual UserLogin UserLogin { get; set; }
        public virtual Service Service { get; set; }
        public virtual ServiceGroup ServiceGroup { get; set; }
    }
}

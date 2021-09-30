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
    
    public partial class Leads_Follow_Up_Details
    {
        public long Leads_Follow_Up_Details_ID { get; set; }
        public int LeadID { get; set; }
        public Nullable<System.DateTime> NextFollowUpDate { get; set; }
        public Nullable<System.TimeSpan> NextFollowUpTime { get; set; }
        public string FollowUpDetails { get; set; }
        public Nullable<int> VisitAllocate { get; set; }
        public Nullable<System.DateTime> VisitDate { get; set; }
        public Nullable<System.TimeSpan> VisitTime { get; set; }
        public string VisitReport { get; set; }
        public Nullable<System.DateTime> ServiceDate { get; set; }
        public Nullable<System.TimeSpan> ServiceTime { get; set; }
        public Nullable<int> LeadStatusID { get; set; }
        public string StatusReason { get; set; }
        public System.DateTime CreatedOn { get; set; }
        public int CreatedBy { get; set; }
        public Nullable<System.DateTime> LastUpdatedOn { get; set; }
        public Nullable<int> LastUpdatedBy { get; set; }
    
        public virtual Employee Employee { get; set; }
        public virtual Employee Employee1 { get; set; }
        public virtual Lead Lead { get; set; }
        public virtual UserLogin UserLogin { get; set; }
        public virtual UserLogin UserLogin1 { get; set; }
        public virtual LeadStatus LeadStatus { get; set; }
        public virtual LeadStatus LeadStatus1 { get; set; }
    }
}

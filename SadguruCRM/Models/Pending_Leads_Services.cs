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
    
    public partial class Pending_Leads_Services
    {
        public int PendingLeadsServicesID { get; set; }
        public int PendingLeadID { get; set; }
        public string Service { get; set; }
        public string Frequency { get; set; }
        public string Rate { get; set; }
        public string OtherInfo { get; set; }
        public Nullable<System.DateTime> CreatedAt { get; set; }
    
        public virtual Pending_Leads Pending_Leads { get; set; }
    }
}

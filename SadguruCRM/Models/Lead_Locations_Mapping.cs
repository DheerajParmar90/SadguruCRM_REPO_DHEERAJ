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
    
    public partial class Lead_Locations_Mapping
    {
        public int Lead_Locations_Mapping_ID { get; set; }
        public int LeadID { get; set; }
        public int LocationID { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<System.DateTime> LastUpdatedDate { get; set; }
    
        public virtual Lead Lead { get; set; }
        public virtual Location Location { get; set; }
    }
}

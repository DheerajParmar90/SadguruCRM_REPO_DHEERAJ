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
    
    public partial class Lead_Cell_No_Mapping
    {
        public int Lead_Cell_No_Mapping_ID { get; set; }
        public int LeadID { get; set; }
        public string Lead_Cell_No { get; set; }
    
        public virtual Lead Lead { get; set; }
    }
}

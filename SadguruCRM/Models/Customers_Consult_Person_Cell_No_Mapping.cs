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
    
    public partial class Customers_Consult_Person_Cell_No_Mapping
    {
        public long Customers_Consult_Person_Cell_No_Mapping_ID { get; set; }
        public long Customers_Consult_Person_Mapping_ID { get; set; }
        public string Consult_Person_Cell_No { get; set; }
    
        public virtual Customers_Consult_Person_Mapping Customers_Consult_Person_Mapping { get; set; }
    }
}

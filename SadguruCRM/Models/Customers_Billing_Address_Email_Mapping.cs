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
    
    public partial class Customers_Billing_Address_Email_Mapping
    {
        public long Customers_Billing_Address_Email_Mapping_ID { get; set; }
        public long Customers_Billing_Address_Mapping_ID { get; set; }
        public string Customers_Billing_Address_Email { get; set; }
    
        public virtual Customers_Billing_Address_Mapping Customers_Billing_Address_Mapping { get; set; }
    }
}

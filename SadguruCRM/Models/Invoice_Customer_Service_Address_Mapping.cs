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
    
    public partial class Invoice_Customer_Service_Address_Mapping
    {
        public long Invoice_Customer_Service_Address_Mapping_ID { get; set; }
        public int InvoiceID { get; set; }
        public long Customers_Service_Address_Mapping_ID { get; set; }
        public long CustomerID { get; set; }
    
        public virtual Customer Customer { get; set; }
        public virtual Invoice Invoice { get; set; }
    }
}

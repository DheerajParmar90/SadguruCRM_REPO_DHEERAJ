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
    
    public partial class Vendor_Services
    {
        public int Vendor_Service_ID { get; set; }
        public int VendorID { get; set; }
        public int ServiceID { get; set; }
    
        public virtual Service Service { get; set; }
        public virtual Vendor Vendor { get; set; }
    }
}

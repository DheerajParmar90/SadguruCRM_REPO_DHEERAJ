using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SadguruCRM.Models.ViewModels
{
    public class CustomerAddress
    {
            public List<Customers_Billing_Address_Mapping> allClients { get; set; }
            public List<Customers_Service_Address_Mapping> allOrders { get; set; }
            

        
    }
}
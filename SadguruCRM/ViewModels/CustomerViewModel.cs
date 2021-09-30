using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SadguruCRM.Models;

namespace SadguruCRM.ViewModels
{
    public class CustomerViewModel
    {
        public Customer customer { get; set; }
        public Customers_Billing_Address_Mapping billing_address { get; set; }
        public List<Customers_Billing_Address_Tel_No_Mapping> list_billing_address_tel { get; set; }
        public List<Customers_Billing_Address_Cell_No_Mapping> list_billing_address_cell { get; set; }
        public List<Customers_Billing_Address_Email_Mapping> list_billing_address_email { get; set; }
        public List<Customers_Service_Address_Mapping> list_service_address { get; set; }
        public List<Customers_Service_Address_Tel_No_Mapping> list_service_address_tel { get; set; }
        public List<Customers_Service_Address_Cell_No_Mapping> list_service_address_cell { get; set; }
        public List<Customers_Service_Address_Email_Mapping> list_service_address_email { get; set; }
        public CustomerViewModel()
        {
            list_billing_address_tel = new List<Customers_Billing_Address_Tel_No_Mapping>();
            list_billing_address_cell = new List<Customers_Billing_Address_Cell_No_Mapping>();
            list_billing_address_email = new List<Customers_Billing_Address_Email_Mapping>();
            list_service_address = new List<Customers_Service_Address_Mapping>();
            list_service_address_tel = new List<Customers_Service_Address_Tel_No_Mapping>();
            list_service_address_cell = new List<Customers_Service_Address_Cell_No_Mapping>();
            list_service_address_email = new List<Customers_Service_Address_Email_Mapping>();
        }
    }
}
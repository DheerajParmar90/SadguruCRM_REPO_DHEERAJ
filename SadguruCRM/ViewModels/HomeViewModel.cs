using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SadguruCRM.Models;

namespace SadguruCRM.ViewModels
{
    public class HomeViewModel
    {
        //public Customer customer { get; set; }
        //public Customers_Billing_Address_Mapping billing_address { get; set; }
        public List<Lead> lead_reminder { get; set; }
        public List<Lead> lead_pending { get; set; }
        public HomeViewModel()
        {
            lead_reminder = new List<Lead>();
            lead_pending = new List<Lead>();
        }
    }
    //public class ledasReminderModel {
    //    public int LeadID { get; set; }
    //    public string LeadNumber { get; set; }
    //    public DateTime? NextFollowUpDateTime { get; set; }
    //    public TimeSpan? NextFollowUpTime { get; set; }
    //    public string CustomerName { get; set; }
    //    public string FirstName { get; set; }
    //    public string LastName { get; set; }
    //}
}
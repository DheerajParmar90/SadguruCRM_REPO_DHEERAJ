using SadguruCRM.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SadguruCRM.ViewModels
{
    public class ConfirmedLeadServicesViewModel
    {
        public int LeadID { get; set; }
        public long? CustomerID { get; set; }
        public string Name { get; set; }
        public string Location { get; set; }
        public string Date { get; set; }
        public List<Lead_Services_Mapping> leadServices { get; set; }
    }
}
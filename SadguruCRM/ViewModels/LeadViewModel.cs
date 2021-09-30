using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SadguruCRM.Models;

namespace SadguruCRM.ViewModels
{
    public class LeadViewModel
    {
        public Lead lead { get; set; }
        public List<Lead_Tel_No_Mapping> leadTelNos { get; set; }
        public List<Lead_Cell_No_Mapping> leadCellNos { get; set; }
        public List<Lead_Email_Mapping> leadEmails { get; set; }
        public List<Lead_Consult_Person> leadConsultPersons { get; set; }
        public List<Lead_Consult_Person_Contact_No_Mapping> leadConsultPersonContactNos { get; set; }
        public List<Lead_Consult_Person_Email_Mapping> leadConsultPersonEmails { get; set; }
        public List<Lead_Services_Mapping> leadServices { get; set; }

        public IEnumerable<SelectListItem> ServiceGroups { get; set; }
        public IEnumerable<SelectListItem> Services { get; set; }
        public IEnumerable<SelectListItem> FrequencyOfServices { get; set; }

        public LeadViewModel()
        {
            leadTelNos = new List<Lead_Tel_No_Mapping>();
            leadCellNos = new List<Lead_Cell_No_Mapping>();
            leadConsultPersons = new List<Lead_Consult_Person>();
            leadConsultPersonContactNos = new List<Lead_Consult_Person_Contact_No_Mapping>();
            leadConsultPersonEmails = new List<Lead_Consult_Person_Email_Mapping>();
            leadServices = new List<Lead_Services_Mapping>();
        }
    }
}
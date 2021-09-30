using SadguruCRM.Helpers;
using SadguruCRM.Models;
using SadguruCRM.ViewModels;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SadguruCRM.Controllers
{
    [VerifyUser]
    public class ConfirmedLeadServicesController : Controller
    {
        private SadguruCRMEntities db = new SadguruCRMEntities();

        private static TimeZoneInfo INDIAN_ZONE = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
        // GET: ConfirmedLeadServices
        public ActionResult Index(int LeadID)
        {
            ConfirmedLeadServicesViewModel vm = new ConfirmedLeadServicesViewModel();
            vm.LeadID = LeadID;
            //vm.leadServices = new List<Lead_Services_Mapping>();
            vm.leadServices = db.Lead_Services_Mapping.Where(x => x.LeadID == LeadID).ToList();
                
            var custFromLead = db.Customers.Where(c => c.LeadID == LeadID).FirstOrDefault();
            if (custFromLead != null)
            {
                vm.CustomerID = custFromLead.CustomerID;
                vm.Name = custFromLead.Title + " " + custFromLead.CustomerName + " " + custFromLead.FirstName + " " + custFromLead.LastName;
                vm.Location = custFromLead.Lead.Location.LocationName;
                vm.Date = ((DateTime)custFromLead.CustomerDate).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
            }
            else
            {
                Lead lead = db.Leads.Find(LeadID);
                vm.Name = lead.Title + " " + lead.CustomerName + " " + lead.FirstName + " " + lead.LastName;
                vm.Location = lead.Location.LocationName;
                vm.Date = ((DateTime)lead.LeadDate).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
            }

            ViewBag.FrequencyOfServiceID = new SelectList(db.FrequencyOfServices, "FrequencyOfServiceID", "FrequencyOfService1");
            ViewBag.PeriodsOfContractID = new SelectList(db.PeriodsOfContracts, "PeriodsOfContractID", "PeriodsOfContract1");
            return View(vm);
        }
        [HttpPost]
        public ActionResult UpdateLeadService(Lead_Services_Mapping map)
        {
            using (SadguruCRMEntities entities = new SadguruCRMEntities())
            {
                Lead_Services_Mapping leadService = (from c in entities.Lead_Services_Mapping
                                                      where c.Lead_Services_Mapping_ID == map.Lead_Services_Mapping_ID
                                            select c).FirstOrDefault();
                leadService.FrequencyOfServiceID = map.FrequencyOfServiceID;
                leadService.PeriodsOfContractID = map.PeriodsOfContractID;
                leadService.ServiceStartDate = map.ServiceStartDate;
                leadService.POCStartDate = map.POCStartDate;
                leadService.POCEndDate = map.POCEndDate;

                entities.SaveChanges();
                CreateServicings(leadService);
            }

            return new EmptyResult();
        }
        public void CreateServicings(Lead_Services_Mapping map) {

            Servicing servicing;
            var existingServicings = db.Servicings.Where(s => s.Lead_Services_Mapping_ID == map.Lead_Services_Mapping_ID);
            if (existingServicings.Count() > 0) {
                db.Servicings.RemoveRange(existingServicings);
                db.SaveChanges();
            }
             
            FrequencyOfService frequency = db.FrequencyOfServices.Find(map.FrequencyOfServiceID);
            DateTime system_date_of_service = (DateTime)map.ServiceStartDate;
            int noOfServicing = 1;


            var branchShortCode = db.Branches.Find(db.Leads.Find(map.LeadID).BranchID).BranchShortCode;
            DateTime today = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, INDIAN_ZONE);
            var year = today.ToString("yy");
            string no = "";
            int intNumber = 0;
            var noToIncreaseAsList = db.Servicings.Where(e => !e.ServicingNo.Contains("-") & !e.ServicingNo.Contains("_")).OrderByDescending(e => e.ServicingID).Take(1).Select(e => e.ServicingNo).ToList();
            if (noToIncreaseAsList.Count == 0)
            {
                no = "S/"+branchShortCode + year + "0001";
                intNumber = 1;
            }
            else
            {
                string string1;
                if (noToIncreaseAsList.First().Contains("S/")) {
                    string1 = noToIncreaseAsList.First().Substring(0, 7);
                }
                else {
                    string1 = noToIncreaseAsList.First().Substring(0, 5);
                }
                //string string2 = custNumberToIncreaseAsList.First().Substring(5);
                string numberAsString = noToIncreaseAsList.First().Replace(string1, "");
                intNumber = Int32.Parse(numberAsString) + 1;
                string number = String.Format("{0:D4}", intNumber);
                no = "S/" + branchShortCode + year + number;
                //referenceNumber = branchShortCode + year + month + referenceNumberToIncreaseAsList.First(). + "/" + today.ToString("yyyy") + "-" + (int.Parse(year) + 1).ToString();
            }

            using (SadguruCRMEntities entities = new SadguruCRMEntities()) {
                if (frequency.FrequencyOfService1 == "One Time")
                {
                    servicing = new Servicing();
                    servicing.ServicingNo = no;
                    servicing.NoOfServicing = noOfServicing;
                    servicing.Servicing_Frequency_Number = (byte)noOfServicing;
                    servicing.System_Servicing_Datetime = system_date_of_service;
                    servicing.Servicing_Datetime = system_date_of_service;
                    servicing.Actual_Servicing_Datetime = null;
                    servicing.ContractID = null;
                    servicing.Contract_Services_Mapping_ID = null;
                    servicing.LeadID = map.LeadID;
                    servicing.Lead_Services_Mapping_ID = map.Lead_Services_Mapping_ID;
                    servicing.CreatedDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, INDIAN_ZONE);
                    servicing.CreatedBy = Convert.ToInt32(Session["UserID"]);
                    //System.Diagnostics.Debug.WriteLine(system_date_of_service);

                    //system_date_of_service = system_date_of_service.AddYears(frequency.Duration_Between_2_Services_Year).AddMonths(frequency.Duration_Between_2_Services_Month).AddDays(frequency.Duration_Between_2_Services_Day);
                    //noOfServicing++;
                    //intNumber++;
                    //no = branchShortCode + year + String.Format("{0:D4}", intNumber);
                    entities.Servicings.Add(servicing);
                }
                else {

                    while (system_date_of_service < map.POCEndDate)
                    {
                        servicing = new Servicing();
                        servicing.ServicingNo = no;
                        servicing.NoOfServicing = noOfServicing;
                        servicing.Servicing_Frequency_Number = (byte)noOfServicing;
                        servicing.System_Servicing_Datetime = system_date_of_service;
                        servicing.Servicing_Datetime = system_date_of_service;
                        servicing.Actual_Servicing_Datetime = null;
                        servicing.ContractID = null;
                        servicing.Contract_Services_Mapping_ID = null;
                        servicing.LeadID = map.LeadID;
                        servicing.Lead_Services_Mapping_ID = map.Lead_Services_Mapping_ID;
                        servicing.CreatedDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, INDIAN_ZONE);
                        servicing.CreatedBy = Convert.ToInt32(Session["UserID"]);
                        //System.Diagnostics.Debug.WriteLine(system_date_of_service);

                        system_date_of_service = system_date_of_service.AddYears(frequency.Duration_Between_2_Services_Year).AddMonths(frequency.Duration_Between_2_Services_Month).AddDays(frequency.Duration_Between_2_Services_Day);
                        noOfServicing++;
                        intNumber++;
                        no = "S/" + branchShortCode + year + String.Format("{0:D4}", intNumber);
                        entities.Servicings.Add(servicing);
                    }
                }
                entities.SaveChanges();
            }
            //for (int i = 1; i <= 1000; i++) {

                //}


        }
        [HttpPost]
        public ActionResult CalculateContractDates(string FOS, string POC, string serviceStartDate, string POCStartDate)
        {
            string POCEndDate;
            using (SadguruCRMEntities entities = new SadguruCRMEntities())
            {
                //int intFOS = Int32.Parse(FOS);
                //FrequencyOfService frequency = db.FrequencyOfServices.Find(intFOS);
                int intPOC = Int32.Parse(POC);
                PeriodsOfContract period = db.PeriodsOfContracts.Find(intPOC);

                DateTime? dtServiceStartDate;
                DateTime? dtPOCStartDate, dtPOCEndDate;
                try
                {
                    dtServiceStartDate = DateTime.ParseExact(serviceStartDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                }
                catch (Exception ex) {
                    dtServiceStartDate = null;
                }
                

                if (POCStartDate != "")
                {
                    dtPOCStartDate = DateTime.ParseExact(POCStartDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                }
                else {
                    if (dtServiceStartDate != null) {
                        dtPOCStartDate = (DateTime)dtServiceStartDate;
                        POCStartDate = serviceStartDate;
                    }
                    else
                    {
                        dtPOCStartDate = null;
                        POCStartDate = "";
                    }
                    
                }
                if (dtPOCStartDate != null)
                {
                    dtPOCEndDate = ((DateTime)dtPOCStartDate).AddYears((int)period.Years).AddMonths((int)period.Months).AddDays((int)period.Days).AddDays(-1);
                    POCEndDate = ((DateTime)dtPOCEndDate).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
                }
                else {
                    dtPOCEndDate = null;
                    POCEndDate = "";
                }
                //leadService.FrequencyOfServiceID = map.FrequencyOfServiceID;
                //leadService.Rate = map.Rate;
                //leadService.GST = map.GST;
                //leadService.Qty = map.Qty;
                //leadService.Tax = map.Tax;
                //leadService.FinalRatePerService = map.FinalRatePerService;
                //leadService.PeriodsOfContractID = map.PeriodsOfContractID;
                //leadService.ServiceStartDate = map.ServiceStartDate;
                //leadService.POCStartDate = map.POCStartDate;
                //leadService.POCEndDate = map.POCEndDate;

                //entities.SaveChanges();
            }

            return Json(new { POCStartDate, POCEndDate }, JsonRequestBehavior.AllowGet);
        }
    }
}
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Rotativa.Options;
using SadguruCRM.Helpers;
using SadguruCRM.Models;
using SadguruCRM.Models.ViewModels;
using SadguruCRM.ViewModels;

namespace SadguruCRM.Controllers
{
    [VerifyUser]
    public class EstimatesController : Controller
    {
        private SadguruCRMEntities db = new SadguruCRMEntities();
        private static TimeZoneInfo INDIAN_ZONE = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");

        // GET: Estimates
        public async Task<ActionResult> Index()
        {
            var estimates = db.Estimates.Include(e => e.Customer).Include(e => e.UserLogin).Include(e => e.UserLogin1).Include(e => e.Lead).Include(e => e.Customer.Customers_Billing_Address_Mapping);
            var estimatesList = await new HelperNonStatic().GenerateServicesShortCodesForEstimates(estimates);
            //foreach (var item in estimates)
            //{
            //    if (item.NewCustomer)
            //    {
            //        item.CustomerName = item.Title + " " + item.FirstName + " " + item.LastName;
            //    }
            //}
            return View(estimatesList);
        }

        // GET: Estimates/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Estimate estimate = db.Estimates.Find(id);
            if (estimate == null)
            {
                return HttpNotFound();
            }


            EstimateViewModel estViewModel = new EstimateViewModel();
            //From here copied from Create
            var custs = db.Customers.Select(s => new
            {
                s.CustomerID,
                //Description = string.Format("{0} - £{1} £{2} £{3} £{4} ", s.LeadID, s.Title, s.FirstName, s.LastName, s.CustomerName)
                CustomerName = s.CustomerID + " - " + s.Title + " " + s.FirstName + " " + s.LastName + " " + s.CustomerName

            }).ToList();
            ViewBag.CustomerID = new SelectList(custs, "CustomerID", "CustomerName", estimate.CustomerID);
            ViewBag.CreatedBy = new SelectList(db.UserLogins, "UserID", "UserName", estimate.CreatedBy);
            ViewBag.LastUpdatedBy = new SelectList(db.UserLogins, "UserID", "UserName", estimate.LastUpdatedBy);
            var leads = db.Leads.Select(s => new
            {
                s.LeadID,
                //Description = string.Format("{0} - £{1} £{2} £{3} £{4} ", s.LeadID, s.Title, s.FirstName, s.LastName, s.CustomerName)
                Description = s.LeadID + " - " + s.Title + " " + s.FirstName + " " + s.LastName + " " + s.CustomerName

            }).ToList();

            ViewBag.LeadID = new SelectList(leads, "LeadID", "Description", estimate.LeadID);
            ViewBag.FrequencyOfServiceID = new SelectList(db.FrequencyOfServices.OrderBy(x => x.Order), "FrequencyOfServiceID", "FrequencyOfService1");
            ViewBag.ServiceGroupID = new SelectList(db.ServiceGroups, "ServiceGroupID", "ServiceGroup1");
            ViewBag.ServiceID = new SelectList(db.Services.Where(x => x.ServiceID == 0), "ServiceID", "ServiceName");
            //ViewBag.LeadID = new SelectList(db.Leads, "LeadID", "Title");

            //ViewBag.LocationID = new SelectList(db.Locations.Select(x => new { x.LocationID, x.LocationName }), "LocationID", "LocationName", estimate.);
            //ViewBag.CityID = new SelectList(db.Cities.Select(x => new { x.CityID, x.CityName }), "CityID", "CityName");
            //ViewBag.StateID = new SelectList(db.States.Select(x => new { x.StateID, x.State1 }), "StateID", "State1");

            // Use FinalEstimateRate to pass Original Estimate Ref No
            var ogEstimate = db.Estimates.Where(x => x.EstimateID == estimate.OriginalEstimateID).Select(x => new { x.EstimateID, ReferenceNo = x.ReferenceNo + " Revision: " + estimate.RevisionNo });
            //foreach (var item in ogEstimate) {
            //    item.ReferenceNo = item.ReferenceNo + " Revision: " + estimate.RevisionNo;
            //}
            ViewBag.OriginalEstimateID = new SelectList(ogEstimate, "EstimateID", "ReferenceNo", estimate.OriginalEstimateID);
            return View(estimate);
        }

        public ActionResult PrintEstimate(int id)
        {
            try
            {
                var cookies = Request.Cookies.AllKeys.ToDictionary(k => k, k => Request.Cookies[k].Value);


                string customSwitches = string.Format("--print-media-type --allow {0}{1} --header-html {0} --footer-html {1}",

                          Url.Action("Header", "DocumentsTemplates", new { area = "" }, "http"), Url.Action("Footer", "DocumentsTemplates", new { area = "" }, "http")
                          //Server.MapPath("../Content/Images/pngwing.com.png")
                          );

                //return new Rotativa.ActionAsPdf("Index", new { id = id })
                //{
                //    FormsAuthenticationCookieName = System.Web.Security.FormsAuthentication.FormsCookieName,
                //    Cookies = cookies,
                //    //CustomSwitches = customSwitches,
                //    PageSize = Rotativa.Options.Size.A4,
                //    PageMargins = new Margins(0, 3, 32, 3)
                //};
                //return new ViewAsPdf("MyPDF.cshtml", model)
                //{
                //    FileName = "MyPDF.pdf",
                //    CustomSwitches = customSwitches
                //};
                return new Rotativa.UrlAsPdf("/Estimates/PDFView/"+id.ToString())
                {
                    FormsAuthenticationCookieName = System.Web.Security.FormsAuthentication.FormsCookieName,
                    Cookies = cookies,
                    CustomSwitches = customSwitches,
                    PageSize = Rotativa.Options.Size.A4,
                    //PageMargins = new Margins(0, 3, 0, 3)
                };
                //var report = new Rotativa.ActionAsPdf("Details", new { id = id });
                //return report;
            }
            catch (Exception ex) {
                string message = ex.Message;
                return null;
            }
            
        }
        [AllowAnonymous]
        public ActionResult PDFView(int id)
        {

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Estimate estimate = db.Estimates.Find(id);
            if (estimate == null)
            {
                return HttpNotFound();
            }


            EstimateViewModel estViewModel = new EstimateViewModel();
            //From here copied from Create
            var custs = db.Customers.Select(s => new
            {
                s.CustomerID,
                //Description = string.Format("{0} - £{1} £{2} £{3} £{4} ", s.LeadID, s.Title, s.FirstName, s.LastName, s.CustomerName)
                CustomerName = s.CustomerID + " - " + s.Title + " " + s.FirstName + " " + s.LastName + " " + s.CustomerName

            }).ToList();
            ViewBag.CustomerID = new SelectList(custs, "CustomerID", "CustomerName", estimate.CustomerID);
            ViewBag.CreatedBy = new SelectList(db.UserLogins, "UserID", "UserName", estimate.CreatedBy);
            ViewBag.LastUpdatedBy = new SelectList(db.UserLogins, "UserID", "UserName", estimate.LastUpdatedBy);
            var leads = db.Leads.Select(s => new
            {
                s.LeadID,
                //Description = string.Format("{0} - £{1} £{2} £{3} £{4} ", s.LeadID, s.Title, s.FirstName, s.LastName, s.CustomerName)
                Description = s.LeadID + " - " + s.Title + " " + s.FirstName + " " + s.LastName + " " + s.CustomerName

            }).ToList();

            ViewBag.LeadID = new SelectList(leads, "LeadID", "Description", estimate.LeadID);
            ViewBag.FrequencyOfServiceID = new SelectList(db.FrequencyOfServices.OrderBy(x => x.Order), "FrequencyOfServiceID", "FrequencyOfService1");
            ViewBag.ServiceGroupID = new SelectList(db.ServiceGroups, "ServiceGroupID", "ServiceGroup1");
            ViewBag.ServiceID = new SelectList(db.Services.Where(x => x.ServiceID == 0), "ServiceID", "ServiceName");
            //ViewBag.LeadID = new SelectList(db.Leads, "LeadID", "Title");

            //ViewBag.LocationID = new SelectList(db.Locations.Select(x => new { x.LocationID, x.LocationName }), "LocationID", "LocationName", estimate.);
            //ViewBag.CityID = new SelectList(db.Cities.Select(x => new { x.CityID, x.CityName }), "CityID", "CityName");
            //ViewBag.StateID = new SelectList(db.States.Select(x => new { x.StateID, x.State1 }), "StateID", "State1");

            // Use FinalEstimateRate to pass Original Estimate Ref No
            var ogEstimate = db.Estimates.Where(x => x.EstimateID == estimate.OriginalEstimateID).Select(x => new { x.EstimateID, ReferenceNo = x.ReferenceNo + " Revision: " + estimate.RevisionNo });
            //foreach (var item in ogEstimate) {
            //    item.ReferenceNo = item.ReferenceNo + " Revision: " + estimate.RevisionNo;
            //}
            ViewBag.OriginalEstimateID = new SelectList(ogEstimate, "EstimateID", "ReferenceNo", estimate.OriginalEstimateID);
            return View(estimate);
            //return View();
        }
        [AllowAnonymous]
        public ActionResult Header() {
            return View();
        }
        // GET: Estimates/Create
        public ActionResult Create()
        {
            var custs = db.Customers.Select(s => new
            {
                s.CustomerID,
                //Description = string.Format("{0} - £{1} £{2} £{3} £{4} ", s.LeadID, s.Title, s.FirstName, s.LastName, s.CustomerName)
                CustomerName = s.CustomerID + " - " + s.Title + " " + s.FirstName + " " + s.LastName + " " + s.CustomerName

            }).ToList();
            ViewBag.CustomerID = new SelectList(custs, "CustomerID", "CustomerName");
            ViewBag.CreatedBy = new SelectList(db.UserLogins, "UserID", "UserName");
            ViewBag.LastUpdatedBy = new SelectList(db.UserLogins, "UserID", "UserName");
            var leads =db.Leads.Select(s => new
                                {
                                    s.LeadID,
                //Description = string.Format("{0} - £{1} £{2} £{3} £{4} ", s.LeadID, s.Title, s.FirstName, s.LastName, s.CustomerName)
                Description = s.LeadID + " - " + s.Title + " " +s.FirstName +" " + s.LastName + " "+s.CustomerName

            }).ToList();

            ViewBag.LeadID = new SelectList(leads, "LeadID", "Description");
            ViewBag.FrequencyOfServiceID = new SelectList(db.FrequencyOfServices.OrderBy(x => x.Order), "FrequencyOfServiceID", "FrequencyOfService1");
            ViewBag.ServiceGroupID = new SelectList(db.ServiceGroups, "ServiceGroupID", "ServiceGroup1");
            ViewBag.ServiceID = new SelectList(db.Services.Where(x => x.ServiceID == 0), "ServiceID", "ServiceName");
            ViewBag.OriginalEstimateID = new SelectList(db.Estimates.Where(x => x.OriginalEstimateID == 0), "EstimateID", "ReferenceNo");
            //ViewBag.LeadID = new SelectList(db.Leads, "LeadID", "Title");

            ViewBag.LocationID = new SelectList(db.Locations.Select(x => new { x.LocationID, x.LocationName }), "LocationID", "LocationName");
            ViewBag.CityID = new SelectList(db.Cities.Select(x => new { x.CityID, x.CityName }), "CityID", "CityName");
            ViewBag.StateID = new SelectList(db.States.Select(x => new { x.StateID, x.State1 }), "StateID", "State1");
            return View();
        }

        // POST: Estimates/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]        
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Create([Bind(Include = "EstimateID,NewCustomer,ExistingCustomer,LeadID,CustomerID,ReferenceNo,GSTinDate,FinalRate,KindAttention,Subject,WelcomeSentence,ServiceScope,Footer,CreatedDate,LastUpdatedDate,CreatedBy,LastUpdatedBy,isNewEstimate,EstimateDate,OriginalEstimateID")] Estimate estimate, FormCollection formValues, int[] Lead_Services_Mapping_ID, int[] Estimate_Services_Mapping_ID, String[] ServiceGroupID, String[] ServiceID, String[] FrequencyOfServiceID, String[] Qty, String[] GST, String[] Tax, String[] Rate, String[] FinalRatePerService, string submit)
        {
            if (ModelState.IsValid)
            {
                if (!estimate.NewCustomer) {
                    estimate.ExistingCustomer = true;
                }
                estimate.isApproved = false;
                estimate.ApprovedBy = null;
                estimate.ApprovedAt = null;
                estimate.CreatedDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, INDIAN_ZONE);
                estimate.CreatedBy = Convert.ToInt32(Session["UserID"]);

                estimate.GSTinDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, INDIAN_ZONE);
                estimate.WelcomeSentence = formValues["WelcomeSentence"];
                estimate.ServiceScope = formValues["ServiceScope"];
                estimate.Footer = formValues["Footer"];
                if (estimate.isNewEstimate)
                {
                    // Original Estimate ID will be updated in Trigger
                    estimate.OriginalEstimateID = null;
                    estimate.RevisionNo = 1;
                }
                else {
                    //Original estimate ID submitted from view so only revison no
                    estimate.RevisionNo = db.Estimates.Where(x => x.OriginalEstimateID == estimate.OriginalEstimateID).Max(x => x.RevisionNo) + 1;
                }
                db.Estimates.Add(estimate);
                db.SaveChanges();

                
                
                    // Length of Lead_Services_Mapping_ID and Estimate_Services_Mapping_ID will be same
                    for (int i = 0; i < Lead_Services_Mapping_ID.Length; i++)
                    {
                        int outServiceGroupID;
                        if (Int32.TryParse(ServiceGroupID[i], out outServiceGroupID))
                        {
                            Estimate_Services_Mapping servicesMapping = new Estimate_Services_Mapping();
                            //Lead_Services_Mapping lead_service;
                            servicesMapping.EstimateID = estimate.EstimateID;


                            servicesMapping.ServiceGroupID = outServiceGroupID;

                            int outServiceID;
                            if (Lead_Services_Mapping_ID[i] > 0)
                            {
                                servicesMapping.Lead_Services_Mapping_ID = Lead_Services_Mapping_ID[i];
                            }

                            else
                                servicesMapping.Lead_Services_Mapping_ID = null;


                            if (Int32.TryParse(ServiceID[i], out outServiceID))
                                servicesMapping.ServiceID = outServiceID;
                            else
                                servicesMapping.ServiceID = 0;
                            int outFrequency;
                            if (Int32.TryParse(FrequencyOfServiceID[i], out outFrequency))
                                servicesMapping.FrequencyOfServiceID = outFrequency;
                            else
                                servicesMapping.FrequencyOfServiceID = 0;
                            short outQty;
                            if (Int16.TryParse(Qty[i], out outQty))
                                servicesMapping.Qty = outQty;
                            else
                                servicesMapping.Qty = null;
                            bool outGST;
                            if (Boolean.TryParse(GST[i], out outGST))
                                servicesMapping.GST = outGST;
                            else
                                servicesMapping.GST = null;
                            Decimal outDecimalTax, outDecimalRate, outFinalRatePerService;
                            if (Decimal.TryParse(Tax[i], out outDecimalTax))
                                servicesMapping.Tax = outDecimalTax;
                            else
                                servicesMapping.Tax = null;
                            if (Decimal.TryParse(Rate[i], out outDecimalRate))
                                servicesMapping.Rate = outDecimalRate;
                            else
                                servicesMapping.Rate = null;
                            if (Decimal.TryParse(FinalRatePerService[i], out outFinalRatePerService))
                                servicesMapping.FinalRatePerService = outFinalRatePerService;
                            else
                                servicesMapping.FinalRatePerService = null;

                            db.Estimate_Services_Mapping.Add(servicesMapping);
                            db.SaveChanges();
                            if (estimate.NewCustomer)
                            {
                                if (Lead_Services_Mapping_ID[i] > 0)
                                {
                                    Lead_Services_Mapping lead_service = db.Lead_Services_Mapping.Find(Lead_Services_Mapping_ID[i]);
                                    lead_service.ServiceGroupID = servicesMapping.ServiceGroupID;
                                    lead_service.ServiceID = servicesMapping.ServiceID;
                                    lead_service.FrequencyOfServiceID = servicesMapping.FrequencyOfServiceID;
                                    lead_service.Qty = servicesMapping.Qty;
                                    lead_service.GST = servicesMapping.GST;
                                    lead_service.Tax = servicesMapping.Tax;
                                    lead_service.Rate = servicesMapping.Rate;
                                    lead_service.FinalRatePerService = servicesMapping.FinalRatePerService;
                                    db.Entry(lead_service).State = EntityState.Modified;
                                    db.SaveChanges();
                                }
                                else if (Lead_Services_Mapping_ID[i] == 0)
                                {

                                    Lead_Services_Mapping lead_service = new Lead_Services_Mapping();
                                    lead_service.LeadID = (int)estimate.LeadID;
                                    lead_service.ServiceGroupID = servicesMapping.ServiceGroupID;
                                    lead_service.ServiceID = servicesMapping.ServiceID;
                                    lead_service.FrequencyOfServiceID = servicesMapping.FrequencyOfServiceID;
                                    lead_service.Qty = servicesMapping.Qty;
                                    lead_service.GST = servicesMapping.GST;
                                    lead_service.Tax = servicesMapping.Tax;
                                    lead_service.Rate = servicesMapping.Rate;
                                    lead_service.FinalRatePerService = servicesMapping.FinalRatePerService;
                                    db.Lead_Services_Mapping.Add(lead_service);
                                    db.SaveChanges();
                                    servicesMapping.Lead_Services_Mapping_ID = lead_service.Lead_Services_Mapping_ID;
                                    db.Entry(servicesMapping).State = EntityState.Modified;
                                    db.SaveChanges();
                                }
                              }
                        

                        }
                    }
                    Lead lead = db.Leads.Find(estimate.LeadID);
                    lead.FinalRate = estimate.FinalRate;
                    db.Entry(lead).State = EntityState.Modified;
                    db.SaveChanges();

                //db.SaveChanges();

                switch (submit)
                {
                    case "Save":
                        return RedirectToAction("Index");
                    case "Save & Create New Estimate":
                        return RedirectToAction("Create");
                }
                return RedirectToAction("Index");
            }

            ViewBag.CustomerID = new SelectList(db.Customers, "CustomerID", "CustomerName", estimate.CustomerID);
            ViewBag.CreatedBy = new SelectList(db.UserLogins, "UserID", "UserName", estimate.CreatedBy);
            ViewBag.LastUpdatedBy = new SelectList(db.UserLogins, "UserID", "UserName", estimate.LastUpdatedBy);
            var stands = db.Leads.Select(s => new
            {
                s.LeadID,
                //Description = string.Format("{0} - £{1} £{2} £{3} £{4} ", s.LeadID, s.Title, s.FirstName, s.LastName, s.CustomerName)
                Description = s.LeadID + " - " + s.Title + s.FirstName + s.LastName + s.CustomerName

            })
                                .ToList();

            ViewBag.LeadID = new SelectList(stands, "LeadID", "Description");
            ViewBag.FrequencyOfServiceID = new SelectList(db.FrequencyOfServices, "FrequencyOfServiceID", "FrequencyOfService1");
            ViewBag.ServiceGroupID = new SelectList(db.ServiceGroups, "ServiceGroupID", "ServiceGroup1");
            ViewBag.ServiceID = new SelectList(db.Services.Where(x => x.ServiceID == 0), "ServiceID", "ServiceName");
            return View(estimate);
        }

        public ActionResult GetCustomerDataForEstimates(string custID)
        {
            int intCustID;

            if (!string.IsNullOrEmpty(custID))
            {
                intCustID = Convert.ToInt32(custID);

                var cust = db.Customers.Where(x => x.CustomerID == intCustID).Select(x => new { x.ConsultPerson, x.ConsultPersonDesignation, x.Branch.BranchName }).FirstOrDefault();

                var billingAddress = db.Customers_Billing_Address_Mapping.Where(x => x.CustomerID == intCustID).FirstOrDefault();
                //var serviceAddresses = db.Customers_Service_Address_Mapping.Where(x => x.CustomerID == intCustID).ToList();

                if (billingAddress == null)
                {
                    return Json("No Billing Address Found", JsonRequestBehavior.AllowGet);
                }
                //else if (serviceAddresses.Count == 0)
                //{
                //    return Json("No Service Address Found", JsonRequestBehavior.AllowGet);
                //}
                else {
                    long billingAddressID = billingAddress.Customers_Billing_Address_Mapping_ID;
                    var billingTelNos = db.Customers_Billing_Address_Tel_No_Mapping.Where(x => x.Customers_Billing_Address_Mapping_ID == billingAddressID).ToList();
                    var billingCellNos = db.Customers_Billing_Address_Cell_No_Mapping.Where(x => x.Customers_Billing_Address_Mapping_ID == billingAddressID).ToList();
                    var billingEmail = db.Customers_Billing_Address_Email_Mapping.Where(x => x.Customers_Billing_Address_Mapping_ID == billingAddressID).ToList();
                }
                Customers_Billing_Address_Mapping bill = new Customers_Billing_Address_Mapping();
                bill.Customers_Billing_Address_Mapping_ID = billingAddress.Customers_Billing_Address_Mapping_ID;
                bill.AddressLine1 = billingAddress.AddressLine1;
                bill.AddressLine2 = billingAddress.AddressLine2;
                bill.AddressLine3 = billingAddress.AddressLine3;
                string LocationName = billingAddress.Location.LocationName;
                string CityName = billingAddress.City.CityName;
                bill.BillingPincode = billingAddress.BillingPincode;
                string State1 = billingAddress.State.State1;
                bill.GSTNo = billingAddress.GSTNo;

                //List<Customers_Service_Address_Mapping> servicesAddressList = new List<Customers_Service_Address_Mapping>();
                //serviceAddresses.ForEach(x =>
                //{
                //    servicesAddressList.Add(new Customers_Service_Address_Mapping
                //    {
                //        Customers_Service_Address_Mapping_ID = x.Customers_Service_Address_Mapping_ID,
                //        CustomerID = x.CustomerID,
                //        AddressLine1 = x.AddressLine1,
                //        AddressLine2 = x.AddressLine2,
                //        AddressLine3 = x.AddressLine3,
                //        ServiceLocationID = x.ServiceLocationID,
                //        ServiceCityID = x.ServiceCityID,
                //        ServicePincode = x.ServicePincode,
                //        ServiceStateID = x.ServiceStateID,
                //        ServiceAddressConsultPerson = x.ServiceAddressConsultPerson
                //    });
                //});
                var billingAddress1 = db.Customers_Billing_Address_Mapping.Where(x => x.CustomerID == intCustID).FirstOrDefault();
                long billingAddressID1 = billingAddress1.Customers_Billing_Address_Mapping_ID;
                var billingTelNos1 = db.Customers_Billing_Address_Tel_No_Mapping.Where(x => x.Customers_Billing_Address_Mapping_ID == billingAddressID1).ToList();
                string strBillingTelNos = String.Join(",", billingTelNos1.Select(x => x.Customers_Billing_Address_Tel_No.ToString()).ToArray());
                var billingCellNos1 = db.Customers_Billing_Address_Cell_No_Mapping.Where(x => x.Customers_Billing_Address_Mapping_ID == billingAddressID1).ToList();
                string strBillingCellNos = String.Join(",", billingCellNos1.Select(x => x.Customers_Billing_Address_Cell_No.ToString()).ToArray());
                var billingEmail1 = db.Customers_Billing_Address_Email_Mapping.Where(x => x.Customers_Billing_Address_Mapping_ID == billingAddressID1).ToList();
                string strBillingEmails = String.Join(",", billingEmail1.Select(x => x.Customers_Billing_Address_Email.ToString()).ToArray());
                return Json(new { bill, /*servicesAddressList,*/ LocationName, CityName, State1, cust, strBillingTelNos, strBillingCellNos, strBillingEmails }, JsonRequestBehavior.AllowGet);
            }
            else
            {

                return Json("Wrong Customer ID", JsonRequestBehavior.AllowGet);
            }

        }
        public ActionResult GetEstimateReferenceNumber(bool IsNewCust, bool IsExistingCust, string LeadID, string CustID)
        {
            int intOutLeadID, intOutCustID;
            string referenceNumber="";
            DateTime today = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, INDIAN_ZONE);
            var year = today.ToString("yy");
            var month = today.ToString("MM");
            if (IsNewCust)
            {
                if (int.TryParse(LeadID, out intOutLeadID))
                {
                    string branchShortCode = db.Leads.Find(intOutLeadID).Branch.BranchShortCode;
                    //var referenceNumberToIncreaseAsList = db.Estimates.Where(e => e.ReferenceNo.Contains(year + month)).OrderByDescending(e => e.EstimateID).Take(1).Select(e => e.ReferenceNo).ToList();
                    var referenceNumberToIncreaseAsList = db.Estimates.OrderByDescending(e => e.EstimateID).Take(1).Select(e => e.ReferenceNo).ToList();
                    if (referenceNumberToIncreaseAsList.Count == 0)
                    {
                        //referenceNumber = branchShortCode + year + month + "0001" + "/" + today.ToString("yyyy") + "-" + (int.Parse(year) + 1).ToString();
                        referenceNumber = "E/" + branchShortCode + year + "0001" + "/" + today.ToString("yyyy") + "-" + (int.Parse(year) + 1).ToString();
                    }
                    else {
                        int lastIndexOfSlash = referenceNumberToIncreaseAsList.First().LastIndexOf("/");
                        string sttring1 = referenceNumberToIncreaseAsList.First().Substring(7);
                        string string2 = referenceNumberToIncreaseAsList.First().Substring(lastIndexOfSlash);
                        string numberAsString = sttring1.Replace(string2, "");
                        string number="";
                        if (Int32.Parse(numberAsString) < 9999)
                        {
                            number = String.Format("{0:D4}", Int32.Parse(numberAsString) + 1);
                        }
                        else if (Int32.Parse(numberAsString) == 9999)
                        {
                            number = "10000";
                        }
                        else if (Int32.Parse(numberAsString) > 9999) {
                            number = (Int32.Parse(numberAsString) + 1).ToString();
                        }
                        //string number = String.Format("{0:D4}", Int32.Parse(numberAsString) + 1);
                        referenceNumber = "E/" + branchShortCode + year + number + "/" + today.ToString("yyyy") + "-" + (int.Parse(year) + 1).ToString();
                        //referenceNumber = branchShortCode + year + month + referenceNumberToIncreaseAsList.First(). + "/" + today.ToString("yyyy") + "-" + (int.Parse(year) + 1).ToString();
                    }
                }
                else
                {
                    return Json("Error: Wrong Lead ID", JsonRequestBehavior.AllowGet);
                }

            }
            else if (IsExistingCust)
            {
                if (int.TryParse(CustID, out intOutCustID))
                {
                    string branchShortCode = db.Customers.Find(intOutCustID).Branch.BranchShortCode;
                    var referenceNumberToIncreaseAsList = db.Estimates.Where(e => e.ReferenceNo.Contains(year + month)).OrderByDescending(e => e.EstimateID).Take(1).Select(e => e.ReferenceNo).ToList();
                    if (referenceNumberToIncreaseAsList.Count == 0)
                    {
                        referenceNumber = branchShortCode + year + month + "0001" + "/" + today.ToString("yyyy") + "-" + (int.Parse(year) + 1).ToString();
                    }
                    else
                    {
                        string numberAsString = referenceNumberToIncreaseAsList.First().Replace(branchShortCode + year + month, "").Replace(today.ToString("yyyy") + "-" + (int.Parse(year) + 1).ToString(), "").Replace("/", "");
                        string number = String.Format("{0:D4}", Int32.Parse(numberAsString) + 1);
                        referenceNumber = branchShortCode + year + month + number + "/" + today.ToString("yyyy") + "-" + (int.Parse(year) + 1).ToString();
                        //referenceNumber = branchShortCode + year + month + referenceNumberToIncreaseAsList.First(). + "/" + today.ToString("yyyy") + "-" + (int.Parse(year) + 1).ToString();
                    }
                }
                else
                {
                    return Json("Error: Wrong Customer ID", JsonRequestBehavior.AllowGet);
                }

            }
            return Json(referenceNumber, JsonRequestBehavior.AllowGet);

        }
        public ActionResult GetExistingEstimates(bool IsNewCust, bool IsExistingCust, string LeadID, string CustID)
        {
            int intOutLeadID, intOutCustID;

            if (IsNewCust) {
                if (int.TryParse(LeadID, out intOutLeadID)) {
                    //                    var result = db.SomeTable.GroupBy(x => x.ID).Select(g => g.OrderByDescending(x => x.ID2).First());

                    //var existingEstimates = db.Estimates.Where(e => e.LeadID == intOutLeadID && e.isNewEstimate).Select(e => new { e.EstimateID, e.ReferenceNo }).ToList();
                    var existingEstimates = db.Estimates.Where(e => e.LeadID == intOutLeadID).GroupBy(x => x.OriginalEstimateID).Select(g => g.OrderByDescending(x => x.RevisionNo).FirstOrDefault()).OrderByDescending(x => x.EstimateID).Select(x => new { x.OriginalEstimateID, x.ReferenceNo });
                    return Json(existingEstimates, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json("Error: Wrong Lead ID", JsonRequestBehavior.AllowGet);
                }

            }
            else if (IsExistingCust)
            {
                if (int.TryParse(CustID, out intOutCustID))
                {
                    //var existingEstimates = db.Estimates.Where(e => e.CustomerID == intOutCustID).Max(e => e.RevisionNo).Select(e => new { e.EstimateID, e.ReferenceNo }).ToList();
                    var existingEstimates = db.Estimates.Where(e => e.CustomerID == intOutCustID).GroupBy(x => x.OriginalEstimateID).Select(g => g.OrderByDescending(x => x.RevisionNo).FirstOrDefault()).OrderByDescending(x => x.EstimateID).Select(x => new { x.OriginalEstimateID, x.ReferenceNo });
                    return Json(existingEstimates, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json("Error: Wrong Customer ID", JsonRequestBehavior.AllowGet);
                }

            }

            //if (!string.IsNullOrEmpty(custID))
            //{
            //    intCustID = Convert.ToInt32(custID);

            //    var cust = db.Customers.Where(x => x.CustomerID == intCustID).Select(x => new { x.ConsultPerson, x.ConsultPersonDesignation, x.Branch.BranchName }).FirstOrDefault();

            //    var billingAddress = db.Customers_Billing_Address_Mapping.Where(x => x.CustomerID == intCustID).FirstOrDefault();
            //    long billingAddressID = billingAddress.Customers_Billing_Address_Mapping_ID;
            //    var billingTelNos = db.Customers_Billing_Address_Tel_No_Mapping.Where(x => x.Customers_Billing_Address_Mapping_ID == billingAddressID).ToList();
            //    var billingCellNos = db.Customers_Billing_Address_Cell_No_Mapping.Where(x => x.Customers_Billing_Address_Mapping_ID == billingAddressID).ToList();
            //    var billingEmail = db.Customers_Billing_Address_Email_Mapping.Where(x => x.Customers_Billing_Address_Mapping_ID == billingAddressID).ToList();

            //    var serviceAddresses = db.Customers_Service_Address_Mapping.Where(x => x.CustomerID == intCustID).ToList();

            //    if (billingAddress == null)
            //    {
            //        return Json("No Business Address Found", JsonRequestBehavior.AllowGet);
            //    }
            //    else if (serviceAddresses.Count == 0)
            //    {
            //        return Json("No Service Address Found", JsonRequestBehavior.AllowGet);
            //    }
            //    Customers_Billing_Address_Mapping bill = new Customers_Billing_Address_Mapping();
            //    bill.Customers_Billing_Address_Mapping_ID = billingAddress.Customers_Billing_Address_Mapping_ID;
            //    bill.AddressLine1 = billingAddress.AddressLine1;
            //    bill.AddressLine2 = billingAddress.AddressLine2;
            //    bill.AddressLine3 = billingAddress.AddressLine3;
            //    string LocationName = billingAddress.Location.LocationName;
            //    string CityName = billingAddress.City.CityName;
            //    bill.BillingPincode = billingAddress.BillingPincode;
            //    string State1 = billingAddress.State.State1;
            //    bill.GSTNo = billingAddress.GSTNo;


            //    var billingAddress1 = db.Customers_Billing_Address_Mapping.Where(x => x.CustomerID == intCustID).FirstOrDefault();
            //    long billingAddressID1 = billingAddress1.Customers_Billing_Address_Mapping_ID;
            //    var billingTelNos1 = db.Customers_Billing_Address_Tel_No_Mapping.Where(x => x.Customers_Billing_Address_Mapping_ID == billingAddressID1).ToList();
            //    string strBillingTelNos = String.Join(",", billingTelNos1.Select(x => x.Customers_Billing_Address_Tel_No.ToString()).ToArray());
            //    var billingCellNos1 = db.Customers_Billing_Address_Cell_No_Mapping.Where(x => x.Customers_Billing_Address_Mapping_ID == billingAddressID1).ToList();
            //    string strBillingCellNos = String.Join(",", billingCellNos1.Select(x => x.Customers_Billing_Address_Cell_No.ToString()).ToArray());
            //    var billingEmail1 = db.Customers_Billing_Address_Email_Mapping.Where(x => x.Customers_Billing_Address_Mapping_ID == billingAddressID1).ToList();
            //    string strBillingEmails = String.Join(",", billingEmail1.Select(x => x.Customers_Billing_Address_Email.ToString()).ToArray());
            //    return Json(new { bill, /*servicesAddressList,*/ LocationName, CityName, State1, cust, strBillingTelNos, strBillingCellNos, strBillingEmails }, JsonRequestBehavior.AllowGet);
            //}
            //else
            //{

            //    return Json("Wrong Customer ID", JsonRequestBehavior.AllowGet);
            //}
            return Json("Some Error Occured", JsonRequestBehavior.AllowGet); ;

        }

        [HttpPost]
        public ActionResult GetServiceMappedForEstimate(string EstimateRefNo)
        {
            int intEstimateID;

            if (!string.IsNullOrEmpty(EstimateRefNo))
            {
                intEstimateID = db.Estimates.Where(e => e.ReferenceNo == EstimateRefNo).OrderByDescending(e => e.EstimateID).First().EstimateID;
                var services = db.Estimate_Services_Mapping.Where(x => x.EstimateID == intEstimateID).ToList();

                if (services.Count == 0)
                {
                    return Json("No Services Found", JsonRequestBehavior.AllowGet);
                }
                List<Estimate_Services_Mapping> servicesList = new List<Estimate_Services_Mapping>();
                services.ForEach(x =>
                {
                    servicesList.Add(new Estimate_Services_Mapping
                    {
                        Estimate_Services_Mapping_ID = x.Estimate_Services_Mapping_ID,
                        //EstimateID = x.EstimateID,
                        ServiceID = x.ServiceID,
                        ServiceGroupID = x.ServiceGroupID,
                        FrequencyOfServiceID = x.FrequencyOfServiceID,
                        Rate = x.Rate,
                        GST = x.GST,
                        Qty = x.Qty,
                        Tax = x.Tax,
                        FinalRatePerService = x.FinalRatePerService
                    });
                });
                return Json(servicesList, JsonRequestBehavior.AllowGet);
            }
            else
            {

                return Json("Wrong Estimate Reference Number", JsonRequestBehavior.AllowGet);
            }

        }
        public string uploadnow(HttpPostedFileWrapper upload)
        {
            string subPath = "~/Images/UploadedImages"; // your code goes here

            bool exists = Directory.Exists(Server.MapPath(subPath));

            if (!exists)
                Directory.CreateDirectory(Server.MapPath(subPath));
            if (upload != null)
            {
                string ImageName = upload.FileName;
                string path = Path.Combine(Server.MapPath(subPath), ImageName);
                upload.SaveAs(path);
                
            }
            return "Image Uploaded";
            //var result = new { responseText = "Successed", ID = "32" };
            //return Json(result, JsonRequestBehavior.AllowGet);
        }
        public ActionResult uploadPartial()
        {
            string subPath = "~/Images/UploadedImages"; // your code goes here

            bool exists = System.IO.Directory.Exists(Server.MapPath(subPath));

            if (!exists)
                System.IO.Directory.CreateDirectory(Server.MapPath(subPath));
            var appData = Server.MapPath(subPath);
            var images = Directory.GetFiles(appData).Select(x => new imagesviewmodel
            {
                Url = Url.Content(subPath + "/" +Path.GetFileName(x))
            });
            return View(images);
        }
        public ActionResult EstimateCreated(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Estimate estimate = db.Estimates.Find(id);
            if (estimate == null)
            {
                return HttpNotFound();
            }
            ViewBag.CustomerID = new SelectList(db.Customers, "CustomerID", "CustomerName", estimate.CustomerID);
            ViewBag.CreatedBy = new SelectList(db.UserLogins, "UserID", "UserName", estimate.CreatedBy);
            ViewBag.LastUpdatedBy = new SelectList(db.UserLogins, "UserID", "UserName", estimate.LastUpdatedBy);
            ViewBag.LeadID = new SelectList(db.Leads, "LeadID", "Title", estimate.LeadID);
            return View(estimate);
        }

        // GET: Estimates/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Estimate estimate = db.Estimates.Find(id);
            if (estimate == null)
            {
                return HttpNotFound();
            }
            //From here copied from Create
            var custs = db.Customers.Select(s => new
            {
                s.CustomerID,
                //Description = string.Format("{0} - £{1} £{2} £{3} £{4} ", s.LeadID, s.Title, s.FirstName, s.LastName, s.CustomerName)
                CustomerName = s.CustomerID + " - " + s.Title + " " + s.FirstName + " " + s.LastName + " " + s.CustomerName

            }).ToList();
            ViewBag.CustomerID = new SelectList(custs, "CustomerID", "CustomerName", estimate.CustomerID);
            ViewBag.CreatedBy = new SelectList(db.UserLogins, "UserID", "UserName", estimate.CreatedBy);
            ViewBag.LastUpdatedBy = new SelectList(db.UserLogins, "UserID", "UserName", estimate.LastUpdatedBy);
            var leads = db.Leads.Select(s => new
            {
                s.LeadID,
                //Description = string.Format("{0} - £{1} £{2} £{3} £{4} ", s.LeadID, s.Title, s.FirstName, s.LastName, s.CustomerName)
                Description = s.LeadID + " - " + s.Title + " " + s.FirstName + " " + s.LastName + " " + s.CustomerName

            }).ToList();

            ViewBag.LeadID = new SelectList(leads, "LeadID", "Description", estimate.LeadID);
            ViewBag.FrequencyOfServiceID = new SelectList(db.FrequencyOfServices.OrderBy(x => x.Order), "FrequencyOfServiceID", "FrequencyOfService1");
            ViewBag.ServiceGroupID = new SelectList(db.ServiceGroups, "ServiceGroupID", "ServiceGroup1");
            ViewBag.ServiceID = new SelectList(db.Services.Where(x => x.ServiceID == 0), "ServiceID", "ServiceName");
            //ViewBag.LeadID = new SelectList(db.Leads, "LeadID", "Title");

            //ViewBag.LocationID = new SelectList(db.Locations.Select(x => new { x.LocationID, x.LocationName }), "LocationID", "LocationName", estimate.);
            //ViewBag.CityID = new SelectList(db.Cities.Select(x => new { x.CityID, x.CityName }), "CityID", "CityName");
            //ViewBag.StateID = new SelectList(db.States.Select(x => new { x.StateID, x.State1 }), "StateID", "State1");

            // Use FinalEstimateRate to pass Original Estimate Ref No
           var ogEstimate = db.Estimates.Where(x => x.EstimateID == estimate.OriginalEstimateID).Select(x => new { x.EstimateID , ReferenceNo = x.ReferenceNo + " Revision: " + estimate.RevisionNo });
            //foreach (var item in ogEstimate) {
            //    item.ReferenceNo = item.ReferenceNo + " Revision: " + estimate.RevisionNo;
            //}
            ViewBag.OriginalEstimateID = new SelectList(ogEstimate, "EstimateID", "ReferenceNo", estimate.OriginalEstimateID);
            return View(estimate);
        }

        // POST: Estimates/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Edit([Bind(Include = "EstimateID,NewCustomer,ExistingCustomer,LeadID,CustomerID,GSTinDate,FinalEstimateRate,KindAttention,Subject,WelcomeSentence,ServiceScope,Footer,CreatedDate,LastUpdatedDate,CreatedBy,LastUpdatedBy,isNewEstimate,EstimateDate,OriginalEstimateID")] Estimate estimate, FormCollection formValues, int[] Lead_Services_Mapping_ID, int[] Estimate_Services_Mapping_ID, String[] ServiceGroupID, String[] ServiceID, String[] FrequencyOfServiceID, String[] Qty, String[] GST, String[] Tax, String[] Rate, String[] FinalRatePerService, string submit)
        {
            if (ModelState.IsValid)
            {
                db.Entry(estimate).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CustomerID = new SelectList(db.Customers, "CustomerID", "CustomerName", estimate.CustomerID);
            ViewBag.CreatedBy = new SelectList(db.UserLogins, "UserID", "UserName", estimate.CreatedBy);
            ViewBag.LastUpdatedBy = new SelectList(db.UserLogins, "UserID", "UserName", estimate.LastUpdatedBy);
            ViewBag.LeadID = new SelectList(db.Leads, "LeadID", "Title", estimate.LeadID);
            return View(estimate);
        }

        // GET: Estimates/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Estimate estimate = db.Estimates.Find(id);
            if (estimate == null)
            {
                return HttpNotFound();
            }


            EstimateViewModel estViewModel = new EstimateViewModel();
            //From here copied from Create
            var custs = db.Customers.Select(s => new
            {
                s.CustomerID,
                //Description = string.Format("{0} - £{1} £{2} £{3} £{4} ", s.LeadID, s.Title, s.FirstName, s.LastName, s.CustomerName)
                CustomerName = s.CustomerID + " - " + s.Title + " " + s.FirstName + " " + s.LastName + " " + s.CustomerName

            }).ToList();
            ViewBag.CustomerID = new SelectList(custs, "CustomerID", "CustomerName", estimate.CustomerID);
            ViewBag.CreatedBy = new SelectList(db.UserLogins, "UserID", "UserName", estimate.CreatedBy);
            ViewBag.LastUpdatedBy = new SelectList(db.UserLogins, "UserID", "UserName", estimate.LastUpdatedBy);
            var leads = db.Leads.Select(s => new
            {
                s.LeadID,
                //Description = string.Format("{0} - £{1} £{2} £{3} £{4} ", s.LeadID, s.Title, s.FirstName, s.LastName, s.CustomerName)
                Description = s.LeadID + " - " + s.Title + " " + s.FirstName + " " + s.LastName + " " + s.CustomerName

            }).ToList();

            ViewBag.LeadID = new SelectList(leads, "LeadID", "Description", estimate.LeadID);
            ViewBag.FrequencyOfServiceID = new SelectList(db.FrequencyOfServices.OrderBy(x => x.Order), "FrequencyOfServiceID", "FrequencyOfService1");
            ViewBag.ServiceGroupID = new SelectList(db.ServiceGroups, "ServiceGroupID", "ServiceGroup1");
            ViewBag.ServiceID = new SelectList(db.Services.Where(x => x.ServiceID == 0), "ServiceID", "ServiceName");
            //ViewBag.LeadID = new SelectList(db.Leads, "LeadID", "Title");

            //ViewBag.LocationID = new SelectList(db.Locations.Select(x => new { x.LocationID, x.LocationName }), "LocationID", "LocationName", estimate.);
            //ViewBag.CityID = new SelectList(db.Cities.Select(x => new { x.CityID, x.CityName }), "CityID", "CityName");
            //ViewBag.StateID = new SelectList(db.States.Select(x => new { x.StateID, x.State1 }), "StateID", "State1");

            // Use FinalEstimateRate to pass Original Estimate Ref No
            var ogEstimate = db.Estimates.Where(x => x.EstimateID == estimate.OriginalEstimateID).Select(x => new { x.EstimateID, ReferenceNo = x.ReferenceNo + " Revision: " + estimate.RevisionNo });
            //foreach (var item in ogEstimate) {
            //    item.ReferenceNo = item.ReferenceNo + " Revision: " + estimate.RevisionNo;
            //}
            ViewBag.OriginalEstimateID = new SelectList(ogEstimate, "EstimateID", "ReferenceNo", estimate.OriginalEstimateID);
            return View(estimate);
        }

        // POST: Estimates/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Estimate estimate = db.Estimates.Find(id);
            db.Estimates.Remove(estimate);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using SadguruCRM.Models;
using System.Dynamic;
using SadguruCRM.Models.ViewModels;
using SadguruCRM.Helpers;
using SadguruCRM.EnumClassesAndHelpers;
using SadguruCRM.ViewModels;
using System.Threading.Tasks;
using DocumentFormat.OpenXml.Wordprocessing;

namespace SadguruCRM.Controllers
{
    [VerifyUser]
    public class CustomersController : Controller
    {
        private SadguruCRMEntities db = new SadguruCRMEntities();
        private static TimeZoneInfo INDIAN_ZONE = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");

        // GET: Customers
        public ActionResult Index()
        {
            var customers = db.Customers.Include(c => c.Branch).Include(c => c.UserLogin).Include(c => c.CustomerStatu).Include(c => c.UserLogin1).Include(c => c.Source).Include(c => c.Source1).Include(c => c.Industry).Include(c => c.Customers_Billing_Address_Mapping);
            foreach (var item in customers)
            {
                if (item.CustomerName == null)
                {
                    item.CustomerName = item.Title + " " + item.FirstName + " " + item.LastName;
                }
                if (item.Customers_Billing_Address_Mapping.Count > 0) {
                    if (item.Customers_Billing_Address_Mapping.First().Location != null)
                    {
                        item.FirstName = item.Customers_Billing_Address_Mapping.First().Location.LocationName;
                    }
                }
                    
            }
            return View(customers.ToList());

            // dynamic mymodel = new ExpandoObject();
            // mymodel.customers = customers;
            // mymodel.city = db.Cities;
            //// var serviceAddress = db.Customers_Service_Address_Mapping.Include(k => k.CityID).Include(k => k.StateID).Include(k => k.LocationID).ToList();
            // //mymodel.ServiceAddress = serviceAddress;
            // CustomerAddress cAddress = new CustomerAddress();
            // mymodel.ServiceAddress = cAddress;
            // Customers_Billing_Address_Mapping cBillingAddress = new Customers_Billing_Address_Mapping();
            // mymodel.BillingAddress = cBillingAddress;
            //return View(mymodel);

            //return View(customers.ToList());
        }

        // GET: Customers/Details/5
        public ActionResult Details(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Customer customer = db.Customers.Find(id);
            if (customer == null)
            {
                return HttpNotFound();
            }
            ViewBag.BranchID = new SelectList(db.Branches, "BranchID", "BranchName", customer.BranchID);
            ViewBag.CreatedBy = new SelectList(db.UserLogins, "UserID", "UserName", customer.CreatedBy);
            ViewBag.CustomerStatusID = new SelectList(db.CustomerStatus, "CustomerStatusID", "Status", customer.CustomerStatusID);
            ViewBag.SourceID = new SelectList(db.Sources, "SourceID", "Source1", customer.SourceID);
            ViewBag.SubSourceID = new SelectList(db.Sources, "SourceID", "Source1", customer.SubSourceID);
            ViewBag.LocationID = new SelectList(db.Locations, "LocationID", "LocationName");
            ViewBag.Locations = new SelectList(db.Locations, "LocationID", "LocationName");
            ViewBag.CityID = new SelectList(db.Cities, "CityID", "CityName");
            ViewBag.StateID = new SelectList(db.States, "StateID", "State1");
            ViewBag.ReligionID = new SelectList(db.Religions, "ReligionID", "ReligionName", customer.ReligionID);
            var enumData = from GSTTypes g in Enum.GetValues(typeof(GSTTypes))
                           select new
                           {
                               ID = (byte)g,
                               Name = EnumHelper.GetEnumDescription(g)
                           };

            ViewBag.GSTTypes = new SelectList(enumData, "ID", "Name", customer.GST_Type_Enum);
            ViewBag.IndustryID = new SelectList(db.Industries, "IndustryID", "IndustryName", customer.IndustryID);
            //List<Lead> leadsOrderedNewCode = new List<Lead>();
            IQueryable<Lead> leads;
            if (customer.LeadID == null)
            {
                leads = db.Leads.Where(l => l.LeadStatus.Status.ToLower() == "done" && !db.Customers.Select(c => c.LeadID).Contains(l.LeadID)).Include(l => l.Branch).Include(l => l.City).Include(l => l.UserLogin).Include(l => l.LeadStatus).Include(l => l.Source).Include(l => l.Source1).Include(l => l.PremisesType);            //new Code
            }
            else
            {
                leads = db.Leads.Where(l => l.LeadID == customer.LeadID).Include(l => l.Branch).Include(l => l.City).Include(l => l.UserLogin).Include(l => l.LeadStatus).Include(l => l.Source).Include(l => l.Source1).Include(l => l.PremisesType);            //new Code
            }


            //var leadsList = await new HelperNonStatic().GenerateCustomerNameAndServicesShortCodes(leads);
            var leadsOrderedNewCode = leads.Select(s => new
            {
                s.LeadID,
                Description = s.LeadID + " - " + s.Title + " " + s.FirstName + " " + s.LastName + " " + s.CustomerName

            }).OrderByDescending(i => i.LeadID).ToList();
            ViewBag.LeadID = new SelectList(leadsOrderedNewCode, "LeadID", "Description", customer.LeadID);

            CustomerViewModel custVM = new CustomerViewModel();
            custVM.customer = customer;
            custVM.billing_address = customer.Customers_Billing_Address_Mapping.FirstOrDefault(i => i.CustomerID == id);
            if (custVM.billing_address == null)
            {

            }
            else
            {
                custVM.list_billing_address_tel = db.Customers_Billing_Address_Tel_No_Mapping.Where(i => i.Customers_Billing_Address_Mapping_ID == custVM.billing_address.Customers_Billing_Address_Mapping_ID).ToList();
                //if (custVM.list_billing_address_tel.Count == 0) {
                //    Customers_Billing_Address_Tel_No_Mapping tel = new Customers_Billing_Address_Tel_No_Mapping();
                //    custVM.list_billing_address_tel.Add(tel);
                //}
                custVM.list_billing_address_cell = db.Customers_Billing_Address_Cell_No_Mapping.Where(i => i.Customers_Billing_Address_Mapping_ID == custVM.billing_address.Customers_Billing_Address_Mapping_ID).ToList();
                //if (custVM.list_billing_address_cell.Count == 0)
                //{
                //    Customers_Billing_Address_Cell_No_Mapping cell = new Customers_Billing_Address_Cell_No_Mapping();
                //    custVM.list_billing_address_cell.Add(cell);
                //}
                custVM.list_billing_address_email = db.Customers_Billing_Address_Email_Mapping.Where(i => i.Customers_Billing_Address_Mapping_ID == custVM.billing_address.Customers_Billing_Address_Mapping_ID).ToList();
                custVM.list_service_address = db.Customers_Service_Address_Mapping.Where(i => i.CustomerID == id).ToList();
                if (custVM.list_service_address.Count == 0)
                {
                    Customers_Service_Address_Mapping servAdd = new Customers_Service_Address_Mapping();
                    custVM.list_service_address.Add(servAdd);
                }
            }

            var service_address_ids = custVM.list_service_address.Select(i => i.Customers_Service_Address_Mapping_ID);

            return View(custVM);
        }

        // GET: Customers/Create
        public async Task<ActionResult> Create()
        {
            ViewBag.BranchID = new SelectList(db.Branches, "BranchID", "BranchName");
            ViewBag.CreatedBy = new SelectList(db.UserLogins, "UserID", "UserName");
            ViewBag.CustomerStatusID = new SelectList(db.CustomerStatus, "CustomerStatusID", "Status");
            ViewBag.LastupdatedBy = new SelectList(db.UserLogins, "UserID", "UserName");
            ViewBag.SourceID = new SelectList(db.Sources.Where(x => x.ParentSourceID == null), "SourceID", "Source1");
            ViewBag.SubSourceID = new SelectList(db.Sources.Where(x => x.ParentSourceID != null), "SourceID", "Source1");
            ViewBag.LocationID = new SelectList(db.Locations, "LocationID", "LocationName");
            ViewBag.CityID = new SelectList(db.Cities, "CityID", "CityName");
            ViewBag.StateID = new SelectList(db.States, "StateID", "State1", 2);
            ViewBag.ReligionID = new SelectList(db.Religions, "ReligionID", "ReligionName");
            var enumData = from GSTTypes g in Enum.GetValues(typeof(GSTTypes))
                           select new
                           {
                               ID = (byte)g,
                               Name = EnumHelper.GetEnumDescription(g)
                           };

            ViewBag.GSTTypes = new SelectList(enumData, "ID", "Name");
            ViewBag.IndustryID = new SelectList(db.Industries, "IndustryID", "IndustryName");

            //List<Lead> leadsOrderedNewCode = new List<Lead>();
            var leads = db.Leads.Where(l => l.LeadStatus.Status.ToLower() == "done" && !db.Customers.Select(c => c.LeadID).Contains(l.LeadID)).Include(l => l.Branch).Include(l => l.City).Include(l => l.UserLogin).Include(l => l.LeadStatus).Include(l => l.Source).Include(l => l.Source1).Include(l => l.PremisesType);            //new Code
            //var leadsList = await new HelperNonStatic().GenerateCustomerNameAndServicesShortCodes(leads);
            var  leadsOrderedNewCode = leads.Select(s => new
            {
                s.LeadID,
                Description = s.LeadID + " - " + s.Title + " " + s.FirstName + " " + s.LastName + " " + s.CustomerName

            }).OrderByDescending(i => i.LeadID).ToList();
            ViewBag.LeadID = new SelectList(leadsOrderedNewCode, "LeadID", "Description");
            return View();
        }

        public ActionResult GetCustomerNumber(string BranchID)
        {
            int intOutBranchID;
            string custNumber = "";
            DateTime today = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, INDIAN_ZONE);
            var year = today.ToString("yy");
            var month = today.ToString("MM");
                if (int.TryParse(BranchID, out intOutBranchID))
                {
                    string branchShortCode = db.Branches.Find(intOutBranchID).BranchShortCode;
                //var custNumberToIncreaseAsList = db.Customers.Where(e => e.CustomerNo.Contains(year)).OrderByDescending(e => e.CustomerID).Take(1).Select(e => e.CustomerNo).ToList();
                var custNumberToIncreaseAsList = db.Customers.OrderByDescending(e => e.CustomerID).Take(1).Select(e => e.CustomerNo).ToList();
                if (custNumberToIncreaseAsList.Count == 0)
                    {
                        custNumber = branchShortCode + year + "0001";
                    }
                    else
                    {
                        string string1 = custNumberToIncreaseAsList.First().Substring(0,5);
                        //string string2 = custNumberToIncreaseAsList.First().Substring(5);
                        string numberAsString = custNumberToIncreaseAsList.First().Replace(string1, "");
                        string number = String.Format("{0:D4}", Int32.Parse(numberAsString) + 1);
                        custNumber = branchShortCode + year +  number ;
                        //referenceNumber = branchShortCode + year + month + referenceNumberToIncreaseAsList.First(). + "/" + today.ToString("yyyy") + "-" + (int.Parse(year) + 1).ToString();
                    }
                }
                else
                {
                    return Json("Error: Wrong Branch ID", JsonRequestBehavior.AllowGet);
                }

            
            return Json(custNumber, JsonRequestBehavior.AllowGet);

        }

        public ActionResult CheckDupliateCustomer(string title, string FirstName, string LastName, string CustomerName, string[] CellNo)

        {
            if (!String.IsNullOrEmpty(title) && ((!String.IsNullOrEmpty(FirstName) && CellNo.Length > 0) || (!String.IsNullOrEmpty(CustomerName) && CellNo.Length > 0)))
            {
                if (title == "M/S.")
                {
                    var custsExisting = db.Customers.Where(i => !String.IsNullOrEmpty(i.CustomerName) && i.Title == title && i.CustomerName == CustomerName);

                    if (custsExisting != null)
                    {
                        foreach (var cust in custsExisting)
                        {
                            var cellNosMappings = db.Customers_Billing_Address_Cell_No_Mapping.Where(i => i.Customers_Billing_Address_Mapping.CustomerID == cust.CustomerID).Select(i => i.Customers_Billing_Address_Cell_No).ToList();
                            cellNosMappings.Add(db.Customers.Find(cust.CustomerID).CellNumber);
                            foreach (var cell in CellNo)
                            {
                                if (!String.IsNullOrEmpty(cust.CellNumber) && cell == cust.CellNumber)
                                {
                                    return Json("1", JsonRequestBehavior.AllowGet);
                                }
                                if (cellNosMappings.Contains(cell))
                                {
                                    return Json("1", JsonRequestBehavior.AllowGet);
                                }
                            }
                        }

                    }
                }
                else
                {
                    var custsExisting = db.Customers.Where(i => i.Title == title && i.FirstName == FirstName && i.LastName == LastName);
                    if (custsExisting != null)
                    {
                        foreach (var cust in custsExisting)
                        {
                            var cellNosMappings = db.Customers_Billing_Address_Cell_No_Mapping.Where(i => i.Customers_Billing_Address_Mapping.CustomerID == cust.CustomerID).Select(i => i.Customers_Billing_Address_Cell_No).ToList();
                            cellNosMappings.Add(db.Customers.Find(cust.CustomerID).CellNumber);
                            foreach (var cell in CellNo)
                            {
                                if (!String.IsNullOrEmpty(cust.CellNumber) && cell == cust.CellNumber)
                                {
                                    return Json("1", JsonRequestBehavior.AllowGet);
                                }
                                if (cellNosMappings.Contains(cell))
                                {
                                    return Json("1", JsonRequestBehavior.AllowGet);
                                }
                            }
                        }
                    }

                }
            }


            return Json("0", JsonRequestBehavior.AllowGet);



        }
        // POST: Customers/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Customer customer,string BillingAddressLine1, string BillingAddressLine2, string BillingAddressLine3, string BillingLocationID, string BillingCityID, string BillingPincode, string BillingStateID, string BillingGSTNo, string[] BillingCellNumber, string[] BillingEmail, string[] BillingTelNo, String[] ServiceAddressLine1, String[] ServiceAddressLine2, String[] ServiceAddressLine3, String[] ServiceLocationID, String[] ServiceCityID, String[] ServicePincode, String[] ServiceStateID, String[] ServiceConsultPerson, String[] ServiceEmailHidden, String[] ServiceTelNoHidden, String[] ServiceCellNoHidden, string SubmitType)
        {
            if (ModelState.IsValid)
            {
                if (customer.Title != "M/S.")
                {
                    customer.CustomerName = null;
                }
                else
                {
                    customer.FirstName = null;
                    customer.LastName = null;
                }
                customer.CreatedBy = Convert.ToInt32(Session["UserID"]);
                customer.CreatedOn = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, INDIAN_ZONE);
                customer.TelNumber = BillingTelNo[0];
                customer.CellNumber = BillingCellNumber[0];
                customer.EmailId = BillingEmail[0];
                db.Customers.Add(customer);
                db.SaveChanges();
                long CustomerId = customer.CustomerID;
                Customers_Billing_Address_Mapping billingAdd = new Customers_Billing_Address_Mapping();
                billingAdd.CustomerID = customer.CustomerID;
                billingAdd.AddressLine1 = BillingAddressLine1;
                billingAdd.AddressLine2 = BillingAddressLine2;
                billingAdd.AddressLine3 = BillingAddressLine3;
                billingAdd.BillingLocationID = Int32.Parse(BillingLocationID);
                billingAdd.BillingCityID = Int32.Parse(BillingCityID);
                billingAdd.BillingStateID = Int32.Parse(BillingStateID);
                billingAdd.BillingPincode = BillingPincode;
                billingAdd.GSTNo = BillingGSTNo;
                db.Customers_Billing_Address_Mapping.Add(billingAdd);
                db.SaveChanges();

                for (int i = 1; i < BillingCellNumber.Length; i++)
                {
                    Customers_Billing_Address_Cell_No_Mapping cell = new Customers_Billing_Address_Cell_No_Mapping();
                    cell.Customers_Billing_Address_Mapping_ID = billingAdd.Customers_Billing_Address_Mapping_ID;
                    cell.Customers_Billing_Address_Cell_No = BillingCellNumber[i];
                    db.Customers_Billing_Address_Cell_No_Mapping.Add(cell);
                    db.SaveChanges();
                }

                for (int i = 1; i < BillingEmail.Length; i++)
                {
                    Customers_Billing_Address_Email_Mapping email = new Customers_Billing_Address_Email_Mapping();
                    email.Customers_Billing_Address_Mapping_ID = billingAdd.Customers_Billing_Address_Mapping_ID;
                    email.Customers_Billing_Address_Email = BillingEmail[i];
                    db.Customers_Billing_Address_Email_Mapping.Add(email);
                    db.SaveChanges();
                }

                for (int i = 1; i < BillingTelNo.Length; i++)
                {
                    Customers_Billing_Address_Tel_No_Mapping tel = new Customers_Billing_Address_Tel_No_Mapping();
                    tel.Customers_Billing_Address_Mapping_ID = billingAdd.Customers_Billing_Address_Mapping_ID;
                    tel.Customers_Billing_Address_Tel_No = BillingTelNo[i];
                    db.Customers_Billing_Address_Tel_No_Mapping.Add(tel);
                    db.SaveChanges();
                }
                //if (customer.IsBillAddSame)
                //{
                   
                //}
                //else {
                    for (int i = 0; i < ServiceAddressLine1.Length; i++)
                    {
                        if ( !String.IsNullOrEmpty(ServiceAddressLine1[i]) ){

                            Customers_Service_Address_Mapping servAdd = new Customers_Service_Address_Mapping();
                            servAdd.CustomerID = customer.CustomerID;
                            servAdd.AddressLine1 = ServiceAddressLine1[i];
                            servAdd.AddressLine2 = ServiceAddressLine2[i];
                            servAdd.AddressLine3 = ServiceAddressLine3[i];
                            servAdd.ServiceLocationID = Int32.Parse(ServiceLocationID[i]);
                            servAdd.ServiceCityID = Int32.Parse(ServiceCityID[i]);
                            servAdd.ServicePincode = ServicePincode[i];
                            servAdd.ServiceStateID = Int32.Parse(ServiceStateID[i]);
                            servAdd.ServiceAddressConsultPerson = ServiceConsultPerson[i];

                            //db.Customers_Service_Address_Mapping.Add(servAdd);
                            //db.SaveChanges();
                            //long servAddrID = servAdd.Customers_Service_Address_Mapping_ID;
                            string[] emails = ServiceEmailHidden[i].Split('|');
                            for (int j = 0; j < emails.Length; j++)
                            {
                                if (j == 0)
                                {
                                    servAdd.Customer_Service_Address_Email_1 = emails[j];
                                }
                                else if (j == 1)
                                {
                                    servAdd.Customer_Service_Address_Email_2 = emails[j];
                                }
                                else if (j == 2)
                                {
                                    servAdd.Customer_Service_Address_Email_3 = emails[j];
                                }
                                else if (j == 3)
                                {
                                    servAdd.Customer_Service_Address_Email_4 = emails[j];
                                }
                                else if (j == 4)
                                {
                                    servAdd.Customer_Service_Address_Email_5 = emails[j];
                                }
                                //Customers_Service_Address_Email_Mapping email = new Customers_Service_Address_Email_Mapping();
                                //email.Customers_Service_Address_Mapping_ID = servAddrID;
                                //email.Customers_Service_Address_Email = emails[j];
                                //db.Customers_Service_Address_Email_Mapping.Add(email);
                                //db.SaveChanges();
                            }
                            string[] TelNos = ServiceTelNoHidden[i].Split('|');
                            for (int j = 0; j < TelNos.Length; j++)
                            {
                                if (j == 0)
                                {
                                    servAdd.Customer_Service_Address_Tel_No_1 = TelNos[j];
                                }
                                else if (j == 1)
                                {
                                    servAdd.Customer_Service_Address_Tel_No_2 = TelNos[j];
                                }
                                else if (j == 2)
                                {
                                    servAdd.Customer_Service_Address_Tel_No_3 = TelNos[j];
                                }
                                else if (j == 3)
                                {
                                    servAdd.Customer_Service_Address_Tel_No_4 = TelNos[j];
                                }
                                else if (j == 4)
                                {
                                    servAdd.Customer_Service_Address_Tel_No_5 = TelNos[j];
                                }
                                //Customers_Service_Address_Tel_No_Mapping tel = new Customers_Service_Address_Tel_No_Mapping();
                                //tel.Customers_Service_Address_Mapping_ID = servAddrID;
                                //tel.Customers_Service_Address_Tel_No = TelNos[j];
                                //db.Customers_Service_Address_Tel_No_Mapping.Add(tel);
                                //db.SaveChanges();
                            }
                            string[] CellNos = ServiceCellNoHidden[i].Split('|');
                            for (int j = 0; j < CellNos.Length; j++)
                            {
                                if (j == 0)
                                {
                                    servAdd.Customer_Service_Address_Cell_No_1 = CellNos[j];
                                }
                                else if (j == 1)
                                {
                                    servAdd.Customer_Service_Address_Cell_No_2 = CellNos[j];
                                }
                                else if (j == 2)
                                {
                                    servAdd.Customer_Service_Address_Cell_No_3 = CellNos[j];
                                }
                                else if (j == 3)
                                {
                                    servAdd.Customer_Service_Address_Cell_No_4 = CellNos[j];
                                }
                                else if (j == 4)
                                {
                                    servAdd.Customer_Service_Address_Cell_No_5 = CellNos[j];
                                }
                                //Customers_Service_Address_Cell_No_Mapping cell = new Customers_Service_Address_Cell_No_Mapping();
                                //cell.Customers_Service_Address_Mapping_ID = servAddrID;
                                //cell.Customers_Service_Address_Cell_No = CellNos[j];
                                //db.Customers_Service_Address_Cell_No_Mapping.Add(cell);
                                //db.SaveChanges();
                            }
                            db.Customers_Service_Address_Mapping.Add(servAdd);
                            db.SaveChanges();

                        }
                        

                        
                    }
                // Reflecting changes in Lead
                if (customer.LeadID != null)
                {
                    Lead lead = db.Leads.Find(customer.LeadID);
                    lead.Title = customer.Title;
                    lead.FirstName = customer.FirstName;
                    lead.LastName = customer.LastName;
                    lead.CustomerName = customer.CustomerName;
                    lead.ConsultPerson = customer.ConsultPerson;
                    lead.ConsultPersonDesignation = customer.ConsultPersonDesignation;
                    lead.SourceID = customer.SourceID;
                    lead.SubSourceID = customer.SubSourceID;
                    lead.BranchID = customer.BranchID;
                    lead.StatusReason = customer.StatusReason;
                    lead.IndustryID = customer.IndustryID;
                    lead.AddressLine1 = customer.Customers_Billing_Address_Mapping.First().AddressLine1;
                    lead.AddressLine2 = customer.Customers_Billing_Address_Mapping.First().AddressLine2;
                    lead.AddressLine3 = customer.Customers_Billing_Address_Mapping.First().AddressLine3;
                    lead.LocationID = customer.Customers_Billing_Address_Mapping.First().BillingLocationID;
                    lead.Pincode = customer.Customers_Billing_Address_Mapping.First().BillingPincode;
                    lead.StateID = customer.Customers_Billing_Address_Mapping.First().BillingStateID;
                    lead.TelNo = BillingTelNo[0];
                    lead.CellNo = BillingCellNumber[0];
                    lead.EmailID = BillingEmail[0];
                    db.Entry(lead).State = EntityState.Modified;
                    db.SaveChanges();


                    //for (int j = 0; j < lead.Lead_Tel_No_Mapping.Count; j++)
                    
                    var telsToDelete = db.Lead_Tel_No_Mapping.Where(tel => tel.LeadID == lead.LeadID && !BillingTelNo.Contains(tel.Lead_Tel_No));
                    db.Lead_Tel_No_Mapping.RemoveRange(telsToDelete);                    
                    db.SaveChanges();
                    var newTels = BillingTelNo.Where(tel => !db.Lead_Tel_No_Mapping.Where(m => m.LeadID == lead.LeadID).Select(m => m.Lead_Tel_No).Contains(tel)).ToList();
                    for (int j = 1; j < newTels.Count; j++)
                    {
                        Lead_Tel_No_Mapping tel = new Lead_Tel_No_Mapping();
                        tel.LeadID = lead.LeadID;
                        tel.Lead_Tel_No = BillingTelNo[j];
                        db.Lead_Tel_No_Mapping.Add(tel);
                        db.SaveChanges();
                    }
                    var celsToDelete = db.Lead_Cell_No_Mapping.Where(cel => cel.LeadID == lead.LeadID && !BillingCellNumber.Contains(cel.Lead_Cell_No));
                    db.Lead_Cell_No_Mapping.RemoveRange(celsToDelete);
                    db.SaveChanges();
                    var newCels = BillingCellNumber.Where(cel => !db.Lead_Cell_No_Mapping.Where(m => m.LeadID == lead.LeadID).Select(m => m.Lead_Cell_No).Contains(cel)).ToList();
                    for (int j = 1; j < newCels.Count; j++)
                    {
                        Lead_Cell_No_Mapping cel = new Lead_Cell_No_Mapping();
                        cel.LeadID = lead.LeadID;
                        cel.Lead_Cell_No = BillingCellNumber[j];
                        db.Lead_Cell_No_Mapping.Add(cel);
                        db.SaveChanges();
                    }
                    var emailsToDelete = db.Lead_Email_Mapping.Where(e => e.LeadID == lead.LeadID && !BillingEmail.Contains(e.Lead_Email));
                    db.Lead_Email_Mapping.RemoveRange(emailsToDelete);
                    db.SaveChanges();
                    var newEmails = BillingEmail.Where(e => !db.Lead_Email_Mapping.Where(m => m.LeadID == lead.LeadID).Select(m => m.Lead_Email).Contains(e)).ToList();
                    for (int j = 1; j < newEmails.Count; j++)
                    {
                        Lead_Email_Mapping mail = new Lead_Email_Mapping();
                        mail.LeadID = lead.LeadID;
                        mail.Lead_Email = BillingEmail[j];
                        db.Lead_Email_Mapping.Add(mail);
                        db.SaveChanges();
                    }


                }
                //}

                if (SubmitType == "Save")
                {
                    //SendMessage("New object added");
                    return RedirectToAction("Index");
                }
                else if (SubmitType == "Save & Create New Lead")
                {
                    //SendMessage("New object added");
                    return RedirectToAction("Create");
                }

                return RedirectToAction("Index");
            }

            ViewBag.BranchID = new SelectList(db.Branches, "BranchID", "BranchName",customer.BranchID);
            ViewBag.CreatedBy = new SelectList(db.UserLogins, "UserID", "UserName", customer.BranchID);
            ViewBag.CustomerStatusID = new SelectList(db.CustomerStatus, "CustomerStatusID", "Status", customer.CustomerStatusID);
            ViewBag.SourceID = new SelectList(db.Sources, "SourceID", "Source1", customer.SourceID);
            ViewBag.SubSourceID = new SelectList(db.Sources, "SourceID", "Source1", customer.SubSourceID);
            ViewBag.LocationID = new SelectList(db.Locations, "LocationID", "LocationName");
            ViewBag.ReligionID = new SelectList(db.Religions, "ReligionID", "ReligionName", customer.ReligionID);
            ViewBag.CityID = new SelectList(db.Cities, "CityID", "CityName", BillingCityID);
            ViewBag.StateID = new SelectList(db.States, "StateID", "State1", BillingStateID);
            ViewBag.ReligionID = new SelectList(db.Religions, "ReligionID", "ReligionName", customer.ReligionID);
            var enumData = from GSTTypes g in Enum.GetValues(typeof(GSTTypes))
                           select new
                           {
                               ID = (byte)g,
                               Name = EnumHelper.GetEnumDescription(g)
                           };

            ViewBag.GSTTypes = new SelectList(enumData, "ID", "Name", customer.GST_Type_Enum);
            ViewBag.IndustryID = new SelectList(db.Industries, "IndustryID", "IndustryName", customer.IndustryID);
            return View(customer);
        }

        // GET: Customers/Edit/5
        public ActionResult Edit(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Customer customer = db.Customers.Find(id);
            if (customer == null)
            {
                return HttpNotFound();
            }
            ViewBag.BranchID = new SelectList(db.Branches, "BranchID", "BranchName", customer.BranchID);
            ViewBag.CreatedBy = new SelectList(db.UserLogins, "UserID", "UserName", customer.CreatedBy);
            ViewBag.CustomerStatusID = new SelectList(db.CustomerStatus, "CustomerStatusID", "Status", customer.CustomerStatusID);
            ViewBag.SourceID = new SelectList(db.Sources, "SourceID", "Source1", customer.SourceID);
            ViewBag.SubSourceID = new SelectList(db.Sources, "SourceID", "Source1", customer.SubSourceID);
            ViewBag.LocationID = new SelectList(db.Locations, "LocationID", "LocationName");
            ViewBag.Locations = new SelectList(db.Locations, "LocationID", "LocationName");
            ViewBag.CityID = new SelectList(db.Cities, "CityID", "CityName");
            ViewBag.StateID = new SelectList(db.States, "StateID", "State1");
            ViewBag.ReligionID = new SelectList(db.Religions, "ReligionID", "ReligionName", customer.ReligionID);
            var enumData = from GSTTypes g in Enum.GetValues(typeof(GSTTypes))
                           select new
                           {
                               ID = (byte)g,
                               Name = EnumHelper.GetEnumDescription(g)
                           };

            ViewBag.GSTTypes = new SelectList(enumData, "ID", "Name", customer.GST_Type_Enum);
            ViewBag.IndustryID = new SelectList(db.Industries, "IndustryID", "IndustryName", customer.IndustryID);
            //List<Lead> leadsOrderedNewCode = new List<Lead>();
            IQueryable<Lead> leads;
            if (customer.LeadID == null)
            {
                leads = db.Leads.Where(l => l.LeadStatus.Status.ToLower() == "done" && !db.Customers.Select(c => c.LeadID).Contains(l.LeadID)).Include(l => l.Branch).Include(l => l.City).Include(l => l.UserLogin).Include(l => l.LeadStatus).Include(l => l.Source).Include(l => l.Source1).Include(l => l.PremisesType);            //new Code
            }
            else {
                leads = db.Leads.Where(l => l.LeadID == customer.LeadID).Include(l => l.Branch).Include(l => l.City).Include(l => l.UserLogin).Include(l => l.LeadStatus).Include(l => l.Source).Include(l => l.Source1).Include(l => l.PremisesType);            //new Code
            }
            
            
            //var leadsList = await new HelperNonStatic().GenerateCustomerNameAndServicesShortCodes(leads);
            var leadsOrderedNewCode = leads.Select(s => new
            {
                s.LeadID,
                Description = s.LeadID + " - " + s.Title + " " + s.FirstName + " " + s.LastName + " " + s.CustomerName

            }).OrderByDescending(i => i.LeadID).ToList();
            ViewBag.LeadID = new SelectList(leadsOrderedNewCode, "LeadID", "Description", customer.LeadID);

            CustomerViewModel custVM = new CustomerViewModel();
            custVM.customer = customer;
            custVM.billing_address = customer.Customers_Billing_Address_Mapping.FirstOrDefault(i => i.CustomerID == id);
            if (custVM.billing_address == null)
            {

            }
            else {
                custVM.list_billing_address_tel = db.Customers_Billing_Address_Tel_No_Mapping.Where(i => i.Customers_Billing_Address_Mapping_ID == custVM.billing_address.Customers_Billing_Address_Mapping_ID).ToList();
                //if (custVM.list_billing_address_tel.Count == 0) {
                //    Customers_Billing_Address_Tel_No_Mapping tel = new Customers_Billing_Address_Tel_No_Mapping();
                //    custVM.list_billing_address_tel.Add(tel);
                //}
                custVM.list_billing_address_cell = db.Customers_Billing_Address_Cell_No_Mapping.Where(i => i.Customers_Billing_Address_Mapping_ID == custVM.billing_address.Customers_Billing_Address_Mapping_ID).ToList();
                //if (custVM.list_billing_address_cell.Count == 0)
                //{
                //    Customers_Billing_Address_Cell_No_Mapping cell = new Customers_Billing_Address_Cell_No_Mapping();
                //    custVM.list_billing_address_cell.Add(cell);
                //}
                custVM.list_billing_address_email = db.Customers_Billing_Address_Email_Mapping.Where(i => i.Customers_Billing_Address_Mapping_ID == custVM.billing_address.Customers_Billing_Address_Mapping_ID).ToList();
                custVM.list_service_address = db.Customers_Service_Address_Mapping.Where(i => i.CustomerID == id).ToList();
                if (custVM.list_service_address.Count == 0)
                {
                    Customers_Service_Address_Mapping servAdd = new Customers_Service_Address_Mapping();
                    servAdd.CustomerID = (long)id;
                    custVM.list_service_address.Add(servAdd);
                }
            }

            var service_address_ids = custVM.list_service_address.Select(i => i.Customers_Service_Address_Mapping_ID);
            
            return View(custVM);
        }

        // POST: Customers/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(CustomerViewModel customerViewModel)
        {
            if (String.IsNullOrEmpty(customerViewModel.list_service_address.ElementAt(0).AddressLine1)) {
                customerViewModel.list_service_address.Remove(customerViewModel.list_service_address.ElementAt(0));
                ModelState.Remove("list_service_address[0].ServiceLocationID");
                ModelState.Remove("list_service_address[0].ServiceStateID");
                ModelState.Remove("list_service_address[0].ServiceCityID");
            }
            //UpdateModel(customerViewModel);
            if (ModelState.IsValid)
            {

                customerViewModel.customer.LastupdatedBy = Convert.ToInt32(Session["UserID"]);
                customerViewModel.customer.LastupdatedOn = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, INDIAN_ZONE);

                //customerViewModel.billing_address.Customers_Billing_Address_Tel_No_Mapping = customerViewModel.list_billing_address_tel;
                //customerViewModel.customer.Customers_Billing_Address_Mapping.Add(customerViewModel.billing_address);

                //Customers_Billing_Address_Mapping bill = db.Customers_Billing_Address_Mapping.Find(customerViewModel.billing_address.Customers_Billing_Address_Mapping_ID);
                foreach (var item in customerViewModel.list_billing_address_tel)
                {
                    if (item.Customers_Billing_Address_Tel_No_Mapping_ID != 0)
                    {
                        if (String.IsNullOrEmpty(item.Customers_Billing_Address_Tel_No))
                        {
                            db.Entry(item).State = EntityState.Deleted;
                            //db.Customers_Billing_Address_Tel_No_Mapping.Remove(item); //throws exception
                        }
                        else
                        {
                            db.Entry(item).State = EntityState.Modified;
                        }
                    }
                    else
                    {
                        Customers_Billing_Address_Tel_No_Mapping tel = new Customers_Billing_Address_Tel_No_Mapping();
                        tel.Customers_Billing_Address_Mapping_ID = customerViewModel.billing_address.Customers_Billing_Address_Mapping_ID;
                        tel.Customers_Billing_Address_Tel_No = item.Customers_Billing_Address_Tel_No;
                        db.Customers_Billing_Address_Tel_No_Mapping.Add(tel);
                    }
                }
                foreach (var item in customerViewModel.list_billing_address_cell)
                {
                    if (item.Customers_Billing_Address_Cell_No_Mapping_ID != 0)
                    {
                        if (String.IsNullOrEmpty(item.Customers_Billing_Address_Cell_No))
                        {
                            db.Entry(item).State = EntityState.Deleted;
                            //db.Customers_Billing_Address_Tel_No_Mapping.Remove(item); //throws exception
                        }
                        else
                        {
                            db.Entry(item).State = EntityState.Modified;
                        }
                    }
                    else
                    {
                        Customers_Billing_Address_Cell_No_Mapping c = new Customers_Billing_Address_Cell_No_Mapping();
                        c.Customers_Billing_Address_Mapping_ID = customerViewModel.billing_address.Customers_Billing_Address_Mapping_ID;
                        c.Customers_Billing_Address_Cell_No = item.Customers_Billing_Address_Cell_No;
                        db.Customers_Billing_Address_Cell_No_Mapping.Add(c);
                    }
                }
                foreach (var item in customerViewModel.list_billing_address_email)
                {
                    if (item.Customers_Billing_Address_Email_Mapping_ID != 0)
                    {
                        if (String.IsNullOrEmpty(item.Customers_Billing_Address_Email))
                        {
                            db.Entry(item).State = EntityState.Deleted;
                            //db.Customers_Billing_Address_Tel_No_Mapping.Remove(item); //throws exception
                        }
                        else
                        {
                            db.Entry(item).State = EntityState.Modified;
                        }
                    }
                    else
                    {
                        Customers_Billing_Address_Email_Mapping c = new Customers_Billing_Address_Email_Mapping();
                        c.Customers_Billing_Address_Mapping_ID = customerViewModel.billing_address.Customers_Billing_Address_Mapping_ID;
                        c.Customers_Billing_Address_Email = item.Customers_Billing_Address_Email;
                        db.Customers_Billing_Address_Email_Mapping.Add(c);
                    }
                }

                var currentServiceAddsIDs = db.Customers_Service_Address_Mapping.Where(s => s.CustomerID == customerViewModel.customer.CustomerID).Select(s => s.Customers_Service_Address_Mapping_ID).ToList();
                foreach (var id in currentServiceAddsIDs) {
                    if (!customerViewModel.list_service_address.Select(s => s.Customers_Service_Address_Mapping_ID).Contains(id)) {
                        db.Entry(db.Customers_Service_Address_Mapping.Find(id)).State = EntityState.Deleted;
                    } 
                }
                    foreach (var item in customerViewModel.list_service_address) {
                    if (!String.IsNullOrEmpty(item.AddressLine1)) {
                        if (item.Customers_Service_Address_Mapping_ID != 0)
                        {
                            //if (String.IsNullOrEmpty(item.Customers_Billing_Address_Email))
                            //{
                             //   db.Entry(item).State = EntityState.Deleted;
                                //db.Customers_Billing_Address_Tel_No_Mapping.Remove(item); //throws exception
                            //}
                            //else
                            //{
                                db.Entry(item).State = EntityState.Modified;
                            //}
                        }
                        else
                        {
                            //Customer_Services_Mapping serv = new Customer_Services_Mapping();
                            //serv.CustomerID = customerViewModel.billing_address.Customers_Billing_Address_Mapping_ID;
                            //serv.Customers_Billing_Address_Email = item.Customers_Billing_Address_Email;
                            db.Customers_Service_Address_Mapping.Add(item);
                        }
                    }
                }
                    //customerViewModel.billing_address = 
                    db.Entry(customerViewModel.billing_address).State = EntityState.Modified;

                db.Entry(customerViewModel.customer).State = EntityState.Modified;
                db.SaveChanges();
                // Reflecting changes in Lead
                if (customerViewModel.customer.LeadID != null)
                {
                    Lead lead = db.Leads.Find(customerViewModel.customer.LeadID);
                    lead.Title = customerViewModel.customer.Title;
                    lead.FirstName = customerViewModel.customer.FirstName;
                    lead.LastName = customerViewModel.customer.LastName;
                    lead.CustomerName = customerViewModel.customer.CustomerName;
                    lead.ConsultPerson = customerViewModel.customer.ConsultPerson;
                    lead.ConsultPersonDesignation = customerViewModel.customer.ConsultPersonDesignation;
                    lead.SourceID = customerViewModel.customer.SourceID;
                    lead.SubSourceID = customerViewModel.customer.SubSourceID;
                    lead.BranchID = customerViewModel.customer.BranchID;
                    lead.StatusReason = customerViewModel.customer.StatusReason;
                    lead.IndustryID = customerViewModel.customer.IndustryID;
                    lead.AddressLine1 = customerViewModel.billing_address.AddressLine1;
                    lead.AddressLine2 = customerViewModel.billing_address.AddressLine2;
                    lead.AddressLine3 = customerViewModel.billing_address.AddressLine3;
                    lead.LocationID = customerViewModel.billing_address.BillingLocationID;
                    lead.Pincode = customerViewModel.billing_address.BillingPincode;
                    lead.StateID = customerViewModel.billing_address.BillingStateID;
                    lead.TelNo = customerViewModel.customer.TelNumber;
                    lead.CellNo = customerViewModel.customer.CellNumber;
                    lead.EmailID = customerViewModel.customer.EmailId;
                    db.Entry(lead).State = EntityState.Modified;
                    db.SaveChanges();


                    var existingTels = db.Lead_Tel_No_Mapping.Where(tel => tel.LeadID == lead.LeadID).Select(t => t.Lead_Tel_No );
                    foreach (var number in existingTels) {
                        if (!customerViewModel.list_billing_address_tel.Select(t => t.Customers_Billing_Address_Tel_No).Contains(number)) {
                            var item = db.Lead_Tel_No_Mapping.Where(t => t.LeadID == lead.LeadID && t.Lead_Tel_No == number);
                            if (item.Count() > 0) {
                                db.Entry(item.First()).State = EntityState.Deleted;
                            }                                
                        }
                    }
                    var newTels = customerViewModel.list_billing_address_tel.Select(tel => tel.Customers_Billing_Address_Tel_No).Where(tel => !existingTels.Contains(tel)).Where(tel => !String.IsNullOrEmpty(tel)).ToList();
                    for (int j = 0; j < newTels.Count; j++)
                    {
                        Lead_Tel_No_Mapping tel = new Lead_Tel_No_Mapping();
                        tel.LeadID = lead.LeadID;
                        tel.Lead_Tel_No = newTels.ElementAt(j);
                        db.Lead_Tel_No_Mapping.Add(tel);
                    }
                    db.SaveChanges();

                    var existingCells = db.Lead_Cell_No_Mapping.Where(c => c.LeadID == lead.LeadID).Select(c => c.Lead_Cell_No);
                    foreach (var number in existingCells)
                    {
                        if (!customerViewModel.list_billing_address_cell.Select(t => t.Customers_Billing_Address_Cell_No).Contains(number))
                        {
                            var item = db.Lead_Cell_No_Mapping.Where(t => t.LeadID == lead.LeadID && t.Lead_Cell_No == number);
                            if (item.Count() > 0)
                            {
                                db.Entry(item.First()).State = EntityState.Deleted;
                            }
                        }
                    }
                    var newCells = customerViewModel.list_billing_address_cell.Select(tel => tel.Customers_Billing_Address_Cell_No).Where(cell => !existingCells.Contains(cell)).Where(cell => !String.IsNullOrEmpty(cell)).ToList();
                    for (int j = 0; j < newCells.Count; j++)
                    {
                        Lead_Cell_No_Mapping cell = new Lead_Cell_No_Mapping();
                        cell.LeadID = lead.LeadID;
                        cell.Lead_Cell_No = newCells.ElementAt(j);
                        db.Lead_Cell_No_Mapping.Add(cell);
                    }
                    db.SaveChanges();


                    var existingEmails = db.Lead_Email_Mapping.Where(c => c.LeadID == lead.LeadID).Select(c => c.Lead_Email);
                    foreach (var mail in existingEmails)
                    {
                        if (!customerViewModel.list_billing_address_email.Select(t => t.Customers_Billing_Address_Email).Contains(mail))
                        {
                            var item = db.Lead_Email_Mapping.Where(t => t.LeadID == lead.LeadID && t.Lead_Email == mail);
                            if (item.Count() > 0)
                            {
                                db.Entry(item.First()).State = EntityState.Deleted;
                            }
                        }
                    }
                    var newEmails = customerViewModel.list_billing_address_email.Select(mail => mail.Customers_Billing_Address_Email).Where(mail => !existingEmails.Contains(mail)).Where(mail => !String.IsNullOrEmpty(mail)).ToList();
                    for (int j = 0; j < newEmails.Count; j++)
                    {
                        Lead_Email_Mapping e = new Lead_Email_Mapping();
                        e.LeadID = lead.LeadID;
                        e.Lead_Email = newEmails.ElementAt(j);
                        db.Lead_Email_Mapping.Add(e);
                    }
                    db.SaveChanges();


                }
                return RedirectToAction("Index");
            }
            ViewBag.BranchID = new SelectList(db.Branches, "BranchID", "BranchName", customerViewModel.customer.BranchID);
            ViewBag.CreatedBy = new SelectList(db.UserLogins, "UserID", "UserName", customerViewModel.customer.CreatedBy);
            ViewBag.CustomerStatusID = new SelectList(db.CustomerStatus, "CustomerStatusID", "Status", customerViewModel.customer.CustomerStatusID);
            ViewBag.SourceID = new SelectList(db.Sources, "SourceID", "Source1", customerViewModel.customer.SourceID);
            ViewBag.SubSourceID = new SelectList(db.Sources, "SourceID", "Source1", customerViewModel.customer.SubSourceID);
            ViewBag.LocationID = new SelectList(db.Locations, "LocationID", "LocationName");
            ViewBag.Locations = new SelectList(db.Locations, "LocationID", "LocationName");
            ViewBag.CityID = new SelectList(db.Cities, "CityID", "CityName");
            ViewBag.StateID = new SelectList(db.States, "StateID", "State1");
            ViewBag.ReligionID = new SelectList(db.Religions, "ReligionID", "ReligionName", customerViewModel.customer.ReligionID);
            var enumData = from GSTTypes g in Enum.GetValues(typeof(GSTTypes))
                           select new
                           {
                               ID = (byte)g,
                               Name = EnumHelper.GetEnumDescription(g)
                           };

            ViewBag.GSTTypes = new SelectList(enumData, "ID", "Name", customerViewModel.customer.GST_Type_Enum);
            ViewBag.IndustryID = new SelectList(db.Industries, "IndustryID", "IndustryName", customerViewModel.customer.IndustryID);

            CustomerViewModel custVM = new CustomerViewModel();
            custVM.customer = customerViewModel.customer;
            custVM.billing_address = customerViewModel.customer.Customers_Billing_Address_Mapping.FirstOrDefault(i => i.CustomerID == customerViewModel.customer.CustomerID);
            custVM.list_billing_address_tel = db.Customers_Billing_Address_Tel_No_Mapping.Where(i => i.Customers_Billing_Address_Mapping_ID == custVM.billing_address.Customers_Billing_Address_Mapping_ID).ToList();
            custVM.list_billing_address_cell = db.Customers_Billing_Address_Cell_No_Mapping.Where(i => i.Customers_Billing_Address_Mapping_ID == custVM.billing_address.Customers_Billing_Address_Mapping_ID).ToList();
            custVM.list_billing_address_email = db.Customers_Billing_Address_Email_Mapping.Where(i => i.Customers_Billing_Address_Mapping_ID == custVM.billing_address.Customers_Billing_Address_Mapping_ID).ToList();
            custVM.list_service_address = db.Customers_Service_Address_Mapping.Where(i => i.CustomerID == customerViewModel.customer.CustomerID).ToList();
            var service_address_ids = custVM.list_service_address.Select(i => i.Customers_Service_Address_Mapping_ID);

            return View(custVM);
        }

        // GET: Customers/Delete/5
        public ActionResult Delete(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Customer customer = db.Customers.Find(id);
            if (customer == null)
            {
                return HttpNotFound();
            }
            ViewBag.BranchID = new SelectList(db.Branches, "BranchID", "BranchName", customer.BranchID);
            ViewBag.CreatedBy = new SelectList(db.UserLogins, "UserID", "UserName", customer.CreatedBy);
            ViewBag.CustomerStatusID = new SelectList(db.CustomerStatus, "CustomerStatusID", "Status", customer.CustomerStatusID);
            ViewBag.SourceID = new SelectList(db.Sources, "SourceID", "Source1", customer.SourceID);
            ViewBag.SubSourceID = new SelectList(db.Sources, "SourceID", "Source1", customer.SubSourceID);
            ViewBag.LocationID = new SelectList(db.Locations, "LocationID", "LocationName");
            ViewBag.Locations = new SelectList(db.Locations, "LocationID", "LocationName");
            ViewBag.CityID = new SelectList(db.Cities, "CityID", "CityName");
            ViewBag.StateID = new SelectList(db.States, "StateID", "State1");
            ViewBag.ReligionID = new SelectList(db.Religions, "ReligionID", "ReligionName", customer.ReligionID);
            var enumData = from GSTTypes g in Enum.GetValues(typeof(GSTTypes))
                           select new
                           {
                               ID = (byte)g,
                               Name = EnumHelper.GetEnumDescription(g)
                           };

            ViewBag.GSTTypes = new SelectList(enumData, "ID", "Name", customer.GST_Type_Enum);
            ViewBag.IndustryID = new SelectList(db.Industries, "IndustryID", "IndustryName", customer.IndustryID);
            //List<Lead> leadsOrderedNewCode = new List<Lead>();
            IQueryable<Lead> leads;
            if (customer.LeadID == null)
            {
                leads = db.Leads.Where(l => l.LeadStatus.Status.ToLower() == "done" && !db.Customers.Select(c => c.LeadID).Contains(l.LeadID)).Include(l => l.Branch).Include(l => l.City).Include(l => l.UserLogin).Include(l => l.LeadStatus).Include(l => l.Source).Include(l => l.Source1).Include(l => l.PremisesType);            //new Code
            }
            else
            {
                leads = db.Leads.Where(l => l.LeadID == customer.LeadID).Include(l => l.Branch).Include(l => l.City).Include(l => l.UserLogin).Include(l => l.LeadStatus).Include(l => l.Source).Include(l => l.Source1).Include(l => l.PremisesType);            //new Code
            }


            //var leadsList = await new HelperNonStatic().GenerateCustomerNameAndServicesShortCodes(leads);
            var leadsOrderedNewCode = leads.Select(s => new
            {
                s.LeadID,
                Description = s.LeadID + " - " + s.Title + " " + s.FirstName + " " + s.LastName + " " + s.CustomerName

            }).OrderByDescending(i => i.LeadID).ToList();
            ViewBag.LeadID = new SelectList(leadsOrderedNewCode, "LeadID", "Description", customer.LeadID);

            CustomerViewModel custVM = new CustomerViewModel();
            custVM.customer = customer;
            custVM.billing_address = customer.Customers_Billing_Address_Mapping.FirstOrDefault(i => i.CustomerID == id);
            if (custVM.billing_address == null)
            {

            }
            else
            {
                custVM.list_billing_address_tel = db.Customers_Billing_Address_Tel_No_Mapping.Where(i => i.Customers_Billing_Address_Mapping_ID == custVM.billing_address.Customers_Billing_Address_Mapping_ID).ToList();
                //if (custVM.list_billing_address_tel.Count == 0) {
                //    Customers_Billing_Address_Tel_No_Mapping tel = new Customers_Billing_Address_Tel_No_Mapping();
                //    custVM.list_billing_address_tel.Add(tel);
                //}
                custVM.list_billing_address_cell = db.Customers_Billing_Address_Cell_No_Mapping.Where(i => i.Customers_Billing_Address_Mapping_ID == custVM.billing_address.Customers_Billing_Address_Mapping_ID).ToList();
                //if (custVM.list_billing_address_cell.Count == 0)
                //{
                //    Customers_Billing_Address_Cell_No_Mapping cell = new Customers_Billing_Address_Cell_No_Mapping();
                //    custVM.list_billing_address_cell.Add(cell);
                //}
                custVM.list_billing_address_email = db.Customers_Billing_Address_Email_Mapping.Where(i => i.Customers_Billing_Address_Mapping_ID == custVM.billing_address.Customers_Billing_Address_Mapping_ID).ToList();
                custVM.list_service_address = db.Customers_Service_Address_Mapping.Where(i => i.CustomerID == id).ToList();
                if (custVM.list_service_address.Count == 0)
                {
                    Customers_Service_Address_Mapping servAdd = new Customers_Service_Address_Mapping();
                    custVM.list_service_address.Add(servAdd);
                }
            }

            var service_address_ids = custVM.list_service_address.Select(i => i.Customers_Service_Address_Mapping_ID);

            return View(custVM);
        }

        // POST: Customers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(long id)
        {
            Customer customer = db.Customers.Find(id);
            Customers_Billing_Address_Mapping bill = db.Customers_Billing_Address_Mapping.Where(b => b.CustomerID == customer.CustomerID).First();
            var billTels = db.Customers_Billing_Address_Tel_No_Mapping.Where(c => c.Customers_Billing_Address_Mapping_ID == bill.Customers_Billing_Address_Mapping_ID);
            db.Customers_Billing_Address_Tel_No_Mapping.RemoveRange(billTels);
            db.Customers_Billing_Address_Cell_No_Mapping.RemoveRange(db.Customers_Billing_Address_Cell_No_Mapping.Where(c => c.Customers_Billing_Address_Mapping_ID == bill.Customers_Billing_Address_Mapping_ID));
            db.Customers_Billing_Address_Email_Mapping.RemoveRange(db.Customers_Billing_Address_Email_Mapping.Where(c => c.Customers_Billing_Address_Mapping_ID == bill.Customers_Billing_Address_Mapping_ID));
            db.Customers_Billing_Address_Mapping.Remove(bill);

            db.Customers_Service_Address_Mapping.RemoveRange(db.Customers_Service_Address_Mapping.Where(s => s.CustomerID == customer.CustomerID));
            db.Customers.Remove(customer);
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
        [HttpPost]
        public ActionResult GetServiceLocations(string LocationID)
        {
            int myLocationID;
            List<SelectListItem> locationList = new List<SelectListItem>();
            if (!string.IsNullOrEmpty(LocationID))
            {
                myLocationID = Convert.ToInt32(LocationID);
                List<Location> locations = db.Locations.Where(x => x.LocationID == myLocationID).ToList();
                locations.ForEach(x =>
                {
                    locationList.Add(new SelectListItem { Text = x.LocationName, Value = x.LocationID.ToString() });
                });
            }
            return Json(locationList, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult GetServiceMapped(string LeadID)
        {
            int intLeadID;

            if (!string.IsNullOrEmpty(LeadID))
            {
                intLeadID = Convert.ToInt32(LeadID);
                var services = db.Lead_Services_Mapping.Where(x => x.LeadID == intLeadID).ToList();

                if (services.Count == 0)
                {
                    return Json("No Services Found", JsonRequestBehavior.AllowGet);
                }
                List<Lead_Services_Mapping> servicesList = new List<Lead_Services_Mapping>();
                services.ForEach(x =>
                {
                    servicesList.Add(new Lead_Services_Mapping
                    {
                        Lead_Services_Mapping_ID = x.Lead_Services_Mapping_ID,
                        LeadID = x.LeadID,
                        ServiceID = x.ServiceID,
                        ServiceGroupID = x.ServiceGroupID,
                        FrequencyOfServiceID = x.FrequencyOfServiceID,
                        Rate = x.Rate,
                        GST = x.GST,
                        Qty = x.Qty,
                        Tax = x.Tax
                    });
                });
                return Json(servicesList, JsonRequestBehavior.AllowGet);
            }
            else
            {

                return Json("Wrong Lead ID", JsonRequestBehavior.AllowGet);
            }

        }
        
    }
}

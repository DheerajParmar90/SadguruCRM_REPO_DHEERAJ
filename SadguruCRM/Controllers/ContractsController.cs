using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Rotativa.Options;
using SadguruCRM.EnumClassesAndHelpers;
using SadguruCRM.Helpers;
using SadguruCRM.Models;
using SadguruCRM.ViewModels;

namespace SadguruCRM.Controllers
{
    [VerifyUser]
    public class ContractsController : Controller
    {
        private SadguruCRMEntities db = new SadguruCRMEntities();
        private static TimeZoneInfo INDIAN_ZONE = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");

        // GET: Contracts
        public ActionResult Index()
        {
            var contracts = db.Contracts.Include(c => c.Branch).Include(c => c.UserLogin).Include(c => c.Customer).Include(c => c.UserLogin1);
            
            foreach (var item in contracts)
            {
                if (item.Customer.CustomerName == null)
                {
                    item.Customer.CustomerName = item.Customer.Title + " " + item.Customer.FirstName + " " + item.Customer.LastName;
                }
                var custID = item.CustomerID;
                var locationID = db.Customers_Billing_Address_Mapping.Where(b => b.CustomerID == custID).FirstOrDefault().BillingLocationID;
                item.Remark = db.Locations.Find(locationID).LocationName;
                var listOfServiceIDs = item.Contract_Services_Mapping.Select(c => c.ServiceID).ToList();
                item.ExecutiveName = String.Join(", ", db.Services.Where(x => listOfServiceIDs.Contains(x.ServiceID)).Select(x => x.ServiceShortCode).ToArray());
                //item.ExecutiveName = 
            }
            return View(contracts.ToList());
        }

        // GET: Contracts/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Contract contract = db.Contracts.Find(id);
            if (contract == null)
            {
                return HttpNotFound();
            }
            ViewBag.LocationID = new SelectList(db.Locations.Select(x => new { x.LocationID, x.LocationName }), "LocationID", "LocationName");
            ViewBag.CityID = new SelectList(db.Cities.Select(x => new { x.CityID, x.CityName }), "CityID", "CityName");
            ViewBag.StateID = new SelectList(db.States.Select(x => new { x.StateID, x.State1 }), "StateID", "State1");
            ViewBag.ServiceGroupID = new SelectList(db.ServiceGroups.Select(x => new { x.ServiceGroupID, x.ServiceGroup1 }), "ServiceGroupID", "ServiceGroup1");
            ViewBag.ServiceID = new SelectList(db.Services.Select(x => new { x.ServiceID, x.ServiceName }), "ServiceID", "ServiceName");
            ViewBag.FrequencyOfServiceID = new SelectList(db.FrequencyOfServices.Select(x => new { x.FrequencyOfServiceID, x.FrequencyOfService1 }), "FrequencyOfServiceID", "FrequencyOfService1");
            ViewBag.BranchID = new SelectList(db.Branches.Select(x => new { x.BranchID, x.BranchName }), "BranchID", "BranchName", contract.BranchID);
            ViewBag.PeriodsOfContract = new SelectList(db.PeriodsOfContracts.Select(x => new { x.PeriodsOfContractID, x.PeriodsOfContract1 }), "PeriodsOfContractID", "PeriodsOfContract1");
            ViewBag.PaymentModes = new SelectList(db.PaymentModes.Select(x => new { x.PaymentModeID, x.PaymentModeName }), "PaymentModeID", "PaymentModeName", contract.PaymentModeID);
            ViewBag.PaymentTerms = new SelectList(db.PaymentTerms.Select(x => new { x.PaymentTermID, x.PaymentTermName }), "PaymentTermID", "PaymentTermName", contract.PaymentTermID);
            //var customers = db.Customers.Select( x=> new { x.CustomerID, x.CustomerName, x.FirstName, x.LastName, x.Title }).ToList();
            var customers = db.Customers.ToList();
            foreach (var item in customers)
            {
                if (item.CustomerName == null)
                {
                    item.CustomerName = item.CustomerID + " " + item.Title + " " + item.FirstName + " " + item.LastName;
                }
            }

            ViewBag.CustomerID = new SelectList(customers.Where(c => c.CustomerID == contract.CustomerID).Select(x => new { x.CustomerName, x.CustomerID }), "CustomerID", "CustomerName", contract.CustomerID);
            var enumData = from EnumClassesAndHelpers.GSTTypes g in Enum.GetValues(typeof(EnumClassesAndHelpers.GSTTypes))
                           select new
                           {
                               ID = (byte)g,
                               Name = EnumHelper.GetEnumDescription(g)
                           };

            ViewBag.GSTTypes = new SelectList(enumData.Select(x => new { x.ID, x.Name }), "ID", "Name");
            ViewBag.FrequencyInvoice = new SelectList(db.FrequencyOfInvoices.Select(x => new { x.FrequencyOfInvoiceID, x.FrequencyOfInvoice1 }), "FrequencyOfInvoiceID", "FrequencyOfInvoice1");
            ViewBag.TypeOfPremises = new SelectList(db.PremisesTypes.Select(x => new { x.PremisesTypeID, x.PremisesType1 }), "PremisesTypeID", "PremisesType1", contract.PremisesTypeID);
            ViewBag.PremisesArea = new SelectList(db.Premises_Area_Master.Select(x => new { x.Premises_Area_ID, x.Premises_Area }), "Premises_Area_ID", "Premises_Area", contract.Premises_Area_ID);
            //ViewBag.BookedBy = new SelectList(db.UserLogins, "UserID", "UserName");
            ViewBag.Executive = new SelectList(db.Employees.Select(x => new { x.EmployeeID, x.Name }), "EmployeeID", "Name");
            ViewBag.BookedBy = new SelectList(db.UserLogins.Select(x => new { x.UserID, x.UserName }), "UserID", "UserName", contract.BookByID);
            //int? LoggedInEmployeeID = db.UserLogins.Find(Convert.ToInt32(Session["UserID"])).EmployeeID;
            //if (LoggedInEmployeeID == null)
            //{
            ViewBag.BookByContactNo = contract.BookByContactNo;
            //}
            //else
            //{
            //  ViewBag.BookByContactNo = db.Employees.Find(LoggedInEmployeeID).PrimaryCellNumber;
            //}

            ViewBag.ShortServiceScope = new SelectList(db.Short_Service_Scope_Master.Select(x => new { x.Short_Service_Scope_ID, x.Short_Service_Scope }), "Short_Service_Scope_ID", "Short_Service_Scope");
            var leads = db.Leads.Where(l => l.LeadID == contract.LeadID).Where(l => db.Customers.Select(c => c.LeadID).Contains(l.LeadID)).Include(l => l.Branch).Include(l => l.City).Include(l => l.UserLogin).Include(l => l.LeadStatus).Include(l => l.Source).Include(l => l.Source1).Include(l => l.PremisesType);            //new Code
            //var leadsList = await new HelperNonStatic().GenerateCustomerNameAndServicesShortCodes(leads);
            var leadsOrderedNewCode = leads.Select(s => new
            {
                s.LeadID,
                Description = s.LeadID + " - " + s.Title + " " + s.FirstName + " " + s.LastName + " " + s.CustomerName

            }).OrderByDescending(i => i.LeadID).ToList();
            ViewBag.LeadID = new SelectList(leadsOrderedNewCode, "LeadID", "Description", contract.LeadID);

            var existingContracts = db.Contracts.Where(c => c.ContractID == contract.ExistingContractsID).Include(c => c.Customer).Select(c => new
            {
                c.ContractID,
                c.ContractNo,
                Description = c.ContractNo + " - " + c.Customer.Title + " " + c.Customer.FirstName + " " + c.Customer.LastName + " " + c.Customer.CustomerName

            }).OrderByDescending(c => c.ContractID).ToList();
            ViewBag.ExistingContractID = new SelectList(existingContracts, "ContractID", "Description", contract.ExistingContractsID);
            return View(contract);
        }
        public ActionResult Print(int id)
        {
            try
            {
                var cookies = Request.Cookies.AllKeys.ToDictionary(k => k, k => Request.Cookies[k].Value);


                //string customSwitches = string.Format("--print-media-type --allow {0}{1} --header-html {0} --footer-html {1}",

                //          Url.Action("Header", "DocumentsTemplates", new { area = "" }, "http"), Url.Action("Footer", "DocumentsTemplates", new { area = "" }, "http")
                //          //Server.MapPath("../Content/Images/pngwing.com.png")
                //          );

                return new Rotativa.UrlAsPdf("/Contracts/PDFView/" + id.ToString())
                {
                    FormsAuthenticationCookieName = System.Web.Security.FormsAuthentication.FormsCookieName,
                    Cookies = cookies,
                    //CustomSwitches = customSwitches,
                    PageSize = Rotativa.Options.Size.A4,
                    PageMargins = new Margins(3, 3, 3, 3)
                };
                //var report = new Rotativa.ActionAsPdf("Details", new { id = id });
                //return report;
            }
            catch (Exception ex)
            {
                string message = ex.Message;
                return null;
            }

        }
        [AllowAnonymous]
        public ActionResult PDFView(int? id)
        {

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Contract contract = db.Contracts.Find(id);
            if (contract == null)
            {
                return HttpNotFound();
            }
            
            return View(contract);
        }
        // GET: Contracts/Create
        public ActionResult Create()
        {
            ViewBag.LocationID = new SelectList(db.Locations.Select(x => new { x.LocationID, x.LocationName }), "LocationID", "LocationName");
            ViewBag.CityID = new SelectList(db.Cities.Select(x => new { x.CityID, x.CityName }), "CityID", "CityName");
            ViewBag.StateID = new SelectList(db.States.Select(x => new { x.StateID, x.State1 }), "StateID", "State1");
            ViewBag.ServiceGroupID = new SelectList(db.ServiceGroups.Select(x => new { x.ServiceGroupID, x.ServiceGroup1 }), "ServiceGroupID", "ServiceGroup1");
            ViewBag.ServiceID = new SelectList(db.Services.Select(x => new { x.ServiceID, x.ServiceName }), "ServiceID", "ServiceName");
            ViewBag.FrequencyOfServiceID = new SelectList(db.FrequencyOfServices.Select(x => new { x.FrequencyOfServiceID, x.FrequencyOfService1 }), "FrequencyOfServiceID", "FrequencyOfService1");
            ViewBag.BranchID = new SelectList(db.Branches.Select( x=> new { x.BranchID, x.BranchName}), "BranchID", "BranchName");
            ViewBag.PeriodsOfContract = new SelectList(db.PeriodsOfContracts.Select( x=> new { x.PeriodsOfContractID, x.PeriodsOfContract1 }), "PeriodsOfContractID", "PeriodsOfContract1");
            ViewBag.PaymentModes = new SelectList(db.PaymentModes.Select(x => new { x.PaymentModeID, x.PaymentModeName }), "PaymentModeID", "PaymentModeName");
            ViewBag.PaymentTerms = new SelectList(db.PaymentTerms.Select(x => new { x.PaymentTermID, x.PaymentTermName }), "PaymentTermID", "PaymentTermName");
            //var customers = db.Customers.Select( x=> new { x.CustomerID, x.CustomerName, x.FirstName, x.LastName, x.Title }).ToList();
            var customers = db.Customers.OrderByDescending(c => c.CustomerID).ToList();
            foreach (var item in customers)
            {
                //if (item.CustomerName == null)
                //{
                    item.CustomerName = item.CustomerNo + " " + item.Title + " " + item.FirstName + " " + item.LastName + " " + item.CustomerName;
                //}
            }

            ViewBag.CustomerID = new SelectList(customers.Select( x=> new { x.CustomerName, x.CustomerID}), "CustomerID", "CustomerName");
            var enumData = from EnumClassesAndHelpers.GSTTypes g in Enum.GetValues(typeof(EnumClassesAndHelpers.GSTTypes))
                           select new
                           {
                               ID = (byte)g,
                               Name = EnumHelper.GetEnumDescription(g)
                           };

            ViewBag.GSTTypes = new SelectList(enumData.Select(x => new { x.ID, x.Name }), "ID", "Name");
            ViewBag.FrequencyInvoice = new SelectList(db.FrequencyOfInvoices.Select(x => new { x.FrequencyOfInvoiceID, x.FrequencyOfInvoice1 }), "FrequencyOfInvoiceID", "FrequencyOfInvoice1");
            ViewBag.TypeOfPremises = new SelectList(db.PremisesTypes.Select(x => new { x.PremisesTypeID, x.PremisesType1 }), "PremisesTypeID", "PremisesType1");
            ViewBag.PremisesArea = new SelectList(db.Premises_Area_Master.Select(x => new { x.Premises_Area_ID, x.Premises_Area }), "Premises_Area_ID", "Premises_Area");
            //ViewBag.BookedBy = new SelectList(db.UserLogins, "UserID", "UserName");
            ViewBag.Executive = new SelectList(db.Employees.Select(x => new { x.EmployeeID, x.Name }), "EmployeeID", "Name");
            ViewBag.BookedBy = new SelectList(db.UserLogins.Select(x => new { x.UserID, x.UserName }), "UserID", "UserName", Convert.ToInt32(Session["UserID"]));
            int? LoggedInEmployeeID = db.UserLogins.Find(Convert.ToInt32(Session["UserID"])).EmployeeID;
            if (LoggedInEmployeeID == null)
            {
                ViewBag.BookByContactNo = "";
            }
            else {
                ViewBag.BookByContactNo = db.Employees.Find(LoggedInEmployeeID).PrimaryCellNumber;
            }
            
            ViewBag.ShortServiceScope = new SelectList(db.Short_Service_Scope_Master.Select(x => new { x.Short_Service_Scope_ID, x.Short_Service_Scope }), "Short_Service_Scope_ID", "Short_Service_Scope");
            ////Select leads only for which customer is created
            var leads = db.Leads.Where(l => db.Customers.Select(c => c.LeadID).Contains(l.LeadID)).Include(l => l.Branch).Include(l => l.City).Include(l => l.UserLogin).Include(l => l.LeadStatus).Include(l => l.Source).Include(l => l.Source1).Include(l => l.PremisesType);            //new Code
            //var leadsList = await new HelperNonStatic().GenerateCustomerNameAndServicesShortCodes(leads);
            var leadsOrderedNewCode = leads.Select(s => new
            {
                s.LeadID,
                Description = s.LeadID + " - " + s.Title + " " + s.FirstName + " " + s.LastName + " " + s.CustomerName

            }).OrderByDescending(i => i.LeadID).ToList();
            ViewBag.LeadID = new SelectList(leadsOrderedNewCode, "LeadID", "Description");

            var existingContracts = db.Contracts.Include(c => c.Customer).Select(c => new
            {
                c.ContractID,
                c.ContractNo,
                Description = c.ContractNo + " - " + c.Customer.Title + " " + c.Customer.FirstName + " " + c.Customer.LastName + " " + c.Customer.CustomerName

            }).OrderByDescending(c => c.ContractID).ToList(); 
            ViewBag.ExistingContractID = new SelectList(existingContracts, "ContractID", "Description");
            return View();
        }

        // POST: Contracts/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        //        public ActionResult Create([Bind(Include = "ContractID,ContractNo,ContractDate,Contract_From_Date,Contract_To_Date,TypeOfContract,PurchaseOrderNo,PurchaseOtrderDate,CustomerID,SiteAddressSameAsBillingAddress,Remark,BalanceAmount,BranchID,CreatedDate,CreatedBy,FinalContractRate,PaidByCustomer,ExecutiveID,ExecutveContactNo,PaymentModeID,PaymentTermID,BookByID,BookByContactNo,SendWelcomeMessage,PremisesTypeID,Premises_Area_ID,AppSqFtArea")] Contract contract, string[] Customers_Service_Address_Mapping_ID, String[] ServiceGroupID, String[] ServiceID,int[] FrequencyOfInvoiceID,int[] ShortServiceScope, String[] FrequencyOfServiceID,int[] PeriodsOfContractID, DateTime?[] ServiceStartDate, DateTime?[] PeriodOfContractStartDate, DateTime?[] PeriodOfContractEndDate, String[] Qty, String[] GST, /*String[] Tax,*/ String[] CGST, String[] SGST, String[] IGST, String[] Rate, String[] FinalRatePerService)
        public ActionResult Create(Contract contract, string[] Customers_Service_Address_Mapping_ID, String[] ServiceGroupID, String[] ServiceID, int[] FrequencyOfInvoiceID, int[] ShortServiceScope, String[] FrequencyOfServiceID, int[] PeriodsOfContractID, DateTime?[] ServiceStartDate, DateTime?[] PeriodOfContractStartDate, DateTime?[] PeriodOfContractEndDate, String[] Qty, String[] GST, /*String[] Tax,*/ String[] CGST, String[] SGST, String[] IGST, String[] Rate, String[] FinalRatePerService)
        {
            contract.CreatedBy = Convert.ToInt32(Session["UserID"]);
            contract.CreatedDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, INDIAN_ZONE);
            if (ModelState.IsValid)
            {
                
                //contract.ContractDate = DateTime.ParseExact(ContractDate, "yyyyMMdd", CultureInfo.InvariantCulture);
                //contract.Contract_From_Date = DateTime.ParseExact(Contract_From_Date, "yyyyMMdd", CultureInfo.InvariantCulture);
                //contract.Contract_To_Date = DateTime.ParseExact(Contract_To_Date, "yyyyMMdd", CultureInfo.InvariantCulture);
                db.Contracts.Add(contract);
                db.SaveChanges();
                try
                {
                    for (int i = 0; i < Customers_Service_Address_Mapping_ID.Length; i++)
                    {
                        Contracts_Service_Address_Mapping servAdd = new Contracts_Service_Address_Mapping();
                        servAdd.Customers_Service_Address_Mapping_ID = Int32.Parse(Customers_Service_Address_Mapping_ID[i]);
                        servAdd.CustomerID = contract.CustomerID;
                        servAdd.ContractID = contract.ContractID;
                        servAdd.CreatedBy = Convert.ToInt32(Session["UserID"]);
                        servAdd.CreatedOn = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, INDIAN_ZONE);
                        db.Contracts_Service_Address_Mapping.Add(servAdd);
                        db.SaveChanges();
                    }
                }
                catch (Exception e) { 
                    string ex = e.Message;
                }
                try
                {
                    for (int i = 0; i < ServiceGroupID.Length; i++)
                    {
                        int outServiceGroupID;
                        if (Int32.TryParse(ServiceGroupID[i], out outServiceGroupID)) {
                            Contract_Services_Mapping servicesMapping = new Contract_Services_Mapping();
                            servicesMapping.ContractID = contract.ContractID;
                            servicesMapping.ServiceGroupID = outServiceGroupID;
                            int outServiceID;
                            if (Int32.TryParse(ServiceID[i], out outServiceID))
                                servicesMapping.ServiceID = outServiceID;
                            else
                                servicesMapping.ServiceID = null;
                            
                            if (FrequencyOfInvoiceID[i] != 0)
                                servicesMapping.FrequencyOfInvoiceID = FrequencyOfInvoiceID[i];
                            else
                                servicesMapping.FrequencyOfInvoiceID = null;
                            if (ShortServiceScope[i] != 0)
                                servicesMapping.Short_Service_Scope_ID = ShortServiceScope[i];
                            else
                                servicesMapping.Short_Service_Scope_ID = null;
                            if (PeriodsOfContractID[i] != 0)
                                servicesMapping.PeriodsOfContractID = PeriodsOfContractID[i];
                            else
                                servicesMapping.PeriodsOfContractID = null;


                            try
                            {
                                servicesMapping.ServiceStartDate = ServiceStartDate[i];
                            }
                            catch (Exception e)
                            {

                            }
                            try
                            {
                                servicesMapping.POCStartDate = PeriodOfContractStartDate[i];
                            }
                            catch (Exception e)
                            {

                            }
                            try
                            {
                                servicesMapping.POCEndDate = PeriodOfContractEndDate[i];
                            }
                            catch (Exception e)
                            {

                            }


                            int outFrequencyOfServiceID;
                            if (Int32.TryParse(FrequencyOfServiceID[i], out outFrequencyOfServiceID))
                                servicesMapping.FrequencyOfServiceID = outFrequencyOfServiceID;
                            else
                                servicesMapping.FrequencyOfServiceID = null;

                            short outQty;
                            if (short.TryParse(Qty[i], out outQty))
                                servicesMapping.Qty = outQty;
                            else
                                servicesMapping.Qty = null;
                            
                            decimal outRate;
                            if (Decimal.TryParse(Rate[i], out outRate))
                                servicesMapping.Rate = Decimal.Parse(Rate[i]);

                            else
                                servicesMapping.Rate = null;

                            bool outGST;
                            if (Boolean.TryParse(GST[i], out outGST))
                                servicesMapping.GST = outGST;
                            else
                                servicesMapping.GST = null;

                            decimal outCGST, outSGST, outIGST, outFinalRatePerService;
                            if (Decimal.TryParse(CGST[i], out outCGST))
                                servicesMapping.CGST = outCGST;
                            else
                                servicesMapping.CGST = null;

                            if (Decimal.TryParse(SGST[i], out outSGST))
                                servicesMapping.SGST = outSGST;
                            else
                                servicesMapping.SGST = null;

                            if (Decimal.TryParse(IGST[i], out outIGST))
                                servicesMapping.IGST = outIGST;
                            else
                                servicesMapping.IGST = null;

                            if (Decimal.TryParse(FinalRatePerService[i], out outFinalRatePerService))
                                servicesMapping.FinalRatePerService = outFinalRatePerService;
                            else
                                servicesMapping.FinalRatePerService = null;

                            //servicesMapping.FinalRatePerService = FinalRatePerService[i];
                            db.Contract_Services_Mapping.Add(servicesMapping);
                            db.SaveChanges();
                            try
                            {
                                CreateServicings(servicesMapping);
                            }
                            catch (Exception e)
                            {
                                string ex = e.Message;
                            }
                            
                        }
                        
                    }
                }
                catch (Exception e) {

                    string ex = e.Message;
                }
                
                //CreateServicings(servicesMapping);
                return RedirectToAction("Index");
            }


            else
            {
                ///var errors = ModelState.Where(x => x.Value.Errors.Count > 0).Select(x => new { x.Key, x.Value.Errors }).ToArray();
                
                IEnumerable<ModelError> allErrors = ModelState.Values.SelectMany(v => v.Errors);
                string msg = "";
                foreach (var item in allErrors)
                {
                    msg += item.ErrorMessage + "  ";
                }
                throw new Exception(msg);
            }
            ViewBag.LocationID = new SelectList(db.Locations.Select(x => new { x.LocationID, x.LocationName}), "LocationID", "LocationName");
            ViewBag.CityID = new SelectList(db.Cities.Select(x => new { x.CityID, x.CityName}), "CityID", "CityName");
            ViewBag.StateID = new SelectList(db.States.Select(x => new { x.StateID, x.State1}), "StateID", "State1", contract.BranchID);
            ViewBag.ServiceGroupID = new SelectList(db.ServiceGroups.Select(x => new { x.ServiceGroupID, x.ServiceGroup1}), "ServiceGroupID", "ServiceGroup1");
            ViewBag.ServiceID = new SelectList(db.Services.Where(x => x.ServiceID == 0).Select(x => new { x.ServiceID, x.ServiceName}), "ServiceID", "ServiceName");
            ViewBag.FrequencyOfServiceID = new SelectList(db.FrequencyOfServices.Select(x => new { x.FrequencyOfServiceID, x.FrequencyOfService1 }), "FrequencyOfServiceID", "FrequencyOfService1");
            ViewBag.BranchID = new SelectList(db.Branches.Select(x => new { x.BranchID, x.BranchName }), "BranchID", "BranchName", contract.BranchID);
            var customers = db.Customers.ToList();
            foreach (var item in customers)
            {
                if (item.CustomerName == null)
                {
                    item.CustomerName = item.Title + " " + item.FirstName + " " + item.LastName;
                }
            }
            ViewBag.CustomerID = new SelectList(customers.Select(x => new { x.CustomerName, x.CustomerID }), "CustomerID", "CustomerName");
            var enumData = from EnumClassesAndHelpers.GSTTypes g in Enum.GetValues(typeof(EnumClassesAndHelpers.GSTTypes))
                           select new
                           {
                               ID = (byte)g,
                               Name = EnumHelper.GetEnumDescription(g)
                           };

            ViewBag.GSTTypes = new SelectList(enumData.Select(x => new { x.ID, x.Name }), "ID", "Name");
            ViewBag.PeriodsOfContract = new SelectList(db.PeriodsOfContracts.Select(x => new { x.PeriodsOfContractID, x.PeriodsOfContract1 }), "PeriodsOfContractID", "PeriodsOfContract1", contract.PeriodsOfContractID);
            ViewBag.PaymentModes = new SelectList(db.PaymentModes.Select(x => new { x.PaymentModeID, x.PaymentModeName }), "PaymentModeID", "PaymentModeName", contract.PaymentModeID);
            ViewBag.PaymentTerms = new SelectList(db.PaymentTerms.Select(x => new { x.PaymentTermID, x.PaymentTermName }), "PaymentTermID", "PaymentTermName", contract.PaymentTermID);
            ViewBag.FrequencyInvoice = new SelectList(db.FrequencyOfInvoices.Select(x => new { x.FrequencyOfInvoiceID, x.FrequencyOfInvoice1 }), "FrequencyOfInvoiceID", "FrequencyOfInvoice1");
            ViewBag.TypeOfPremises = new SelectList(db.PremisesTypes.Select(x => new { x.PremisesTypeID, x.PremisesType1 }), "PremisesTypeID", "PremisesType1");
            ViewBag.PremisesArea = new SelectList(db.Premises_Area_Master.Select(x => new { x.Premises_Area_ID, x.Premises_Area }), "Premises_Area_ID", "Premises_Area");
            //ViewBag.BookedBy = new SelectList(db.UserLogins, "UserID", "UserName");
            ViewBag.Executive = new SelectList(db.Employees.Select(x => new { x.EmployeeID, x.Name }), "EmployeeID", "Name");
            ViewBag.BookedBy = new SelectList(db.UserLogins.Select(x => new { x.UserID, x.UserName }), "UserID", "UserName");
            ViewBag.ShortServiceScope = new SelectList(db.Short_Service_Scope_Master.Select(x => new { x.Short_Service_Scope_ID, x.Short_Service_Scope }), "Short_Service_Scope_ID", "Short_Service_Scope");
            var leads = db.Leads.Where(l => db.Customers.Select(c => c.LeadID).Contains(l.LeadID)).Include(l => l.Branch).Include(l => l.City).Include(l => l.UserLogin).Include(l => l.LeadStatus).Include(l => l.Source).Include(l => l.Source1).Include(l => l.PremisesType);            //new Code
            //var leadsList = await new HelperNonStatic().GenerateCustomerNameAndServicesShortCodes(leads);
            var leadsOrderedNewCode = leads.Select(s => new
            {
                s.LeadID,
                Description = s.LeadID + " - " + s.Title + " " + s.FirstName + " " + s.LastName + " " + s.CustomerName

            }).OrderByDescending(i => i.LeadID).ToList();
            ViewBag.LeadID = new SelectList(leadsOrderedNewCode, "LeadID", "Description");

            var existingContracts = db.Contracts.Include(c => c.Customer).Select(c => new
            {
                c.ContractID,
                c.ContractNo,
                Description = c.ContractNo + " - " + c.Customer.Title + " " + c.Customer.FirstName + " " + c.Customer.LastName + " " + c.Customer.CustomerName

            }).OrderByDescending(c => c.ContractID).ToList();
            ViewBag.ExistingContractID = new SelectList(existingContracts, "ContractID", "Description");

            return View(contract);
        }

        // GET: Contracts/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Contract contract = db.Contracts.Find(id);
            if (contract == null)
            {
                return HttpNotFound();
            }
            ViewBag.LocationID = new SelectList(db.Locations.Select(x => new { x.LocationID, x.LocationName }), "LocationID", "LocationName");
            ViewBag.CityID = new SelectList(db.Cities.Select(x => new { x.CityID, x.CityName }), "CityID", "CityName");
            ViewBag.StateID = new SelectList(db.States.Select(x => new { x.StateID, x.State1 }), "StateID", "State1");
            ViewBag.ServiceGroupID = new SelectList(db.ServiceGroups.Select(x => new { x.ServiceGroupID, x.ServiceGroup1 }), "ServiceGroupID", "ServiceGroup1");
            ViewBag.ServiceID = new SelectList(db.Services.Select(x => new { x.ServiceID, x.ServiceName }), "ServiceID", "ServiceName");
            ViewBag.FrequencyOfServiceID = new SelectList(db.FrequencyOfServices.Select(x => new { x.FrequencyOfServiceID, x.FrequencyOfService1 }), "FrequencyOfServiceID", "FrequencyOfService1");
            ViewBag.BranchID = new SelectList(db.Branches.Select(x => new { x.BranchID, x.BranchName }), "BranchID", "BranchName", contract.BranchID);
            ViewBag.PeriodsOfContract = new SelectList(db.PeriodsOfContracts.Select(x => new { x.PeriodsOfContractID, x.PeriodsOfContract1 }), "PeriodsOfContractID", "PeriodsOfContract1");
            ViewBag.PaymentModes = new SelectList(db.PaymentModes.Select(x => new { x.PaymentModeID, x.PaymentModeName }), "PaymentModeID", "PaymentModeName", contract.PaymentModeID);
            ViewBag.PaymentTerms = new SelectList(db.PaymentTerms.Select(x => new { x.PaymentTermID, x.PaymentTermName }), "PaymentTermID", "PaymentTermName", contract.PaymentTermID);
            //var customers = db.Customers.Select( x=> new { x.CustomerID, x.CustomerName, x.FirstName, x.LastName, x.Title }).ToList();
            var customers = db.Customers.ToList();
            foreach (var item in customers)
            {
                if (item.CustomerName == null)
                {
                    item.CustomerName = item.CustomerID + " " + item.Title + " " + item.FirstName + " " + item.LastName;
                }
            }

            ViewBag.CustomerID = new SelectList(customers.Where(c => c.CustomerID == contract.CustomerID).Select(x => new { x.CustomerName, x.CustomerID }), "CustomerID", "CustomerName", contract.CustomerID);
            var enumData = from EnumClassesAndHelpers.GSTTypes g in Enum.GetValues(typeof(EnumClassesAndHelpers.GSTTypes))
                           select new
                           {
                               ID = (byte)g,
                               Name = EnumHelper.GetEnumDescription(g)
                           };

            ViewBag.GSTTypes = new SelectList(enumData.Select(x => new { x.ID, x.Name }), "ID", "Name");
            ViewBag.FrequencyInvoice = new SelectList(db.FrequencyOfInvoices.Select(x => new { x.FrequencyOfInvoiceID, x.FrequencyOfInvoice1 }), "FrequencyOfInvoiceID", "FrequencyOfInvoice1");
            ViewBag.TypeOfPremises = new SelectList(db.PremisesTypes.Select(x => new { x.PremisesTypeID, x.PremisesType1 }), "PremisesTypeID", "PremisesType1", contract.PremisesTypeID);
            ViewBag.PremisesArea = new SelectList(db.Premises_Area_Master.Select(x => new { x.Premises_Area_ID, x.Premises_Area }), "Premises_Area_ID", "Premises_Area", contract.Premises_Area_ID);
            //ViewBag.BookedBy = new SelectList(db.UserLogins, "UserID", "UserName");
            ViewBag.Executive = new SelectList(db.Employees.Select(x => new { x.EmployeeID, x.Name }), "EmployeeID", "Name");
            ViewBag.BookedBy = new SelectList(db.UserLogins.Select(x => new { x.UserID, x.UserName }), "UserID", "UserName", contract.BookByID);
            //int? LoggedInEmployeeID = db.UserLogins.Find(Convert.ToInt32(Session["UserID"])).EmployeeID;
            //if (LoggedInEmployeeID == null)
            //{
            ViewBag.BookByContactNo = contract.BookByContactNo;
            //}
            //else
            //{
            //  ViewBag.BookByContactNo = db.Employees.Find(LoggedInEmployeeID).PrimaryCellNumber;
            //}

            ViewBag.ShortServiceScope = new SelectList(db.Short_Service_Scope_Master.Select(x => new { x.Short_Service_Scope_ID, x.Short_Service_Scope }), "Short_Service_Scope_ID", "Short_Service_Scope");
            var leads = db.Leads.Where(l => l.LeadID == contract.LeadID).Where(l => db.Customers.Select(c => c.LeadID).Contains(l.LeadID)).Include(l => l.Branch).Include(l => l.City).Include(l => l.UserLogin).Include(l => l.LeadStatus).Include(l => l.Source).Include(l => l.Source1).Include(l => l.PremisesType);            //new Code
            //var leadsList = await new HelperNonStatic().GenerateCustomerNameAndServicesShortCodes(leads);
            var leadsOrderedNewCode = leads.Select(s => new
            {
                s.LeadID,
                Description = s.LeadID + " - " + s.Title + " " + s.FirstName + " " + s.LastName + " " + s.CustomerName

            }).OrderByDescending(i => i.LeadID).ToList();
            ViewBag.LeadID = new SelectList(leadsOrderedNewCode, "LeadID", "Description", contract.LeadID);

            var existingContracts = db.Contracts.Where(c => c.ContractID == contract.ExistingContractsID).Include(c => c.Customer).Select(c => new
            {
                c.ContractID,
                c.ContractNo,
                Description = c.ContractNo + " - " + c.Customer.Title + " " + c.Customer.FirstName + " " + c.Customer.LastName + " " + c.Customer.CustomerName

            }).OrderByDescending(c => c.ContractID).ToList();
            ViewBag.ExistingContractID = new SelectList(existingContracts, "ContractID", "Description", contract.ExistingContractsID);
            return View(contract);
        }

        // POST: Contracts/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Contract contract)
        {

            contract.LastUpdatedBy = Convert.ToInt32(Session["UserID"]);
            contract.LastUpdatedDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, INDIAN_ZONE);
            if (ModelState.IsValid)
            {
                db.Entry(contract).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            else
            {
                ///var errors = ModelState.Where(x => x.Value.Errors.Count > 0).Select(x => new { x.Key, x.Value.Errors }).ToArray();

                IEnumerable<ModelError> allErrors = ModelState.Values.SelectMany(v => v.Errors);
                string msg = "";
                foreach (var item in allErrors)
                {
                    msg += item.ErrorMessage + "  ";
                }
                throw new Exception(msg);
            }
            ViewBag.BranchID = new SelectList(db.Branches, "BranchID", "BranchName", contract.BranchID);
            ViewBag.CreatedBy = new SelectList(db.UserLogins, "UserID", "UserName", contract.CreatedBy);
            ViewBag.CustomerID = new SelectList(db.Customers, "CustomerID", "CustomerName", contract.CustomerID);
            ViewBag.LastUpdatedBy = new SelectList(db.UserLogins, "UserID", "UserName", contract.LastUpdatedBy);
            return View(contract);
        }
        [HttpPost]
        public ActionResult GetCustomerDataForContract(string custID)
        {
            int intCustID;

            if (!string.IsNullOrEmpty(custID))
            {
                intCustID = Convert.ToInt32(custID);
                string billmsg = "", servmsg ="", PremisesTypeID="";

                var cust = db.Customers.Where(x => x.CustomerID == intCustID).Select(x => new { x.CustomerNo, x.ConsultPerson, x.ConsultPersonDesignation, x.TelNumber, x.CellNumber, x.EmailId, x.GstinNo, x.BranchID, x.GST_Type_Enum, x.CustomerType, x.LeadID }).FirstOrDefault();

                var billingAddress = db.Customers_Billing_Address_Mapping.Where(x => x.CustomerID == intCustID).FirstOrDefault();
                long billingAddressID = billingAddress.Customers_Billing_Address_Mapping_ID;
                //var billingTelNos = db.Customers_Billing_Address_Tel_No_Mapping.Where(x => x.Customers_Billing_Address_Mapping_ID == billingAddressID).ToList();
                //var billingCellNos = db.Customers_Billing_Address_Cell_No_Mapping.Where(x => x.Customers_Billing_Address_Mapping_ID == billingAddressID).ToList();
                //var billingEmail = db.Customers_Billing_Address_Email_Mapping.Where(x => x.Customers_Billing_Address_Mapping_ID == billingAddressID).ToList();

                var serviceAddresses = db.Customers_Service_Address_Mapping.Where(x => x.CustomerID == intCustID).ToList();

                if (billingAddress == null)
                {
                    billmsg = "No Business Address Found";
                    //return Json("No Business Address Found", JsonRequestBehavior.AllowGet);
                }
                if (serviceAddresses.Count == 0) {
                    servmsg = "No Service Address Found";
                    //return Json("No Service Address Found", JsonRequestBehavior.AllowGet);
                }
                Customers_Billing_Address_Mapping bill = new Customers_Billing_Address_Mapping();
                bill.Customers_Billing_Address_Mapping_ID = billingAddress.Customers_Billing_Address_Mapping_ID;
                bill.AddressLine1 = billingAddress.AddressLine1;
                bill.AddressLine2 = billingAddress.AddressLine2;
                bill.AddressLine3 = billingAddress.AddressLine3;
                //bill.Bra = billingAddress.BillingLocationID;
                bill.BillingLocationID = billingAddress.BillingLocationID;
                bill.BillingCityID = billingAddress.BillingCityID;
                bill.BillingPincode = billingAddress.BillingPincode;
                bill.BillingStateID = billingAddress.BillingStateID;
                bill.GSTNo = cust.GstinNo;
                var premiseType = db.PremisesTypes.Where(p => p.PremisesType1 == cust.CustomerType).FirstOrDefault();
                if (premiseType != null) {
                    PremisesTypeID = premiseType.PremisesTypeID.ToString();
                }
                var LeadID = cust.LeadID;
                int? Premises_Area_ID = 0;
                if(LeadID != null)
                {
                    Premises_Area_ID = db.Leads.Find(LeadID).Premises_Area_ID;
                }
                //var Premises_Area_ID = db.Leads.Where()

                List<Customers_Service_Address_Mapping> servicesAddressList = new List<Customers_Service_Address_Mapping>();
                serviceAddresses.ForEach(x =>
                {
                    servicesAddressList.Add(new Customers_Service_Address_Mapping
                    {
                        Customers_Service_Address_Mapping_ID = x.Customers_Service_Address_Mapping_ID,
                        CustomerID = x.CustomerID,
                        AddressLine1 = x.AddressLine1,
                        AddressLine2 = x.AddressLine2,
                        AddressLine3 = x.AddressLine3,
                        ServiceLocationID = x.ServiceLocationID,
                        ServiceCityID = x.ServiceCityID,
                        ServicePincode = x.ServicePincode,
                        ServiceStateID = x.ServiceStateID,
                        ServiceAddressConsultPerson = x.ServiceAddressConsultPerson,
                        Customer_Service_Address_Tel_No_1 = x.Customer_Service_Address_Tel_No_1,
                        Customer_Service_Address_Cell_No_1 = x.Customer_Service_Address_Cell_No_1,
                        Customer_Service_Address_Email_1 = x.Customer_Service_Address_Email_1,
                    });
                });
                return Json(new { billmsg, servmsg, bill, servicesAddressList, cust, PremisesTypeID, Premises_Area_ID }, JsonRequestBehavior.AllowGet);
            }
            else
            {

                return Json("Wrong Customer ID", JsonRequestBehavior.AllowGet);
            }

        }


        public ActionResult GetContractServicesMapped(string ContractID)
        {
            int intContractID;

            if (!string.IsNullOrEmpty(ContractID))
            {
                intContractID = Convert.ToInt32(ContractID);
                string billmsg = "", servmsg = "";

                Contract contract = db.Contracts.Find(intContractID);
                Customer customer = db.Customers.Find(contract.CustomerID);

                var contractServices = contract.Contract_Services_Mapping.ToList();


                if (contractServices.Count == 0)
                {
                    return Json("No Services Found", JsonRequestBehavior.AllowGet);
                }
                List<Contract_Services_Mapping> servicesList = new List<Contract_Services_Mapping>();
                contractServices.ForEach(x =>
                {
                    servicesList.Add(new Contract_Services_Mapping
                    {
                        Contract_Services_Mapping_ID = x.Contract_Services_Mapping_ID,
                        ContractID = x.ContractID,
                        ServiceID = x.ServiceID,//
                        ServiceGroupID = x.ServiceGroupID,//
                        FrequencyOfServiceID = x.FrequencyOfServiceID,
                        Rate = x.Rate,
                        GST = x.GST,
                        SGST = x.SGST,
                        CGST = x.CGST,
                        IGST = x.IGST,
                        Qty = x.Qty,
                        Tax = x.Tax,
                        FinalRatePerService = x.FinalRatePerService,
                        Short_Service_Scope_ID = x.Short_Service_Scope_ID,
                        PeriodsOfContractID = x.PeriodsOfContractID,
                        ServiceStartDate = x.ServiceStartDate,
                        POCStartDate = x.POCStartDate,
                        POCEndDate = x.POCEndDate,
                        FrequencyOfInvoiceID = x.FrequencyOfInvoiceID,
                        ServiceStartDateInString = x.ServiceStartDate.ToString() == "" ? "" : Convert.ToDateTime(x.ServiceStartDate).ToString("dd/MM/yyyy"),
                        POCStartDateInString = x.POCStartDate.ToString() == "" ? "" : Convert.ToDateTime(x.POCStartDate).ToString("dd/MM/yyyy"),
                        POCEndDateInString = x.POCEndDate.ToString() == "" ? "" : Convert.ToDateTime(x.POCEndDate).ToString("dd/MM/yyyy")
                    }) ;
                });
                return Json(servicesList, JsonRequestBehavior.AllowGet);
            }
            else
            {

                return Json("Wrong Contract ID", JsonRequestBehavior.AllowGet);
            }

        }
        public ActionResult GetContractCustID(string ContractID)
        {
            int intContractID;

            if (!string.IsNullOrEmpty(ContractID))
            {
                intContractID = Convert.ToInt32(ContractID);

                var contractCustID = db.Contracts.Find(intContractID).CustomerID;

                var branchShortCode = db.Customers.Find(contractCustID).Branch.BranchShortCode;

                DateTime today = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, INDIAN_ZONE);
                var year = today.ToString("yy");
                string contNo = "";
                var contractToIncreaseAsList = db.Contracts.OrderByDescending(e => e.ContractID).Take(1).Select(e => e.ContractNo).ToList();
                if (contractToIncreaseAsList.Count == 0)
                {
                    contNo = "C/" + branchShortCode + year + "0001";
                }
                else
                {
                    string string1;
                    if (contractToIncreaseAsList.First().Contains("C/"))
                    {
                        string1 = contractToIncreaseAsList.First().Substring(0, 7);
                    }
                    else
                    {
                        string1 = contractToIncreaseAsList.First().Substring(0, 5);
                    }
                    //string string1 = contractToIncreaseAsList.First().Substring(0, 5);
                    //string string2 = custNumberToIncreaseAsList.First().Substring(5);
                    string numberAsString = contractToIncreaseAsList.First().Replace(string1, "");
                    string number = String.Format("{0:D4}", Int32.Parse(numberAsString) + 1);
                    contNo = "C/" + branchShortCode + year + number;
                    //referenceNumber = branchShortCode + year + month + referenceNumberToIncreaseAsList.First(). + "/" + today.ToString("yyyy") + "-" + (int.Parse(year) + 1).ToString();
                }

                return Json(new { contractCustID, contNo} , JsonRequestBehavior.AllowGet);
            }
            else
            {

                return Json("Wrong Contract ID", JsonRequestBehavior.AllowGet);
            }

        }

        public void CreateServicings(Contract_Services_Mapping map)
        {
            //IF the Servicings for this COntract's Leads are already created then assign those servicings, this Contract ID
            {
                Contract contract = map.Contract;
                if (contract != null) {
                    if (contract.CustomerID != null) {
                        Customer cust = db.Customers.Find(contract.CustomerID);
                        if (cust != null)
                        {
                            Lead lead = db.Leads.Find(cust.LeadID) ;
                            if (lead != null)
                            {
                                var servicings = db.Servicings.Where(s => s.LeadID == lead.LeadID);
                                if (servicings != null && servicings.Count() > 0)
                                {
                                    foreach (Servicing ser in servicings)
                                    {
                                        ser.ContractID = map.ContractID;
                                        ser.Contract_Services_Mapping_ID = map.Contract_Services_Mapping_ID;
                                        db.Entry(ser).State = EntityState.Modified;

                                    }
                                    db.SaveChanges();
                                    return;

                                }
                            }
                        }
                    }
                    
                }
            }

            Servicing servicing;
            FrequencyOfService frequency = db.FrequencyOfServices.Find(map.FrequencyOfServiceID);
            DateTime system_date_of_service = (DateTime)map.ServiceStartDate;
            int noOfServicing = 1;


            var branchShortCode = db.Branches.Find(map.Contract.Customer.BranchID).BranchShortCode;
            DateTime today = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, INDIAN_ZONE);
            var year = today.ToString("yy");
            string no = "";
            int intNumber = 0;
            var noToIncreaseAsList = db.Servicings.Where(e => !e.ServicingNo.Contains("-") & !e.ServicingNo.Contains("_")).OrderByDescending(e => e.ServicingID).Take(1).Select(e => e.ServicingNo).ToList();
            if (noToIncreaseAsList.Count == 0)
            {
                no = "S/" + branchShortCode + year + "0001";
                intNumber = 1;
            }
            else
            {
                string string1;
                if (noToIncreaseAsList.First().Contains("S/"))
                {
                    string1 = noToIncreaseAsList.First().Substring(0, 7);
                }
                else
                {
                    string1 = noToIncreaseAsList.First().Substring(0, 5);
                }
                //string string2 = custNumberToIncreaseAsList.First().Substring(5);
                string numberAsString = noToIncreaseAsList.First().Replace(string1, "");
                intNumber = Int32.Parse(numberAsString) + 1;
                string number = String.Format("{0:D4}", intNumber);
                no = "S/" + branchShortCode + year + number;
                //referenceNumber = branchShortCode + year + month + referenceNumberToIncreaseAsList.First(). + "/" + today.ToString("yyyy") + "-" + (int.Parse(year) + 1).ToString();
            }

            using (SadguruCRMEntities entities = new SadguruCRMEntities())
            {
                //if (frequency.FrequencyOfService1 == "One Time")
                //{
                //    servicing = new Servicing();
                //    servicing.ServicingNo = no;
                //    servicing.NoOfServicing = noOfServicing;
                //    servicing.Servicing_Frequency_Number = (byte)noOfServicing;
                //    servicing.System_Servicing_Datetime = system_date_of_service;
                //    servicing.Servicing_Datetime = system_date_of_service;
                //    servicing.Actual_Servicing_Datetime = null;
                //    servicing.ContractID = null;
                //    servicing.Contract_Services_Mapping_ID = null;
                //    servicing.LeadID = map.LeadID;
                //    servicing.Lead_Services_Mapping_ID = map.Lead_Services_Mapping_ID;
                //    servicing.CreatedDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, INDIAN_ZONE);
                //    servicing.CreatedBy = Convert.ToInt32(Session["UserID"]);
                //    //System.Diagnostics.Debug.WriteLine(system_date_of_service);

                //    //system_date_of_service = system_date_of_service.AddYears(frequency.Duration_Between_2_Services_Year).AddMonths(frequency.Duration_Between_2_Services_Month).AddDays(frequency.Duration_Between_2_Services_Day);
                //    //noOfServicing++;
                //    //intNumber++;
                //    //no = branchShortCode + year + String.Format("{0:D4}", intNumber);
                //    entities.Servicings.Add(servicing);
                //}
                if (frequency.Duration_Between_2_Services_Day > 0 || frequency.Duration_Between_2_Services_Month > 0 || frequency.Duration_Between_2_Services_Year > 0) {
                    while (system_date_of_service < map.POCEndDate)
                    {
                        servicing = new Servicing();
                        servicing.ServicingNo = "Contract_" + map.ContractID.ToString() + "_MapID_" + map.Contract_Services_Mapping_ID + "_" + noOfServicing.ToString();
                        servicing.NoOfServicing = noOfServicing;
                        servicing.Servicing_Frequency_Number = (byte)noOfServicing;
                        servicing.System_Servicing_Datetime = system_date_of_service;
                        servicing.Servicing_Datetime = system_date_of_service;
                        servicing.Actual_Servicing_Datetime = null;
                        servicing.ContractID = map.ContractID;
                        servicing.Contract_Services_Mapping_ID = map.Contract_Services_Mapping_ID;
                        system_date_of_service = system_date_of_service.AddYears(frequency.Duration_Between_2_Services_Year).AddMonths(frequency.Duration_Between_2_Services_Month).AddDays(frequency.Duration_Between_2_Services_Day);
                        servicing.CreatedDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, INDIAN_ZONE);
                        servicing.CreatedBy = Convert.ToInt32(Session["UserID"]);
                        //System.Diagnostics.Debug.WriteLine(system_date_of_service);

                        noOfServicing++;
                        entities.Servicings.Add(servicing);
                    }
                    entities.SaveChanges();
                }
                
            }
            //for (int i = 1; i <= 1000; i++) {

            //}


        }

        public ActionResult ContractServices(int ContractID)
        {
            ContractServicesViewModel vm = new ContractServicesViewModel();
            vm.ContractID = ContractID;
            //vm.leadServices = new List<Lead_Services_Mapping>();
            vm.contractServices = db.Contract_Services_Mapping.Where(x => x.ContractID == ContractID).ToList();
            ViewBag.FrequencyOfServiceID = new SelectList(db.FrequencyOfServices, "FrequencyOfServiceID", "FrequencyOfService1");
            ViewBag.PeriodsOfContractID = new SelectList(db.PeriodsOfContracts, "PeriodsOfContractID", "PeriodsOfContract1");
            return View(vm);
        }


        // GET: Contracts/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Contract contract = db.Contracts.Find(id);
            if (contract == null)
            {
                return HttpNotFound();
            }
            ViewBag.LocationID = new SelectList(db.Locations.Select(x => new { x.LocationID, x.LocationName }), "LocationID", "LocationName");
            ViewBag.CityID = new SelectList(db.Cities.Select(x => new { x.CityID, x.CityName }), "CityID", "CityName");
            ViewBag.StateID = new SelectList(db.States.Select(x => new { x.StateID, x.State1 }), "StateID", "State1");
            ViewBag.ServiceGroupID = new SelectList(db.ServiceGroups.Select(x => new { x.ServiceGroupID, x.ServiceGroup1 }), "ServiceGroupID", "ServiceGroup1");
            ViewBag.ServiceID = new SelectList(db.Services.Select(x => new { x.ServiceID, x.ServiceName }), "ServiceID", "ServiceName");
            ViewBag.FrequencyOfServiceID = new SelectList(db.FrequencyOfServices.Select(x => new { x.FrequencyOfServiceID, x.FrequencyOfService1 }), "FrequencyOfServiceID", "FrequencyOfService1");
            ViewBag.BranchID = new SelectList(db.Branches.Select(x => new { x.BranchID, x.BranchName }), "BranchID", "BranchName", contract.BranchID);
            ViewBag.PeriodsOfContract = new SelectList(db.PeriodsOfContracts.Select(x => new { x.PeriodsOfContractID, x.PeriodsOfContract1 }), "PeriodsOfContractID", "PeriodsOfContract1");
            ViewBag.PaymentModes = new SelectList(db.PaymentModes.Select(x => new { x.PaymentModeID, x.PaymentModeName }), "PaymentModeID", "PaymentModeName", contract.PaymentModeID);
            ViewBag.PaymentTerms = new SelectList(db.PaymentTerms.Select(x => new { x.PaymentTermID, x.PaymentTermName }), "PaymentTermID", "PaymentTermName", contract.PaymentTermID);
            //var customers = db.Customers.Select( x=> new { x.CustomerID, x.CustomerName, x.FirstName, x.LastName, x.Title }).ToList();
            var customers = db.Customers.ToList();
            foreach (var item in customers)
            {
                if (item.CustomerName == null)
                {
                    item.CustomerName = item.CustomerID + " " + item.Title + " " + item.FirstName + " " + item.LastName;
                }
            }

            ViewBag.CustomerID = new SelectList(customers.Where(c => c.CustomerID == contract.CustomerID).Select(x => new { x.CustomerName, x.CustomerID }), "CustomerID", "CustomerName", contract.CustomerID);
            var enumData = from EnumClassesAndHelpers.GSTTypes g in Enum.GetValues(typeof(EnumClassesAndHelpers.GSTTypes))
                           select new
                           {
                               ID = (byte)g,
                               Name = EnumHelper.GetEnumDescription(g)
                           };

            ViewBag.GSTTypes = new SelectList(enumData.Select(x => new { x.ID, x.Name }), "ID", "Name");
            ViewBag.FrequencyInvoice = new SelectList(db.FrequencyOfInvoices.Select(x => new { x.FrequencyOfInvoiceID, x.FrequencyOfInvoice1 }), "FrequencyOfInvoiceID", "FrequencyOfInvoice1");
            ViewBag.TypeOfPremises = new SelectList(db.PremisesTypes.Select(x => new { x.PremisesTypeID, x.PremisesType1 }), "PremisesTypeID", "PremisesType1", contract.PremisesTypeID);
            ViewBag.PremisesArea = new SelectList(db.Premises_Area_Master.Select(x => new { x.Premises_Area_ID, x.Premises_Area }), "Premises_Area_ID", "Premises_Area", contract.Premises_Area_ID);
            //ViewBag.BookedBy = new SelectList(db.UserLogins, "UserID", "UserName");
            ViewBag.Executive = new SelectList(db.Employees.Select(x => new { x.EmployeeID, x.Name }), "EmployeeID", "Name");
            ViewBag.BookedBy = new SelectList(db.UserLogins.Select(x => new { x.UserID, x.UserName }), "UserID", "UserName", contract.BookByID);
            //int? LoggedInEmployeeID = db.UserLogins.Find(Convert.ToInt32(Session["UserID"])).EmployeeID;
            //if (LoggedInEmployeeID == null)
            //{
            ViewBag.BookByContactNo = contract.BookByContactNo;
            //}
            //else
            //{
            //  ViewBag.BookByContactNo = db.Employees.Find(LoggedInEmployeeID).PrimaryCellNumber;
            //}

            ViewBag.ShortServiceScope = new SelectList(db.Short_Service_Scope_Master.Select(x => new { x.Short_Service_Scope_ID, x.Short_Service_Scope }), "Short_Service_Scope_ID", "Short_Service_Scope");
            var leads = db.Leads.Where(l => l.LeadID == contract.LeadID).Where(l => db.Customers.Select(c => c.LeadID).Contains(l.LeadID)).Include(l => l.Branch).Include(l => l.City).Include(l => l.UserLogin).Include(l => l.LeadStatus).Include(l => l.Source).Include(l => l.Source1).Include(l => l.PremisesType);            //new Code
            //var leadsList = await new HelperNonStatic().GenerateCustomerNameAndServicesShortCodes(leads);
            var leadsOrderedNewCode = leads.Select(s => new
            {
                s.LeadID,
                Description = s.LeadID + " - " + s.Title + " " + s.FirstName + " " + s.LastName + " " + s.CustomerName

            }).OrderByDescending(i => i.LeadID).ToList();
            ViewBag.LeadID = new SelectList(leadsOrderedNewCode, "LeadID", "Description", contract.LeadID);

            var existingContracts = db.Contracts.Where(c => c.ContractID == contract.ExistingContractsID).Include(c => c.Customer).Select(c => new
            {
                c.ContractID,
                c.ContractNo,
                Description = c.ContractNo + " - " + c.Customer.Title + " " + c.Customer.FirstName + " " + c.Customer.LastName + " " + c.Customer.CustomerName

            }).OrderByDescending(c => c.ContractID).ToList();
            ViewBag.ExistingContractID = new SelectList(existingContracts, "ContractID", "Description", contract.ExistingContractsID);
            return View(contract);
        }

        // POST: Contracts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {

                var Contracts_Service_Address_Mappings = db.Contracts_Service_Address_Mapping.Where(c => c.ContractID == id);
                db.Contracts_Service_Address_Mapping.RemoveRange(Contracts_Service_Address_Mappings);


                var Contract_Services_Mappings = db.Contract_Services_Mapping.Where(c => c.ContractID == id);
                db.Contract_Services_Mapping.RemoveRange(Contract_Services_Mappings);

                Contract contract = db.Contracts.Find(id);
                db.Contracts.Remove(contract);
                db.SaveChanges();
            }
            catch (Exception ex) { 
            
            }
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

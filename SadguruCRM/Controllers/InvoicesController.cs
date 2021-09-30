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

namespace SadguruCRM.Controllers
{
    [VerifyUser]
    public class InvoicesController : Controller
    {
        private SadguruCRMEntities db = new SadguruCRMEntities();
        private static TimeZoneInfo INDIAN_ZONE = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");

        // GET: Invoices
        public ActionResult Index()
        {
            EnumClassesAndHelpers.TaxInvoiceTypes InvoiceType;
            bool result = EnumClassesAndHelpers.TaxInvoiceTypes.TryParse(Request.QueryString["type"], out InvoiceType);
            if (result)
            {
                if (InvoiceType.Equals(EnumClassesAndHelpers.TaxInvoiceTypes.NewTaxInvoice))
                {
                    var invoices = db.Invoices.Where(x => x.Tax_Invoice_Type_Enum == (byte)EnumClassesAndHelpers.TaxInvoiceTypes.NewTaxInvoice).Include(i => i.Contract).Include(i => i.Customer).Include(i => i.UserLogin).Include(i => i.UserLogin1);
                    foreach (var item in invoices)
                    {
                        if (item.Customer.CustomerName == null)
                        {
                            item.Customer.CustomerName = item.Customer.Title + " " + item.Customer.FirstName + " " + item.Customer.LastName;
                        }
                        var services = item.Invoice_Services_Mapping.Select(i => i.ServiceID).ToList();
                        var shortCodes = String.Join(", ", db.Services.Where(i => services.Contains(i.ServiceID)).Select(i => i.ServiceShortCode));
                        item.ExecutiveName = shortCodes;

                        var billAddrs = item.Customer.Customers_Billing_Address_Mapping.Select(i => i.Customers_Billing_Address_Mapping_ID).ToList();
                        //var telNos = String.Join(", ", db.Customers_Billing_Address_Tel_No_Mapping.Where(i => billAddrs.Contains(i.Customers_Billing_Address_Mapping_ID)).Select(i => i.Customers_Billing_Address_Tel_No));
                        //item.Customer.TelNumber = telNos;

                        //var celNos = String.Join(", ", db.Customers_Billing_Address_Cell_No_Mapping.Where(i => billAddrs.Contains(i.Customers_Billing_Address_Mapping_ID)).Select(i => i.Customers_Billing_Address_Cell_No));
                        //item.Customer.CellNumber = celNos;
                    }
                    ViewBag.InvoiceType = (byte)EnumClassesAndHelpers.TaxInvoiceTypes.NewTaxInvoice;
                    ViewBag.InvoiceTypeName = EnumHelper.GetEnumDescription(InvoiceType);
                    ViewBag.BankID = new SelectList(db.Bank_Master, "BankID", "BankName");
                    ViewBag.PaymentModes = new SelectList(db.PaymentModes, "PaymentModeID", "PaymentModeName");
                    return View(invoices.ToList());
                }
                else if (InvoiceType.Equals(EnumClassesAndHelpers.TaxInvoiceTypes.ProformaInvoice))
                {
                    var invoices = db.Invoices.Where(x => x.Tax_Invoice_Type_Enum == (byte)EnumClassesAndHelpers.TaxInvoiceTypes.ProformaInvoice).Include(i => i.Contract).Include(i => i.Customer).Include(i => i.UserLogin).Include(i => i.UserLogin1);
                    foreach (var item in invoices)
                    {
                        if (item.Customer.CustomerName == null)
                        {
                            item.Customer.CustomerName = item.Customer.Title + " " + item.Customer.FirstName + " " + item.Customer.LastName;
                        }
                        var services = item.Invoice_Services_Mapping.Select(i => i.ServiceID).ToList();
                        var shortCodes = String.Join(", ", db.Services.Where(i => services.Contains(i.ServiceID)).Select(i => i.ServiceShortCode));
                        item.ExecutiveName = shortCodes;

                        //var billAddrs = item.Customer.Customers_Billing_Address_Mapping.Select(i => i.Customers_Billing_Address_Mapping_ID).ToList();
                        //var telNos = String.Join(", ", db.Customers_Billing_Address_Tel_No_Mapping.Where(i => billAddrs.Contains(i.Customers_Billing_Address_Mapping_ID)).Select(i => i.Customers_Billing_Address_Tel_No));
                        //item.Customer.TelNumber = telNos;

                        //var celNos = String.Join(", ", db.Customers_Billing_Address_Cell_No_Mapping.Where(i => billAddrs.Contains(i.Customers_Billing_Address_Mapping_ID)).Select(i => i.Customers_Billing_Address_Cell_No));
                        //item.Customer.CellNumber = celNos;
                    }
                    ViewBag.InvoiceType = (byte)EnumClassesAndHelpers.TaxInvoiceTypes.ProformaInvoice;
                    ViewBag.InvoiceTypeName = EnumHelper.GetEnumDescription(InvoiceType);
                    ViewBag.BankID = new SelectList(db.Bank_Master, "BankID", "BankName");
                    ViewBag.PaymentModes = new SelectList(db.PaymentModes, "PaymentModeID", "PaymentModeName");
                    return View(invoices.ToList());
                }
                else {
                    return null;
                }
            }
            else
            {
                return null;
            }
            
            
        }

        // GET: Invoices/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Invoice invoice = db.Invoices.Find(id);
            if (invoice == null)
            {
                return HttpNotFound();
            }
            EnumClassesAndHelpers.TaxInvoiceTypes InvoiceType;
            bool result = EnumClassesAndHelpers.TaxInvoiceTypes.TryParse(invoice.Tax_Invoice_Type_Enum.ToString(), out InvoiceType);
            if (result)
            {

                if (InvoiceType.Equals(EnumClassesAndHelpers.TaxInvoiceTypes.NewTaxInvoice))
                {
                    ViewBag.InvoiceType = (byte)EnumClassesAndHelpers.TaxInvoiceTypes.NewTaxInvoice;

                }
                else if (InvoiceType.Equals(EnumClassesAndHelpers.TaxInvoiceTypes.ProformaInvoice))
                {
                    ViewBag.InvoiceType = (byte)EnumClassesAndHelpers.TaxInvoiceTypes.ProformaInvoice;
                }
            }
            ViewBag.InvoiceTypeName = EnumHelper.GetEnumDescription(InvoiceType);
            ViewBag.ContractID = new SelectList(db.Contracts.Where(c => c.ContractID == invoice.ContractID), "ContractID", "ContractNo");
            //ViewBag.CustomerID = new SelectList(db.Customers, "CustomerID", "CustomerName");
            var customers = db.Customers.ToList();
            foreach (var item in customers)
            {
                if (item.CustomerName == null)
                {
                    item.CustomerName = item.CustomerNo + " - " + item.Title + " " + item.FirstName + " " + item.LastName;
                }
                else
                {
                    item.CustomerName = item.CustomerNo + " - " + item.CustomerName;
                }
            }

            ViewBag.CustomerID = new SelectList(customers.Where(c => c.CustomerID == invoice.CustomerID).OrderByDescending(c => c.CustomerID), "CustomerID", "CustomerName", invoice.CustomerID);
            ViewBag.CreatedBy = new SelectList(db.UserLogins, "UserID", "UserName");
            ViewBag.LastUpdatedBy = new SelectList(db.UserLogins, "UserID", "UserName");
            ViewBag.LocationID = new SelectList(db.Locations, "LocationID", "LocationName");
            ViewBag.CityID = new SelectList(db.Cities, "CityID", "CityName");
            ViewBag.StateID = new SelectList(db.States, "StateID", "State1");
            ViewBag.ServiceGroupID = new SelectList(db.ServiceGroups, "ServiceGroupID", "ServiceGroup1");
            ViewBag.ServiceID = new SelectList(db.Services.Where(x => x.ServiceID == 0), "ServiceID", "ServiceName");
            ViewBag.FrequencyOfServiceID = new SelectList(db.FrequencyOfServices, "FrequencyOfServiceID", "FrequencyOfService1");
            ViewBag.BranchID = new SelectList(db.Branches, "BranchID", "BranchName");
            ViewBag.PeriodsOfContract = new SelectList(db.PeriodsOfContracts, "PeriodsOfContractID", "PeriodsOfContract1");
            ViewBag.PaymentModes = new SelectList(db.PaymentModes, "PaymentModeID", "PaymentModeName");
            ViewBag.PaymentTerms = new SelectList(db.PaymentTerms, "PaymentTermID", "PaymentTermName");
            //ViewBag.TypeOfPremisesID = new SelectList(db.PremisesTypes, "PremisesTypeID", "PremisesType1");
            var enumData = from EnumClassesAndHelpers.GSTTypes g in Enum.GetValues(typeof(EnumClassesAndHelpers.GSTTypes))
                           select new
                           {
                               ID = (byte)g,
                               Name = EnumHelper.GetEnumDescription(g)
                           };

            ViewBag.GSTTypes = new SelectList(enumData, "ID", "Name");

            ViewBag.BankID = new SelectList(db.Bank_Master, "BankID", "BankName");
            ViewBag.ShortServiceScope = new SelectList(db.Short_Service_Scope_Master.Select(x => new { x.Short_Service_Scope_ID, x.Short_Service_Scope }), "Short_Service_Scope_ID", "Short_Service_Scope");
            ViewBag.FrequencyInvoice = new SelectList(db.FrequencyOfInvoices.Select(x => new { x.FrequencyOfInvoiceID, x.FrequencyOfInvoice1 }), "FrequencyOfInvoiceID", "FrequencyOfInvoice1");
            ViewBag.TypeOfPremises = new SelectList(db.PremisesTypes.Select(x => new { x.PremisesTypeID, x.PremisesType1 }), "PremisesTypeID", "PremisesType1");
            ViewBag.PremisesArea = new SelectList(db.Premises_Area_Master.Select(x => new { x.Premises_Area_ID, x.Premises_Area }), "Premises_Area_ID", "Premises_Area");

            return View(invoice);
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

                return new Rotativa.UrlAsPdf("/Invoices/PDFView/" + id.ToString())
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
            Invoice invoice = db.Invoices.Find(id);
            if (invoice == null)
            {
                return HttpNotFound();
            }
            EnumClassesAndHelpers.TaxInvoiceTypes InvoiceType;
            bool result = EnumClassesAndHelpers.TaxInvoiceTypes.TryParse(invoice.Tax_Invoice_Type_Enum.ToString(), out InvoiceType);
            if (result)
            {

                if (InvoiceType.Equals(EnumClassesAndHelpers.TaxInvoiceTypes.NewTaxInvoice))
                {
                    ViewBag.InvoiceType = (byte)EnumClassesAndHelpers.TaxInvoiceTypes.NewTaxInvoice;

                }
                else if (InvoiceType.Equals(EnumClassesAndHelpers.TaxInvoiceTypes.ProformaInvoice))
                {
                    ViewBag.InvoiceType = (byte)EnumClassesAndHelpers.TaxInvoiceTypes.ProformaInvoice;
                }
            }
            ViewBag.InvoiceTypeName = EnumHelper.GetEnumDescription(InvoiceType);
            return View(invoice);
        }

        // GET: Invoices/Create
        public ActionResult Create()
        {
            EnumClassesAndHelpers.TaxInvoiceTypes InvoiceType;
            bool result = EnumClassesAndHelpers.TaxInvoiceTypes.TryParse(Request.QueryString["type"], out InvoiceType);
            if (result)
            {

                if (InvoiceType.Equals(EnumClassesAndHelpers.TaxInvoiceTypes.NewTaxInvoice))
                {
                    ViewBag.InvoiceType = (byte)EnumClassesAndHelpers.TaxInvoiceTypes.NewTaxInvoice;
                    
                }
                else if (InvoiceType.Equals(EnumClassesAndHelpers.TaxInvoiceTypes.ProformaInvoice))
                {
                    ViewBag.InvoiceType = (byte)EnumClassesAndHelpers.TaxInvoiceTypes.ProformaInvoice;
                }
            }
            ViewBag.InvoiceTypeName = EnumHelper.GetEnumDescription(InvoiceType);
            ViewBag.ContractID = new SelectList(db.Contracts, "ContractID", "ContractNo");
            //ViewBag.CustomerID = new SelectList(db.Customers, "CustomerID", "CustomerName");
            var customers = db.Customers.ToList();
            foreach (var item in customers)
            {
                if (item.CustomerName == null)
                {
                    item.CustomerName = item.CustomerNo + " - " + item.Title + " " + item.FirstName + " " + item.LastName;
                }
                else {
                    item.CustomerName = item.CustomerNo + " - " + item.CustomerName;
                }
            }

            ViewBag.CustomerID = new SelectList(customers.OrderByDescending(c => c.CustomerID), "CustomerID", "CustomerName");
            ViewBag.CreatedBy = new SelectList(db.UserLogins, "UserID", "UserName");
            ViewBag.LastUpdatedBy = new SelectList(db.UserLogins, "UserID", "UserName");
            ViewBag.LocationID = new SelectList(db.Locations, "LocationID", "LocationName");
            ViewBag.CityID = new SelectList(db.Cities, "CityID", "CityName");
            ViewBag.StateID = new SelectList(db.States, "StateID", "State1");
            ViewBag.ServiceGroupID = new SelectList(db.ServiceGroups, "ServiceGroupID", "ServiceGroup1");
            ViewBag.ServiceID = new SelectList(db.Services.Where(x => x.ServiceID == 0), "ServiceID", "ServiceName");
            ViewBag.FrequencyOfServiceID = new SelectList(db.FrequencyOfServices, "FrequencyOfServiceID", "FrequencyOfService1");
            ViewBag.BranchID = new SelectList(db.Branches, "BranchID", "BranchName");
            ViewBag.PeriodsOfContract = new SelectList(db.PeriodsOfContracts, "PeriodsOfContractID", "PeriodsOfContract1");
            ViewBag.PaymentModes = new SelectList(db.PaymentModes, "PaymentModeID", "PaymentModeName");
            ViewBag.PaymentTerms = new SelectList(db.PaymentTerms, "PaymentTermID", "PaymentTermName");
            //ViewBag.TypeOfPremisesID = new SelectList(db.PremisesTypes, "PremisesTypeID", "PremisesType1");
            var enumData = from EnumClassesAndHelpers.GSTTypes g in Enum.GetValues(typeof(EnumClassesAndHelpers.GSTTypes))
                           select new
                           {
                               ID = (byte)g,
                               Name = EnumHelper.GetEnumDescription(g)
                            };

            ViewBag.GSTTypes = new SelectList(enumData, "ID", "Name");

            ViewBag.BankID = new SelectList(db.Bank_Master, "BankID", "BankName");
            ViewBag.ShortServiceScope = new SelectList(db.Short_Service_Scope_Master.Select(x => new { x.Short_Service_Scope_ID, x.Short_Service_Scope }), "Short_Service_Scope_ID", "Short_Service_Scope");
            ViewBag.FrequencyInvoice = new SelectList(db.FrequencyOfInvoices.Select(x => new { x.FrequencyOfInvoiceID, x.FrequencyOfInvoice1 }), "FrequencyOfInvoiceID", "FrequencyOfInvoice1");
            ViewBag.TypeOfPremises = new SelectList(db.PremisesTypes.Select(x => new { x.PremisesTypeID, x.PremisesType1 }), "PremisesTypeID", "PremisesType1");
            ViewBag.PremisesArea = new SelectList(db.Premises_Area_Master.Select(x => new { x.Premises_Area_ID, x.Premises_Area }), "Premises_Area_ID", "Premises_Area");

            ViewBag.UnassignedAdvanceEntriesOfAllCustomers = db.Collection_Entry.Where(c => c.InvoiceID == null && c.CustomerID != null && c.LeadID == null && c.isAdvanceEntry == true).ToList();

            Invoice invoice = new Invoice();
            return View(invoice);
        }

        public ActionResult GetNewInvoiceNo(string CustID)
        {
            int intCustID;

            if (!string.IsNullOrEmpty(CustID))
            {
                intCustID = Convert.ToInt32(CustID);

                var CustomerID = db.Customers.Find(intCustID).CustomerID;

                //var lead = db.Leads.Where(x => x.LeadID == intLeadID).Select(x => new { x.ConsultPerson, x.ConsultPersonDesignation, x.AddressLine1, x.AddressLine2, x.AddressLine3, x.City.CityName, x.Location.LocationName, x.State.State1, x.Branch.BranchName, x.Pincode, x.TelNo, x.CellNo, x.EmailID, Premise = x.PremisesType.PremisesType1, VisitAllocated = x.Employee.Name, VisitAllocatedDesignation = x.Employee.EmployeeDesignation.Designation, VisitAllocatedCellNumber = x.Employee.ContactNumber, TeleCaller = x.UserLogin1.UserName, x.Title, x.CustomerName, x.FirstName, x.LastName, x.SourceID, x.SubSourceID, x.BranchID, x.IndustryID, x.LocationID, x.CityID, x.StateID, ExtraTelNos = x.Lead_Tel_No_Mapping.Select(o => o.Lead_Tel_No), ExtraCellNos = x.Lead_Cell_No_Mapping.Select(o => o.Lead_Cell_No), ExtraEmails = x.Lead_Email_Mapping.Select(o => o.Lead_Email), PremisesType = x.PremisesType.PremisesType1 }).FirstOrDefault();
                var branchShortCode = db.Customers.Find(intCustID).Branch.BranchShortCode;

                DateTime today = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, INDIAN_ZONE);
                var year = today.ToString("yy");
                string invoiceNo = "";
                var invoiceNoToIncreaseAsList = db.Invoices.OrderByDescending(e => e.InvoiceID).Take(1).Select(e => e.InvoiceNo).ToList();
                if (invoiceNoToIncreaseAsList.Count == 0)
                {
                    invoiceNo = branchShortCode + year + "0001";
                }
                else
                {
                    string string1 = "";
                    if (invoiceNoToIncreaseAsList.First().Contains("T/") || invoiceNoToIncreaseAsList.First().Contains("P/"))
                    {
                        string1 = invoiceNoToIncreaseAsList.First().Substring(0, 7);
                    }
                    else {
                        string1 = invoiceNoToIncreaseAsList.First().Substring(0, 5);
                        //string string2 = custNumberToIncreaseAsList.First().Substring(5);
                    }
                    string numberAsString = invoiceNoToIncreaseAsList.First().Replace(string1, "");
                    string number = String.Format("{0:D4}", Int32.Parse(numberAsString) + 1);
                    invoiceNo = branchShortCode + year + number;
                    //referenceNumber = branchShortCode + year + month + referenceNumberToIncreaseAsList.First(). + "/" + today.ToString("yyyy") + "-" + (int.Parse(year) + 1).ToString();

                }


                return Json(new { CustomerID, invoiceNo}, JsonRequestBehavior.AllowGet);
            }
            else
            {

                return Json("Wrong Lead ID", JsonRequestBehavior.AllowGet);
            }

        }
        // POST: Invoices/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Invoice invoice, byte Tax_Invoice_Type_Enum, byte? GST_Type_Enum, String[] Customers_Service_Address_Mapping_ID, String[] ServiceGroupID, String[] ServiceID, String[] FrequencyOfServiceID,int[] PeriodsOfContractID, DateTime?[] ServiceStartDate, DateTime?[] PeriodOfContractStartDate, DateTime?[] PeriodOfContractEndDate, DateTime?[] PeriodOfInvoiceStartDate, DateTime?[] PeriodOfInvoiceEndDate, String[] SACCode, String[] Qty, String[] GST, /*String[] Tax,*/ String[] Rate, String[] FinalRatePerService,int[] ShortServiceScope,String[] CGST, String[] SGST, String[] IGST, DateTime? PaymentReceivedOn, bool TDSapplicable, decimal? TDSAmount, Decimal? BadDebtsAmount, string ChequeNo, DateTime? ChequeDate, string ChequeName, int? BankID, string DraweeName, string SubmitType)
        {   
            //CultureInfo culture = new CultureInfo("en-AU");
            //System.Threading.Thread.CurrentThread.CurrentCulture = culture;
            //System.Threading.Thread.CurrentThread.CurrentUICulture = culture;
            
            if (ModelState.IsValid)

            {
                invoice.CreatedBy = Convert.ToInt32(Session["UserID"]);
                invoice.CreatedDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, INDIAN_ZONE);
                invoice.Tax_Invoice_Type_Enum = Tax_Invoice_Type_Enum;
                invoice.GST_Type_Enum = GST_Type_Enum;
                
                //invoice.InvoiceDate = DateTime.ParseExact(InvoiceDate, "d/M/yyyy", CultureInfo.InvariantCulture);
                //invoice.PurchaseOrderDate = DateTime.ParseExact(PurchaseOrderDate, "d/M/yyyy", CultureInfo.InvariantCulture);
                //invoice.Period_Of_Contract_Start_Date = DateTime.ParseExact(Period_Of_Contract_Start_Date, "d/M/yyyy", CultureInfo.InvariantCulture);
                //invoice.Period_Of_Contract_End_Date = DateTime.ParseExact(Period_Of_Contract_End_Date, "d/M/yyyy", CultureInfo.InvariantCulture);
                //invoice.Period_Of_Invoice_Start_Date = DateTime.ParseExact(Period_Of_Invoice_Start_Date, "d/M/yyyy", CultureInfo.InvariantCulture);
                //invoice.Period_Of_Invoice_End_Date = DateTime.ParseExact(Period_Of_Invoice_End_Date, "d/M/yyyy", CultureInfo.InvariantCulture);
                db.Invoices.Add(invoice);

                db.DbccCheckIdent<Invoice>(db.Invoices.Max(p => p.InvoiceID));
                db.SaveChanges();

                for (int i = 0; i < Customers_Service_Address_Mapping_ID.Length; i++)
                {
                    Invoice_Customer_Service_Address_Mapping servAdd = new Invoice_Customer_Service_Address_Mapping();
                    servAdd.Customers_Service_Address_Mapping_ID = Int32.Parse(Customers_Service_Address_Mapping_ID[i]);
                    servAdd.InvoiceID = invoice.InvoiceID;
                    servAdd.CustomerID = invoice.CustomerID;
                    db.Invoice_Customer_Service_Address_Mapping.Add(servAdd);
                    //db.DbccCheckIdent<Invoice_Customer_Service_Address_Mapping>(db.Invoice_Customer_Service_Address_Mapping.Max(p => p.Invoice_Customer_Service_Address_Mapping_ID));
                    db.SaveChanges();
                }

                try
                {
                    for (int i = 0; i < ServiceGroupID.Length; i++)
                    {
                        int outServiceGroupID;
                        if (Int32.TryParse(ServiceGroupID[i], out outServiceGroupID))
                        {
                            Invoice_Services_Mapping servicesMapping = new Invoice_Services_Mapping();
                            servicesMapping.InvoiceID = invoice.InvoiceID;
                            servicesMapping.CustomerID = invoice.CustomerID;
                            servicesMapping.ServiceGroupID = outServiceGroupID;
                            int outServiceID;
                            if (Int32.TryParse(ServiceID[i], out outServiceID))
                                servicesMapping.ServiceID = outServiceID;
                            //else
                                //servicesMapping.ServiceID = null;

                            //if (FrequencyOfInvoiceID[i] != 0)
                            //    servicesMapping.FrequencyOfInvoiceID = FrequencyOfInvoiceID[i];
                            //else
                            //    servicesMapping.FrequencyOfInvoiceID = null;

                            //NEEDED
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
                            try
                            {
                                servicesMapping.POIStartDate = PeriodOfInvoiceStartDate[i];
                            }
                            catch (Exception e)
                            {

                            }
                            try
                            {
                                servicesMapping.POIEndDate = PeriodOfInvoiceEndDate[i];
                            }
                            catch (Exception e)
                            {

                            }


                            int outFrequencyOfServiceID;
                            if (Int32.TryParse(FrequencyOfServiceID[i], out outFrequencyOfServiceID))
                                servicesMapping.FrequencyOfServiceID = outFrequencyOfServiceID;
  

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

                            //decimal outCGST, outSGST, outIGST, outFinalRatePerService;
                            //if (Decimal.TryParse(CGST[i], out outCGST))
                            //    servicesMapping.CGST = outCGST;
                            //else
                            //    servicesMapping.CGST = null;

                            //if (Decimal.TryParse(SGST[i], out outSGST))
                            //    servicesMapping.SGST = outSGST;
                            //else
                            //    servicesMapping.SGST = null;

                            //if (Decimal.TryParse(IGST[i], out outIGST))
                            //    servicesMapping.IGST = outIGST;
                            //else
                            //    servicesMapping.IGST = null;

                            //if (Decimal.TryParse(FinalRatePerService[i], out outFinalRatePerService))
                            //    servicesMapping.FinalRatePerService = outFinalRatePerService;
                            //else
                            //    servicesMapping.FinalRatePerService = null;

                            //servicesMapping.FinalRatePerService = FinalRatePerService[i];
                            db.Invoice_Services_Mapping.Add(servicesMapping);

                            db.DbccCheckIdent<Invoice_Services_Mapping>(db.Invoice_Services_Mapping.Max(p => p.Invoice_Services_Mapping_ID));
                            db.SaveChanges();
                            try
                            {
                                //CreateServicings(servicesMapping);
                            }
                            catch (Exception e)
                            {
                                string ex = e.Message;
                            }

                        }

                    }
                }
                catch (Exception e)
                {

                    string ex = e.Message;
                }
                
                try
                {
                    //if (invoice.PaidByCustomer != "")
                    // No Collection Entry for Proforma Invoice
                    if (invoice.Tax_Invoice_Type_Enum == (byte)TaxInvoiceTypes.NewTaxInvoice)
                    {
                        Collection_Entry collection_Entry = new Collection_Entry();
                        collection_Entry.InvoiceID = invoice.InvoiceID;
                        collection_Entry.ReceivedOn = PaymentReceivedOn;
                        collection_Entry.PaymentModeID = invoice.PaymentModeID;
                        Decimal amount;         
                        if (Decimal.TryParse(invoice.PaidByCustomer, out amount))
                            collection_Entry.Amount = amount;
                        collection_Entry.TDSapplicable = TDSapplicable;
                        collection_Entry.TDSAmount = TDSAmount;
                        collection_Entry.BadDebtsAmount = BadDebtsAmount;
                        collection_Entry.ChequeNo = ChequeNo;
                        collection_Entry.ChequeDate = ChequeDate;
                        collection_Entry.ChequeName = ChequeName;
                        collection_Entry.BankID = BankID;
                        collection_Entry.DraweeName = DraweeName;
                        collection_Entry.CreatedBy = Convert.ToInt32(Session["UserID"]);
                        collection_Entry.CreatedOn = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, INDIAN_ZONE);
                        db.Collection_Entry.Add(collection_Entry);

                        db.DbccCheckIdent<Collection_Entry>(db.Collection_Entry.Max(p => p.Collection_Entry_ID));
                        db.SaveChanges();

                    }
                }
                catch (Exception ex) {
                    string s = ex.Message;
                }
                if (SubmitType == "Save")
                {
                    //SendMessage("New object added");
                    return RedirectToAction("Index", new { type = Tax_Invoice_Type_Enum });
                }
                else if (SubmitType == "Save & Create New Lead")
                {
                    //SendMessage("New object added");
                    return RedirectToAction("Create", new { type = Tax_Invoice_Type_Enum });
                }
                return RedirectToAction("Index", new { type = Tax_Invoice_Type_Enum });
            }

            ViewBag.ContractID = new SelectList(db.Contracts, "ContractID", "ContractNo", invoice.ContractID);
            var customers = db.Customers.ToList();
            foreach (var item in customers)
            {
                if (item.CustomerName == null)
                {
                    item.CustomerName = item.CustomerID + " - " + item.Title + " " + item.FirstName + " " + item.LastName;
                }
                else
                {
                    item.CustomerName = item.CustomerID + " - " + item.CustomerName;
                }
            }
            ViewBag.InvoiceType = (byte)Tax_Invoice_Type_Enum;
            ViewBag.InvoiceTypeName = (byte)Tax_Invoice_Type_Enum;
            ViewBag.CustomerID = new SelectList(customers, "CustomerID", "CustomerName", invoice.CustomerID);
            ViewBag.CreatedBy = new SelectList(db.UserLogins, "UserID", "UserName", invoice.CreatedBy);
            ViewBag.LastUpdatedBy = new SelectList(db.UserLogins, "UserID", "UserName", invoice.LastUpdatedBy);
            ViewBag.LocationID = new SelectList(db.Locations, "LocationID", "LocationName");
            ViewBag.CityID = new SelectList(db.Cities, "CityID", "CityName");
            ViewBag.StateID = new SelectList(db.States, "StateID", "State1");
            ViewBag.ServiceGroupID = new SelectList(db.ServiceGroups, "ServiceGroupID", "ServiceGroup1");
            ViewBag.ServiceID = new SelectList(db.Services.Where(x => x.ServiceID == 0), "ServiceID", "ServiceName");
            ViewBag.FrequencyOfServiceID = new SelectList(db.FrequencyOfServices, "FrequencyOfServiceID", "FrequencyOfService1");
            ViewBag.BranchID = new SelectList(db.Branches, "BranchID", "BranchName");
            ViewBag.PeriodsOfContract = new SelectList(db.PeriodsOfContracts, "PeriodsOfContractID", "PeriodsOfContract1");
            ViewBag.PaymentModes = new SelectList(db.PaymentModes, "PaymentModeID", "PaymentModeName", invoice.PaymentModeID);
            ViewBag.PaymentTerms = new SelectList(db.PaymentTerms, "PaymentTermID", "PaymentTermName", invoice.PaymentTermID);
            ViewBag.TypeOfPremisesID = new SelectList(db.PremisesTypes, "PremisesTypeID", "PremisesType1", invoice.TypeOfPremisesID);
            var enumData = from EnumClassesAndHelpers.GSTTypes g in Enum.GetValues(typeof(EnumClassesAndHelpers.GSTTypes))
                           select new
                           {
                               ID = (byte)g,
                               Name = EnumHelper.GetEnumDescription(g)
                           };
            ViewBag.GSTTypes = new SelectList(enumData, "ID", "Name");

            ViewBag.BankID = new SelectList(db.Bank_Master, "BankID", "BankName");
            return View(invoice);
        }
        [HttpPost]
        public ActionResult GetCustomerContracts(string custID)
        {
            int intCustID;

            if (!string.IsNullOrEmpty(custID))
            {
                intCustID = Convert.ToInt32(custID);

                //Get Contracts
                var contracts = db.Contracts.Where(x => x.CustomerID == intCustID).OrderByDescending(c => c.ContractID).ToList();
                int noOfContracts = contracts.Count;
                //List<Contract> contractsList = new List<Contract>();
                //var ContractDate = (db.Contracts.Where(x => x.ContractID == intContractID).First().ContractDate).Value.ToString("dd/MM/yyyy");
                var customer = db.Customers.Find(intCustID);
                var Lead = db.Leads.Find(customer.LeadID);
                var ConsultPerson = customer.ConsultPerson;
                var ConsultPersonDesignation = customer.ConsultPersonDesignation;
                var billingAddress = db.Customers_Billing_Address_Mapping.Where(x => x.CustomerID == intCustID).FirstOrDefault();
                long billingAddressID = billingAddress.Customers_Billing_Address_Mapping_ID;
                string strBillingTelNos = customer.TelNumber;
                string strBillingCellNos = customer.CellNumber;
                string strBillingEmails = customer.EmailId;

                //var serviceAddresses = db.Contracts_Service_Address_Mapping.Where(x => x.ContractID == intContractID).ToList();
                var serviceAddresses = db.Customers_Service_Address_Mapping.Where(x => x.CustomerID == intCustID).ToList();



                if (billingAddress == null)
                {
                    return Json("No Business Address Found", JsonRequestBehavior.AllowGet);
                }
                //else if (serviceAddresses.Count == 0)
                //{
                //    return Json("No Service Address Found", JsonRequestBehavior.AllowGet);
                //}
                Customers_Billing_Address_Mapping bill = new Customers_Billing_Address_Mapping();
                bill.Customers_Billing_Address_Mapping_ID = billingAddress.Customers_Billing_Address_Mapping_ID;
                bill.AddressLine1 = billingAddress.AddressLine1;
                bill.AddressLine2 = billingAddress.AddressLine2;
                bill.AddressLine3 = billingAddress.AddressLine3;
                bill.BillingLocationID = billingAddress.BillingLocationID;
                bill.BillingCityID = billingAddress.BillingCityID;
                bill.BillingPincode = billingAddress.BillingPincode;
                bill.BillingStateID = billingAddress.BillingStateID;
                bill.GSTNo = customer.GstinNo;

                List<Customers_Service_Address_Mapping> servicesAddressList = new List<Customers_Service_Address_Mapping>();
                serviceAddresses.ForEach(x =>
                {
                    var custServAdd = db.Customers_Service_Address_Mapping.Find(x.Customers_Service_Address_Mapping_ID);
                    Customers_Service_Address_Mapping serv = new Customers_Service_Address_Mapping();
                    serv.Customers_Service_Address_Mapping_ID = custServAdd.Customers_Service_Address_Mapping_ID;
                    serv.AddressLine1 = custServAdd.AddressLine1;
                    serv.AddressLine2 = custServAdd.AddressLine2;
                    serv.AddressLine3 = custServAdd.AddressLine3;
                    serv.ServiceLocationID = custServAdd.ServiceLocationID;
                    serv.ServiceCityID = custServAdd.ServiceCityID;
                    serv.ServicePincode = custServAdd.ServicePincode;
                    serv.ServiceAddressConsultPerson = custServAdd.ServiceAddressConsultPerson;
                    serv.Customer_Service_Address_Cell_No_1 = custServAdd.Customer_Service_Address_Cell_No_1;
                    serv.Customer_Service_Address_Tel_No_1 = custServAdd.Customer_Service_Address_Tel_No_1;
                    serv.Customer_Service_Address_Email_1 = custServAdd.Customer_Service_Address_Email_1;
                    long serviceMappingID = serv.Customers_Service_Address_Mapping_ID;
                    List<Customers_Service_Address_Tel_No_Mapping> telNosList = db.Customers_Service_Address_Tel_No_Mapping.Where(y => y.Customers_Service_Address_Mapping_ID == serviceMappingID).ToList();
                    string strServiceTelNos = String.Join(",", telNosList.Select(y => y.Customers_Service_Address_Tel_No.ToString()).ToArray());
                    serv.ServiceAddressTelNosCSV = strServiceTelNos;
                    List<Customers_Service_Address_Cell_No_Mapping> cellNosList = db.Customers_Service_Address_Cell_No_Mapping.Where(y => y.Customers_Service_Address_Mapping_ID == serviceMappingID).ToList();
                    string strServiceCellNos = String.Join(",", cellNosList.Select(y => y.Customers_Service_Address_Cell_No.ToString()).ToArray());
                    serv.ServiceAddressCellNosCSV = strServiceCellNos;
                    List<Customers_Service_Address_Email_Mapping> emailsList = db.Customers_Service_Address_Email_Mapping.Where(y => y.Customers_Service_Address_Mapping_ID == serviceMappingID).ToList();
                    string strServiceEmails = String.Join(",", emailsList.Select(y => y.Customers_Service_Address_Email.ToString()).ToArray());
                    serv.ServiceAddressEmailsCSV = strServiceEmails;
                    servicesAddressList.Add(serv);
                });

                if (noOfContracts > 0) {
                    var contractsArray = contracts.Select(x => new { x.ContractID, x.ContractNo }).ToArray();
                    //contracts.ForEach(x =>
                    //{
                    //    contractsList.Add(db.Contracts.Find(x.ContractID));
                    //});
                    return Json(new { noOfContracts, contractsArray, Lead.TypeOfPremisesID, Lead.Premises_Area_ID, Lead.PremisesAppSqFtArea, customer.GST_Type_Enum, ConsultPerson, ConsultPersonDesignation, bill, strBillingTelNos, strBillingCellNos, strBillingEmails, servicesAddressList }, JsonRequestBehavior.AllowGet);
                }
                return Json(new { noOfContracts, Lead.TypeOfPremisesID, Lead.Premises_Area_ID, Lead.PremisesAppSqFtArea, customer.GST_Type_Enum, ConsultPerson, ConsultPersonDesignation, bill, strBillingTelNos, strBillingCellNos, strBillingEmails, servicesAddressList }, JsonRequestBehavior.AllowGet);

                

            }
            else
            {

                return Json("Wrong Customer ID", JsonRequestBehavior.AllowGet);
            }

        }
        [HttpPost]
        public ActionResult GetContractDetails(string contractID)
        {
            int intContractID;

            if (!string.IsNullOrEmpty(contractID))
            {
                intContractID = Convert.ToInt32(contractID);

                //Get Contracts
                var custID = db.Contracts.Where(x => x.ContractID == intContractID).First().CustomerID;
                var ContractDate = (db.Contracts.Where(x => x.ContractID == intContractID).First().ContractDate).Value.ToString("dd/MM/yyyy");
                var customer = db.Customers.Find(custID);
                var Lead = db.Leads.Find(customer.LeadID);
                var ConsultPerson = customer.ConsultPerson;
                var ConsultPersonDesignation = customer.ConsultPersonDesignation;



                var billingAddress = db.Customers_Billing_Address_Mapping.Where(x => x.CustomerID == custID).FirstOrDefault();
                long billingAddressID = billingAddress.Customers_Billing_Address_Mapping_ID;
                string strBillingTelNos = customer.TelNumber;
                string strBillingCellNos = customer.CellNumber;
                string strBillingEmails = customer.EmailId;

                var serviceAddresses = db.Contracts_Service_Address_Mapping.Where(x => x.ContractID == intContractID).ToList();



                if (billingAddress == null)
                {
                    return Json("No Business Address Found", JsonRequestBehavior.AllowGet);
                }
                //else if (serviceAddresses.Count == 0)
                //{
                //    return Json("No Service Address Found", JsonRequestBehavior.AllowGet);
                //}
                Customers_Billing_Address_Mapping bill = new Customers_Billing_Address_Mapping();
                bill.Customers_Billing_Address_Mapping_ID = billingAddress.Customers_Billing_Address_Mapping_ID;
                bill.AddressLine1 = billingAddress.AddressLine1;
                bill.AddressLine2 = billingAddress.AddressLine2;
                bill.AddressLine3 = billingAddress.AddressLine3;
                bill.BillingLocationID = billingAddress.BillingLocationID;
                bill.BillingCityID = billingAddress.BillingCityID;
                bill.BillingPincode = billingAddress.BillingPincode;
                bill.BillingStateID = billingAddress.BillingStateID;
                bill.GSTNo = customer.GstinNo;

                List<Customers_Service_Address_Mapping> servicesAddressList = new List<Customers_Service_Address_Mapping>();
                serviceAddresses.ForEach(x =>
                {
                    //Customers_Service_Address_Mapping serv = new Customers_Service_Address_Mapping();
                    //serv.Customers_Service_Address_Mapping_ID = x.Customers_Service_Address_Mapping_ID;
                    //serv.AddressLine1 = x.AddressLine1;
                    //serv.AddressLine2 = x.AddressLine2;
                    //serv.AddressLine3 = x.AddressLine3;
                    //serv.BillingLocationID = billingAddress.BillingLocationID;
                    //serv.BillingCityID = billingAddress.BillingCityID;
                    //serv.BillingPincode = billingAddress.BillingPincode;
                    //serv.BillingStateID = billingAddress.BillingStateID;
                    var custServAdd = db.Customers_Service_Address_Mapping.Find(x.Customers_Service_Address_Mapping_ID);
                    Customers_Service_Address_Mapping serv = new Customers_Service_Address_Mapping();
                    serv.Customers_Service_Address_Mapping_ID = custServAdd.Customers_Service_Address_Mapping_ID;
                    serv.AddressLine1 = custServAdd.AddressLine1;
                    serv.AddressLine2 = custServAdd.AddressLine2;
                    serv.AddressLine3 = custServAdd.AddressLine3;
                    serv.ServiceLocationID = custServAdd.ServiceLocationID;
                    serv.ServiceCityID = custServAdd.ServiceCityID;
                    serv.ServicePincode = custServAdd.ServicePincode;
                    serv.ServiceAddressConsultPerson = custServAdd.ServiceAddressConsultPerson;
                    serv.Customer_Service_Address_Cell_No_1 = custServAdd.Customer_Service_Address_Cell_No_1;
                    serv.Customer_Service_Address_Tel_No_1 = custServAdd.Customer_Service_Address_Tel_No_1;
                    serv.Customer_Service_Address_Email_1 = custServAdd.Customer_Service_Address_Email_1;
                    long serviceMappingID = serv.Customers_Service_Address_Mapping_ID;
                    List<Customers_Service_Address_Tel_No_Mapping> telNosList = db.Customers_Service_Address_Tel_No_Mapping.Where(y => y.Customers_Service_Address_Mapping_ID == serviceMappingID).ToList();
                    string strServiceTelNos = String.Join(",", telNosList.Select(y => y.Customers_Service_Address_Tel_No.ToString()).ToArray());
                    serv.ServiceAddressTelNosCSV = strServiceTelNos;
                    List<Customers_Service_Address_Cell_No_Mapping> cellNosList = db.Customers_Service_Address_Cell_No_Mapping.Where(y => y.Customers_Service_Address_Mapping_ID == serviceMappingID).ToList();
                    string strServiceCellNos = String.Join(",", cellNosList.Select(y => y.Customers_Service_Address_Cell_No.ToString()).ToArray());
                    serv.ServiceAddressCellNosCSV = strServiceCellNos;
                    List<Customers_Service_Address_Email_Mapping> emailsList = db.Customers_Service_Address_Email_Mapping.Where(y => y.Customers_Service_Address_Mapping_ID == serviceMappingID).ToList();
                    string strServiceEmails = String.Join(",", emailsList.Select(y => y.Customers_Service_Address_Email.ToString()).ToArray());
                    serv.ServiceAddressEmailsCSV = strServiceEmails;
                    servicesAddressList.Add(serv);
                });

                return Json(new { Lead.TypeOfPremisesID, Lead.Premises_Area_ID,Lead.PremisesAppSqFtArea, customer.GST_Type_Enum ,ContractDate, ConsultPerson, ConsultPersonDesignation, bill, strBillingTelNos, strBillingCellNos, strBillingEmails, servicesAddressList }, JsonRequestBehavior.AllowGet);

            }
            else
            {

                return Json("Wrong Customer ID", JsonRequestBehavior.AllowGet);
            }

        }
        // GET: Invoices/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Invoice invoice = db.Invoices.Find(id);
            if (invoice == null)
            {
                return HttpNotFound();
            }
            EnumClassesAndHelpers.TaxInvoiceTypes InvoiceType;
            bool result = EnumClassesAndHelpers.TaxInvoiceTypes.TryParse(invoice.Tax_Invoice_Type_Enum.ToString(), out InvoiceType);
            if (result)
            {

                if (InvoiceType.Equals(EnumClassesAndHelpers.TaxInvoiceTypes.NewTaxInvoice))
                {
                    ViewBag.InvoiceType = (byte)EnumClassesAndHelpers.TaxInvoiceTypes.NewTaxInvoice;

                }
                else if (InvoiceType.Equals(EnumClassesAndHelpers.TaxInvoiceTypes.ProformaInvoice))
                {
                    ViewBag.InvoiceType = (byte)EnumClassesAndHelpers.TaxInvoiceTypes.ProformaInvoice;
                }
            }
            ViewBag.InvoiceTypeName = EnumHelper.GetEnumDescription(InvoiceType);
            ViewBag.ContractID = new SelectList(db.Contracts.Where(c => c.ContractID == invoice.ContractID), "ContractID", "ContractNo");
            //ViewBag.CustomerID = new SelectList(db.Customers, "CustomerID", "CustomerName");
            var customers = db.Customers.ToList();
            foreach (var item in customers)
            {
                if (item.CustomerName == null)
                {
                    item.CustomerName = item.CustomerNo + " - " + item.Title + " " + item.FirstName + " " + item.LastName;
                }
                else
                {
                    item.CustomerName = item.CustomerNo + " - " + item.CustomerName;
                }
            }

            ViewBag.CustomerID = new SelectList(customers.Where(c => c.CustomerID == invoice.CustomerID).OrderByDescending(c => c.CustomerID), "CustomerID", "CustomerName", invoice.CustomerID);
            ViewBag.CreatedBy = new SelectList(db.UserLogins, "UserID", "UserName");
            ViewBag.LastUpdatedBy = new SelectList(db.UserLogins, "UserID", "UserName");
            ViewBag.LocationID = new SelectList(db.Locations, "LocationID", "LocationName");
            ViewBag.CityID = new SelectList(db.Cities, "CityID", "CityName");
            ViewBag.StateID = new SelectList(db.States, "StateID", "State1");
            ViewBag.ServiceGroupID = new SelectList(db.ServiceGroups, "ServiceGroupID", "ServiceGroup1");
            ViewBag.ServiceID = new SelectList(db.Services.Where(x => x.ServiceID == 0), "ServiceID", "ServiceName");
            ViewBag.FrequencyOfServiceID = new SelectList(db.FrequencyOfServices, "FrequencyOfServiceID", "FrequencyOfService1");
            ViewBag.BranchID = new SelectList(db.Branches, "BranchID", "BranchName");
            ViewBag.PeriodsOfContract = new SelectList(db.PeriodsOfContracts, "PeriodsOfContractID", "PeriodsOfContract1");
            ViewBag.PaymentModes = new SelectList(db.PaymentModes, "PaymentModeID", "PaymentModeName");
            ViewBag.PaymentTerms = new SelectList(db.PaymentTerms, "PaymentTermID", "PaymentTermName");
            //ViewBag.TypeOfPremisesID = new SelectList(db.PremisesTypes, "PremisesTypeID", "PremisesType1");
            var enumData = from EnumClassesAndHelpers.GSTTypes g in Enum.GetValues(typeof(EnumClassesAndHelpers.GSTTypes))
                           select new
                           {
                               ID = (byte)g,
                               Name = EnumHelper.GetEnumDescription(g)
                           };

            ViewBag.GSTTypes = new SelectList(enumData, "ID", "Name");

            ViewBag.BankID = new SelectList(db.Bank_Master, "BankID", "BankName");
            ViewBag.ShortServiceScope = new SelectList(db.Short_Service_Scope_Master.Select(x => new { x.Short_Service_Scope_ID, x.Short_Service_Scope }), "Short_Service_Scope_ID", "Short_Service_Scope");
            ViewBag.FrequencyInvoice = new SelectList(db.FrequencyOfInvoices.Select(x => new { x.FrequencyOfInvoiceID, x.FrequencyOfInvoice1 }), "FrequencyOfInvoiceID", "FrequencyOfInvoice1");
            ViewBag.TypeOfPremises = new SelectList(db.PremisesTypes.Select(x => new { x.PremisesTypeID, x.PremisesType1 }), "PremisesTypeID", "PremisesType1");
            ViewBag.PremisesArea = new SelectList(db.Premises_Area_Master.Select(x => new { x.Premises_Area_ID, x.Premises_Area }), "Premises_Area_ID", "Premises_Area");

            return View(invoice);
        }

        // POST: Invoices/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Invoice invoice, byte Tax_Invoice_Type_Enum, byte? GST_Type_Enum, String[] Customers_Service_Address_Mapping_ID, String[] ServiceGroupID, String[] ServiceID, String[] FrequencyOfServiceID, int[] PeriodsOfContractID, DateTime?[] ServiceStartDate, DateTime?[] PeriodOfContractStartDate, DateTime?[] PeriodOfContractEndDate, DateTime?[] PeriodOfInvoiceStartDate, DateTime?[] PeriodOfInvoiceEndDate, String[] SACCode, String[] Qty, String[] GST, /*String[] Tax,*/ String[] Rate, String[] FinalRatePerService, int[] ShortServiceScope, String[] CGST, String[] SGST, String[] IGST, DateTime? PaymentReceivedOn, bool TDSapplicable, decimal? TDSAmount, Decimal? BadDebtsAmount, string ChequeNo, DateTime? ChequeDate, string ChequeName, int? BankID, string DraweeName, string SubmitType)
        {
            if (ModelState.IsValid)
            {

                invoice.LastUpdatedBy = Convert.ToInt32(Session["UserID"]);
                invoice.LastUpdatedDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, INDIAN_ZONE);
                db.Entry(invoice).State = EntityState.Modified;
                db.SaveChanges();

                //DELETE CURRENT SERVICE ADDS AND SERVS AND ADD NEW INSTEAD OF UPDATE
                var serviceAdds = db.Invoice_Customer_Service_Address_Mapping.Where(s => s.InvoiceID == invoice.InvoiceID);
                db.Invoice_Customer_Service_Address_Mapping.RemoveRange(serviceAdds);
                var servs = db.Invoice_Services_Mapping.Where(s => s.InvoiceID == invoice.InvoiceID);
                db.Invoice_Services_Mapping.RemoveRange(servs);
                db.SaveChanges();


                for (int i = 0; i < Customers_Service_Address_Mapping_ID.Length; i++)
                {
                    Invoice_Customer_Service_Address_Mapping servAdd = new Invoice_Customer_Service_Address_Mapping();
                    servAdd.Customers_Service_Address_Mapping_ID = Int32.Parse(Customers_Service_Address_Mapping_ID[i]);
                    servAdd.InvoiceID = invoice.InvoiceID;
                    servAdd.CustomerID = invoice.CustomerID;
                    db.Invoice_Customer_Service_Address_Mapping.Add(servAdd);
                    db.SaveChanges();
                }

                try
                {
                    for (int i = 0; i < ServiceGroupID.Length; i++)
                    {
                        int outServiceGroupID;
                        if (Int32.TryParse(ServiceGroupID[i], out outServiceGroupID))
                        {
                            Invoice_Services_Mapping servicesMapping = new Invoice_Services_Mapping();
                            servicesMapping.InvoiceID = invoice.InvoiceID;
                            servicesMapping.CustomerID = invoice.CustomerID;
                            servicesMapping.ServiceGroupID = outServiceGroupID;
                            int outServiceID;
                            if (Int32.TryParse(ServiceID[i], out outServiceID))
                                servicesMapping.ServiceID = outServiceID;
                            //else
                            //servicesMapping.ServiceID = null;

                            //if (FrequencyOfInvoiceID[i] != 0)
                            //    servicesMapping.FrequencyOfInvoiceID = FrequencyOfInvoiceID[i];
                            //else
                            //    servicesMapping.FrequencyOfInvoiceID = null;

                            //NEEDED
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
                            try
                            {
                                servicesMapping.POIStartDate = PeriodOfInvoiceStartDate[i];
                            }
                            catch (Exception e)
                            {

                            }
                            try
                            {
                                servicesMapping.POIEndDate = PeriodOfInvoiceEndDate[i];
                            }
                            catch (Exception e)
                            {

                            }


                            int outFrequencyOfServiceID;
                            if (Int32.TryParse(FrequencyOfServiceID[i], out outFrequencyOfServiceID))
                                servicesMapping.FrequencyOfServiceID = outFrequencyOfServiceID;


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

                            //decimal outCGST, outSGST, outIGST, outFinalRatePerService;
                            //if (Decimal.TryParse(CGST[i], out outCGST))
                            //    servicesMapping.CGST = outCGST;
                            //else
                            //    servicesMapping.CGST = null;

                            //if (Decimal.TryParse(SGST[i], out outSGST))
                            //    servicesMapping.SGST = outSGST;
                            //else
                            //    servicesMapping.SGST = null;

                            //if (Decimal.TryParse(IGST[i], out outIGST))
                            //    servicesMapping.IGST = outIGST;
                            //else
                            //    servicesMapping.IGST = null;

                            //if (Decimal.TryParse(FinalRatePerService[i], out outFinalRatePerService))
                            //    servicesMapping.FinalRatePerService = outFinalRatePerService;
                            //else
                            //    servicesMapping.FinalRatePerService = null;

                            //servicesMapping.FinalRatePerService = FinalRatePerService[i];
                            db.Invoice_Services_Mapping.Add(servicesMapping);
                            db.SaveChanges();
                            try
                            {
                                //CreateServicings(servicesMapping);
                            }
                            catch (Exception e)
                            {
                                string ex = e.Message;
                            }

                        }

                    }
                }
                catch (Exception e)
                {

                    string ex = e.Message;
                }
                return RedirectToAction("Index", new { type = Tax_Invoice_Type_Enum });
            }
            ViewBag.ContractID = new SelectList(db.Contracts, "ContractID", "ContractNo", invoice.ContractID);
            ViewBag.CustomerID = new SelectList(db.Customers, "CustomerID", "CustomerName", invoice.CustomerID);
            ViewBag.CreatedBy = new SelectList(db.UserLogins, "UserID", "UserName", invoice.CreatedBy);
            ViewBag.LastUpdatedBy = new SelectList(db.UserLogins, "UserID", "UserName", invoice.LastUpdatedBy);
            return View(invoice);
        }

        // GET: Invoices/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Invoice invoice = db.Invoices.Find(id);
            if (invoice == null)
            {
                return HttpNotFound();
            }
            EnumClassesAndHelpers.TaxInvoiceTypes InvoiceType;
            bool result = EnumClassesAndHelpers.TaxInvoiceTypes.TryParse(invoice.Tax_Invoice_Type_Enum.ToString(), out InvoiceType);
            if (result)
            {

                if (InvoiceType.Equals(EnumClassesAndHelpers.TaxInvoiceTypes.NewTaxInvoice))
                {
                    ViewBag.InvoiceType = (byte)EnumClassesAndHelpers.TaxInvoiceTypes.NewTaxInvoice;

                }
                else if (InvoiceType.Equals(EnumClassesAndHelpers.TaxInvoiceTypes.ProformaInvoice))
                {
                    ViewBag.InvoiceType = (byte)EnumClassesAndHelpers.TaxInvoiceTypes.ProformaInvoice;
                }
            }
            ViewBag.InvoiceTypeName = EnumHelper.GetEnumDescription(InvoiceType);
            ViewBag.ContractID = new SelectList(db.Contracts.Where(c => c.ContractID == invoice.ContractID), "ContractID", "ContractNo");
            //ViewBag.CustomerID = new SelectList(db.Customers, "CustomerID", "CustomerName");
            var customers = db.Customers.ToList();
            foreach (var item in customers)
            {
                if (item.CustomerName == null)
                {
                    item.CustomerName = item.CustomerNo + " - " + item.Title + " " + item.FirstName + " " + item.LastName;
                }
                else
                {
                    item.CustomerName = item.CustomerNo + " - " + item.CustomerName;
                }
            }

            ViewBag.CustomerID = new SelectList(customers.Where(c => c.CustomerID == invoice.CustomerID).OrderByDescending(c => c.CustomerID), "CustomerID", "CustomerName", invoice.CustomerID);
            ViewBag.CreatedBy = new SelectList(db.UserLogins, "UserID", "UserName");
            ViewBag.LastUpdatedBy = new SelectList(db.UserLogins, "UserID", "UserName");
            ViewBag.LocationID = new SelectList(db.Locations, "LocationID", "LocationName");
            ViewBag.CityID = new SelectList(db.Cities, "CityID", "CityName");
            ViewBag.StateID = new SelectList(db.States, "StateID", "State1");
            ViewBag.ServiceGroupID = new SelectList(db.ServiceGroups, "ServiceGroupID", "ServiceGroup1");
            ViewBag.ServiceID = new SelectList(db.Services.Where(x => x.ServiceID == 0), "ServiceID", "ServiceName");
            ViewBag.FrequencyOfServiceID = new SelectList(db.FrequencyOfServices, "FrequencyOfServiceID", "FrequencyOfService1");
            ViewBag.BranchID = new SelectList(db.Branches, "BranchID", "BranchName");
            ViewBag.PeriodsOfContract = new SelectList(db.PeriodsOfContracts, "PeriodsOfContractID", "PeriodsOfContract1");
            ViewBag.PaymentModes = new SelectList(db.PaymentModes, "PaymentModeID", "PaymentModeName");
            ViewBag.PaymentTerms = new SelectList(db.PaymentTerms, "PaymentTermID", "PaymentTermName");
            //ViewBag.TypeOfPremisesID = new SelectList(db.PremisesTypes, "PremisesTypeID", "PremisesType1");
            var enumData = from EnumClassesAndHelpers.GSTTypes g in Enum.GetValues(typeof(EnumClassesAndHelpers.GSTTypes))
                           select new
                           {
                               ID = (byte)g,
                               Name = EnumHelper.GetEnumDescription(g)
                           };

            ViewBag.GSTTypes = new SelectList(enumData, "ID", "Name");

            ViewBag.BankID = new SelectList(db.Bank_Master, "BankID", "BankName");
            ViewBag.ShortServiceScope = new SelectList(db.Short_Service_Scope_Master.Select(x => new { x.Short_Service_Scope_ID, x.Short_Service_Scope }), "Short_Service_Scope_ID", "Short_Service_Scope");
            ViewBag.FrequencyInvoice = new SelectList(db.FrequencyOfInvoices.Select(x => new { x.FrequencyOfInvoiceID, x.FrequencyOfInvoice1 }), "FrequencyOfInvoiceID", "FrequencyOfInvoice1");
            ViewBag.TypeOfPremises = new SelectList(db.PremisesTypes.Select(x => new { x.PremisesTypeID, x.PremisesType1 }), "PremisesTypeID", "PremisesType1");
            ViewBag.PremisesArea = new SelectList(db.Premises_Area_Master.Select(x => new { x.Premises_Area_ID, x.Premises_Area }), "Premises_Area_ID", "Premises_Area");

            return View(invoice);
        }

        // POST: Invoices/Delete/5
        [HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)

        {
            Invoice invoice = db.Invoices.Find(id);
            byte taxInvoiceType = invoice.Tax_Invoice_Type_Enum;
            var serviceAdds = db.Invoice_Customer_Service_Address_Mapping.Where(s => s.InvoiceID == id);
            db.Invoice_Customer_Service_Address_Mapping.RemoveRange(serviceAdds);
            var servs = db.Invoice_Services_Mapping.Where(s => s.InvoiceID == id);
            db.Invoice_Services_Mapping.RemoveRange(servs);
            var collections = db.Collection_Entry.Where(c => c.InvoiceID == id);
            db.Collection_Entry.RemoveRange(collections);
            db.Invoices.Remove(invoice);
            db.SaveChanges();
            return RedirectToAction("Index", new { type = taxInvoiceType });
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        public ActionResult GetInvoiceServicesMapped(string InvoiceID)
        {
            int intInvoiceID;

            if (!string.IsNullOrEmpty(InvoiceID))
            {
                intInvoiceID = Convert.ToInt32(InvoiceID);
                string billmsg = "", servmsg = "";

                Invoice invoice = db.Invoices.Find(intInvoiceID);
                Customer customer = db.Customers.Find(invoice.CustomerID);

                var invoiceServices = invoice.Invoice_Services_Mapping.ToList();


                if (invoiceServices.Count == 0)
                {
                    return Json("No Services Found", JsonRequestBehavior.AllowGet);
                }
                List<Invoice_Services_Mapping> servicesList = new List<Invoice_Services_Mapping>();
                invoiceServices.ForEach(x =>
                {
                    servicesList.Add(new Invoice_Services_Mapping
                    {
                        Invoice_Services_Mapping_ID = x.Invoice_Services_Mapping_ID,
                        //ContractID = x.ContractID,
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
                        POCEndDateInString = x.POCEndDate.ToString() == "" ? "" : Convert.ToDateTime(x.POCEndDate).ToString("dd/MM/yyyy"),
                        POIStartDateInString = x.POIStartDate.ToString() == "" ? "" : Convert.ToDateTime(x.POIStartDate).ToString("dd/MM/yyyy"),
                        POIEndDateInString = x.POIEndDate.ToString() == "" ? "" : Convert.ToDateTime(x.POIEndDate).ToString("dd/MM/yyyy")
                    });
                });
                return Json(servicesList, JsonRequestBehavior.AllowGet);
            }
            else
            {

                return Json("Wrong Contract ID", JsonRequestBehavior.AllowGet);
            }

        }

        [HttpPost]
        public ActionResult CalculateInvoiceDates(string POIStartDate)
        {
            //string POCEndDate;
            //using (SadguruCRMEntities entities = new SadguruCRMEntities())
            //{
            //    //int intFOS = Int32.Parse(FOS);
            //    //FrequencyOfService frequency = db.FrequencyOfServices.Find(intFOS);
            //    int intPOC = Int32.Parse(POC);
            //    PeriodsOfContract period = db.PeriodsOfContracts.Find(intPOC);

            //    DateTime? dtServiceStartDate;
            //    DateTime? dtPOCStartDate, dtPOCEndDate;
            //    try
            //    {
            //        dtServiceStartDate = DateTime.ParseExact(serviceStartDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            //    }
            //    catch (Exception ex)
            //    {
            //        dtServiceStartDate = null;
            //    }


            //    if (POCStartDate != "")
            //    {
            //        dtPOCStartDate = DateTime.ParseExact(POCStartDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            //    }
            //    else
            //    {
            //        if (dtServiceStartDate != null)
            //        {
            //            dtPOCStartDate = (DateTime)dtServiceStartDate;
            //            POCStartDate = serviceStartDate;
            //        }
            //        else
            //        {
            //            dtPOCStartDate = null;
            //            POCStartDate = "";
            //        }

            //    }
            //    if (dtPOCStartDate != null)
            //    {
            //        dtPOCEndDate = ((DateTime)dtPOCStartDate).AddYears((int)period.Years).AddMonths((int)period.Months).AddDays((int)period.Days).AddDays(-1);
            //        POCEndDate = ((DateTime)dtPOCEndDate).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
            //    }
            //    else
            //    {
            //        dtPOCEndDate = null;
            //        POCEndDate = "";
            //    }
            //    //leadService.FrequencyOfServiceID = map.FrequencyOfServiceID;
            //    //leadService.Rate = map.Rate;
            //    //leadService.GST = map.GST;
            //    //leadService.Qty = map.Qty;
            //    //leadService.Tax = map.Tax;
            //    //leadService.FinalRatePerService = map.FinalRatePerService;
            //    //leadService.PeriodsOfContractID = map.PeriodsOfContractID;
            //    //leadService.ServiceStartDate = map.ServiceStartDate;
            //    //leadService.POCStartDate = map.POCStartDate;
            //    //leadService.POCEndDate = map.POCEndDate;

            //    //entities.SaveChanges();
            //}

            //return Json(new { POCStartDate, POCEndDate }, JsonRequestBehavior.AllowGet);
            return Json(null , JsonRequestBehavior.AllowGet);
        }
    }

}

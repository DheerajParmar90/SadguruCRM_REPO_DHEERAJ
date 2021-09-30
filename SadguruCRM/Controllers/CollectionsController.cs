using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using SadguruCRM.EnumClassesAndHelpers;
using SadguruCRM.Helpers;
using SadguruCRM.Models;
using SadguruCRM.ViewModels;

namespace SadguruCRM.Controllers

{
    [VerifyUser]
    public class CollectionsController : Controller
    {
        private SadguruCRMEntities db = new SadguruCRMEntities();
        private static TimeZoneInfo INDIAN_ZONE = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");

        // GET: Collections
        public ActionResult Index()
        {
            var collection_Entry = db.Collection_Entry.Include(c => c.Bank_Master).Include(c => c.Invoice).Include(c => c.Lead).Include(c => c.PaymentMode);
            foreach (var item in collection_Entry)
            {
                if (item.Invoice.Customer.CustomerName == null)
                {
                    item.Invoice.Customer.CustomerName = item.Invoice.Customer.Title + " " + item.Invoice.Customer.FirstName + " " + item.Invoice.Customer.LastName;
                }
                item.Invoice.BalanceAmount = (db.Collection_Entry.Where(t => t.InvoiceID == item.Invoice.InvoiceID).Sum(i => i.Amount)).ToString();
            }

            ViewBag.BankID = new SelectList(db.Bank_Master, "BankID", "BankName");
            ViewBag.PaymentModes = new SelectList(db.PaymentModes, "PaymentModeID", "PaymentModeName");
            return View(collection_Entry.ToList());
        }
        // GET: Collections
        public ActionResult WithGSTCollectionsOld()
        {
            List<WithGSTCollectionViewModel> vm = new List<WithGSTCollectionViewModel>();
            
            foreach (Invoice invoice in db.Invoices) {
                vm.Add(new WithGSTCollectionViewModel
                {
                    singleInvoice = invoice,
                    listCollectionEntries = db.Collection_Entry.Where( o => o.InvoiceID == invoice.InvoiceID).Include(c => c.Bank_Master).Include(c => c.Invoice).Include(c => c.Lead).Include(c => c.PaymentMode).ToList()
                });
            }
                //var collection_Entry = db.Collection_Entry.Include(c => c.Bank_Master).Include(c => c.Invoice).Include(c => c.Lead).Include(c => c.PaymentMode);
            foreach (var item in vm)
            {
                if (item.singleInvoice.Customer.CustomerName == null)
                {
                    item.singleInvoice.Customer.CustomerName = item.singleInvoice.Customer.Title + " " + item.singleInvoice.Customer.FirstName + " " + item.singleInvoice.Customer.LastName;
                }
                //item.singleInvoice.BalanceAmount = (db.Collection_Entry.Where(t => t.InvoiceID == item.singleInvoice.InvoiceID).Sum(i => i.Amount)).ToString();
                item.singleInvoice.BalanceAmount = item.listCollectionEntries.Sum(i => i.Amount).ToString();
            }

            ViewBag.BankID = new SelectList(db.Bank_Master, "BankID", "BankName");
            ViewBag.PaymentModes = new SelectList(db.PaymentModes, "PaymentModeID", "PaymentModeName");
            return View(vm);
        }
        public ActionResult WithGSTCollections()
        {
            //List<WithGSTCollectionViewModel> vm = new List<WithGSTCollectionViewModel>();
            var listCollectionsWithGST = db.Collection_Entry.Where(o => o.LeadID == null || o.isAdvanceEntry == true).Include(c => c.Bank_Master).Include(c => c.Invoice).Include(c => c.Lead).Include(c => c.Customer).Include(c => c.PaymentMode).ToList();

            //foreach (Invoice invoice in db.Invoices)
            //{
            //    vm.Add(new WithGSTCollectionViewModel
            //    {
            //        singleInvoice = invoice,
            //        listCollectionEntries = db.Collection_Entry.Where(o => o.InvoiceID == invoice.InvoiceID).Include(c => c.Bank_Master).Include(c => c.Invoice).Include(c => c.Lead).Include(c => c.PaymentMode).ToList()
            //    });
            //}
            //var collection_Entry = db.Collection_Entry.Include(c => c.Bank_Master).Include(c => c.Invoice).Include(c => c.Lead).Include(c => c.PaymentMode);
            foreach (var item in listCollectionsWithGST)
            {
                if (item.Invoice != null)
                {
                    if (item.Invoice.Customer.CustomerName == null)
                    {
                        item.Invoice.Customer.CustomerName = item.Invoice.Customer.Title + " " + item.Invoice.Customer.FirstName + " " + item.Invoice.Customer.LastName;
                    }
                    //item.singleInvoice.BalanceAmount = (db.Collection_Entry.Where(t => t.InvoiceID == item.singleInvoice.InvoiceID).Sum(i => i.Amount)).ToString();

                    item.Invoice.AlreadyPaidSum = item.Invoice.Collection_Entry.Sum(i => i.Amount);
                    item.Invoice.BalanceAmount = (item.Invoice.FinalInvoiceRate - item.Invoice.Collection_Entry.Sum(i => i.Amount)).ToString();

                    // it is actually Final rate without Tax i.e. Basic Amount
                    item.Invoice.ExecutiveName = item.Invoice.Invoice_Services_Mapping.Sum(i => i.Rate * (i.Qty == null? 1 : i.Qty)).ToString();
                    //item.Invoice.SGSTSumOfAllServices = item.Invoice.Invoice_Services_Mapping.Sum(i => i.SGST);
                    //item.Invoice.CGSTSumOfAllServices = item.Invoice.Invoice_Services_Mapping.Sum(i => i.CGST);
                    //item.Invoice.IGSTSumOfAllServices = item.Invoice.Invoice_Services_Mapping.Sum(i => i.IGST);
                    if (item.Invoice.Customer.GST_Type_Enum == null)
                    {

                        item.Invoice.SGSTSumOfAllServices = 0;
                        item.Invoice.CGSTSumOfAllServices = 0;
                        item.Invoice.IGSTSumOfAllServices = 0;
                    }
                    else if (item.Invoice.Customer.GST_Type_Enum == (byte)EnumClassesAndHelpers.GSTTypes.V1)
                    {
                        item.Invoice.SGSTSumOfAllServices = item.Invoice.Invoice_Services_Mapping.Sum(i => i.Rate * i.Qty * 9 / 100);
                        item.Invoice.CGSTSumOfAllServices = item.Invoice.Invoice_Services_Mapping.Sum(i => i.Rate * i.Qty * 9 / 100);
                        item.Invoice.IGSTSumOfAllServices = 0;
                    }
                    if (item.Invoice.Customer.GST_Type_Enum == (byte)EnumClassesAndHelpers.GSTTypes.V2)
                    {
                        item.Invoice.SGSTSumOfAllServices = 0;
                        item.Invoice.CGSTSumOfAllServices = 0;
                        item.Invoice.IGSTSumOfAllServices = item.Invoice.Invoice_Services_Mapping.Sum(i => i.Rate * i.Qty * 18 / 100); ;
                    }
                }
                else if (item.Customer != null) {
                    item.Invoice = new Invoice();
                    //item.Invoice.Customer = item.Customer;

                    item.Invoice.Customer = new Customer();
                    //var tempCust = item.Customer.
                    //to avoid appending customer name again n again which results in customer name double
                    //if (item.Customer.CustomerName != null && !item.Customer.CustomerName.ToString().Contains(item.Customer.Title))
                    //{
                    if (item.Customer.CustomerName != null)
                    {
                        if (item.Customer.CustomerName.Contains(item.Customer.Title))
                        {
                            //This means This Customer Name has alrready been appended
                            item.Invoice.Customer.CustomerName = item.Customer.CustomerName;

                            //item.Customer.CustomerName = item.Customer.CustomerName.Replace(item.Customer.Title, "");
                            //if (item.Customer.FirstName != null)
                            //    item.Customer.CustomerName = item.Customer.CustomerName.Replace(item.Customer.FirstName, "");
                            //if (item.Customer.LastName != null)
                            //    item.Customer.CustomerName = item.Customer.CustomerName.Replace(item.Customer.LastName, "");
                        }
                        else
                        {
                            item.Invoice.Customer.CustomerName = item.Customer.Title + " " + item.Customer.FirstName + " " + item.Customer.LastName + " " + item.Customer.CustomerName;
                        }

                    }
                    else
                    {
                        item.Invoice.Customer.CustomerName = item.Customer.Title + " " + item.Customer.FirstName + " " + item.Customer.LastName + " " + item.Customer.CustomerName;
                    }
                        //item.Invoice.Customer.CustomerName = item.Customer.Title + " " + item.Customer.FirstName + " " + item.Customer.LastName + " " + item.Customer.CustomerName;
                    //}
                    //else {
                    //    item.Invoice.Customer.CustomerName = item.Customer.CustomerName;
                    //}

                    item.Invoice.Customer.Lead = new Lead();
                    try {
                        var firstOrDefault = item.Customer.Collection_Entry.Where(c => c.InvoiceID == null && c.isAdvanceEntry == true).OrderByDescending(c => c.Collection_Entry_ID).FirstOrDefault();
                        //item.Invoice.FinalInvoiceRate = item.Customer.Collection_Entry.Where(c => c.InvoiceID == null && c.isAdvanceEntry == true).OrderByDescending(c => c.Collection_Entry_ID).FirstOrDefault().TotalAmountToUseOnlyForCustomer;
                        //item.Invoice.FinalInvoiceRate = item.Customer.Collection_Entry.Where(c => c.InvoiceID == null && c.isAdvanceEntry == true).OrderByDescending(c => c.Collection_Entry_ID).DefaultIfEmpty().First().TotalAmountToUseOnlyForCustomer;
                        if (firstOrDefault != null) {
                            item.Invoice.FinalInvoiceRate = firstOrDefault.TotalAmountToUseOnlyForCustomer;
                        }
                        else {
                            item.Invoice.FinalInvoiceRate = null;
                        }
                    }
                    catch (Exception ex) {
                        string message = ex.Message;
                    }
                    item.Invoice.AlreadyPaidSum = item.Customer.Collection_Entry.Where(c => c.InvoiceID == null && c.isAdvanceEntry == true).Sum(i => i.Amount);
                    //item.Invoice.BalanceAmount = (item.Invoice.FinalInvoiceRate - item.Customer.Collection_Entry.Where(c => c.InvoiceID == null && c.isAdvanceEntry == true).Sum(i => i.Amount)).ToString();
                    item.Invoice.BalanceAmount = (item.Invoice.FinalInvoiceRate - item.Invoice.AlreadyPaidSum).ToString();

                    item.Invoice.Customer.Lead.Location = new Location();
                    if (item.Customer.Lead != null)
                    {
                        item.Invoice.Customer.Lead.Location.LocationName = item.Customer.Lead.Location.LocationName;
                    }
                    item.Invoice.Customer.TelNumber = item.Customer.TelNumber;
                    item.Invoice.Customer.CellNumber = item.Customer.CellNumber;

                    //item.Invoice.AlreadyPaidSum = null;
                    //item.Invoice.BalanceAmount = "";

                    // it is actually Final rate without Tax i.e. Basic Amount
                    //item.Invoice.ExecutiveName = item.Lead.Lead_Services_Mapping.Sum(i => i.Rate * i.Qty).ToString();
                    //item.Invoice.SGSTSumOfAllServices = item.Lead.Lead_Services_Mapping.Sum(i => i.SGST);
                    //item.Invoice.CGSTSumOfAllServices = item.Lead.Lead_Services_Mapping.Sum(i => i.CGST);
                    //item.Invoice.IGSTSumOfAllServices = item.Lead.Lead_Services_Mapping.Sum(i => i.IGST);
                }
                else if (item.Lead != null)
                {
                    item.Invoice = new Invoice();
                    item.Invoice.Customer = new Customer();
                    item.Invoice.Customer.Lead = item.Lead;

                    item.Invoice.Customer.CustomerName = item.Lead.Title + " " + item.Lead.FirstName + " " + item.Lead.LastName + " " + item.Lead.CustomerName;

                    //item.Customer = new Customer();
                    item.Invoice.Customer.TelNumber = item.Lead.TelNo;
                    item.Invoice.Customer.CellNumber = item.Lead.CellNo;

                    //item.singleInvoice.BalanceAmount = (db.Collection_Entry.Where(t => t.InvoiceID == item.singleInvoice.InvoiceID).Sum(i => i.Amount)).ToString();
                    item.Invoice.FinalInvoiceRate = Decimal.Parse(item.Lead.FinalRate);

                    item.Invoice.AlreadyPaidSum = item.Lead.Collection_Entry.Sum(i => i.Amount);
                    if (!String.IsNullOrEmpty(item.Lead.FinalRate))
                    {
                        item.Invoice.BalanceAmount = (Decimal.Parse(item.Lead.FinalRate) - item.Lead.Collection_Entry.Sum(i => i.Amount)).ToString();
                    }
                    else
                    {
                        item.Invoice.BalanceAmount = "";
                    }
                    // IMP will be used in future. Do Not Delete
                    // it is actually Final rate without Tax i.e. Basic Amount
                    //item.Invoice.ExecutiveName = item.Lead.Lead_Services_Mapping.Sum(i => i.Rate * i.Qty).ToString();
                    //item.Invoice.SGSTSumOfAllServices = item.Lead.Lead_Services_Mapping.Sum(i => i.SGST);
                    //item.Invoice.CGSTSumOfAllServices = item.Lead.Lead_Services_Mapping.Sum(i => i.CGST);
                    //item.Invoice.IGSTSumOfAllServices = item.Lead.Lead_Services_Mapping.Sum(i => i.IGST);
                }
                
            }
            //ViewBag.BasicAmount = db.Invoice_Services_Mapping.Sum(i => i.Rate * i.Qty);
            List<SelectListItem> SearchOptionsList = new List<SelectListItem>();

            var serviceGroups = db.ServiceGroups.OrderBy(i => i.Order).ToList();
            SelectListGroup selectListGroup;
            int groupID;

            for (int i = 0; i < serviceGroups.Count(); i++)
            {

                selectListGroup = new SelectListGroup { Name = serviceGroups.ElementAt(i).ServiceGroup1 };
                groupID = serviceGroups.ElementAt(i).ServiceGroupID;
                var services = db.Services.Where(o => o.ServiceGroupID == groupID).OrderBy(o => o.Order).ToList();
                for (int j = 0; j < services.Count(); j++)
                {
                    SearchOptionsList.Add(new SelectListItem() { Text = services.ElementAt(j).ServiceName, Value = "ServiceID_" + services.ElementAt(j).ServiceID, Group = selectListGroup });
                }
            }

            selectListGroup = new SelectListGroup { Name = "Payment Status" };
            SearchOptionsList.Add(new SelectListItem() { Text = "Paid", Value = "PaymentStatusID_Paid", Group = selectListGroup });
            SearchOptionsList.Add(new SelectListItem() { Text = "Unpaid / Balance", Value = "PaymentStatusID_Unpaid", Group = selectListGroup });

            selectListGroup = new SelectListGroup { Name = "Payment Mode" };
            var paymentModes = db.PaymentModes.OrderBy(o => o.Order).ToList();
            for (int i = 0; i < paymentModes.Count(); i++)
            {
                SearchOptionsList.Add(new SelectListItem() { Text = paymentModes.ElementAt(i).PaymentModeName, Value = "PaymentModeID_" + paymentModes.ElementAt(i).PaymentModeID.ToString(), Group = selectListGroup });
            }

            selectListGroup = new SelectListGroup { Name = "Branch" };
            var branches = db.Branches.OrderBy(o => o.Order).ToList();
            for (int i = 0; i < branches.Count(); i++)
            {
                SearchOptionsList.Add(new SelectListItem() { Text = branches.ElementAt(i).BranchName, Value = "BranchID_" + branches.ElementAt(i).BranchID.ToString(), Group = selectListGroup });
            }


            selectListGroup = new SelectListGroup { Name = "Other" };
            SearchOptionsList.Add(new SelectListItem() { Text = "TDS Amount", Value = "TDSAmount", Group = selectListGroup });
            SearchOptionsList.Add(new SelectListItem() { Text = "Bad Debts Amount", Value = "BadDebtsAmount", Group = selectListGroup });

            var dropdownList = new SelectList(SearchOptionsList.Select(item => new SelectListItem
            {
                Text = item.Text,
                Value = item.Value,
                // Assign the Group to the item by some appropriate selection method
                Group = item.Group
            }).ToList(), "Value", "Text", "Group.Name", -1);
            ViewBag.SearchOptions = dropdownList;

            ViewBag.BankID = new SelectList(db.Bank_Master, "BankID", "BankName");
            ViewBag.PaymentModes = new SelectList(db.PaymentModes, "PaymentModeID", "PaymentModeName");
            return View(listCollectionsWithGST);
        }
        public ActionResult WithGSTCollectionsSearch(FormCollection formCollection, string[] SearchBy)
        {
            //List<WithGSTCollectionViewModel> vm = new List<WithGSTCollectionViewModel>();
            try
            {
                ViewBag.SearchBy = SearchBy;
                DateTime fromDate, toDate;
                int month = 0, year = 0;
                if (String.IsNullOrEmpty(formCollection["FromDate"].ToString()))
                {
                    fromDate = DateTime.Parse("01/01/1990");
                }
                else
                {
                    fromDate = DateTime.Parse(formCollection["FromDate"]);
                }
                if (String.IsNullOrEmpty(formCollection["ToDate"].ToString()))
                {
                    toDate = DateTime.Parse("01/01/2099");
                }
                else
                {
                    toDate = DateTime.Parse(formCollection["ToDate"]);
                }
                if (!String.IsNullOrEmpty(formCollection["SelectMonth"].ToString()) && !String.IsNullOrEmpty(formCollection["SelectYear"].ToString()))
                {
                    month = Int32.Parse(formCollection["SelectMonth"]);
                    year = Int32.Parse(formCollection["SelectYear"]);
                }

                var listCollectionsWithGST = db.Collection_Entry
                    .Where(o => o.LeadID == null)
                    .Where(o => o.Invoice.InvoiceDate > fromDate)
                    .Where(o => o.Invoice.InvoiceDate < toDate)
                    .Include(c => c.Bank_Master)
                    .Include(c => c.Invoice)
                    .Include(c => c.Lead)
                    .Include(c => c.PaymentMode)
                    .ToList();
                if (month > 0 && year > 0)
                {
                    listCollectionsWithGST = listCollectionsWithGST.Where(i => i.Invoice.InvoiceDate.Value.Month == month).Where(i => i.Invoice.InvoiceDate.Value.Year == year).ToList();
                }

                List<int> servicesSelected = new List<int>();
                //var servicesSelected = SearchBy.Where(i => i.Contains("ServiceID_")).ToArray();
                foreach (var item in SearchBy.Where(i => i.Contains("ServiceID_")).ToArray()) {
                    servicesSelected.Add(Int32.Parse(item.Replace("ServiceID_", "")));
                }
                List<int?> paymemtModesSelected = new List<int?>();
                foreach (var item in SearchBy.Where(i => i.Contains("PaymentModeID_")).ToArray())
                {
                    paymemtModesSelected.Add(Int32.Parse(item.Replace("PaymentModeID_", "")));
                }
                List<int> branchesSelected = new List<int>();
                foreach (var item in SearchBy.Where(i => i.Contains("BranchID_")).ToArray())
                {
                    branchesSelected.Add(Int32.Parse(item.Replace("BranchID_", "")));
                }
                if (servicesSelected.Count() > 0)
                {
                    //listCollectionsWithGST = listCollectionsWithGST.Where(i => i.Invoice.Invoice_Services_Mapping.Select(o => o.ServiceID).Contains(p => p.)))
                    List<int?> invoiceIDs = listCollectionsWithGST.Select(o => o.InvoiceID).Distinct().ToList();
                    List<int> invoiceServices;
                    foreach (var item in invoiceIDs)
                    {
                        invoiceServices = db.Invoice_Services_Mapping.Where(o => o.InvoiceID == item.Value).Select(o => o.ServiceID).ToList();
                        if (!invoiceServices.Any(i => servicesSelected.Contains(i)))
                        {
                            listCollectionsWithGST.RemoveAll(i => i.InvoiceID == item);
                        }
                    }
                }


                if (paymemtModesSelected.Count() > 0)
                {
                    //var tempList = listCollectionsWithGST;
                    foreach (var item in listCollectionsWithGST.ToList())
                    {

                        if (!paymemtModesSelected.Contains(item.PaymentModeID))
                        {
                            listCollectionsWithGST.Remove(item);
                        }
                    }
                }
                
                if (branchesSelected.Count() > 0)
                {
                    //var tempList = listCollectionsWithGST;
                    foreach (var item in listCollectionsWithGST.Select(o => o.Invoice).ToList())
                    {

                        if (!branchesSelected.Contains(item.Customer.BranchID))
                        {
                            listCollectionsWithGST.RemoveAll(i => i.InvoiceID == item.InvoiceID);
                        }
                    }
                }



                //foreach (Invoice invoice in db.Invoices)
                //{
                //    vm.Add(new WithGSTCollectionViewModel
                //    {
                //        singleInvoice = invoice,
                //        listCollectionEntries = db.Collection_Entry.Where(o => o.InvoiceID == invoice.InvoiceID).Include(c => c.Bank_Master).Include(c => c.Invoice).Include(c => c.Lead).Include(c => c.PaymentMode).ToList()
                //    });
                //}
                //var collection_Entry = db.Collection_Entry.Include(c => c.Bank_Master).Include(c => c.Invoice).Include(c => c.Lead).Include(c => c.PaymentMode);
                foreach (var item in listCollectionsWithGST)
                {
                    if (item.Invoice.Customer.CustomerName == null)
                    {
                        item.Invoice.Customer.CustomerName = item.Invoice.Customer.Title + " " + item.Invoice.Customer.FirstName + " " + item.Invoice.Customer.LastName;
                    }
                    //item.singleInvoice.BalanceAmount = (db.Collection_Entry.Where(t => t.InvoiceID == item.singleInvoice.InvoiceID).Sum(i => i.Amount)).ToString();

                    item.Invoice.AlreadyPaidSum = item.Invoice.Collection_Entry.Sum(i => i.Amount);
                    item.Invoice.BalanceAmount = (item.Invoice.FinalInvoiceRate - item.Invoice.Collection_Entry.Sum(i => i.Amount)).ToString();

                    // it is actually Final rate without Tax i.e. Basic Amount
                    item.Invoice.ExecutiveName = item.Invoice.Invoice_Services_Mapping.Sum(i => i.Rate * i.Qty).ToString();
                    item.Invoice.SGSTSumOfAllServices = item.Invoice.Invoice_Services_Mapping.Sum(i => i.SGST);
                    item.Invoice.CGSTSumOfAllServices = item.Invoice.Invoice_Services_Mapping.Sum(i => i.CGST);
                    item.Invoice.IGSTSumOfAllServices = item.Invoice.Invoice_Services_Mapping.Sum(i => i.IGST);

                    
                }
                //ViewBag.BasicAmount = db.Invoice_Services_Mapping.Sum(i => i.Rate * i.Qty);
                List<SelectListItem> SearchOptionsList = new List<SelectListItem>();

                var serviceGroups = db.ServiceGroups.OrderBy(i => i.Order).ToList();
                SelectListGroup selectListGroup;
                int groupID;

                for (int i = 0; i < serviceGroups.Count(); i++)
                {

                    selectListGroup = new SelectListGroup { Name = serviceGroups.ElementAt(i).ServiceGroup1 };
                    groupID = serviceGroups.ElementAt(i).ServiceGroupID;
                    var services = db.Services.Where(o => o.ServiceGroupID == groupID).OrderBy(o => o.Order).ToList();
                    for (int j = 0; j < services.Count(); j++)
                    {
                        SearchOptionsList.Add(new SelectListItem() { Text = services.ElementAt(j).ServiceName, Value = "ServiceID_" + services.ElementAt(j).ServiceID, Group = selectListGroup, Selected = servicesSelected.Contains(services.ElementAt(j).ServiceID) ? true : false });
                    }
                }

                selectListGroup = new SelectListGroup { Name = "Payment Status" };
                SearchOptionsList.Add(new SelectListItem() { Text = "Paid", Value = "PaymentStatusID_Paid", Group = selectListGroup });
                SearchOptionsList.Add(new SelectListItem() { Text = "Unpaid / Balance", Value = "PaymentStatusID_Unpaid", Group = selectListGroup });

                selectListGroup = new SelectListGroup { Name = "Payment Mode" };
                var paymentModes = db.PaymentModes.OrderBy(o => o.Order).ToList();
                for (int i = 0; i < paymentModes.Count(); i++)
                {
                    SearchOptionsList.Add(new SelectListItem() { Text = paymentModes.ElementAt(i).PaymentModeName, Value = "PaymentModeID_"+paymentModes.ElementAt(i).PaymentModeID.ToString(), Group = selectListGroup });
                }

                selectListGroup = new SelectListGroup { Name = "Branch" };
                var branches = db.Branches.OrderBy(o => o.Order).ToList();
                for (int i = 0; i < branches.Count(); i++)
                {
                    SearchOptionsList.Add(new SelectListItem() { Text = branches.ElementAt(i).BranchName, Value = "BranchID_"+branches.ElementAt(i).BranchID.ToString(), Group = selectListGroup });
                }


                selectListGroup = new SelectListGroup { Name = "Other" };
                SearchOptionsList.Add(new SelectListItem() { Text = "TDS Amount", Value = "TDSAmount", Group = selectListGroup });
                SearchOptionsList.Add(new SelectListItem() { Text = "Bad Debts Amount", Value = "BadDebtsAmount", Group = selectListGroup });

                var dropdownList = new SelectList(SearchOptionsList.Select(item => new SelectListItem
                {
                    Text = item.Text,
                    Value = item.Value,
                    // Assign the Group to the item by some appropriate selection method
                    Group = item.Group
                }).ToList(), "Value", "Text", "Group.Name", -1);
                ViewBag.SearchOptions = dropdownList;

                ViewBag.BankID = new SelectList(db.Bank_Master, "BankID", "BankName");
                ViewBag.PaymentModes = new SelectList(db.PaymentModes, "PaymentModeID", "PaymentModeName");
                return View("WithGSTCollections", listCollectionsWithGST);
            }
            catch (Exception ex) {
                return RedirectToAction("WithGSTCollections");
            }
            
            
        }
        [HttpPost]
        public ContentResult AddCollectionWithGST(FormCollection formCollection)
        {
            string message = "";
            try
            {
                Collection_Entry collection_Entry = new Collection_Entry();
                if (formCollection["InvoiceID"] != "") {
                    collection_Entry.InvoiceID = int.Parse(formCollection["InvoiceID"]);
                }
                if (formCollection["CustomerID"] != "")
                {
                    collection_Entry.CustomerID = int.Parse(formCollection["CustomerID"]);
                }
                if (formCollection["LeadID"] != "")
                {
                    collection_Entry.LeadID = int.Parse(formCollection["LeadID"]);
                }
                collection_Entry.Amount = decimal.Parse(formCollection["PaidByCustomer"]);
                collection_Entry.ReceivedOn = DateTime.Parse(formCollection["PaymentReceivedOn"]);
                collection_Entry.TDSapplicable = bool.Parse(formCollection["TDSapplicable"]);
                collection_Entry.TDSAmount = decimal.Parse(formCollection["TDSAmount"]);
                collection_Entry.BadDebtsAmount = decimal.Parse(formCollection["BadDebtsAmount"]);
                collection_Entry.PaymentModeID = int.Parse(formCollection["PaymentModeID"]);
                collection_Entry.ChequeNo = formCollection["ChequeNo"];
                
                collection_Entry.ChequeName = formCollection["ChequeName"];
                if (!String.IsNullOrEmpty(formCollection["ChequeDate"]))
                {
                    collection_Entry.ChequeDate = DateTime.Parse(formCollection["ChequeDate"]);
                }
                if (formCollection["BankID"].ToString() != "")
                {
                    collection_Entry.BankID = int.Parse(formCollection["BankID"]);
                }
                collection_Entry.DraweeName = formCollection["DraweeName"];

                var isPartPayment = formCollection["isPartPayment"];
                if (isPartPayment == "true")
                {
                    collection_Entry.isPartPayment = true;
                }
                else {
                    collection_Entry.isPartPayment = false;
                }

                collection_Entry.CreatedBy = Convert.ToInt32(Session["UserID"]);
                collection_Entry.CreatedOn = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, INDIAN_ZONE);
                db.Collection_Entry.Add(collection_Entry);
                db.SaveChanges();
                message = "Record Saved Successfully!";

                //string message = "PersonId: " + personId;
                //message += "\nName: " + name;
                //message += "\nGender: " + gender;
                //message += "\nCity: " + city;
            }
            catch (Exception ex) {
                message = ex.Message;
            }



            return Content(message);
        }

        public ActionResult WithoutGSTCollections()
        {
            //List<WithGSTCollectionViewModel> vm = new List<WithGSTCollectionViewModel>();
            var listCollectionsWithoutGST = db.Collection_Entry.Where(o => o.LeadID != null).Include(c => c.Bank_Master).Include(c => c.Invoice).Include(c => c.Lead).Include(c => c.PaymentMode).ToList();

            //foreach (Invoice invoice in db.Invoices)
            //{
            //    vm.Add(new WithGSTCollectionViewModel
            //    {
            //        singleInvoice = invoice,
            //        listCollectionEntries = db.Collection_Entry.Where(o => o.InvoiceID == invoice.InvoiceID).Include(c => c.Bank_Master).Include(c => c.Invoice).Include(c => c.Lead).Include(c => c.PaymentMode).ToList()
            //    });
            //}
            //var collection_Entry = db.Collection_Entry.Include(c => c.Bank_Master).Include(c => c.Invoice).Include(c => c.Lead).Include(c => c.PaymentMode);
            foreach (var item in listCollectionsWithoutGST)
            {
                if (item.Lead.CustomerName == null)
                {
                    item.Lead.CustomerName = item.Lead.Title + " " + item.Lead.FirstName + " " + item.Lead.LastName;
                }
                //item.singleInvoice.BalanceAmount = (db.Collection_Entry.Where(t => t.InvoiceID == item.singleInvoice.InvoiceID).Sum(i => i.Amount)).ToString();

                item.Lead.AlreadyPaidSum = item.Lead.Collection_Entry.Sum(i => i.Amount);
                item.Lead.BalanceAmount = (Decimal.Parse(item.Lead.FinalRate)  - item.Lead.Collection_Entry.Sum(i => i.Amount)).ToString();

                // it is actually Final rate without Tax i.e. Basic Amount
                item.Lead.ConsultPerson = item.Lead.Lead_Services_Mapping.Sum(i => i.Rate * i.Qty).ToString();
                item.Lead.SGSTSumOfAllServices = item.Lead.Lead_Services_Mapping.Sum(i => i.SGST);
                item.Lead.CGSTSumOfAllServices = item.Lead.Lead_Services_Mapping.Sum(i => i.CGST);
                item.Lead.IGSTSumOfAllServices = item.Lead.Lead_Services_Mapping.Sum(i => i.IGST);
            }
            //ViewBag.BasicAmount = db.Invoice_Services_Mapping.Sum(i => i.Rate * i.Qty);
            List<SelectListItem> SearchOptionsList = new List<SelectListItem>();

            var serviceGroups = db.ServiceGroups.OrderBy(i => i.Order).ToList();
            SelectListGroup selectListGroup;
            int groupID;

            for (int i = 0; i < serviceGroups.Count(); i++)
            {

                selectListGroup = new SelectListGroup { Name = serviceGroups.ElementAt(i).ServiceGroup1 };
                groupID = serviceGroups.ElementAt(i).ServiceGroupID;
                var services = db.Services.Where(o => o.ServiceGroupID == groupID).OrderBy(o => o.Order).ToList();
                for (int j = 0; j < services.Count(); j++)
                {
                    SearchOptionsList.Add(new SelectListItem() { Text = services.ElementAt(j).ServiceName, Value = "ServiceID_" + services.ElementAt(j).ServiceID, Group = selectListGroup });
                }
            }

            selectListGroup = new SelectListGroup { Name = "Payment Status" };
            SearchOptionsList.Add(new SelectListItem() { Text = "Paid", Value = "PaymentStatusID_Paid", Group = selectListGroup });
            SearchOptionsList.Add(new SelectListItem() { Text = "Unpaid / Balance", Value = "PaymentStatusID_Unpaid", Group = selectListGroup });

            selectListGroup = new SelectListGroup { Name = "Payment Mode" };
            var paymentModes = db.PaymentModes.OrderBy(o => o.Order).ToList();
            for (int i = 0; i < paymentModes.Count(); i++)
            {
                SearchOptionsList.Add(new SelectListItem() { Text = paymentModes.ElementAt(i).PaymentModeName, Value = "PaymentModeID_" + paymentModes.ElementAt(i).PaymentModeID.ToString(), Group = selectListGroup });
            }

            selectListGroup = new SelectListGroup { Name = "Branch" };
            var branches = db.Branches.OrderBy(o => o.Order).ToList();
            for (int i = 0; i < branches.Count(); i++)
            {
                SearchOptionsList.Add(new SelectListItem() { Text = branches.ElementAt(i).BranchName, Value = "BranchID_" + branches.ElementAt(i).BranchID.ToString(), Group = selectListGroup });
            }


            selectListGroup = new SelectListGroup { Name = "Other" };
            SearchOptionsList.Add(new SelectListItem() { Text = "TDS Amount", Value = "TDSAmount", Group = selectListGroup });
            SearchOptionsList.Add(new SelectListItem() { Text = "Bad Debts Amount", Value = "BadDebtsAmount", Group = selectListGroup });

            var dropdownList = new SelectList(SearchOptionsList.Select(item => new SelectListItem
            {
                Text = item.Text,
                Value = item.Value,
                // Assign the Group to the item by some appropriate selection method
                Group = item.Group
            }).ToList(), "Value", "Text", "Group.Name", -1);
            ViewBag.SearchOptions = dropdownList;

            ViewBag.BankID = new SelectList(db.Bank_Master, "BankID", "BankName");
            ViewBag.PaymentModes = new SelectList(db.PaymentModes, "PaymentModeID", "PaymentModeName");
            return View(listCollectionsWithoutGST);
        }

        // GET: Collections/Details/5
        public ActionResult Details(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Collection_Entry collection_Entry = db.Collection_Entry.Find(id);
            if (collection_Entry == null)
            {
                return HttpNotFound();
            }
            return View(collection_Entry);
        }

        // GET: Collections/Create
        public ActionResult Create()
        {
            ViewBag.CollectionType = Request.QueryString["type"];

            var leads = db.Leads.Where(o => o.Lead_Services_Mapping.Count > 0 &&  o.Lead_Services_Mapping.Where(map => map.GST == true).Count() > 0).OrderByDescending(l => l.LeadID).Select(s => new
            {
                s.LeadID,
                //Description = string.Format("{0} - £{1} £{2} £{3} £{4} ", s.LeadID, s.Title, s.FirstName, s.LastName, s.CustomerName)
                Description = s.LeadID + " - " + s.Title + s.FirstName + s.LastName + s.CustomerName

            }).ToList();


            var totalAdvanceEntries = db.Collection_Entry.Where(c => c.isAdvanceEntry == true).ToList();
            //Evaluation times out for below code if ToList is not added above
            SelectList leadsSelectList = new SelectList(leads.Where(l => totalAdvanceEntries.Where(t => t.LeadID == l.LeadID).Count() == 0), "LeadID", "Description");
            foreach (var option in leadsSelectList) {
                if (totalAdvanceEntries.Where(a => a.LeadID.ToString() == option.Value).Count() > 0) {
                    option.Disabled = true;
                    option.Text = option.Text + " - " + "Advance already created.";
                }
            }
            //leads.RemoveRange(to)
            ViewBag.Leads = leadsSelectList;
            var invoices = db.Invoices.Where(i => i.Tax_Invoice_Type_Enum == (byte)TaxInvoiceTypes.ProformaInvoice).OrderByDescending(i => i.InvoiceID).Select(s => new
            {
                s.InvoiceID,
                //Description = string.Format("{0} - £{1} £{2} £{3} £{4} ", s.InvoiceNo, s.Customer.Title, s.Customer.CustomerName, s.Customer.FirstName, s.Customer.LastName)
                Description = (s.InvoiceNo + "-" + s.Customer.Title + " " + s.Customer.CustomerName + " " + s.Customer.FirstName + " " + s.Customer.LastName)
                //Description = s.InvoiceNo

            }).ToList();
            SelectList invoicesSelectList = new SelectList(invoices.Where(i => totalAdvanceEntries.Where(t => t.InvoiceID == i.InvoiceID).Count() == 0), "InvoiceID", "Description");
            ViewBag.Invoices = invoicesSelectList;

            var customers = db.Customers.OrderByDescending(i => i.CustomerID).Select(s => new
            {
                s.CustomerID,
                //Description = string.Format("{0} - £{1} £{2} £{3} £{4} ", s.InvoiceNo, s.Customer.Title, s.Customer.CustomerName, s.Customer.FirstName, s.Customer.LastName)
                Description = (s.CustomerNo + "-" + s.Title + " " + s.CustomerName + " " + s.FirstName + " " + s.LastName)
                //Description = s.InvoiceNo

            }).ToList();
            SelectList customersSelectList = new SelectList(customers.Where(c => totalAdvanceEntries.Where(t => t.CustomerID == c.CustomerID).Count() == 0), "CustomerID", "Description");

            ViewBag.Customers = customersSelectList;
            ViewBag.BankID = new SelectList(db.Bank_Master, "BankID", "BankName");
            ViewBag.PaymentModes = new SelectList(db.PaymentModes, "PaymentModeID", "PaymentModeName");
            return View();
        }

        // POST: Collections/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Collection_Entry collection_Entry)
        {
            if (ModelState.IsValid)
            {
                if (collection_Entry.InvoiceID != null)
                {
                    collection_Entry.CustomerID = null;
                    collection_Entry.LeadID = null;
                    collection_Entry.TotalAmountToUseOnlyForCustomer = null;
                    collection_Entry.BalanceUseOnlyForCustomer = null;
                }
                if (collection_Entry.CustomerID != null)
                {
                    collection_Entry.InvoiceID = null;
                    collection_Entry.LeadID = null;
                }
                if (collection_Entry.LeadID != null)
                {
                    collection_Entry.InvoiceID = null;
                    collection_Entry.CustomerID = null;
                    collection_Entry.TotalAmountToUseOnlyForCustomer = null;
                    collection_Entry.BalanceUseOnlyForCustomer = null;
                }


                collection_Entry.CreatedBy = Convert.ToInt32(Session["UserID"]);
                collection_Entry.CreatedOn = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, INDIAN_ZONE);
                db.Collection_Entry.Add(collection_Entry);
                db.SaveChanges();
                //if (collection_Entry.InvoiceID == null)
                //{
                //    return RedirectToAction("WithoutGSTCollections");
                //}
                //else
                //{
                //    return RedirectToAction("WithGSTCollections");
                //}
                return RedirectToAction("WithGSTCollections");
                return RedirectToAction("Index");
            }
            
            var leads = db.Leads.Where(o => o.Lead_Services_Mapping.Count > 0).Select(s => new
            {
                s.LeadID,
                //Description = string.Format("{0} - £{1} £{2} £{3} £{4} ", s.LeadID, s.Title, s.FirstName, s.LastName, s.CustomerName)
                Description = s.LeadID + " - " + s.Title + s.FirstName + s.LastName + s.CustomerName

            }).ToList();
            ViewBag.Leads = new SelectList(leads, "LeadID", "Description");
            var invoices = db.Invoices.Select(s => new
            {
                s.InvoiceID,
                //Description = string.Format("{0} - £{1} £{2} £{3} £{4} ", s.LeadID, s.Title, s.FirstName, s.LastName, s.CustomerName)
                Description = s.InvoiceNo

            }).ToList();
            ViewBag.Invoices = new SelectList(invoices, "InvoiceID", "Description");
            ViewBag.BankID = new SelectList(db.Bank_Master, "BankID", "BankName", collection_Entry.BankID);
            ViewBag.PaymentModeID = new SelectList(db.PaymentModes, "PaymentModeID", "PaymentModeName", collection_Entry.PaymentModeID);
            if (collection_Entry.InvoiceID == null)
            {
                return RedirectToAction("WithoutGSTCollections");
            }
            else {
                return RedirectToAction("WithGSTCollections");
            }
            return View(collection_Entry);
        }
        public ActionResult GetLeadsDataForCreateCollection(string LeadID)
        {
            int intLeadID;

            if (!string.IsNullOrEmpty(LeadID))
            {
                intLeadID = Convert.ToInt32(LeadID);

                var TotalAmount = db.Leads.Find(intLeadID).FinalRate != null ? Decimal.Parse(db.Leads.Find(intLeadID).FinalRate) : 0;
                var AlreadyPaid = db.Collection_Entry.Where(i => i.LeadID == intLeadID).Count() > 0 ? db.Collection_Entry.Where(i => i.LeadID == intLeadID).Sum(i => i.Amount) : 0;
                var BalanceAmount = TotalAmount - AlreadyPaid;
                return Json(new { TotalAmount, AlreadyPaid, BalanceAmount }, JsonRequestBehavior.AllowGet);
            }
            else
            {

                return Json("Wrong Lead ID", JsonRequestBehavior.AllowGet);
            }

        }
        public ActionResult GetInvoicesDataForCreateCollection(string InvoiceID)
        {
            int intInvoiceID;

            if (!string.IsNullOrEmpty(InvoiceID))
            {
                intInvoiceID = Convert.ToInt32(InvoiceID);

                var TotalAmount = db.Invoices.Find(intInvoiceID).FinalInvoiceRate != null ? db.Invoices.Find(intInvoiceID).FinalInvoiceRate : 0;
                //var AlreadyPaid = db.Collection_Entry.Where(i => i.LeadID == intLeadID).Count() > 0 ? db.Collection_Entry.Where(i => i.LeadID == intLeadID).Sum(i => i.Amount) : 0;
                //var BalanceAmount = TotalAmount - AlreadyPaid;
                return Json(new { TotalAmount/*, AlreadyPaid, BalanceAmount */}, JsonRequestBehavior.AllowGet);
            }
            else
            {

                return Json("Wrong Invoice ID", JsonRequestBehavior.AllowGet);
            }

        }
        public ActionResult GetCustomersDataForCreateCollection(string LeadID)
        {
            int intLeadID;

            if (!string.IsNullOrEmpty(LeadID))
            {
                intLeadID = Convert.ToInt32(LeadID);

                var TotalAmount = db.Leads.Find(intLeadID).FinalRate != null ? Decimal.Parse(db.Leads.Find(intLeadID).FinalRate) : 0; 
                var AlreadyPaid = db.Collection_Entry.Where(i => i.LeadID == intLeadID).Count() > 0 ? db.Collection_Entry.Where(i => i.LeadID == intLeadID).Sum(i => i.Amount) : 0;
                var BalanceAmount = TotalAmount - AlreadyPaid;
                return Json(new { TotalAmount, AlreadyPaid, BalanceAmount }, JsonRequestBehavior.AllowGet);
            }
            else
            {

                return Json("Wrong Customer ID", JsonRequestBehavior.AllowGet);
            }

        }
        // GET: Collections/Edit/5
        public ActionResult Edit(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Collection_Entry collection_Entry = db.Collection_Entry.Find(id);
            if (collection_Entry == null)
            {
                return HttpNotFound();
            }
            ViewBag.CollectionType = Request.QueryString["type"];
            ViewBag.ViewType = Request.QueryString["viewtype"] == null ? "" : Request.QueryString["viewtype"];

            var leads = db.Leads.Where(o => o.Lead_Services_Mapping.Count > 0).OrderByDescending(l => l.LeadID).Select(s => new
            {
                s.LeadID,
                //Description = string.Format("{0} - £{1} £{2} £{3} £{4} ", s.LeadID, s.Title, s.FirstName, s.LastName, s.CustomerName)
                Description = s.LeadID + " - " + s.Title + s.FirstName + s.LastName + s.CustomerName

            }).ToList();
            ViewBag.Leads = new SelectList(leads, "LeadID", "Description", collection_Entry.LeadID);
            var invoices = db.Invoices.OrderByDescending(i => i.InvoiceID).Select(s => new
            {
                s.InvoiceID,
                //Description = string.Format("{0} - £{1} £{2} £{3} £{4} ", s.InvoiceNo, s.Customer.Title, s.Customer.CustomerName, s.Customer.FirstName, s.Customer.LastName)
                Description = (s.InvoiceNo + "-" + s.Customer.Title + " " + s.Customer.CustomerName + " " + s.Customer.FirstName + " " + s.Customer.LastName)
                //Description = s.InvoiceNo

            }).ToList();
            var customers = db.Customers.OrderByDescending(i => i.CustomerID).Select(s => new
            {
                s.CustomerID,
                //Description = string.Format("{0} - £{1} £{2} £{3} £{4} ", s.InvoiceNo, s.Customer.Title, s.Customer.CustomerName, s.Customer.FirstName, s.Customer.LastName)
                Description = (s.CustomerNo + "-" + s.Title + " " + s.CustomerName + " " + s.FirstName + " " + s.LastName)
                //Description = s.InvoiceNo

            }).ToList();
            ViewBag.Invoices = new SelectList(invoices, "InvoiceID", "Description", collection_Entry.InvoiceID);
            ViewBag.Customers = new SelectList(customers, "CustomerID", "Description", collection_Entry.CustomerID);
            ViewBag.BankID = new SelectList(db.Bank_Master, "BankID", "BankName", collection_Entry.BankID);
            ViewBag.PaymentModes = new SelectList(db.PaymentModes, "PaymentModeID", "PaymentModeName", collection_Entry.PaymentModeID);


            ViewBag.WithGST = Request.QueryString["withgst"];

            return View(collection_Entry);
        }

        // POST: Collections/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Collection_Entry collection_Entry, string WithGST)
        {
            if (ModelState.IsValid)
            {
                if (collection_Entry.InvoiceID != null)
                {
                    collection_Entry.CustomerID = null;
                    collection_Entry.LeadID = null;
                    collection_Entry.TotalAmountToUseOnlyForCustomer = null;
                    collection_Entry.BalanceUseOnlyForCustomer = null;
                }
                if (collection_Entry.CustomerID != null)
                {
                    collection_Entry.InvoiceID = null;
                    collection_Entry.LeadID = null;
                }
                if (collection_Entry.LeadID != null)
                {
                    collection_Entry.InvoiceID = null;
                    collection_Entry.CustomerID = null;
                    collection_Entry.TotalAmountToUseOnlyForCustomer = null;
                    collection_Entry.BalanceUseOnlyForCustomer = null;
                }

                collection_Entry.LastUpdatedBy = Convert.ToInt32(Session["UserID"]);
                collection_Entry.LastUpdatedDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, INDIAN_ZONE);
                db.Entry(collection_Entry).State = EntityState.Modified;
                db.SaveChanges();
                if (WithGST == "true") {
                    return RedirectToAction("WithGSTCollections");
                }
                else
                {
                    return RedirectToAction("WithoutGSTCollections");
                }
                //return RedirectToAction("Index");
            }
            ViewBag.BankID = new SelectList(db.Bank_Master, "BankID", "BankName", collection_Entry.BankID);
            ViewBag.InvoiceID = new SelectList(db.Invoices, "InvoiceID", "InvoiceNo", collection_Entry.InvoiceID);
            ViewBag.LeadID = new SelectList(db.Leads, "LeadID", "Title", collection_Entry.LeadID);
            ViewBag.PaymentModeID = new SelectList(db.PaymentModes, "PaymentModeID", "PaymentModeName", collection_Entry.PaymentModeID);
            return View(collection_Entry);
        }

        // GET: Collections/Delete/5
        public ActionResult Delete(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Collection_Entry collection_Entry = db.Collection_Entry.Find(id);
            if (collection_Entry == null)
            {
                return HttpNotFound();
            }
            return View(collection_Entry);
        }

        // POST: Collections/Delete/5
        [HttpPost, ActionName("DeleteConfirmed")]
        //[ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(long id, string withgst)
        {
            Collection_Entry collection_Entry = db.Collection_Entry.Find(id);
            db.Collection_Entry.Remove(collection_Entry);
            db.SaveChanges();
            if (withgst == "true")
            {

                return RedirectToAction("WithGSTCollections");
            }
            else
            {
                return RedirectToAction("WithoutGSTCollections");

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

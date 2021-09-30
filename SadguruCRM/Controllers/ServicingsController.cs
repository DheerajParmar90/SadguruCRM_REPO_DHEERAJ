using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Rotativa.Options;
using SadguruCRM.Helpers;
using SadguruCRM.Models;

namespace SadguruCRM.Controllers
{
    [VerifyUser]
    public class ServicingsController : Controller
    {
        private SadguruCRMEntities db = new SadguruCRMEntities();
        private static TimeZoneInfo INDIAN_ZONE = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");

        // GET: Servicings
        public ActionResult Index()
        {

            ViewBag.BranchID = new SelectList(db.Branches, "BranchID", "BranchName");

            ViewBag.ServiceGroupID = new SelectList(db.ServiceGroups, "ServiceGroupID", "ServiceGroup1");
            ViewBag.ServiceID = new SelectList(db.Services.Where(x => x.ServiceID == 0), "ServiceID", "ServiceName");
            var servicings = db.Servicings
                .Include(s => s.Contract)
                .Include(s => s.Customer)
                .Include(s => s.FrequencyOfService)
                .Include(s => s.PaymentMode)
                .Include(s => s.PaymentTerm).Include(s => s.PremisesType)
                .Include(s => s.ServiceGroup).Include(s => s.Service)
                .Include(s => s.UserLogin).Include(s => s.UserLogin1)
                .Include(s => s.Vendor)
                //.Where(s => s.VendorID != null)
                .Where(s => s.Servicing_Technician_Mapping.Count > 0 || s.Vendor != null)
                .OrderBy(s => s.System_Servicing_Datetime)
                ;
            return View(servicings.ToList());
        }
        public ActionResult DatewiseServiceSchedule()
        {

            ViewBag.BranchID = new SelectList(db.Branches, "BranchID", "BranchName");

            ViewBag.ServiceGroupID = new SelectList(db.ServiceGroups, "ServiceGroupID", "ServiceGroup1");
            ViewBag.ServiceID = new SelectList(db.Services.Where(x => x.ServiceID == 0), "ServiceID", "ServiceName");
            var servicings = db.Servicings.Include(s => s.Lead_Services_Mapping).Include(s => s.PaymentMode).Include(s => s.PaymentTerm)
                .Include(s => s.PremisesType).Include(s => s.ServiceGroup).Include(s => s.Service).Include(s => s.UserLogin).Include(s => s.UserLogin1)
                .Include(s => s.Vendor)

                .Where(s => s.Servicing_Technician_Mapping.Count == 0 && s.VendorID == null)
                .OrderBy(s => s.System_Servicing_Datetime).Include(s => s.Lead).ToList();
            //foreach (var item in servicings)
            //{
            //    if (item.Lead != null)
            //    {
            //        if (item.Lead.Customers.Count > 0)
            //        {
            //            item.Lead.Customers.First().CustomerName = item.Lead.Customers.First().Title + " " + item.Lead.Customers.First().FirstName + " " + item.Lead.Customers.First().LastName + item.Lead.Customers.First().CustomerName;
            //            //name
            //        }
            //    }

            //}
            return View(servicings.ToList());
        }
        [HttpPost]
        public ActionResult Index(DateTime? FromDate, DateTime? ToDate, int Month, int Year, int? BranchID, int? ServiceGroupID, int? ServiceID, string Duration)
        {
            var servicings = db.Servicings
                .Include(s => s.Lead_Services_Mapping).Include(s => s.Customer)
                .Include(s => s.FrequencyOfService).Include(s => s.PaymentMode)
                .Include(s => s.PaymentTerm).Include(s => s.PremisesType)
                .Include(s => s.ServiceGroup).Include(s => s.Service)
                .Include(s => s.UserLogin).Include(s => s.UserLogin1)
                .Include(s => s.Vendor)
                .Where(s => s.Servicing_Technician_Mapping != null || s.Vendor != null)
                .OrderBy(s => s.System_Servicing_Datetime);
            if (FromDate != null)
            {
                servicings = servicings.Where(s => s.System_Servicing_Datetime > FromDate).OrderBy(s => s.System_Servicing_Datetime);
            }
            if (ToDate != null)
            {
                servicings = servicings.Where(s => s.System_Servicing_Datetime < ToDate).OrderBy(s => s.System_Servicing_Datetime);
            }
            if (Month > 0)
            {
                servicings = servicings.Where(s => ((DateTime)s.System_Servicing_Datetime).Month == Month).OrderBy(s => s.System_Servicing_Datetime);
            }
            if (Year > 0)
            {
                servicings = servicings.Where(s => ((DateTime)s.System_Servicing_Datetime).Year == Year).OrderBy(s => s.System_Servicing_Datetime);
            }
            if (BranchID != null)
            {
                servicings = servicings.Where(s => s.Customer.BranchID == BranchID).OrderBy(s => s.System_Servicing_Datetime);
            }
            if (ServiceGroupID != null)
            {
                servicings = servicings.Where(s => s.Lead_Services_Mapping.ServiceGroupID == ServiceGroupID).OrderBy(s => s.System_Servicing_Datetime);
            }
            if (ServiceID != null)
            {
                servicings = servicings.Where(s => s.Lead_Services_Mapping.ServiceID == ServiceID).OrderBy(s => s.System_Servicing_Datetime);
            }
            if (Duration != "")
            {
                DateTime today = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, INDIAN_ZONE);

                if (Duration == "WeekBefore")
                {
                    DateTime date = today.AddDays(-7);
                    servicings = servicings.Where(s => ((DateTime)s.System_Servicing_Datetime) == date).OrderBy(s => s.System_Servicing_Datetime);
                }
                else if (Duration == "ThreeDaysBefore")
                {
                    DateTime date = today.AddDays(-3);
                    servicings = servicings.Where(s => ((DateTime)s.System_Servicing_Datetime) == date).OrderBy(s => s.System_Servicing_Datetime);
                }
                else if (Duration == "TwoDaysBefore")
                {
                    DateTime date = today.AddDays(-2);
                    servicings = servicings.Where(s => ((DateTime)s.System_Servicing_Datetime) == date).OrderBy(s => s.System_Servicing_Datetime);
                }
                else if (Duration == "Yesterday")
                {
                    DateTime date = today.AddDays(-1);
                    servicings = servicings.Where(s => ((DateTime)s.System_Servicing_Datetime) == date).OrderBy(s => s.System_Servicing_Datetime);
                }
                else if (Duration == "Today")
                {
                    servicings = servicings.Where(s => ((DateTime)s.System_Servicing_Datetime) == today).OrderBy(s => s.System_Servicing_Datetime);
                }
            }
            ViewBag.BranchID = new SelectList(db.Branches, "BranchID", "BranchName");

            ViewBag.ServiceGroupID = new SelectList(db.ServiceGroups, "ServiceGroupID", "ServiceGroup1");
            ViewBag.ServiceID = new SelectList(db.Services.Where(x => x.ServiceID == 0), "ServiceID", "ServiceName");

            return View(servicings.ToList());
        }
        [HttpPost]
        public ActionResult DatewiseServiceSchedule(DateTime? FromDate, DateTime? ToDate, int Month, int Year, int? BranchID, int? ServiceGroupID, int? ServiceID, string Duration)
        {
            var servicings = db.Servicings.Include(s => s.Lead_Services_Mapping).Include(s => s.Customer)
                .Include(s => s.FrequencyOfService).Include(s => s.PaymentMode).Include(s => s.PaymentTerm).Include(s => s.PremisesType)
                .Include(s => s.ServiceGroup).Include(s => s.Service).Include(s => s.UserLogin).Include(s => s.UserLogin1)
                .Include(s => s.Vendor)
                .Where(s => s.Servicing_Technician_Mapping.Count == 0 && s.VendorID == null)
                .OrderBy(s => s.System_Servicing_Datetime);
            if (FromDate != null)
            {
                servicings = servicings.Where(s => s.System_Servicing_Datetime > FromDate).OrderBy(s => s.System_Servicing_Datetime);
            }
            if (ToDate != null)
            {
                servicings = servicings.Where(s => s.System_Servicing_Datetime < ToDate).OrderBy(s => s.System_Servicing_Datetime);
            }
            if (Month > 0)
            {
                servicings = servicings.Where(s => ((DateTime)s.System_Servicing_Datetime).Month == Month).OrderBy(s => s.System_Servicing_Datetime);
            }
            if (Year > 0)
            {
                servicings = servicings.Where(s => ((DateTime)s.System_Servicing_Datetime).Year == Year).OrderBy(s => s.System_Servicing_Datetime);
            }
            if (BranchID != null)
            {
                servicings = servicings.Where(s => s.Customer.BranchID == BranchID).OrderBy(s => s.System_Servicing_Datetime);
            }
            if (ServiceGroupID != null)
            {
                servicings = servicings.Where(s => s.Lead_Services_Mapping.ServiceGroupID == ServiceGroupID).OrderBy(s => s.System_Servicing_Datetime);
            }
            if (ServiceID != null)
            {
                servicings = servicings.Where(s => s.Lead_Services_Mapping.ServiceID == ServiceID).OrderBy(s => s.System_Servicing_Datetime);
            }
            if (Duration != "")
            {
                DateTime today = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, INDIAN_ZONE);

                if (Duration == "WeekBefore")
                {
                    DateTime date = today.AddDays(-7);
                    servicings = servicings.Where(s => ((DateTime)s.System_Servicing_Datetime) == date).OrderBy(s => s.System_Servicing_Datetime);
                }
                else if (Duration == "ThreeDaysBefore")
                {
                    DateTime date = today.AddDays(-3);
                    servicings = servicings.Where(s => ((DateTime)s.System_Servicing_Datetime) == date).OrderBy(s => s.System_Servicing_Datetime);
                }
                else if (Duration == "TwoDaysBefore")
                {
                    DateTime date = today.AddDays(-2);
                    servicings = servicings.Where(s => ((DateTime)s.System_Servicing_Datetime) == date).OrderBy(s => s.System_Servicing_Datetime);
                }
                else if (Duration == "Yesterday")
                {
                    DateTime date = today.AddDays(-1);
                    servicings = servicings.Where(s => ((DateTime)s.System_Servicing_Datetime) == date).OrderBy(s => s.System_Servicing_Datetime);
                }
                else if (Duration == "Today")
                {
                    servicings = servicings.Where(s => ((DateTime)s.System_Servicing_Datetime) == today).OrderBy(s => s.System_Servicing_Datetime);
                }
            }
            ViewBag.BranchID = new SelectList(db.Branches, "BranchID", "BranchName");

            ViewBag.ServiceGroupID = new SelectList(db.ServiceGroups, "ServiceGroupID", "ServiceGroup1");
            ViewBag.ServiceID = new SelectList(db.Services.Where(x => x.ServiceID == 0), "ServiceID", "ServiceName");

            return View(servicings.ToList());
        }
        public ActionResult LeadServiceServicings(int id)
        {
            var servicings = db.Servicings.Where(x => x.Lead_Services_Mapping_ID == id).Include(s => s.Customer).Include(s => s.FrequencyOfService).Include(s => s.PaymentMode).Include(s => s.PaymentTerm).Include(s => s.PremisesType).Include(s => s.ServiceGroup).Include(s => s.Service).Include(s => s.UserLogin).Include(s => s.UserLogin1).Include(s => s.Vendor);
            return View(servicings.ToList());
        }
        public ActionResult ContractServiceServicings(int id)
        {
            var servicings = db.Servicings.Where(x => x.Contract_Services_Mapping_ID == id).Include(s => s.Customer).Include(s => s.FrequencyOfService).Include(s => s.PaymentMode).Include(s => s.PaymentTerm).Include(s => s.PremisesType).Include(s => s.ServiceGroup).Include(s => s.Service).Include(s => s.UserLogin).Include(s => s.UserLogin1).Include(s => s.Vendor);
            return View(servicings.ToList());
        }
        public ActionResult ContractServicings(int id)
        {
            var servicings = db.Servicings.Where(x => x.ContractID == id).Include(s => s.Customer).Include(s => s.FrequencyOfService).Include(s => s.PaymentMode).Include(s => s.PaymentTerm).Include(s => s.PremisesType).Include(s => s.ServiceGroup).Include(s => s.Service).Include(s => s.UserLogin).Include(s => s.UserLogin1).Include(s => s.Vendor);
            return View(servicings.ToList());
        }

        // GET: Servicings/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Servicing servicing = db.Servicings.Find(id);
            if (servicing == null)  
            {
                return HttpNotFound();
            }
            return View(servicing);
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

                return new Rotativa.UrlAsPdf("/Servicings/PDFView/" + id.ToString())
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
            Servicing servicing = db.Servicings.Find(id);
            if (servicing == null)
            {
                return HttpNotFound();
            }

            return View(servicing);
        }
        // GET: Servicings/Create
        public ActionResult Create()
        {
            var customers = db.Customers.ToList();
            foreach (var item in customers)
            {
                if (item.CustomerName == null)
                {
                    item.CustomerName = item.Title + " " + item.FirstName + " " + item.LastName;
                }
            }
            ViewBag.CustomerID = new SelectList(customers, "CustomerID", "CustomerName");
            ViewBag.FrequencyOfServiceID = new SelectList(db.FrequencyOfServices, "FrequencyOfServiceID", "FrequencyOfService1");
            ViewBag.PaymentModeID = new SelectList(db.PaymentModes, "PaymentModeID", "PaymentModeName");
            ViewBag.PaymentTermID = new SelectList(db.PaymentTerms, "PaymentTermID", "PaymentTermName");
            ViewBag.TypeOfPremisesID = new SelectList(db.PremisesTypes, "PremisesTypeID", "PremisesType1");
            ViewBag.ServiceGroupID = new SelectList(db.ServiceGroups, "ServiceGroupID", "ServiceGroup1");
            ViewBag.ServiceID = new SelectList(db.Services.Where(x => x.ServiceID == 0), "ServiceID", "ServiceName");
            ViewBag.CreatedBy = new SelectList(db.UserLogins, "UserID", "UserName");
            ViewBag.LastUpdatedBy = new SelectList(db.UserLogins, "UserID", "UserName");
            ViewBag.VendorID = new SelectList(db.Vendors, "VendorID", "VendorName");


            ViewBag.LocationID = new SelectList(db.Locations, "LocationID", "LocationName");
            ViewBag.CityID = new SelectList(db.Cities, "CityID", "CityName");
            ViewBag.StateID = new SelectList(db.States, "StateID", "State1");
            ViewBag.BranchID = new SelectList(db.Branches, "BranchID", "BranchName");
            ViewBag.TechnicianID = new SelectList(db.Employees.Where(x => x.DesignationID == 2), "EmployeeID", "Name");
            ViewBag.ChemicalID = new SelectList(db.Chemicals_Master, "ChemicalID", "Chemical");
            ViewBag.UOMID = new SelectList(db.UOM_Master, "UOMID", "UOM");
            ViewBag.ServiceStatusID = new SelectList(db.Service_Status_Master, "Service_Status_ID", "Service_Status");
            return View();
        }

        // POST: Servicings/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ServicingID,ServicingNo,CustomerID,TypeOfPremisesID,PremisesArea,PremisesAppSqFtArea,Period_Of_Contract_Start_Date,Period_Of_Contract_End_Date,ServiceGroupID,ServiceID,FrequencyOfServiceID,Servicing_Frequency_Number,Quantity,System_Servicing_Datetime,Servicing_Datetime,Actual_Servicing_Datetime,VendorID,PaymentModeID,PaymentTermID,PaidByCustomer,BalanceAmount,ServicingInstructions,Service_Status_ID")] Servicing servicing, int Customers_Billing_Address_Mapping_ID, int[] TechnicianID, int ChemicalID, int ChemicalTechnicianID, string ChemicalQty, byte UOMID)
        {

            if (ModelState.IsValid)
            {
                servicing.CreatedDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, INDIAN_ZONE);
                servicing.CreatedBy = Convert.ToInt32(Session["UserID"]);
                db.Servicings.Add(servicing);
                db.SaveChanges();

                for (int i = 0; i < TechnicianID.Length; i++)
                {
                    Servicing_Technician_Mapping servTech = new Servicing_Technician_Mapping();
                    servTech.ServicingID = servicing.ServicingID;
                    servTech.TechnicianID = TechnicianID[i];
                    db.Servicing_Technician_Mapping.Add(servTech);
                    db.SaveChanges();
                }
                Servicing_Chemicals_Issued servChem = new Servicing_Chemicals_Issued();
                servChem.ServicingID = servicing.ServicingID;
                servChem.ChemicalID = ChemicalID;
                servChem.TechnicianID = ChemicalTechnicianID;
                servChem.UOMID = UOMID;
                servChem.ChemicalQty = Decimal.Parse(ChemicalQty);
                servChem.CreatedDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, INDIAN_ZONE);
                db.Servicing_Chemicals_Issued.Add(servChem);
                db.SaveChanges();


                return RedirectToAction("Index");
            }
            var customers = db.Customers.ToList();
            foreach (var item in customers)
            {
                if (item.CustomerName == null)
                {
                    item.CustomerName = item.Title + " " + item.FirstName + " " + item.LastName;
                }
            }

            ViewBag.CustomerID = new SelectList(customers, "CustomerID", "CustomerName", servicing.CustomerID);
            ViewBag.FrequencyOfServiceID = new SelectList(db.FrequencyOfServices, "FrequencyOfServiceID", "FrequencyOfService1", servicing.FrequencyOfServiceID);
            ViewBag.PaymentModeID = new SelectList(db.PaymentModes, "PaymentModeID", "PaymentModeName", servicing.PaymentModeID);
            ViewBag.PaymentTermID = new SelectList(db.PaymentTerms, "PaymentTermID", "PaymentTermName", servicing.PaymentTermID);
            ViewBag.TypeOfPremisesID = new SelectList(db.PremisesTypes, "PremisesTypeID", "PremisesType1", servicing.TypeOfPremisesID);
            ViewBag.ServiceGroupID = new SelectList(db.ServiceGroups, "ServiceGroupID", "ServiceGroup1", servicing.ServiceGroupID);
            ViewBag.ServiceID = new SelectList(db.Services, "ServiceID", "ServiceName", servicing.ServiceID);
            ViewBag.CreatedBy = new SelectList(db.UserLogins, "UserID", "UserName", servicing.CreatedBy);
            ViewBag.LastUpdatedBy = new SelectList(db.UserLogins, "UserID", "UserName", servicing.LastUpdatedBy);
            ViewBag.VendorID = new SelectList(db.Vendors, "VendorID", "VendorName", servicing.VendorID);

            ViewBag.LocationID = new SelectList(db.Locations, "LocationID", "LocationName");
            ViewBag.CityID = new SelectList(db.Cities, "CityID", "CityName");
            ViewBag.StateID = new SelectList(db.States, "StateID", "State1");
            ViewBag.BranchID = new SelectList(db.Branches, "BranchID", "BranchName");
            ViewBag.TechnicianID = new SelectList(db.Employees.Where(x => x.DesignationID == 2), "EmployeeID", "Name");
            ViewBag.ChemicalID = new SelectList(db.Chemicals_Master, "ChemicalID", "Chemical");
            ViewBag.UOMID = new SelectList(db.UOM_Master, "UOMID", "UOM");
            ViewBag.ServiceStatusID = new SelectList(db.Service_Status_Master, "Service_Status_ID", "Service_Status");
            return View(servicing);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateEditComplaint(int ServicingID,string ComplaintServicingNo, int? ComplaintID,string ComplaintNo,string ComplaintContent,
                                            DateTime? Complaint_Servicing_Datetime,DateTime? Complaint_Servicing_Time,
                                            DateTime? Complaint_Actual_Servicing_Datetime, DateTime? Complaint_Actual_Servicing_Time,
                                            int[] ComplaintTechnicianID, int? ComplaintVendorID,
                                            int? ComplaintChemicalID, int? ComplaintChemicalTechnicianID, string ComplaintChemicalQty, byte? ComplaintUOMID,
                                            int? ComplaintStatusID, string ComplaintServicingInstructions)
        {

            Servicing servicing;
            if (ComplaintID == null)
            {

                servicing = new Servicing();
            }
            else {
                servicing = db.Servicings.Find(ComplaintID);
            }
            var originalServicing = db.Servicings.Find(ServicingID);
            //var currentNumberOfComplaintsForThisServicing = (from m in db.Servicings
            //                                         where m.ServicingNo.Contains(thisServicingNumber)
            //                                         select m).Count();
            //servicing.ServicingNo = thisServicingNumber + "-" + currentNumberOfComplaintsForThisServicing.ToString();
            servicing.ServicingNo = ComplaintNo;
            servicing.ComplaintContent = ComplaintContent;
            servicing.System_Servicing_Datetime = originalServicing.System_Servicing_Datetime;
            servicing.Servicing_Datetime = Complaint_Servicing_Datetime;
            servicing.Actual_Servicing_Datetime = Complaint_Actual_Servicing_Datetime;
            servicing.ContractID = originalServicing.ContractID;
            servicing.Contract_Services_Mapping_ID = originalServicing.Contract_Services_Mapping_ID;
            servicing.LeadID = originalServicing.LeadID;
            servicing.Lead_Services_Mapping_ID = originalServicing.Lead_Services_Mapping_ID;
            servicing.VendorID = ComplaintVendorID;
            servicing.Service_Status_ID = ComplaintStatusID;
            servicing.ServicingInstructions = ComplaintServicingInstructions;
            if (Complaint_Servicing_Time != null)
            {
                servicing.Servicing_Datetime = ((DateTime)servicing.Servicing_Datetime).AddHours(((DateTime)Complaint_Servicing_Time).Hour);
                servicing.Servicing_Datetime = ((DateTime)servicing.Servicing_Datetime).AddMinutes(((DateTime)Complaint_Servicing_Time).Minute);
            }
            if (Complaint_Actual_Servicing_Time != null)
            {
                servicing.Actual_Servicing_Datetime = ((DateTime)servicing.Actual_Servicing_Datetime).AddHours(((DateTime)Complaint_Actual_Servicing_Time).Hour);
                servicing.Actual_Servicing_Datetime = ((DateTime)servicing.Actual_Servicing_Datetime).AddMinutes(((DateTime)Complaint_Actual_Servicing_Time).Minute);
            }

            servicing.CreatedDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, INDIAN_ZONE);
            servicing.CreatedBy = Convert.ToInt32(Session["UserID"]);

            if (ComplaintID == null)
            {
                db.Servicings.Add(servicing);
            }
            else
            {
                db.Entry(servicing).State = EntityState.Modified;
            }
            db.SaveChanges();
            try
            {
                if (ComplaintTechnicianID != null)
                {
                    var existingTechnicians = db.Servicing_Technician_Mapping.Where(t => t.ServicingID == servicing.ServicingID).Select(t => t.TechnicianID);
                    for (int i = 0; i < ComplaintTechnicianID.Length; i++)
                    {
                        if (!existingTechnicians.Contains(ComplaintTechnicianID[i])) {
                            Servicing_Technician_Mapping servTech = new Servicing_Technician_Mapping();
                            servTech.ServicingID = servicing.ServicingID;
                            servTech.TechnicianID = ComplaintTechnicianID[i];
                            db.Servicing_Technician_Mapping.Add(servTech);
                            db.SaveChanges();
                        }
                        
                    }
                }
            }
            catch (Exception ex) { 
            
            }
            try
            {
                if (ComplaintChemicalID != null)
                {
                    Servicing_Chemicals_Issued servChem = new Servicing_Chemicals_Issued();
                    servChem.ServicingID = servicing.ServicingID;
                    servChem.ChemicalID = (int)ComplaintChemicalID;
                    servChem.TechnicianID = (int)ComplaintChemicalTechnicianID;
                    servChem.UOMID = (byte)ComplaintUOMID;
                    servChem.ChemicalQty = Decimal.Parse(ComplaintChemicalQty);
                    servChem.CreatedDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, INDIAN_ZONE);
                    db.Servicing_Chemicals_Issued.Add(servChem);
                    db.SaveChanges();
                }
            }
            catch (Exception ex) { 
            }
            
                


                return RedirectToAction("Edit", new { id = originalServicing.ServicingID});
            
            var customers = db.Customers.ToList();
            foreach (var item in customers)
            {
                if (item.CustomerName == null)
                {
                    item.CustomerName = item.Title + " " + item.FirstName + " " + item.LastName;
                }
            }

            ViewBag.CustomerID = new SelectList(customers, "CustomerID", "CustomerName", servicing.CustomerID);
            ViewBag.FrequencyOfServiceID = new SelectList(db.FrequencyOfServices, "FrequencyOfServiceID", "FrequencyOfService1", servicing.FrequencyOfServiceID);
            ViewBag.PaymentModeID = new SelectList(db.PaymentModes, "PaymentModeID", "PaymentModeName", servicing.PaymentModeID);
            ViewBag.PaymentTermID = new SelectList(db.PaymentTerms, "PaymentTermID", "PaymentTermName", servicing.PaymentTermID);
            ViewBag.TypeOfPremisesID = new SelectList(db.PremisesTypes, "PremisesTypeID", "PremisesType1", servicing.TypeOfPremisesID);
            ViewBag.ServiceGroupID = new SelectList(db.ServiceGroups, "ServiceGroupID", "ServiceGroup1", servicing.ServiceGroupID);
            ViewBag.ServiceID = new SelectList(db.Services, "ServiceID", "ServiceName", servicing.ServiceID);
            ViewBag.CreatedBy = new SelectList(db.UserLogins, "UserID", "UserName", servicing.CreatedBy);
            ViewBag.LastUpdatedBy = new SelectList(db.UserLogins, "UserID", "UserName", servicing.LastUpdatedBy);
            ViewBag.VendorID = new SelectList(db.Vendors, "VendorID", "VendorName", servicing.VendorID);

            ViewBag.LocationID = new SelectList(db.Locations, "LocationID", "LocationName");
            ViewBag.CityID = new SelectList(db.Cities, "CityID", "CityName");
            ViewBag.StateID = new SelectList(db.States, "StateID", "State1");
            ViewBag.BranchID = new SelectList(db.Branches, "BranchID", "BranchName");
            ViewBag.TechnicianID = new SelectList(db.Employees.Where(x => x.DesignationID == 2), "EmployeeID", "Name");
            ViewBag.ChemicalID = new SelectList(db.Chemicals_Master, "ChemicalID", "Chemical");
            ViewBag.UOMID = new SelectList(db.UOM_Master, "UOMID", "UOM");
            ViewBag.ServiceStatusID = new SelectList(db.Service_Status_Master, "Service_Status_ID", "Service_Status");
            return View(servicing);
        }

        [HttpPost]
        public ActionResult GetCustomerDataForServicing(string custID)
        {
            int intCustID;

            if (!string.IsNullOrEmpty(custID))
            {
                intCustID = Convert.ToInt32(custID);
                var cust = db.Customers.Find(intCustID);
                var billingAddress = db.Customers_Billing_Address_Mapping.Where(x => x.CustomerID == intCustID).FirstOrDefault();
                long billingAddressID = billingAddress.Customers_Billing_Address_Mapping_ID;
                //var billingTelNos = db.Customers_Billing_Address_Tel_No_Mapping.Where(x => x.Customers_Billing_Address_Mapping_ID == billingAddressID).ToList();
                //var billingCellNos = db.Customers_Billing_Address_Cell_No_Mapping.Where(x => x.Customers_Billing_Address_Mapping_ID == billingAddressID).ToList();
                //var billingEmail = db.Customers_Billing_Address_Email_Mapping.Where(x => x.Customers_Billing_Address_Mapping_ID == billingAddressID).ToList();
                var billingTelNos = db.Customers_Billing_Address_Tel_No_Mapping.Where(x => x.Customers_Billing_Address_Mapping_ID == billingAddressID).ToList();
                string strBillingTelNos = String.Join(",", billingTelNos.Select(x => x.Customers_Billing_Address_Tel_No.ToString()).ToArray());
                var billingCellNos = db.Customers_Billing_Address_Cell_No_Mapping.Where(x => x.Customers_Billing_Address_Mapping_ID == billingAddressID).ToList();
                string strBillingCellNos = String.Join(",", billingCellNos.Select(x => x.Customers_Billing_Address_Cell_No.ToString()).ToArray());
                var billingEmail = db.Customers_Billing_Address_Email_Mapping.Where(x => x.Customers_Billing_Address_Mapping_ID == billingAddressID).ToList();
                string strBillingEmails = String.Join(",", billingEmail.Select(x => x.Customers_Billing_Address_Email.ToString()).ToArray());


                //var serviceAddresses = db.Customers_Service_Address_Mapping.Where(x => x.CustomerID == intCustID).ToList();

                if (billingAddress == null)
                {
                    return Json("No Business Address Found", JsonRequestBehavior.AllowGet);
                }
                //else if (serviceAddresses.Count == 0)
                //{
                //    return Json("No Service Address Found", JsonRequestBehavior.AllowGet);
                //}
                Customer customer = new Customer();
                customer.CustomerID = cust.CustomerID;
                customer.ConsultPerson = cust.ConsultPerson;
                customer.ConsultPersonDesignation = cust.ConsultPersonDesignation;
                customer.CellNumber = cust.CellNumber;
                customer.EmailId = cust.EmailId;
                customer.TelNumber = cust.TelNumber;

                string TypeOfPremiseID="";
                string PremiseArea = "";
                string SqFtArea = "";
                if (cust.Lead != null) {
                    TypeOfPremiseID = cust.Lead.TypeOfPremisesID.ToString();
                    if (cust.Lead.Premises_Area_ID != null)
                    {
                        PremiseArea = db.Premises_Area_Master.Find(cust.Lead.Premises_Area_ID).Premises_Area;
                    }
                    SqFtArea = cust.Lead.PremisesAppSqFtArea;

                }

                Customers_Billing_Address_Mapping bill = new Customers_Billing_Address_Mapping();
                bill.Customers_Billing_Address_Mapping_ID = billingAddress.Customers_Billing_Address_Mapping_ID;
                bill.AddressLine1 = billingAddress.AddressLine1;
                bill.AddressLine2 = billingAddress.AddressLine2;
                bill.AddressLine3 = billingAddress.AddressLine3;
                bill.BillingLocationID = billingAddress.BillingLocationID;
                bill.BillingCityID = billingAddress.BillingCityID;
                bill.BillingPincode = billingAddress.BillingPincode;
                bill.BillingStateID = billingAddress.BillingStateID;
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
                return Json(new { customer, bill, strBillingTelNos, strBillingCellNos, strBillingEmails, TypeOfPremiseID, PremiseArea, SqFtArea }, JsonRequestBehavior.AllowGet);
            }
            else
            {

                return Json("Wrong Customer ID", JsonRequestBehavior.AllowGet);
            }

        }

        // GET: Servicings/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Servicing servicing = db.Servicings.Find(id);
            servicing.Period_Of_Contract_Start_Date = servicing.Lead_Services_Mapping.POCStartDate;
            servicing.Period_Of_Contract_End_Date = servicing.Lead_Services_Mapping.POCEndDate;
            if (servicing == null)
            {
                return HttpNotFound();
            }
            var customers = db.Customers.ToList();
            foreach (var item in customers)
            {
                //if (item.CustomerName == null)
                //{
                    item.CustomerName = item.CustomerNo + " " + item.Title + " " + item.FirstName + " " + item.LastName + " " + item.CustomerName;
                //}
            }
            ViewBag.CustomerID = new SelectList(customers, "CustomerID", "CustomerName", servicing.Lead.Customers.First().CustomerID);
            ViewBag.FrequencyOfServiceID = new SelectList(db.FrequencyOfServices, "FrequencyOfServiceID", "FrequencyOfService1", servicing.Lead_Services_Mapping.FrequencyOfServiceID);
            ViewBag.PaymentModeList = new SelectList(db.PaymentModes, "PaymentModeID", "PaymentModeName", servicing.PaymentModeID);
            ViewBag.PaymentTermList = new SelectList(db.PaymentTerms, "PaymentTermID", "PaymentTermName", servicing.PaymentTermID);
            ViewBag.TypeOfPremisesID = new SelectList(db.PremisesTypes, "PremisesTypeID", "PremisesType1", servicing.TypeOfPremisesID);
            ViewBag.ServiceGroupID = new SelectList(db.ServiceGroups, "ServiceGroupID", "ServiceGroup1", servicing.Lead_Services_Mapping.ServiceGroupID);
            ViewBag.ServiceID = new SelectList(db.Services.Where(s => s.ServiceGroupID == servicing.Lead_Services_Mapping.ServiceGroupID), "ServiceID", "ServiceName", servicing.Lead_Services_Mapping.ServiceID);
            ViewBag.CreatedBy = new SelectList(db.UserLogins, "UserID", "UserName", servicing.CreatedBy);
            ViewBag.LastUpdatedBy = new SelectList(db.UserLogins, "UserID", "UserName", servicing.LastUpdatedBy);
            ViewBag.VendorList = new SelectList(db.Vendors, "VendorID", "VendorName", servicing.VendorID);
                       
            ViewBag.LocationID = new SelectList(db.Locations, "LocationID", "LocationName");
            ViewBag.CityID = new SelectList(db.Cities, "CityID", "CityName");
            ViewBag.StateID = new SelectList(db.States, "StateID", "State1");
            ViewBag.BranchID = new SelectList(db.Branches, "BranchID", "BranchName");
            ViewBag.TechnicianID = new SelectList(db.Employees.Where(x => x.DesignationID == 2), "EmployeeID", "Name");
            if (servicing.Servicing_Technician_Mapping.Count() > 0)
            {
                ViewBag.TechnicianIDsArray = servicing.Servicing_Technician_Mapping.Select(t => t.TechnicianID).ToArray();
            }
            else {
                ViewBag.TechnicianIDsArray = new int[1] {0};
            }
            if (servicing.Servicing_Chemicals_Issued.Count() > 0)
            {
                ViewBag.ChemicalsList = new SelectList(db.Chemicals_Master, "ChemicalID", "Chemical", servicing.Servicing_Chemicals_Issued.First().ChemicalID);
                ViewBag.UOMList = new SelectList(db.UOM_Master, "UOMID", "UOM", servicing.Servicing_Chemicals_Issued.First().UOMID);
                ViewBag.ChemicalTechnicianList = new SelectList(db.Employees.Where(x => x.DesignationID == 2), "EmployeeID", "Name", servicing.Servicing_Chemicals_Issued.First().TechnicianID);
                ViewBag.ChemQty = servicing.Servicing_Chemicals_Issued.First().ChemicalQty;
                
            }
            else {
                ViewBag.ChemicalsList = new SelectList(db.Chemicals_Master, "ChemicalID", "Chemical");
                ViewBag.UOMList = new SelectList(db.UOM_Master, "UOMID", "UOM");
                ViewBag.ChemicalTechnicianList = new SelectList(db.Employees.Where(x => x.DesignationID == 2), "EmployeeID", "Name");
                ViewBag.ChemQty = "";
            }
            ViewBag.ServiceStatusID = new SelectList(db.Service_Status_Master, "Service_Status_ID", "Service_Status");

            ViewBag.ServicePlusComplaintsCount = (from m in db.Servicings
                                                  where m.ServicingNo.Contains(servicing.ServicingNo)
                                                  select m).Count().ToString();
            var complaints = (from m in db.Servicings
                              where m.ServicingNo.Contains(servicing.ServicingNo) && m.ServicingID != id
                              select m).ToList();
            ViewBag.Complaints = complaints;
            //complaints.Include(c => c.Servicing_Technician_Mapping).Select(c => c.Servicing_Technician_Mapping).Select(map => map.)

            servicing.Servicing_Time = ((DateTime)servicing.Servicing_Datetime).TimeOfDay;


            var ChemicalsListForCheckBoxList = new SelectList(db.Chemicals_Master, "ChemicalID", "Chemical");

            if (servicing.Servicing_Chemicals_Issued.Count() > 0)
            {
                foreach (var item in (servicing.Servicing_Chemicals_Issued)){
                    string strChemicalID = item.ChemicalID.ToString();
                    ChemicalsListForCheckBoxList.Where(c => c.Value == strChemicalID).FirstOrDefault().Selected = true;
                }
            }
            ViewBag.ChemicalsListForCheckBoxList = ChemicalsListForCheckBoxList;

            ViewBag.UOMListForCheckBoxList = new SelectList(db.UOM_Master, "UOMID", "UOM");

            ViewBag.ExisitngChemicalsListWithUOMandQty = servicing.Servicing_Chemicals_Issued.Select(c => new { c.ChemicalID, c.ChemicalQty, c.UOMID }).ToArray();
            return View(servicing);
        }

        // POST: Servicings/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Servicing servicingParam, int[] TechnicianID, int? ChemicalID, int? ChemicalTechnicianID, string ChemicalQty, 
            byte? UOMID, DateTime? Servicing_Time, DateTime? Actual_Servicing_Time,
            bool[] ChemicalSelectedListBox
            , int[] ChemicalIDListBox
            , int[] UOMIDListBox
            , decimal[] ChemicalQtyListBox
            )
        {
                Servicing servicing = db.Servicings.Find(servicingParam.ServicingID);
                //Lead lead = db.Leads.Find(servicing.LeadID);
                servicing.TypeOfPremisesID = servicingParam.TypeOfPremisesID;
                servicing.PremisesArea = servicingParam.PremisesArea;
                servicing.PremisesAppSqFtArea = servicingParam.PremisesAppSqFtArea;
                if (servicingParam.Servicing_Datetime != null)
                {
                    servicing.Servicing_Datetime = servicingParam.Servicing_Datetime;
                    if (Servicing_Time != null)
                    {
                        servicing.Servicing_Datetime = ((DateTime)servicing.Servicing_Datetime).AddHours(((DateTime)Servicing_Time).Hour);
                        servicing.Servicing_Datetime = ((DateTime)servicing.Servicing_Datetime).AddMinutes(((DateTime)Servicing_Time).Minute);
                        servicing.Servicing_Time = ((DateTime)Servicing_Time).TimeOfDay;
                    }
                }
                if (servicingParam.Actual_Servicing_Datetime != null)
                {
                    servicing.Actual_Servicing_Datetime = servicingParam.Actual_Servicing_Datetime;
                    if (Actual_Servicing_Time != null)
                    {
                        servicing.Actual_Servicing_Datetime = ((DateTime)servicing.Actual_Servicing_Datetime).AddHours(((DateTime)Actual_Servicing_Time).Hour);
                        servicing.Actual_Servicing_Datetime = ((DateTime)servicing.Actual_Servicing_Datetime).AddMinutes(((DateTime)Actual_Servicing_Time).Minute);
                        servicing.Actual_Servicing_Time = ((DateTime)Actual_Servicing_Time).TimeOfDay;
                }
                }
                servicing.VendorID = servicingParam.VendorID;
                servicing.PaymentModeID = servicingParam.PaymentModeID;
                servicing.PaymentTermID = servicingParam.PaymentTermID;
                servicing.PaidByCustomer = servicingParam.PaidByCustomer;
                servicing.BalanceAmount = servicingParam.BalanceAmount;
                servicing.Service_Status_ID = servicingParam.Service_Status_ID;
                servicing.ServicingInstructions = servicingParam.ServicingInstructions;
                servicing.LastUpdatedBy = Convert.ToInt32(Session["UserID"]);
                servicing.LastUpdatedDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, INDIAN_ZONE);

                if (servicing.Servicing_Datetime != null && (TechnicianID != null || servicing.VendorID != null) && servicing.ActualServiceID == null) {
                    var branchShortCode = db.Branches.Find(db.Leads.Find(servicing.LeadID).BranchID).BranchShortCode;
                    DateTime today = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, INDIAN_ZONE);
                    var year = today.ToString("yy");
                    var totalServicesAssignedBeforeThisServiceDateTime = db.Servicings.Where(s => s.ActualServiceID != null && s.Servicing_Datetime < servicing.Servicing_Datetime).Count();
                    servicing.ActualServiceID = "S/" + branchShortCode + year + String.Format("{0:D4}", (totalServicesAssignedBeforeThisServiceDateTime + 1));
                }
                
                db.Entry(servicing).State = EntityState.Modified;
                db.SaveChanges();
                try
                {
                    if (TechnicianID != null)
                    {
                        var existingTechnicians = db.Servicing_Technician_Mapping.Where(t => t.ServicingID == servicing.ServicingID).Select(t => t.TechnicianID);
                        for (int i = 0; i < TechnicianID.Length; i++)
                        {
                            if (!existingTechnicians.Contains(TechnicianID[i]))
                            {
                                Servicing_Technician_Mapping servTech = new Servicing_Technician_Mapping();
                                servTech.ServicingID = servicing.ServicingID;
                                servTech.TechnicianID = TechnicianID[i];
                                db.Servicing_Technician_Mapping.Add(servTech);
                                db.SaveChanges();
                            }

                        }
                    }
                }
                catch (Exception ex)
                {

                }
                try
                {
                var existingChemicalsIssued = db.Servicing_Chemicals_Issued.Where(c => c.ServicingID == servicing.ServicingID);
                db.Servicing_Chemicals_Issued.RemoveRange(existingChemicalsIssued);
                db.SaveChanges();
                //Because Checked Checkboxes submit 2 values True, False. So add checkbox in List and skip next false duplicate item
                List<bool> ChemicalSelectedListBoxInTheFormOfList = new List<bool>();
                    for (int i = 0; i < ChemicalSelectedListBox.Length; i++) {
                        ChemicalSelectedListBoxInTheFormOfList.Add(ChemicalSelectedListBox[i]);
                        if (ChemicalSelectedListBox[i]) {
                            i++;
                        }
                    }
                    for (int i = 0; i < ChemicalSelectedListBoxInTheFormOfList.Count(); i++)
                    {
                        if (ChemicalSelectedListBoxInTheFormOfList.ElementAt(i)) {
                            Servicing_Chemicals_Issued servChem = new Servicing_Chemicals_Issued();
                            servChem.ServicingID = servicing.ServicingID;
                            servChem.ChemicalID = ChemicalIDListBox[i];
                            servChem.TechnicianID = (int)ChemicalTechnicianID;
                            if (UOMIDListBox[i] == 0)
                            {

                                servChem.UOMID = null;
                            }
                            else
                            {
                                servChem.UOMID = (byte)UOMIDListBox[i];
                            }


                            servChem.ChemicalQty = ChemicalQtyListBox[i];
                            servChem.CreatedDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, INDIAN_ZONE);
                            db.Servicing_Chemicals_Issued.Add(servChem);
                        }
                    
                        //db.DbccCheckIdent<Servicing_Chemicals_Issued>(db.Servicing_Chemicals_Issued.Max(p => p.Servicing_Chemicals_Issued_ID));
                        db.SaveChanges();

                    }
                //if (ChemicalID != null)
                //{
                //    Servicing_Chemicals_Issued servChem = new Servicing_Chemicals_Issued();
                //    servChem.ServicingID = servicing.ServicingID;
                //    servChem.ChemicalID = (int)ChemicalID;
                //    servChem.TechnicianID = (int)ChemicalTechnicianID;
                //    servChem.UOMID = (byte)UOMID;
                //    servChem.ChemicalQty = Decimal.Parse(ChemicalQty);
                //    servChem.CreatedDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, INDIAN_ZONE);
                //    db.Servicing_Chemicals_Issued.Add(servChem);
                //    db.SaveChanges();
                //}
            }
                catch (Exception ex)
                {
                }



                return RedirectToAction("DatewiseServiceSchedule");
            
            ViewBag.CustomerID = new SelectList(db.Customers, "CustomerID", "CustomerName", servicingParam.CustomerID);
            ViewBag.FrequencyOfServiceID = new SelectList(db.FrequencyOfServices, "FrequencyOfServiceID", "FrequencyOfService1", servicingParam.FrequencyOfServiceID);
            ViewBag.PaymentModeID = new SelectList(db.PaymentModes, "PaymentModeID", "PaymentModeName", servicingParam.PaymentModeID);
            ViewBag.PaymentTermID = new SelectList(db.PaymentTerms, "PaymentTermID", "PaymentTermName", servicingParam.PaymentTermID);
            ViewBag.TypeOfPremisesID = new SelectList(db.PremisesTypes, "PremisesTypeID", "PremisesType1", servicingParam.TypeOfPremisesID);
            ViewBag.ServiceGroupID = new SelectList(db.ServiceGroups, "ServiceGroupID", "ServiceGroup1", servicingParam.ServiceGroupID);
            ViewBag.ServiceID = new SelectList(db.Services, "ServiceID", "ServiceName", servicingParam.ServiceID);
            ViewBag.CreatedBy = new SelectList(db.UserLogins, "UserID", "UserName", servicingParam.CreatedBy);
            ViewBag.LastUpdatedBy = new SelectList(db.UserLogins, "UserID", "UserName", servicingParam.LastUpdatedBy);
            ViewBag.VendorID = new SelectList(db.Vendors, "VendorID", "VendorName", servicingParam.VendorID);
            return View(servicingParam);
        }

        // GET: Servicings/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Servicing servicing = db.Servicings.Find(id);
            if (servicing == null)
            {
                return HttpNotFound();
            }
            return View(servicing);
        }

        // POST: Servicings/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Servicing servicing = db.Servicings.Find(id);
            db.Servicings.Remove(servicing);
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

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using SadguruCRM.Helpers;
using SadguruCRM.Models;

namespace SadguruCRM.Controllers
{
    [VerifyUser]
    public class LeadsHistoryController : Controller
    {
        System.Diagnostics.Stopwatch watch = new System.Diagnostics.Stopwatch();
        private SadguruCRMEntities db = new SadguruCRMEntities();
        private static TimeZoneInfo INDIAN_ZONE = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");

        // GET: LeadsHistory
        public ActionResult Index()
        {
            var leads_History = db.Leads_History.Include(l => l.Branch).Include(l => l.City).Include(l => l.Employee).Include(l => l.UserLogin).Include(l => l.LeadStatus).Include(l => l.Location).Include(l => l.Premises_Area_Master).Include(l => l.Source).Include(l => l.State).Include(l => l.Source1).Include(l => l.UserLogin1).Include(l => l.PremisesType);
            return View(leads_History.ToList());
        }
        public ActionResult History(int? LeadID)
        {
            if (TempData["ErrorMessage"] != null)
            {
                ViewBag.ErrorMessage = TempData["ErrorMessage"].ToString();
            }
            if (!watch.IsRunning)
            {
                watch = System.Diagnostics.Stopwatch.StartNew();
            }
            var leads_History = db.Leads_History.Where(x => x.LeadID == LeadID).Include(l => l.Branch).Include(l => l.City).Include(l => l.Employee).Include(l => l.UserLogin).Include(l => l.LeadStatus).Include(l => l.Location).Include(l => l.Premises_Area_Master).Include(l => l.Source).Include(l => l.State).Include(l => l.Source1).Include(l => l.UserLogin1).Include(l => l.PremisesType).OrderBy(l => l.History_Created_On);
            foreach (var item in leads_History)
            {
                if (item.CustomerName == null)
                {
                    item.CustomerName = item.Title + " " + item.FirstName + " " + item.LastName;
                }
                
            }
            var Lead = db.Leads.Find(LeadID);
            ViewBag.LeadID = LeadID;
            if (Lead.VisitAllocate != null) { ViewBag.VisitAllocate = new SelectList(db.Employees, "EmployeeID", "Name", db.Leads.Find(LeadID).VisitAllocate); }
            else { ViewBag.VisitAllocate = new SelectList(db.Employees, "EmployeeID", "Name"); }
            
            ViewBag.LeadStatusID = new SelectList(db.LeadStatuses, "StatusID", "Status",db.Leads.Find(LeadID).LeadStatusID);

            var listOfServiceIDs = db.Lead_Services_Mapping.Where(x => x.LeadID == LeadID).Select(c => c.ServiceID).ToList();
            var services = db.Services.ToList();
            ViewBag.ServicesShortCodes = String.Join(", ", services.Where(x => listOfServiceIDs.Contains(x.ServiceID)).Select(x => x.ServiceShortCode).ToArray());
            ViewBag.FinalRate = Lead.FinalRate;

            watch.Stop();
            long totalTime;
            if (TempData["TimeTaken"] != null)
            {
                totalTime = Int64.Parse(TempData["TimeTaken"].ToString()) + watch.ElapsedMilliseconds;
            }
            else
            {
                totalTime = watch.ElapsedMilliseconds;
            }
            ViewBag.TimeTaken = totalTime;
            return View(leads_History.ToList());
        }
        [HttpPost]
        public ActionResult History(int LeadID, string FollowUpDetails, string NextFollowUpDate, string NextFollowUpTime, int? VisitAllocate, string VisitDate, string VisitTime, DateTime? ServiceDate, string ServiceTime, int LeadStatusID, string StatusReason)
        {
            if (TempData["ErrorMessage"] != null)
            {
                ViewBag.ErrorMessage = TempData["ErrorMessage"].ToString();
            }
            if (!watch.IsRunning)
            {
                watch = System.Diagnostics.Stopwatch.StartNew();
            }
            var result = db.Leads.SingleOrDefault(b => b.LeadID == LeadID);
                if (result != null)
                {  
                        if (!String.IsNullOrEmpty(NextFollowUpDate))
                        {
                            result.NextFollowUpDateTime = Convert.ToDateTime(NextFollowUpDate);
                            if (!String.IsNullOrEmpty(NextFollowUpTime))
                            {
                                result.NextFollowUpTime = TimeSpan.Parse((DateTime.Parse(NextFollowUpTime)).ToString("HH:mm"));
                                result.NextFollowUpDateTime += result.NextFollowUpTime;
                            }
                        }
                        if (!String.IsNullOrEmpty(VisitDate))
                        {
                            result.VisitDateTime = Convert.ToDateTime(VisitDate);
                            if (!String.IsNullOrEmpty(VisitTime))
                            {
                                result.VisitTime = TimeSpan.Parse((DateTime.Parse(VisitTime)).ToString("HH:mm"));
                                result.VisitDateTime += result.VisitTime;
                            }

                        }
                        if (!String.IsNullOrEmpty(ServiceDate.ToString()))
                        {
                            result.ServiceDate = ServiceDate;
                            if (!String.IsNullOrEmpty(ServiceTime))
                            {
                                result.ServiceTime = TimeSpan.Parse((DateTime.Parse(ServiceTime)).ToString("HH:mm"));
                                result.ServiceDate += result.ServiceTime;
                            }
                        }
                
                        if (!String.IsNullOrEmpty(FollowUpDetails)) { result.FollowUpDetails = FollowUpDetails; }
                        result.VisitAllocate = VisitAllocate;
                        result.LeadStatusID = LeadStatusID;
                        result.LastUpdatedDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, INDIAN_ZONE);
                        result.LastUpdatedBy = Convert.ToInt32(Session["UserID"]);
                        if (!String.IsNullOrEmpty(StatusReason)) { result.StatusReason = StatusReason; }
                
                        db.SaveChanges();
                }
            

            var leads_History = db.Leads_History.Where(x => x.LeadID == LeadID).Include(l => l.Branch).Include(l => l.City).Include(l => l.Employee).Include(l => l.UserLogin).Include(l => l.LeadStatus).Include(l => l.Location).Include(l => l.Premises_Area_Master).Include(l => l.Source).Include(l => l.State).Include(l => l.Source1).Include(l => l.UserLogin1).Include(l => l.PremisesType).OrderBy(l => l.History_Created_On);
            foreach (var item in leads_History)
            {
                if (item.CustomerName == null)
                {
                    item.CustomerName = item.Title + " " + item.FirstName + " " + item.LastName;
                }

            }
            ViewBag.LeadID = LeadID;
            ViewBag.VisitAllocate = new SelectList(db.Employees, "EmployeeID", "Name");
            ViewBag.LeadStatusID = new SelectList(db.LeadStatuses, "StatusID", "Status");
            watch.Stop();
            long totalTime;
            if (TempData["TimeTaken"] != null)
            {
                totalTime = Int64.Parse(TempData["TimeTaken"].ToString()) + watch.ElapsedMilliseconds;
            }
            else
            {
                totalTime = watch.ElapsedMilliseconds;
            }
            ViewBag.TimeTaken = totalTime;
            return View(leads_History.ToList());
        }
        // GET: LeadsHistory/Details/5
        public ActionResult Details(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Leads_History leads_History = db.Leads_History.Find(id);
            if (leads_History == null)
            {
                return HttpNotFound();
            }
            return View(leads_History);
        }

        // GET: LeadsHistory/Create
        public ActionResult Create()
        {
            ViewBag.BranchID = new SelectList(db.Branches, "BranchID", "BranchName");
            ViewBag.CityID = new SelectList(db.Cities, "CityID", "CityName");
            ViewBag.VisitAllocate = new SelectList(db.Employees, "EmployeeID", "Name");
            ViewBag.CreatedBy = new SelectList(db.UserLogins, "UserID", "UserName");
            ViewBag.LeadStatusID = new SelectList(db.LeadStatuses, "StatusID", "Status");
            ViewBag.LocationID = new SelectList(db.Locations, "LocationID", "LocationName");
            ViewBag.Premises_Area_ID = new SelectList(db.Premises_Area_Master, "Premises_Area_ID", "Premises_Area");
            ViewBag.SourceID = new SelectList(db.Sources, "SourceID", "Source1");
            ViewBag.StateID = new SelectList(db.States, "StateID", "State1");
            ViewBag.SubSourceID = new SelectList(db.Sources, "SourceID", "Source1");
            ViewBag.TeleCallerID = new SelectList(db.UserLogins, "UserID", "UserName");
            ViewBag.TypeOfPremisesID = new SelectList(db.PremisesTypes, "PremisesTypeID", "PremisesType1");
            return View();
        }

        // POST: LeadsHistory/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Leads_History_ID,LeadID,LeadDate,Title,CustomerName,TelNo,CellNo,Pincode,CityID,EmailID,SourceID,SubSourceID,TypeOfPremisesID,TeleCallerID,BranchID,VisitAllocate,VisitDateTime,VisitReport,RATE,TAX,FinalRate,CustomerPriority,NextFollowUpDateTime,SpecialInstructions,FollowUpDetails,LeadStatusID,CreatedDate,LastUpdatedDate,LocationID,AddressLine1,AddressLine2,AddressLine3,FirstName,LastName,NoOfWings,NoOfFloors,LeasdClosedDateTime,CreatedBy,LastUpdatedBy,ConsultPerson,ConsultPersonDesignation,Industry,PremisesArea,PremisesAppSqFtArea,StatusReason,StateID,Premises_Area_ID,ServiceDate,ServiceTime,VisitTime,NextFollowUpTime,NoOfFlats,History_Created_On")] Leads_History leads_History)
        {
            if (ModelState.IsValid)
            {
                db.Leads_History.Add(leads_History);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.BranchID = new SelectList(db.Branches, "BranchID", "BranchName", leads_History.BranchID);
            ViewBag.CityID = new SelectList(db.Cities, "CityID", "CityName", leads_History.CityID);
            ViewBag.VisitAllocate = new SelectList(db.Employees, "EmployeeID", "Name", leads_History.VisitAllocate);
            ViewBag.CreatedBy = new SelectList(db.UserLogins, "UserID", "UserName", leads_History.CreatedBy);
            ViewBag.LeadStatusID = new SelectList(db.LeadStatuses, "StatusID", "Status", leads_History.LeadStatusID);
            ViewBag.LocationID = new SelectList(db.Locations, "LocationID", "LocationName", leads_History.LocationID);
            ViewBag.Premises_Area_ID = new SelectList(db.Premises_Area_Master, "Premises_Area_ID", "Premises_Area", leads_History.Premises_Area_ID);
            ViewBag.SourceID = new SelectList(db.Sources, "SourceID", "Source1", leads_History.SourceID);
            ViewBag.StateID = new SelectList(db.States, "StateID", "State1", leads_History.StateID);
            ViewBag.SubSourceID = new SelectList(db.Sources, "SourceID", "Source1", leads_History.SubSourceID);
            ViewBag.TeleCallerID = new SelectList(db.UserLogins, "UserID", "UserName", leads_History.TeleCallerID);
            ViewBag.TypeOfPremisesID = new SelectList(db.PremisesTypes, "PremisesTypeID", "PremisesType1", leads_History.TypeOfPremisesID);
            return View(leads_History);
        }

        // GET: LeadsHistory/Edit/5
        public ActionResult Edit(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Leads_History leads_History = db.Leads_History.Find(id);
            if (leads_History == null)
            {
                return HttpNotFound();
            }
            ViewBag.BranchID = new SelectList(db.Branches, "BranchID", "BranchName", leads_History.BranchID);
            ViewBag.CityID = new SelectList(db.Cities, "CityID", "CityName", leads_History.CityID);
            ViewBag.VisitAllocate = new SelectList(db.Employees, "EmployeeID", "Name", leads_History.VisitAllocate);
            ViewBag.CreatedBy = new SelectList(db.UserLogins, "UserID", "UserName", leads_History.CreatedBy);
            ViewBag.LeadStatusID = new SelectList(db.LeadStatuses, "StatusID", "Status", leads_History.LeadStatusID);
            ViewBag.LocationID = new SelectList(db.Locations, "LocationID", "LocationName", leads_History.LocationID);
            ViewBag.Premises_Area_ID = new SelectList(db.Premises_Area_Master, "Premises_Area_ID", "Premises_Area", leads_History.Premises_Area_ID);
            ViewBag.SourceID = new SelectList(db.Sources, "SourceID", "Source1", leads_History.SourceID);
            ViewBag.StateID = new SelectList(db.States, "StateID", "State1", leads_History.StateID);
            ViewBag.SubSourceID = new SelectList(db.Sources, "SourceID", "Source1", leads_History.SubSourceID);
            ViewBag.TeleCallerID = new SelectList(db.UserLogins, "UserID", "UserName", leads_History.TeleCallerID);
            ViewBag.TypeOfPremisesID = new SelectList(db.PremisesTypes, "PremisesTypeID", "PremisesType1", leads_History.TypeOfPremisesID);
            return View(leads_History);
        }

        // POST: LeadsHistory/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Leads_History_ID,LeadID,LeadDate,Title,CustomerName,TelNo,CellNo,Pincode,CityID,EmailID,SourceID,SubSourceID,TypeOfPremisesID,TeleCallerID,BranchID,VisitAllocate,VisitDateTime,VisitReport,RATE,TAX,FinalRate,CustomerPriority,NextFollowUpDateTime,SpecialInstructions,FollowUpDetails,LeadStatusID,CreatedDate,LastUpdatedDate,LocationID,AddressLine1,AddressLine2,AddressLine3,FirstName,LastName,NoOfWings,NoOfFloors,LeasdClosedDateTime,CreatedBy,LastUpdatedBy,ConsultPerson,ConsultPersonDesignation,Industry,PremisesArea,PremisesAppSqFtArea,StatusReason,StateID,Premises_Area_ID,ServiceDate,ServiceTime,VisitTime,NextFollowUpTime,NoOfFlats,History_Created_On")] Leads_History leads_History)
        {
            if (ModelState.IsValid)
            {
                db.Entry(leads_History).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.BranchID = new SelectList(db.Branches, "BranchID", "BranchName", leads_History.BranchID);
            ViewBag.CityID = new SelectList(db.Cities, "CityID", "CityName", leads_History.CityID);
            ViewBag.VisitAllocate = new SelectList(db.Employees, "EmployeeID", "Name", leads_History.VisitAllocate);
            ViewBag.CreatedBy = new SelectList(db.UserLogins, "UserID", "UserName", leads_History.CreatedBy);
            ViewBag.LeadStatusID = new SelectList(db.LeadStatuses, "StatusID", "Status", leads_History.LeadStatusID);
            ViewBag.LocationID = new SelectList(db.Locations, "LocationID", "LocationName", leads_History.LocationID);
            ViewBag.Premises_Area_ID = new SelectList(db.Premises_Area_Master, "Premises_Area_ID", "Premises_Area", leads_History.Premises_Area_ID);
            ViewBag.SourceID = new SelectList(db.Sources, "SourceID", "Source1", leads_History.SourceID);
            ViewBag.StateID = new SelectList(db.States, "StateID", "State1", leads_History.StateID);
            ViewBag.SubSourceID = new SelectList(db.Sources, "SourceID", "Source1", leads_History.SubSourceID);
            ViewBag.TeleCallerID = new SelectList(db.UserLogins, "UserID", "UserName", leads_History.TeleCallerID);
            ViewBag.TypeOfPremisesID = new SelectList(db.PremisesTypes, "PremisesTypeID", "PremisesType1", leads_History.TypeOfPremisesID);
            return View(leads_History);
        }

        // GET: LeadsHistory/Delete/5
        public ActionResult Delete(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Leads_History leads_History = db.Leads_History.Find(id);
            if (leads_History == null)
            {
                return HttpNotFound();
            }
            return View(leads_History);
        }

        // POST: LeadsHistory/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(long id)
        {
            Leads_History leads_History = db.Leads_History.Find(id);
            db.Leads_History.Remove(leads_History);
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

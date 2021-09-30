using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using SadguruCRM.Models;

namespace SadguruCRM.Controllers
{
    public class ServiceEstimateTemplatesController : Controller
    {
        private SadguruCRMEntities db = new SadguruCRMEntities();
        private static TimeZoneInfo INDIAN_ZONE = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");

        // GET: ServiceEstimateTemplates
        public ActionResult Index()
        {
            var service_Estimate_Templates = db.Service_Estimate_Templates.Include(s => s.UserLogin).Include(s => s.Service);
            return View(service_Estimate_Templates.ToList());
        }

        // GET: ServiceEstimateTemplates/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Service_Estimate_Templates service_Estimate_Templates = db.Service_Estimate_Templates.Find(id);
            if (service_Estimate_Templates == null)
            {
                return HttpNotFound();
            }
            return View(service_Estimate_Templates);
        }

        // GET: ServiceEstimateTemplates/Create
        public ActionResult Create()
        {
            ViewBag.CreatedByUserID = new SelectList(db.UserLogins, "UserID", "UserName");
            ViewBag.ServiceGroupID = new SelectList(db.ServiceGroups, "ServiceGroupID", "ServiceGroup1");
            ViewBag.ServiceID = new SelectList(db.Services, "ServiceID", "ServiceName");
            return View();
        }

        // POST: ServiceEstimateTemplates/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Service_Estimate_Template_ID,ServiceGroupID,ServiceID,Subject,General_Instruction,General_Disinfestation,Frequency_Of_Services,Charges_Per_Annum,Terms_Of_Payment")] Service_Estimate_Templates service_Estimate_Templates)
        {
            if (ModelState.IsValid)
            {
                service_Estimate_Templates.CreatedOn = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, INDIAN_ZONE);
                service_Estimate_Templates.CreatedByUserID = Convert.ToInt32(Session["UserID"]);
                db.Service_Estimate_Templates.Add(service_Estimate_Templates);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.CreatedByUserID = new SelectList(db.UserLogins, "UserID", "UserName", service_Estimate_Templates.CreatedByUserID);
            ViewBag.ServiceID = new SelectList(db.Services, "ServiceID", "ServiceName", service_Estimate_Templates.ServiceID);
            return View(service_Estimate_Templates);
        }
        [HttpPost]
        public ActionResult GetServices(string ServiceGroupID)
        {
            int GroupID;
            List<SelectListItem> servicesList = new List<SelectListItem>();
            if (!string.IsNullOrEmpty(ServiceGroupID))
            {
                GroupID = Convert.ToInt32(ServiceGroupID);
                List<Service> services = db.Services.Where(x => x.ServiceGroupID == GroupID).ToList();
                services.ForEach(x =>
                {
                    servicesList.Add(new SelectListItem { Text = x.ServiceName, Value = x.ServiceID.ToString() });
                });
            }
            return Json(servicesList, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult CheckExistingServiceTemplate(string ServiceID)
        {
            int ServiceToCheckID;
            //List<SelectListItem> servicesList = new List<SelectListItem>();
            if (!string.IsNullOrEmpty(ServiceID))
            {
                ServiceToCheckID = Convert.ToInt32(ServiceID);
                var template = db.Service_Estimate_Templates.Where(x => x.ServiceID == ServiceToCheckID).FirstOrDefault();
                if(template != null)
                {
                    return Json("AlreadyExists", JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json("NoRecord", JsonRequestBehavior.AllowGet);
                }
            }
            return Json("", JsonRequestBehavior.AllowGet);
        }
        // GET: ServiceEstimateTemplates/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Service_Estimate_Templates service_Estimate_Templates = db.Service_Estimate_Templates.Find(id);
            if (service_Estimate_Templates == null)
            {
                return HttpNotFound();
            }
            ViewBag.CreatedByUserID = new SelectList(db.UserLogins, "UserID", "UserName", service_Estimate_Templates.CreatedByUserID);
            ViewBag.ServiceGroupID = new SelectList(db.ServiceGroups, "ServiceGroupID", "ServiceGroup1", service_Estimate_Templates.ServiceGroupID);
            ViewBag.ServiceID = new SelectList(db.Services, "ServiceID", "ServiceName", service_Estimate_Templates.ServiceID);
            return View(service_Estimate_Templates);
        }

        // POST: ServiceEstimateTemplates/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Service_Estimate_Template_ID,ServiceGroupID,ServiceID,Subject,General_Instruction,General_Disinfestation,Frequency_Of_Services,Charges_Per_Annum,Terms_Of_Payment,CreatedOn,CreatedByUserID,LastUpdatedOn,LastUpdatedByUserID")] Service_Estimate_Templates service_Estimate_Templates)
        {
            if (ModelState.IsValid)
            {
                service_Estimate_Templates.LastUpdatedByUserID = Convert.ToInt32(Session["UserID"]);
                service_Estimate_Templates.LastUpdatedOn = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, INDIAN_ZONE);
                db.Entry(service_Estimate_Templates).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CreatedByUserID = new SelectList(db.UserLogins, "UserID", "UserName", service_Estimate_Templates.CreatedByUserID);
            ViewBag.ServiceID = new SelectList(db.Services, "ServiceID", "ServiceName", service_Estimate_Templates.ServiceID);
            return View(service_Estimate_Templates);
        }

        // GET: ServiceEstimateTemplates/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Service_Estimate_Templates service_Estimate_Templates = db.Service_Estimate_Templates.Find(id);
            if (service_Estimate_Templates == null)
            {
                return HttpNotFound();
            }
            return View(service_Estimate_Templates);
        }

        // POST: ServiceEstimateTemplates/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Service_Estimate_Templates service_Estimate_Templates = db.Service_Estimate_Templates.Find(id);
            db.Service_Estimate_Templates.Remove(service_Estimate_Templates);
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

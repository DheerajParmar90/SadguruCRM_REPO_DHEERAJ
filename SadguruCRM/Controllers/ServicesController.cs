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
    public class ServicesController : Controller
    {
        private SadguruCRMEntities db = new SadguruCRMEntities();

        private static TimeZoneInfo INDIAN_ZONE = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");

        // GET: Services
        public ActionResult Index()
        {
            var services = db.Services.OrderBy(x => x.Order).Include(s => s.ServiceGroup);
            return View(services.ToList());
        }

        // GET: Services/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Service service = db.Services.Find(id);
            if (service == null)
            {
                return HttpNotFound();
            }
            return View(service);
        }

        // GET: Services/Create
        public ActionResult Create()
        {
            ViewBag.ServiceGroupID = new SelectList(db.ServiceGroups, "ServiceGroupID", "ServiceGroup1");
            ViewBag.ServiceSubGroupID = new SelectList(db.ServiceGroups, "ServiceGroupID", "ServiceGroup1");
            return View();
        }

        // POST: Services/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ServiceID,ServiceName,ServiceShortCode,GST,SACCode,ServiceGroupID,ServiceSubGroupID,Order")] Service service)
        {
            if (ModelState.IsValid)
            {
                service.CreatedDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, INDIAN_ZONE);
                db.Services.Add(service);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.ServiceGroupID = new SelectList(db.ServiceGroups, "ServiceGroupID", "ServiceGroup1", service.ServiceGroupID);
            ViewBag.ServiceSubGroupID = new SelectList(db.ServiceGroups, "ServiceGroupID", "ServiceGroup1", service.ServiceSubGroupID);
            return View(service);
        }

        // GET: Services/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Service service = db.Services.Find(id);
            if (service == null)
            {
                return HttpNotFound();
            }
            ViewBag.ServiceGroupID = new SelectList(db.ServiceGroups, "ServiceGroupID", "ServiceGroup1", service.ServiceGroupID);
            ViewBag.ServiceSubGroupID = new SelectList(db.ServiceGroups, "ServiceGroupID", "ServiceGroup1", service.ServiceSubGroupID);
            return View(service);
        }

        // POST: Services/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ServiceID,ServiceName,ServiceShortCode,GST,SACCode,ServiceGroupID,ServiceSubGroupID,CreatedDate,Order")] Service service)
        {
            if (ModelState.IsValid)
            {
                service.LastUpdateddate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, INDIAN_ZONE);
                db.Entry(service).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ServiceGroupID = new SelectList(db.ServiceGroups, "ServiceGroupID", "ServiceGroup1", service.ServiceGroupID);
            ViewBag.ServiceSubGroupID = new SelectList(db.ServiceGroups, "ServiceGroupID", "ServiceGroup1", service.ServiceSubGroupID);
            return View(service);
        }

        // GET: Services/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Service service = db.Services.Find(id);
            if (service == null)
            {
                return HttpNotFound();
            }
            return View(service);
        }

        // POST: Services/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Service service = db.Services.Find(id);
            db.Services.Remove(service);
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
        public ActionResult GetServiceScope(string ServiceID)
        {
            int intOutServiceID;
            string serviceScope = "NA";
            if (!string.IsNullOrEmpty(ServiceID))
            {
                if (int.TryParse(ServiceID, out intOutServiceID)) {
                    var serviceScopeAsList = db.Service_Scope_Master.Where(s => s.ServiceID == intOutServiceID).Take(1).Select(s => s.Service_Scope).ToList();
                    if (serviceScopeAsList.Count > 0)
                    {
                        serviceScope = serviceScopeAsList.First();
                    }
                }

                
                return Json(serviceScope, JsonRequestBehavior.AllowGet);
            }
            else
            {

                return Json("Wrong Service ID", JsonRequestBehavior.AllowGet);
            }

        }
    }
}

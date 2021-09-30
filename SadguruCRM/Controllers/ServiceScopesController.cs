using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using SadguruCRM.Helpers;
using SadguruCRM.Models;

namespace SadguruCRM.Controllers
{
    [VerifyUser]
    public class ServiceScopesController : Controller
    {
        private SadguruCRMEntities db = new SadguruCRMEntities();
        private static TimeZoneInfo INDIAN_ZONE = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");

        // GET: ServiceScopes
        public ActionResult Index()
        {
            var service_Scope_Master = db.Service_Scope_Master.Include(s => s.UserLogin).Include(s => s.Service).Include(s => s.ServiceGroup);
            return View(service_Scope_Master.ToList());
        }

        // GET: ServiceScopes/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Service_Scope_Master service_Scope_Master = db.Service_Scope_Master.Find(id);
            if (service_Scope_Master == null)
            {
                return HttpNotFound();
            }
            return View(service_Scope_Master);
        }

        // GET: ServiceScopes/Create
        public ActionResult Create()
        {
            ViewBag.CreatedByUserID = new SelectList(db.UserLogins, "UserID", "UserName");
            ViewBag.ServiceID = new SelectList(db.Services.Where(x => x.ServiceID == 0), "ServiceID", "ServiceName");
            ViewBag.ServiceGroupID = new SelectList(db.ServiceGroups, "ServiceGroupID", "ServiceGroup1");
            return View();
        }

        // POST: ServiceScopes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Create([Bind(Include = "Service_Scope_ID,ServiceGroupID,ServiceID,Service_Scope")] Service_Scope_Master service_Scope_Master)
        {
            if (ModelState.IsValid)
            {
                service_Scope_Master.CreatedOn = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, INDIAN_ZONE);
                service_Scope_Master.CreatedByUserID = Convert.ToInt32(Session["UserID"]);
                db.Service_Scope_Master.Add(service_Scope_Master);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.CreatedByUserID = new SelectList(db.UserLogins, "UserID", "UserName", service_Scope_Master.CreatedByUserID);
            ViewBag.ServiceID = new SelectList(db.Services, "ServiceID", "ServiceName", service_Scope_Master.ServiceID);
            ViewBag.ServiceGroupID = new SelectList(db.ServiceGroups, "ServiceGroupID", "ServiceGroup1", service_Scope_Master.ServiceGroupID);
            return View(service_Scope_Master);
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

        // GET: ServiceScopes/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Service_Scope_Master service_Scope_Master = db.Service_Scope_Master.Find(id);
            if (service_Scope_Master == null)
            {
                return HttpNotFound();
            }
            
            ViewBag.ServiceGroupID = new SelectList(db.ServiceGroups, "ServiceGroupID", "ServiceGroup1", service_Scope_Master.ServiceGroupID);
            ViewBag.ServiceID = new SelectList(db.Services.Where(x => x.ServiceGroupID == service_Scope_Master.ServiceGroupID), "ServiceID", "ServiceName", service_Scope_Master.ServiceID);
            return View(service_Scope_Master);
        }

        // POST: ServiceScopes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Edit([Bind(Include = "Service_Scope_ID,ServiceGroupID,ServiceID,Service_Scope,CreatedOn,CreatedByUserID")] Service_Scope_Master service_Scope_Master)
        {
            if (ModelState.IsValid)
            {
                service_Scope_Master.LastUpdatedOn = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, INDIAN_ZONE);
                service_Scope_Master.LastUpdatedByUserID = Convert.ToInt32(Session["UserID"]);
                db.Entry(service_Scope_Master).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CreatedByUserID = new SelectList(db.UserLogins, "UserID", "UserName", service_Scope_Master.CreatedByUserID);
            ViewBag.ServiceID = new SelectList(db.Services, "ServiceID", "ServiceName", service_Scope_Master.ServiceID);
            ViewBag.ServiceGroupID = new SelectList(db.ServiceGroups, "ServiceGroupID", "ServiceGroup1", service_Scope_Master.ServiceGroupID);
            return View(service_Scope_Master);
        }

        // GET: ServiceScopes/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Service_Scope_Master service_Scope_Master = db.Service_Scope_Master.Find(id);
            if (service_Scope_Master == null)
            {
                return HttpNotFound();
            }
            return View(service_Scope_Master);
        }

        // POST: ServiceScopes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Service_Scope_Master service_Scope_Master = db.Service_Scope_Master.Find(id);
            db.Service_Scope_Master.Remove(service_Scope_Master);
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

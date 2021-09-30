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
    public class ShortServiceScopeController : Controller
    {
        private SadguruCRMEntities db = new SadguruCRMEntities();

        private static TimeZoneInfo INDIAN_ZONE = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");

        // GET: ShortServiceScope
        public ActionResult Index()
        {
            return View(db.Short_Service_Scope_Master.ToList());
        }

        // GET: ShortServiceScope/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Short_Service_Scope_Master short_Service_Scope_Master = db.Short_Service_Scope_Master.Find(id);
            if (short_Service_Scope_Master == null)
            {
                return HttpNotFound();
            }
            return View(short_Service_Scope_Master);
        }

        // GET: ShortServiceScope/Create
        public ActionResult Create()
        {

            ViewBag.ServiceGroupID = new SelectList(db.ServiceGroups, "ServiceGroupID", "ServiceGroup1");
            ViewBag.ServiceID = new SelectList(db.Services.Where(x => x.ServiceID == 0), "ServiceID", "ServiceName");
            return View();
        }

        // POST: ShortServiceScope/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Short_Service_Scope_ID,Short_Service_Scope,Order,ServiceGroupID,ServiceID")] Short_Service_Scope_Master short_Service_Scope_Master)
        {
            if (ModelState.IsValid)
            {
                short_Service_Scope_Master.CreatedDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, INDIAN_ZONE);
                db.Short_Service_Scope_Master.Add(short_Service_Scope_Master);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(short_Service_Scope_Master);
        }

        // GET: ShortServiceScope/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
;
            Short_Service_Scope_Master short_Service_Scope_Master = db.Short_Service_Scope_Master.Find(id);

            ViewBag.ServiceGroupID = new SelectList(db.ServiceGroups, "ServiceGroupID", "ServiceGroup1", short_Service_Scope_Master.ServiceGroupID);
            ViewBag.ServiceID = new SelectList(db.Services.Where(x => x.ServiceGroupID == short_Service_Scope_Master.ServiceGroupID), "ServiceID", "ServiceName", short_Service_Scope_Master.ServiceID);
            if (short_Service_Scope_Master == null)
            {
                return HttpNotFound();
            }
            return View(short_Service_Scope_Master);
        }

        // POST: ShortServiceScope/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Short_Service_Scope_ID,Short_Service_Scope,CreatedDate,Order,ServiceGroupID,ServiceID")] Short_Service_Scope_Master short_Service_Scope_Master)
        {
            if (ModelState.IsValid)
            {
                short_Service_Scope_Master.LastUpdateddate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, INDIAN_ZONE);
                db.Entry(short_Service_Scope_Master).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(short_Service_Scope_Master);
        }

        // GET: ShortServiceScope/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Short_Service_Scope_Master short_Service_Scope_Master = db.Short_Service_Scope_Master.Find(id);
            if (short_Service_Scope_Master == null)
            {
                return HttpNotFound();
            }
            return View(short_Service_Scope_Master);
        }

        // POST: ShortServiceScope/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Short_Service_Scope_Master short_Service_Scope_Master = db.Short_Service_Scope_Master.Find(id);
            db.Short_Service_Scope_Master.Remove(short_Service_Scope_Master);
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

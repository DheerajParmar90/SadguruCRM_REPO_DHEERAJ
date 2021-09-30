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
    public class LeadStatusController : Controller
    {
        private SadguruCRMEntities db = new SadguruCRMEntities();
        private static TimeZoneInfo INDIAN_ZONE = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");

        // GET: LeadStatus
        public ActionResult Index()
        {
            return View(db.LeadStatuses.OrderBy(x => x.Order).ToList());
        }

        // GET: LeadStatus/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LeadStatus leadStatus = db.LeadStatuses.Find(id);
            if (leadStatus == null)
            {
                return HttpNotFound();
            }
            return View(leadStatus);
        }

        // GET: LeadStatus/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: LeadStatus/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "StatusID,Status,ParentSourceID,Order")] LeadStatus leadStatus)
        {
            if (ModelState.IsValid)
            {
                leadStatus.CreatedDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, INDIAN_ZONE);
                db.LeadStatuses.Add(leadStatus);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(leadStatus);
        }

        // GET: LeadStatus/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LeadStatus leadStatus = db.LeadStatuses.Find(id);
            if (leadStatus == null)
            {
                return HttpNotFound();
            }
            return View(leadStatus);
        }

        // POST: LeadStatus/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "StatusID,Status,ParentSourceID,CreatedDate,Order")] LeadStatus leadStatus)
        {
            if (ModelState.IsValid)
            {
                leadStatus.LastUpdatedDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, INDIAN_ZONE);
                db.Entry(leadStatus).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(leadStatus);
        }

        // GET: LeadStatus/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LeadStatus leadStatus = db.LeadStatuses.Find(id);
            if (leadStatus == null)
            {
                return HttpNotFound();
            }
            return View(leadStatus);
        }

        // POST: LeadStatus/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            LeadStatus leadStatus = db.LeadStatuses.Find(id);
            db.LeadStatuses.Remove(leadStatus);
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

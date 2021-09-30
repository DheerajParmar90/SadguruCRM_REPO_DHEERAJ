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
    public class PaymentTermsController : Controller
    {
        private SadguruCRMEntities db = new SadguruCRMEntities();
        private static TimeZoneInfo INDIAN_ZONE = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");

        // GET: PaymentTerms
        public ActionResult Index()
        {
            var paymentTerms = db.PaymentTerms.Include(p => p.UserLogin).Include(p => p.UserLogin1);
            return View(paymentTerms.ToList());
        }

        // GET: PaymentTerms/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PaymentTerm paymentTerm = db.PaymentTerms.Find(id);
            if (paymentTerm == null)
            {
                return HttpNotFound();
            }
            return View(paymentTerm);
        }

        // GET: PaymentTerms/Create
        public ActionResult Create()
        {
            ViewBag.CreatedBy = new SelectList(db.UserLogins, "UserID", "UserName");
            ViewBag.LastUpdatedBy = new SelectList(db.UserLogins, "UserID", "UserName");
            return View();
        }

        // POST: PaymentTerms/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "PaymentTermID,PaymentTermName,Order")] PaymentTerm paymentTerm)
        {
            if (ModelState.IsValid)
            {
                paymentTerm.CreatedDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, INDIAN_ZONE);
                paymentTerm.CreatedBy = Convert.ToInt32(Session["UserID"]);
                db.PaymentTerms.Add(paymentTerm);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.CreatedBy = new SelectList(db.UserLogins, "UserID", "UserName", paymentTerm.CreatedBy);
            ViewBag.LastUpdatedBy = new SelectList(db.UserLogins, "UserID", "UserName", paymentTerm.LastUpdatedBy);
            return View(paymentTerm);
        }

        // GET: PaymentTerms/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PaymentTerm paymentTerm = db.PaymentTerms.Find(id);
            if (paymentTerm == null)
            {
                return HttpNotFound();
            }
            ViewBag.CreatedBy = new SelectList(db.UserLogins, "UserID", "UserName", paymentTerm.CreatedBy);
            ViewBag.LastUpdatedBy = new SelectList(db.UserLogins, "UserID", "UserName", paymentTerm.LastUpdatedBy);
            return View(paymentTerm);
        }

        // POST: PaymentTerms/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "PaymentTermID,PaymentTermName,CreatedDate,CreatedBy,Order")] PaymentTerm paymentTerm)
        {
            if (ModelState.IsValid)
            {
                paymentTerm.LastUpdatedDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, INDIAN_ZONE);
                paymentTerm.LastUpdatedBy = Convert.ToInt32(Session["UserID"]);
                db.Entry(paymentTerm).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CreatedBy = new SelectList(db.UserLogins, "UserID", "UserName", paymentTerm.CreatedBy);
            ViewBag.LastUpdatedBy = new SelectList(db.UserLogins, "UserID", "UserName", paymentTerm.LastUpdatedBy);
            return View(paymentTerm);
        }

        // GET: PaymentTerms/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PaymentTerm paymentTerm = db.PaymentTerms.Find(id);
            if (paymentTerm == null)
            {
                return HttpNotFound();
            }
            return View(paymentTerm);
        }

        // POST: PaymentTerms/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            PaymentTerm paymentTerm = db.PaymentTerms.Find(id);
            db.PaymentTerms.Remove(paymentTerm);
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

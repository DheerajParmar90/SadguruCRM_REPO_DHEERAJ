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
    public class PaymentModesController : Controller
    {
        private SadguruCRMEntities db = new SadguruCRMEntities();
        private static TimeZoneInfo INDIAN_ZONE = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");

        // GET: PaymentModes
        public ActionResult Index()
        {
            var paymentModes = db.PaymentModes.Include(p => p.UserLogin).Include(p => p.UserLogin1).OrderBy(x => x.Order);
            return View(paymentModes.ToList());
        }

        // GET: PaymentModes/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PaymentMode paymentMode = db.PaymentModes.Find(id);
            if (paymentMode == null)
            {
                return HttpNotFound();
            }
            return View(paymentMode);
        }

        // GET: PaymentModes/Create
        public ActionResult Create()
        {
            ViewBag.CreatedBy = new SelectList(db.UserLogins, "UserID", "UserName");
            ViewBag.LastUpdatedBy = new SelectList(db.UserLogins, "UserID", "UserName");
            return View();
        }

        // POST: PaymentModes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "PaymentModeID,PaymentModeName,Order")] PaymentMode paymentMode)
        {
            if (ModelState.IsValid)
            {
                paymentMode.CreatedDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, INDIAN_ZONE);
                paymentMode.CreatedBy = Convert.ToInt32(Session["UserID"]);
                db.PaymentModes.Add(paymentMode);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.CreatedBy = new SelectList(db.UserLogins, "UserID", "UserName", paymentMode.CreatedBy);
            ViewBag.LastUpdatedBy = new SelectList(db.UserLogins, "UserID", "UserName", paymentMode.LastUpdatedBy);
            return View(paymentMode);
        }

        // GET: PaymentModes/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PaymentMode paymentMode = db.PaymentModes.Find(id);
            if (paymentMode == null)
            {
                return HttpNotFound();
            }
            ViewBag.CreatedBy = new SelectList(db.UserLogins, "UserID", "UserName", paymentMode.CreatedBy);
            ViewBag.LastUpdatedBy = new SelectList(db.UserLogins, "UserID", "UserName", paymentMode.LastUpdatedBy);
            return View(paymentMode);
        }

        // POST: PaymentModes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "PaymentModeID,PaymentModeName,CreatedDate,CreatedBy,Order")] PaymentMode paymentMode)
        {
            if (ModelState.IsValid)
            {
                paymentMode.LastUpdatedDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, INDIAN_ZONE);
                paymentMode.LastUpdatedBy = Convert.ToInt32(Session["UserID"]);
                db.Entry(paymentMode).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CreatedBy = new SelectList(db.UserLogins, "UserID", "UserName", paymentMode.CreatedBy);
            ViewBag.LastUpdatedBy = new SelectList(db.UserLogins, "UserID", "UserName", paymentMode.LastUpdatedBy);
            return View(paymentMode);
        }

        // GET: PaymentModes/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PaymentMode paymentMode = db.PaymentModes.Find(id);
            if (paymentMode == null)
            {
                return HttpNotFound();
            }
            return View(paymentMode);
        }

        // POST: PaymentModes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            PaymentMode paymentMode = db.PaymentModes.Find(id);
            db.PaymentModes.Remove(paymentMode);
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

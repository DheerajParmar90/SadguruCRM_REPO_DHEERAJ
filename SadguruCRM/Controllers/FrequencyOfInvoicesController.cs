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
    public class FrequencyOfInvoicesController : Controller
    {
        private SadguruCRMEntities db = new SadguruCRMEntities();
        private static TimeZoneInfo INDIAN_ZONE = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");

        // GET: FrequencyOfInvoices
        public ActionResult Index()
        {
            var frequencyOfInvoices = db.FrequencyOfInvoices.Include(f => f.UserLogin);
            return View(frequencyOfInvoices.ToList());
        }

        // GET: FrequencyOfInvoices/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            FrequencyOfInvoice frequencyOfInvoice = db.FrequencyOfInvoices.Find(id);
            if (frequencyOfInvoice == null)
            {
                return HttpNotFound();
            }
            return View(frequencyOfInvoice);
        }

        // GET: FrequencyOfInvoices/Create
        public ActionResult Create()
        {
            ViewBag.CreatedBy = new SelectList(db.UserLogins, "UserID", "UserName");
            return View();
        }

        // POST: FrequencyOfInvoices/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "FrequencyOfInvoiceID,FrequencyOfInvoice1,Order,Duration_Between_2_Invoices_Year,Duration_Between_2_Invoices_Month,Duration_Between_2_Invoices_Day")] FrequencyOfInvoice frequencyOfInvoice)
        {
            if (ModelState.IsValid)
            {
                frequencyOfInvoice.CreatedBy = Convert.ToInt32(Session["UserID"]);
                frequencyOfInvoice.CreatedDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, INDIAN_ZONE);
                db.FrequencyOfInvoices.Add(frequencyOfInvoice);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.CreatedBy = new SelectList(db.UserLogins, "UserID", "UserName", frequencyOfInvoice.CreatedBy);
            return View(frequencyOfInvoice);
        }

        // GET: FrequencyOfInvoices/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            FrequencyOfInvoice frequencyOfInvoice = db.FrequencyOfInvoices.Find(id);
            if (frequencyOfInvoice == null)
            {
                return HttpNotFound();
            }
            ViewBag.CreatedBy = new SelectList(db.UserLogins, "UserID", "UserName", frequencyOfInvoice.CreatedBy);
            return View(frequencyOfInvoice);
        }

        // POST: FrequencyOfInvoices/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "FrequencyOfInvoiceID,FrequencyOfInvoice1,Order,Duration_Between_2_Invoices_Year,Duration_Between_2_Invoices_Month,Duration_Between_2_Invoices_Day,CreatedBy,CreatedDate")] FrequencyOfInvoice frequencyOfInvoice)
        {
            if (ModelState.IsValid)
            {
                frequencyOfInvoice.LastUpdatedDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, INDIAN_ZONE);
                db.Entry(frequencyOfInvoice).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CreatedBy = new SelectList(db.UserLogins, "UserID", "UserName", frequencyOfInvoice.CreatedBy);
            return View(frequencyOfInvoice);
        }

        // GET: FrequencyOfInvoices/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            FrequencyOfInvoice frequencyOfInvoice = db.FrequencyOfInvoices.Find(id);
            if (frequencyOfInvoice == null)
            {
                return HttpNotFound();
            }
            return View(frequencyOfInvoice);
        }

        // POST: FrequencyOfInvoices/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            FrequencyOfInvoice frequencyOfInvoice = db.FrequencyOfInvoices.Find(id);
            db.FrequencyOfInvoices.Remove(frequencyOfInvoice);
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

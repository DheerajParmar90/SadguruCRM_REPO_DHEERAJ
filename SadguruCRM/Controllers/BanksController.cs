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
    public class BanksController : Controller
    {
        private SadguruCRMEntities db = new SadguruCRMEntities();

        private static TimeZoneInfo INDIAN_ZONE = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");

        // GET: Banks
        public ActionResult Index()
        {
            var bank_Master = db.Bank_Master.Include(b => b.UserLogin).Include(b => b.UserLogin1);
            return View(bank_Master.ToList());
        }

        // GET: Banks/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Bank_Master bank_Master = db.Bank_Master.Find(id);
            if (bank_Master == null)
            {
                return HttpNotFound();
            }
            return View(bank_Master);
        }

        // GET: Banks/Create
        public ActionResult Create()
        {
            ViewBag.CreatedBy = new SelectList(db.UserLogins, "UserID", "UserName");
            ViewBag.LastUpdatedBy = new SelectList(db.UserLogins, "UserID", "UserName");

            return View();
        }

        // POST: Banks/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "BankID,BankName,Order")] Bank_Master bank_Master)
        {
            if (ModelState.IsValid)
            {
                bank_Master.CreatedBy = Convert.ToInt32(Session["UserID"]);
                bank_Master.CreatedOn = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, INDIAN_ZONE);
                db.Bank_Master.Add(bank_Master);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.CreatedBy = new SelectList(db.UserLogins, "UserID", "UserName", bank_Master.CreatedBy);
            ViewBag.LastUpdatedBy = new SelectList(db.UserLogins, "UserID", "UserName", bank_Master.LastUpdatedBy);
            return View(bank_Master);
        }

        // GET: Banks/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Bank_Master bank_Master = db.Bank_Master.Find(id);
            if (bank_Master == null)
            {
                return HttpNotFound();
            }
            ViewBag.CreatedBy = new SelectList(db.UserLogins, "UserID", "UserName", bank_Master.CreatedBy);
            ViewBag.LastUpdatedBy = new SelectList(db.UserLogins, "UserID", "UserName", bank_Master.LastUpdatedBy);
            return View(bank_Master);
        }

        // POST: Banks/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "BankID,BankName,CreatedOn,CreatedBy,Order")] Bank_Master bank_Master)
        {
            if (ModelState.IsValid)
            {
                bank_Master.LastUpdatedBy = Convert.ToInt32(Session["UserID"]);
                bank_Master.LastUpdatedOn = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, INDIAN_ZONE);
                db.Entry(bank_Master).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CreatedBy = new SelectList(db.UserLogins, "UserID", "UserName", bank_Master.CreatedBy);
            ViewBag.LastUpdatedBy = new SelectList(db.UserLogins, "UserID", "UserName", bank_Master.LastUpdatedBy);
            return View(bank_Master);
        }

        // GET: Banks/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Bank_Master bank_Master = db.Bank_Master.Find(id);
            if (bank_Master == null)
            {
                return HttpNotFound();
            }
            return View(bank_Master);
        }

        // POST: Banks/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Bank_Master bank_Master = db.Bank_Master.Find(id);
            db.Bank_Master.Remove(bank_Master);
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

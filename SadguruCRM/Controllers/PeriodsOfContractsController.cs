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
    public class PeriodsOfContractsController : Controller
    {
        private SadguruCRMEntities db = new SadguruCRMEntities();

        private static TimeZoneInfo INDIAN_ZONE = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
        // GET: PeriodsOfContracts
        public ActionResult Index()
        {
            return View(db.PeriodsOfContracts.OrderBy(x => x.Order).ToList());
        }

        // GET: PeriodsOfContracts/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PeriodsOfContract periodsOfContract = db.PeriodsOfContracts.Find(id);
            if (periodsOfContract == null)
            {
                return HttpNotFound();
            }
            return View(periodsOfContract);
        }

        // GET: PeriodsOfContracts/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: PeriodsOfContracts/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "PeriodsOfContractID,PeriodsOfContract1,Years,Months,Days,Order")] PeriodsOfContract periodsOfContract)
        {
            if (ModelState.IsValid)
            {

                if (periodsOfContract.Years == null)
                {
                    periodsOfContract.Years = 0;
                }
                if (periodsOfContract.Months == null)
                {
                    periodsOfContract.Months = 0;
                }
                if (periodsOfContract.Days == null)
                {
                    periodsOfContract.Days = 0;
                }
                periodsOfContract.CreatedDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, INDIAN_ZONE);
                db.PeriodsOfContracts.Add(periodsOfContract);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(periodsOfContract);
        }

        // GET: PeriodsOfContracts/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PeriodsOfContract periodsOfContract = db.PeriodsOfContracts.Find(id);
            if (periodsOfContract == null)
            {
                return HttpNotFound();
            }
            return View(periodsOfContract);
        }

        // POST: PeriodsOfContracts/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "PeriodsOfContractID,PeriodsOfContract1,Years,Months,Days,CreatedDate,Order")] PeriodsOfContract periodsOfContract)
        {
            if (ModelState.IsValid)

            {
                if (periodsOfContract.Years == null)
                {
                    periodsOfContract.Years = 0;
                }
                if (periodsOfContract.Months == null)
                {
                    periodsOfContract.Months = 0;
                }
                if (periodsOfContract.Days == null)
                {
                    periodsOfContract.Days = 0;
                }
                periodsOfContract.LastUpdatedDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, INDIAN_ZONE);
                db.Entry(periodsOfContract).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(periodsOfContract);
        }

        // GET: PeriodsOfContracts/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PeriodsOfContract periodsOfContract = db.PeriodsOfContracts.Find(id);
            if (periodsOfContract == null)
            {
                return HttpNotFound();
            }
            return View(periodsOfContract);
        }

        // POST: PeriodsOfContracts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            PeriodsOfContract periodsOfContract = db.PeriodsOfContracts.Find(id);
            db.PeriodsOfContracts.Remove(periodsOfContract);
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

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
    public class FrequencyOfServicesController : Controller
    {
        private SadguruCRMEntities db = new SadguruCRMEntities();
        private static TimeZoneInfo INDIAN_ZONE = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");

        // GET: FrequencyOfServices
        public ActionResult Index()
        {
            return View(db.FrequencyOfServices.OrderBy(x => x.Order).ToList());
        }

        // GET: FrequencyOfServices/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            FrequencyOfService frequencyOfService = db.FrequencyOfServices.Find(id);
            if (frequencyOfService == null)
            {
                return HttpNotFound();
            }
            return View(frequencyOfService);
        }

        // GET: FrequencyOfServices/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: FrequencyOfServices/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(FrequencyOfService frequencyOfService)
        {
            if (ModelState.IsValid)
            {
                frequencyOfService.CreatedDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, INDIAN_ZONE);
                frequencyOfService.CreatedByUserID = Convert.ToInt32(Session["UserID"]);
                db.FrequencyOfServices.Add(frequencyOfService);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(frequencyOfService);
        }

        // GET: FrequencyOfServices/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            FrequencyOfService frequencyOfService = db.FrequencyOfServices.Find(id);
            if (frequencyOfService == null)
            {
                return HttpNotFound();
            }
            return View(frequencyOfService);
        }

        // POST: FrequencyOfServices/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(FrequencyOfService frequencyOfService)
        {
            if (ModelState.IsValid)
            {
                frequencyOfService.LastUpdatedDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, INDIAN_ZONE);
                db.Entry(frequencyOfService).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(frequencyOfService);
        }

        // GET: FrequencyOfServices/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            FrequencyOfService frequencyOfService = db.FrequencyOfServices.Find(id);
            if (frequencyOfService == null)
            {
                return HttpNotFound();
            }
            return View(frequencyOfService);
        }

        // POST: FrequencyOfServices/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            FrequencyOfService frequencyOfService = db.FrequencyOfServices.Find(id);
            db.FrequencyOfServices.Remove(frequencyOfService);
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

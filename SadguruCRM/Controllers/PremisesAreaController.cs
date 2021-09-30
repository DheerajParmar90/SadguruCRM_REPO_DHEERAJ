using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using SadguruCRM.Models;

namespace SadguruCRM.Controllers
{
    public class PremisesAreaController : Controller
    {
        private SadguruCRMEntities db = new SadguruCRMEntities();
        private static TimeZoneInfo INDIAN_ZONE = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");

        // GET: PremisesArea
        public ActionResult Index()
        {
            var premises_Area_Master = db.Premises_Area_Master.Include(p => p.UserLogin).Include(p => p.UserLogin1);
            return View(premises_Area_Master.ToList());
        }

        // GET: PremisesArea/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Premises_Area_Master premises_Area_Master = db.Premises_Area_Master.Find(id);
            if (premises_Area_Master == null)
            {
                return HttpNotFound();
            }
            return View(premises_Area_Master);
        }

        // GET: PremisesArea/Create
        public ActionResult Create()
        {
            ViewBag.CreatedBy = new SelectList(db.UserLogins, "UserID", "UserName");
            ViewBag.LastUpdatedBy = new SelectList(db.UserLogins, "UserID", "UserName");
            return View();
        }

        // POST: PremisesArea/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Premises_Area_ID,Premises_Area,Order")] Premises_Area_Master premises_Area_Master)
        {
            if (ModelState.IsValid)
            {
                premises_Area_Master.CreatedBy = Convert.ToInt32(Session["UserID"]);
                premises_Area_Master.CreatedDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, INDIAN_ZONE);
                db.Premises_Area_Master.Add(premises_Area_Master);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.CreatedBy = new SelectList(db.UserLogins, "UserID", "UserName", premises_Area_Master.CreatedBy);
            ViewBag.LastUpdatedBy = new SelectList(db.UserLogins, "UserID", "UserName", premises_Area_Master.LastUpdatedBy);
            return View(premises_Area_Master);
        }

        // GET: PremisesArea/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Premises_Area_Master premises_Area_Master = db.Premises_Area_Master.Find(id);
            if (premises_Area_Master == null)
            {
                return HttpNotFound();
            }
            ViewBag.CreatedBy = new SelectList(db.UserLogins, "UserID", "UserName", premises_Area_Master.CreatedBy);
            ViewBag.LastUpdatedBy = new SelectList(db.UserLogins, "UserID", "UserName", premises_Area_Master.LastUpdatedBy);
            return View(premises_Area_Master);
        }

        // POST: PremisesArea/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Premises_Area_ID,Premises_Area,CreatedDate,CreatedBy,Order")] Premises_Area_Master premises_Area_Master)
        {
            if (ModelState.IsValid)
            {

                premises_Area_Master.LastUpdatedBy = Convert.ToInt32(Session["UserID"]);
                premises_Area_Master.LastUpdatedDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, INDIAN_ZONE);
                db.Entry(premises_Area_Master).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CreatedBy = new SelectList(db.UserLogins, "UserID", "UserName", premises_Area_Master.CreatedBy);
            ViewBag.LastUpdatedBy = new SelectList(db.UserLogins, "UserID", "UserName", premises_Area_Master.LastUpdatedBy);
            return View(premises_Area_Master);
        }

        // GET: PremisesArea/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Premises_Area_Master premises_Area_Master = db.Premises_Area_Master.Find(id);
            if (premises_Area_Master == null)
            {
                return HttpNotFound();
            }
            return View(premises_Area_Master);
        }

        // POST: PremisesArea/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Premises_Area_Master premises_Area_Master = db.Premises_Area_Master.Find(id);
            db.Premises_Area_Master.Remove(premises_Area_Master);
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

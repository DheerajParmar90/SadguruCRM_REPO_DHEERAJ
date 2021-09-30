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
    public class EmployeeDesignationsController : Controller
    {
        private SadguruCRMEntities db = new SadguruCRMEntities();
        private static TimeZoneInfo INDIAN_ZONE = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");

        // GET: EmployeeDesignations
        public ActionResult Index()
        {
            return View(db.EmployeeDesignations.OrderBy(x => x.Order).ToList());
        }

        // GET: EmployeeDesignations/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            EmployeeDesignation employeeDesignation = db.EmployeeDesignations.Find(id);
            if (employeeDesignation == null)
            {
                return HttpNotFound();
            }
            return View(employeeDesignation);
        }

        // GET: EmployeeDesignations/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: EmployeeDesignations/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "EmployeeDesignationID,Designation,Order")] EmployeeDesignation employeeDesignation)
        {
            if (ModelState.IsValid)
            {
                employeeDesignation.CreatedDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, INDIAN_ZONE);
                db.EmployeeDesignations.Add(employeeDesignation);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(employeeDesignation);
        }

        // GET: EmployeeDesignations/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            EmployeeDesignation employeeDesignation = db.EmployeeDesignations.Find(id);
            if (employeeDesignation == null)
            {
                return HttpNotFound();
            }
            return View(employeeDesignation);
        }

        // POST: EmployeeDesignations/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "EmployeeDesignationID,Designation,CreatedDate,CreatedByUserID,Order")] EmployeeDesignation employeeDesignation)
        {
            if (ModelState.IsValid)
            {
                employeeDesignation.LastUpdatedDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, INDIAN_ZONE);
                db.Entry(employeeDesignation).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(employeeDesignation);
        }

        // GET: EmployeeDesignations/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            EmployeeDesignation employeeDesignation = db.EmployeeDesignations.Find(id);
            if (employeeDesignation == null)
            {
                return HttpNotFound();
            }
            return View(employeeDesignation);
        }

        // POST: EmployeeDesignations/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            EmployeeDesignation employeeDesignation = db.EmployeeDesignations.Find(id);
            db.EmployeeDesignations.Remove(employeeDesignation);
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

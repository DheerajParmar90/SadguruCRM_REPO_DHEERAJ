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
    public class DepartmentsController : Controller
    {
        private SadguruCRMEntities db = new SadguruCRMEntities();
        private static TimeZoneInfo INDIAN_ZONE = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");

        // GET: Departments
        public ActionResult Index()
        {
            var department_Master = db.Department_Master.Include(d => d.UserLogin).Include(d => d.UserLogin1);
            return View(department_Master.ToList());
        }

        // GET: Departments/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Department_Master department_Master = db.Department_Master.Find(id);
            if (department_Master == null)
            {
                return HttpNotFound();
            }
            return View(department_Master);
        }

        // GET: Departments/Create
        public ActionResult Create()
        {
            ViewBag.CreatedByUserID = new SelectList(db.UserLogins, "UserID", "UserName");
            ViewBag.UpdatedByUserID = new SelectList(db.UserLogins, "UserID", "UserName");
            return View();
        }

        // POST: Departments/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Department_Master Department_Master)
        {
            if (ModelState.IsValid)
            {

                Department_Master.CreatedDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, INDIAN_ZONE);
                db.Department_Master.Add(Department_Master);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(Department_Master);
        }

        // GET: Departments/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Department_Master department_Master = db.Department_Master.Find(id);
            if (department_Master == null)
            {
                return HttpNotFound();
            }
            ViewBag.CreatedByUserID = new SelectList(db.UserLogins, "UserID", "UserName", department_Master.CreatedByUserID);
            ViewBag.UpdatedByUserID = new SelectList(db.UserLogins, "UserID", "UserName", department_Master.UpdatedByUserID);
            return View(department_Master);
        }

        // POST: Departments/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Department_Master Department_Master)
        {
            if (ModelState.IsValid)
            {

                Department_Master.LastUpdatedDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, INDIAN_ZONE);
                db.Entry(Department_Master).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(Department_Master);
        }

        // GET: Departments/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Department_Master department_Master = db.Department_Master.Find(id);
            if (department_Master == null)
            {
                return HttpNotFound();
            }
            return View(department_Master);
        }

        // POST: Departments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Department_Master department_Master = db.Department_Master.Find(id);
            db.Department_Master.Remove(department_Master);
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

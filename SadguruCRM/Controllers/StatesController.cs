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
    public class StatesController : Controller
    {
        private SadguruCRMEntities db = new SadguruCRMEntities();

        // GET: States
        public ActionResult Index()
        {
            var states = db.States.Include(s => s.UserLogin).Include(s => s.UserLogin1);
            return View(states.ToList());
        }

        // GET: States/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            State state = db.States.Find(id);
            if (state == null)
            {
                return HttpNotFound();
            }
            return View(state);
        }

        // GET: States/Create
        public ActionResult Create()
        {
            ViewBag.CreatedByUserID = new SelectList(db.UserLogins, "UserID", "UserName");
            ViewBag.UpdatedByUserID = new SelectList(db.UserLogins, "UserID", "UserName");
            return View();
        }

        // POST: States/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "StateID,State1,CreatedDate,LastUpdatedDate,CreatedByUserID,UpdatedByUserID")] State state)
        {
            if (ModelState.IsValid)
            {
                db.States.Add(state);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.CreatedByUserID = new SelectList(db.UserLogins, "UserID", "UserName", state.CreatedByUserID);
            ViewBag.UpdatedByUserID = new SelectList(db.UserLogins, "UserID", "UserName", state.UpdatedByUserID);
            return View(state);
        }

        // GET: States/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            State state = db.States.Find(id);
            if (state == null)
            {
                return HttpNotFound();
            }
            ViewBag.CreatedByUserID = new SelectList(db.UserLogins, "UserID", "UserName", state.CreatedByUserID);
            ViewBag.UpdatedByUserID = new SelectList(db.UserLogins, "UserID", "UserName", state.UpdatedByUserID);
            return View(state);
        }

        // POST: States/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "StateID,State1,CreatedDate,LastUpdatedDate,CreatedByUserID,UpdatedByUserID")] State state)
        {
            if (ModelState.IsValid)
            {
                db.Entry(state).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CreatedByUserID = new SelectList(db.UserLogins, "UserID", "UserName", state.CreatedByUserID);
            ViewBag.UpdatedByUserID = new SelectList(db.UserLogins, "UserID", "UserName", state.UpdatedByUserID);
            return View(state);
        }

        // GET: States/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            State state = db.States.Find(id);
            if (state == null)
            {
                return HttpNotFound();
            }
            return View(state);
        }

        // POST: States/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            State state = db.States.Find(id);
            db.States.Remove(state);
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

﻿using System;
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
    public class PremisesTypesController : Controller
    {
        private SadguruCRMEntities db = new SadguruCRMEntities();

        private static TimeZoneInfo INDIAN_ZONE = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");

        // GET: PremisesTypes
        public ActionResult Index()
        {
            return View(db.PremisesTypes.OrderBy(x => x.Order).ToList());
        }

        // GET: PremisesTypes/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PremisesType premisesType = db.PremisesTypes.Find(id);
            if (premisesType == null)
            {
                return HttpNotFound();
            }
            return View(premisesType);
        }

        // GET: PremisesTypes/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: PremisesTypes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "PremisesTypeID,PremisesType1,CreatedDate,Order")] PremisesType premisesType)
        {
            if (ModelState.IsValid)
            {
                premisesType.CreatedDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, INDIAN_ZONE);
                db.PremisesTypes.Add(premisesType);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(premisesType);
        }

        // GET: PremisesTypes/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PremisesType premisesType = db.PremisesTypes.Find(id);
            if (premisesType == null)
            {
                return HttpNotFound();
            }
            return View(premisesType);
        }

        // POST: PremisesTypes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "PremisesTypeID,PremisesType1,CreatedDate,Order")] PremisesType premisesType)
        {
            if (ModelState.IsValid)
            {
                premisesType.LastUpdatedDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, INDIAN_ZONE);
                db.Entry(premisesType).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(premisesType);
        }

        // GET: PremisesTypes/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PremisesType premisesType = db.PremisesTypes.Find(id);
            if (premisesType == null)
            {
                return HttpNotFound();
            }
            return View(premisesType);
        }

        // POST: PremisesTypes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            PremisesType premisesType = db.PremisesTypes.Find(id);
            db.PremisesTypes.Remove(premisesType);
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

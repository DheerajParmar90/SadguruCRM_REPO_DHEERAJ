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
   
    public class SourcesController : Controller
    {
        private SadguruCRMEntities db = new SadguruCRMEntities();
        private static TimeZoneInfo INDIAN_ZONE = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");

        // GET: Sources
        public ActionResult Index()
        {
            //var list = db.Sources.Include(x => db.Sources.Select(y => y.SourceID == x.ParentSourceID)).ToList();
            //var list = from s in db.Sources
            //            join sa in db.Sources on s.ParentSourceID equals sa.SourceID
            //            select s;
            //var list = from m in db.Sources
            //           join md in db.Sources on m.ParentSourceID equals md.SourceID
            //            into j
            //            from r in j.DefaultIfEmpty()
            //            select new { m.SourceID,m.Source1, ParentSource = r.Source1 };
            var list = db.Sources.Where( x => x.ParentSourceID == null).ToList();
            //foreach (var item in list) {
            //    var parent = db.Sources.Where(x => x.SourceID == item.ParentSourceID).FirstOrDefault();
            //    item.ParentSourceName = parent == null ? "" : parent.Source1; 
            //}
            return View(list);
        }

        // GET: Sources/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Source source = db.Sources.Find(id);
            if (source == null)
            {
                return HttpNotFound();
            }
            return View(source);
        }

        // GET: Sources/Create
        public ActionResult Create()
        {
            ViewBag.ParentSourceID = new SelectList(db.Sources.Where(x => x.ParentSourceID == null || x.ParentSourceID == 0), "SourceID", "Source1");
            return View();
        }

        // POST: Sources/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "SourceID,Source1,Order")] Source source)
        {
            if (ModelState.IsValid)
            {
                source.CreatedDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, INDIAN_ZONE);

                //source.CreatedB = Convert.ToInt32(Session["UserID"]);
                db.Sources.Add(source);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(source);
        }

        // GET: Sources/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Source source = db.Sources.Find(id);

            ViewBag.ParentSource = new SelectList(db.Sources.Where(x => x.ParentSourceID == null || x.ParentSourceID == 0), "SourceID", "Source1", source.ParentSourceID);
            if (source == null)
            {
                return HttpNotFound();
            }
            return View(source);
        }

        // POST: Sources/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "SourceID,Source1,Order,CreatedDate")] Source source)
        {
            if (ModelState.IsValid)
            {
                source.LastUpdatedDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, INDIAN_ZONE);
                db.Entry(source).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(source);
        }

        // GET: Sources/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Source source = db.Sources.Find(id);
            if (source == null)
            {
                return HttpNotFound();
            }
            return View(source);
        }

        // POST: Sources/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Source source = db.Sources.Find(id);
            db.Sources.Remove(source);
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

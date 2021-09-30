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
    public class ProductCategoryController : Controller
    {
        private SadguruCRMEntities db = new SadguruCRMEntities();
        private static TimeZoneInfo INDIAN_ZONE = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");

        // GET: ProductCategory
        public ActionResult Index()
        {
            var product_Category_Master = db.Product_Category_Master.Include(p => p.UserLogin).Include(p => p.UserLogin1);
            foreach (var prod in product_Category_Master) {
                if (prod.Parent_Category_ID != null) {
                    var parent = db.Product_Category_Master.Find(prod.Parent_Category_ID);
                    if (parent != null) {
                        prod.Parent_Category_Name = parent.Product_Category;
                    }
                
                }
            }
            return View(product_Category_Master.ToList());
        }

        // GET: ProductCategory/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product_Category_Master product_Category_Master = db.Product_Category_Master.Find(id);
            if (product_Category_Master == null)
            {
                return HttpNotFound();
            }
            if (product_Category_Master.Parent_Category_ID != null) {
                var parent = db.Product_Category_Master.Find(product_Category_Master.Parent_Category_ID);
                if (parent != null) {
                    product_Category_Master.Parent_Category_Name = parent.Product_Category;
                }
            }
            return View(product_Category_Master);
        }

        // GET: ProductCategory/Create
        public ActionResult Create()
        {
            ViewBag.Parent_Category_ID = new SelectList(db.Product_Category_Master.Where(c => c.Parent_Category_ID == null), "Product_Category_ID", "Product_Category");
            return View();
        }

        // POST: ProductCategory/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Product_Category_Master product_Category_Master)
        {
            if (ModelState.IsValid)

            {
                product_Category_Master.CreatedDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, INDIAN_ZONE);
                product_Category_Master.CreatedBy = Convert.ToInt32(Session["UserID"]);
                db.Product_Category_Master.Add(product_Category_Master);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.CreatedBy = new SelectList(db.UserLogins, "UserID", "UserName", product_Category_Master.CreatedBy);
            ViewBag.LastUpdatedBy = new SelectList(db.UserLogins, "UserID", "UserName", product_Category_Master.LastUpdatedBy);
            return View(product_Category_Master);
        }

        // GET: ProductCategory/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product_Category_Master product_Category_Master = db.Product_Category_Master.Find(id);
            if (product_Category_Master == null)
            {
                return HttpNotFound();
            }

            ViewBag.Parent_Category_ID = new SelectList(db.Product_Category_Master.Where(c => c.Parent_Category_ID == null && c.Product_Category_ID != product_Category_Master.Product_Category_ID), "Product_Category_ID", "Product_Category", product_Category_Master.Parent_Category_ID);
            return View(product_Category_Master);
        }

        // POST: ProductCategory/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Product_Category_Master product_Category_Master)
        {
            if (ModelState.IsValid)
            {
                product_Category_Master.LastUpdatedBy = Convert.ToInt32(Session["UserID"]);
                product_Category_Master.LastUpdatedDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, INDIAN_ZONE); ;
                db.Entry(product_Category_Master).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.Parent_Category_ID = new SelectList(db.Product_Category_Master.Where(c => c.Parent_Category_ID == null), "Product_Category_ID", "Product_Category", product_Category_Master.Parent_Category_ID);
            return View(product_Category_Master);
        }

        // GET: ProductCategory/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product_Category_Master product_Category_Master = db.Product_Category_Master.Find(id);
            if (product_Category_Master == null)
            {
                return HttpNotFound();
            }

            if (product_Category_Master.Parent_Category_ID != null)
            {
                var parent = db.Product_Category_Master.Find(product_Category_Master.Parent_Category_ID);
                if (parent != null)
                {
                    product_Category_Master.Parent_Category_Name = parent.Product_Category;
                }
            }
            ViewBag.ChildCategoriesCount = db.Product_Category_Master.Where(c => c.Parent_Category_ID == product_Category_Master.Product_Category_ID).Count();
            ViewBag.ProductsCount = db.Products_Master.Where(c => c.Product_Category_ID == product_Category_Master.Product_Category_ID).Count();
            return View(product_Category_Master);
        }

        // POST: ProductCategory/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Product_Category_Master product_Category_Master = db.Product_Category_Master.Find(id);
            if (db.Product_Category_Master.Where(c => c.Parent_Category_ID == product_Category_Master.Product_Category_ID).Count() > 0) {
                db.Product_Category_Master.RemoveRange(db.Product_Category_Master.Where(c => c.Parent_Category_ID == product_Category_Master.Product_Category_ID));
            }
            db.Product_Category_Master.Remove(product_Category_Master);
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

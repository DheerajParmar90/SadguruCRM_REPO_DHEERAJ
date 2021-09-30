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
    public class ProductsController : Controller
    {
        private SadguruCRMEntities db = new SadguruCRMEntities();
        private static TimeZoneInfo INDIAN_ZONE = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");

        // GET: Products
        public ActionResult Index()
        {
            var products_Master = db.Products_Master.Include(p => p.Product_Category_Master).Include(p => p.UserLogin).Include(p => p.UserLogin1);
            return View(products_Master.ToList());
        }

        // GET: Products/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Products_Master products_Master = db.Products_Master.Find(id);
            if (products_Master == null)
            {
                return HttpNotFound();
            }
            return View(products_Master);
        }

        // GET: Products/Create
        public ActionResult Create()
        {
            ViewBag.Product_Category_ID = new SelectList(db.Product_Category_Master.Where(c => c.Parent_Category_ID == null), "Product_Category_ID", "Product_Category");
            ViewBag.Product_SubCategory_ID = new SelectList(db.Product_Category_Master.Where(c => c.Parent_Category_ID != null), "Product_Category_ID", "Product_Category");
            ViewBag.VendorID = new SelectList(db.Vendors.Where(v => v.SupplierType == "PRODUCTS"), "VendorID", "VendorName");

            ViewBag.SubCategories = db.Product_Category_Master.Where(c => c.Parent_Category_ID != null)
                .Select(c => new { Value = c.Product_Category_ID, Text = c.Product_Category, Parent = c.Parent_Category_ID}).ToList();
            return View();
        }

        // POST: Products/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Products_Master products_Master)
        {
            if (ModelState.IsValid)
            {
                products_Master.CreatedDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, INDIAN_ZONE);
                products_Master.CreatedByUserID = Convert.ToInt32(Session["UserID"]);
                db.Products_Master.Add(products_Master);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.Product_Category_ID = new SelectList(db.Product_Category_Master.Where(c => c.Parent_Category_ID == null), "Product_Category_ID", "Product_Category");
            ViewBag.Product_SubCategory_ID = new SelectList(db.Product_Category_Master.Where(c => c.Parent_Category_ID != null), "Product_Category_ID", "Product_Category");
            ViewBag.VendorID = new SelectList(db.Vendors.Where(v => v.SupplierType == "PRODUCTS"), "VendorID", "VendorName");
            ViewBag.SubCategories = db.Product_Category_Master.Where(c => c.Parent_Category_ID != null)
                .Select(c => new { Value = c.Product_Category_ID, Text = c.Product_Category, Parent = c.Parent_Category_ID }).ToList();
            return View(products_Master);
        }

        // GET: Products/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Products_Master products_Master = db.Products_Master.Find(id);
            if (products_Master == null)
            {
                return HttpNotFound();
            }
            ViewBag.Product_Category_ID = new SelectList(db.Product_Category_Master.Where(c => c.Parent_Category_ID == null), "Product_Category_ID", "Product_Category", products_Master.Product_Category_ID);
            ViewBag.Product_SubCategory_ID = new SelectList(db.Product_Category_Master.Where(c => c.Parent_Category_ID == products_Master.Product_Category_ID), "Product_Category_ID", "Product_Category", products_Master.Product_SubCategory_ID);
            ViewBag.VendorID = new SelectList(db.Vendors.Where(v => v.SupplierType == "PRODUCTS"), "VendorID", "VendorName", products_Master.VendorID);
            ViewBag.SubCategories = db.Product_Category_Master.Where(c => c.Parent_Category_ID != null)
                .Select(c => new { Value = c.Product_Category_ID, Text = c.Product_Category, Parent = c.Parent_Category_ID }).ToList();
            return View(products_Master);
        }

        // POST: Products/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Products_Master products_Master)
        {
            if (ModelState.IsValid)
            {
                products_Master.LastUpdatedDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, INDIAN_ZONE);
                products_Master.LastUpdatedByUserID = Convert.ToInt32(Session["UserID"]);
                db.Entry(products_Master).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.Product_Category_ID = new SelectList(db.Product_Category_Master.Where(c => c.Parent_Category_ID == null), "Product_Category_ID", "Product_Category", products_Master.Product_Category_ID);
            ViewBag.Product_SubCategory_ID = new SelectList(db.Product_Category_Master.Where(c => c.Parent_Category_ID == products_Master.Product_Category_ID), "Product_Category_ID", "Product_Category", products_Master.Product_SubCategory_ID);
            ViewBag.VendorID = new SelectList(db.Vendors.Where(v => v.SupplierType == "PRODUCTS"), "VendorID", "VendorName", products_Master.VendorID);
            ViewBag.SubCategories = db.Product_Category_Master.Where(c => c.Parent_Category_ID != null)
                .Select(c => new { Value = c.Product_Category_ID, Text = c.Product_Category, Parent = c.Parent_Category_ID }).ToList();
            return View(products_Master);
        }

        // GET: Products/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Products_Master products_Master = db.Products_Master.Find(id);
            if (products_Master == null)
            {
                return HttpNotFound();
            }
            return View(products_Master);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Products_Master products_Master = db.Products_Master.Find(id);
            db.Products_Master.Remove(products_Master);
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

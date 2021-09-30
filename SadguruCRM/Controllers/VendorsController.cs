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
    public class VendorsController : Controller
    {
        private SadguruCRMEntities db = new SadguruCRMEntities();

        private static TimeZoneInfo INDIAN_ZONE = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");

        // GET: Vendors
        public ActionResult Index()
        {
            var vendors = db.Vendors.Include(v => v.City).Include(v => v.Location).Include(v => v.State).Include(v => v.UserLogin).Include(v => v.UserLogin1).Include(v => v.UserLogin2).Include(v => v.UserLogin3);
            return View(vendors.ToList());
        }

        // GET: Vendors/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Vendor vendor = db.Vendors.Find(id);
            if (vendor == null)
            {
                return HttpNotFound();
            }
            return View(vendor);
        }

        // GET: Vendors/Create
        public ActionResult Create()
        {
            int maharashtraStateID = db.States.Where(s => s.State1 == "Maharashtra").First().StateID;

            ViewBag.CityID = new SelectList(db.Cities, "CityID", "CityName");
            ViewBag.LocationID = new SelectList(db.Locations, "LocationID", "LocationName");
            ViewBag.Cities = new SelectList(db.Cities, "CityID", "CityName");
            ViewBag.Locations = new SelectList(db.Locations, "LocationID", "LocationName");
            ViewBag.StateID = new SelectList(db.States, "StateID", "State1", maharashtraStateID);
            ViewBag.SupplierType = new SelectList(new List<SelectListItem>
                                                {
                                                    new SelectListItem { Selected = true, Text = string.Empty, Value = "-1"},
                                                    new SelectListItem { Selected = false, Text = "SERVICES", Value = "SERVICES"},
                                                    new SelectListItem { Selected = false, Text = "PRODUCTS", Value = "PRODUCTS"},
                                                }, "Value", "Text");

            ViewBag.Services = new SelectList(db.Services, "ServiceID", "ServiceName");

            return View();
        }

        // POST: Vendors/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Vendor vendor, int[] Services, int[] Cities, int[] Locations)
        {
            if (ModelState.IsValid)
            {

                vendor.CreatedDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, INDIAN_ZONE);

                vendor.CreatedByUserID = Convert.ToInt32(Session["UserID"]);
                db.Vendors.Add(vendor);
                db.SaveChanges();
                if (Services != null) {
                    foreach (int i in Services)
                    {
                        Vendor_Services ser = new Vendor_Services();
                        ser.VendorID = vendor.VendorID;
                        ser.ServiceID = i;
                        db.Vendor_Services.Add(ser);
                        db.SaveChanges();
                    }
                }
                if (Cities != null) {
                    foreach (int i in Cities)
                    {
                        Vendor_Cities city = new Vendor_Cities();
                        city.VendorID = vendor.VendorID;
                        city.CityID = i;
                        db.Vendor_Cities.Add(city);
                        db.SaveChanges();
                    }
                }
                if (Locations != null) {
                    foreach (int i in Locations)
                    {
                        Vendor_Locations loc = new Vendor_Locations();
                        loc.VendorID = vendor.VendorID;
                        loc.LocationID = i;
                        db.Vendor_Locations.Add(loc);
                        db.SaveChanges();
                    }
                }
                
                return RedirectToAction("Index");
            }

            ViewBag.CityID = new SelectList(db.Cities, "CityID", "CityName", vendor.CityID);
            ViewBag.LocationID = new SelectList(db.Locations, "LocationID", "LocationName", vendor.LocationID);
            ViewBag.StateID = new SelectList(db.States, "StateID", "State1", vendor.StateID);
            return View(vendor);
        }

        // GET: Vendors/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Vendor vendor = db.Vendors.Find(id);
            if (vendor == null)
            {
                return HttpNotFound();
            }
            ViewBag.CityID = new SelectList(db.Cities, "CityID", "CityName", vendor.CityID);
            ViewBag.LocationID = new SelectList(db.Locations, "LocationID", "LocationName", vendor.LocationID);
            ViewBag.Cities = new SelectList(db.Cities, "CityID", "CityName");
            ViewBag.Locations = new SelectList(db.Locations, "LocationID", "LocationName");
            ViewBag.StateID = new SelectList(db.States, "StateID", "State1", vendor.StateID);
            ViewBag.SupplierType = new SelectList(new List<SelectListItem>
                                                {
                                                    new SelectListItem { Selected = true, Text = string.Empty, Value = "-1"},
                                                    new SelectListItem { Selected = false, Text = "SERVICES", Value = "SERVICES"},
                                                    new SelectListItem { Selected = false, Text = "PRODUCTS", Value = "PRODUCTS"},
                                                }, "Value", "Text",vendor.SupplierType);

            ViewBag.Services = new SelectList(db.Services, "ServiceID", "ServiceName");

            ViewBag.ServicesArray = vendor.Vendor_Services.Select(s => s.ServiceID).ToArray();
            ViewBag.CitiesArray = vendor.Vendor_Cities.Select(s => s.CityID).ToArray();
            ViewBag.LocationsArray = vendor.Vendor_Locations.Select(s => s.LocationID).ToArray();

            return View(vendor);
        }

        // POST: Vendors/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Vendor vendor,int[] Services, int[] Cities, int[] Locations)
        {
            if (ModelState.IsValid)
            {
                db.Entry(vendor).State = EntityState.Modified;
                db.SaveChanges();


                    var existingServices = db.Vendor_Services.Where(s => s.VendorID == vendor.VendorID).ToList();
                    foreach (var service in existingServices) {
                        if (!Services.Contains(service.ServiceID)) {
                            db.Vendor_Services.Remove(service);
                        }
                    }
                    foreach (var newSerID in Services)
                    {
                        if (!existingServices.Select(s => s.ServiceID).Contains(newSerID))
                        {
                            Vendor_Services ser = new Vendor_Services();
                            ser.VendorID = vendor.VendorID;
                            ser.ServiceID = newSerID;
                            db.Vendor_Services.Add(ser);
                        }
                    }                    
                    db.SaveChanges();


                var existingCities = db.Vendor_Cities.Where(s => s.VendorID == vendor.VendorID).ToList();
                foreach (var city in existingCities)
                {
                    if (!Cities.Contains(city.CityID))
                    {
                        db.Vendor_Cities.Remove(city);
                    }
                }
                foreach (var newCityID in Cities)
                {
                    if (!existingCities.Select(s => s.CityID).Contains(newCityID))
                    {
                        Vendor_Cities city = new Vendor_Cities();
                        city.VendorID = vendor.VendorID;
                        city.CityID = newCityID;
                        db.Vendor_Cities.Add(city);
                    }
                }
                db.SaveChanges();

                var existingLocations = db.Vendor_Locations.Where(s => s.VendorID == vendor.VendorID).ToList();
                foreach (var loc in existingLocations)
                {
                    if (!Locations.Contains(loc.LocationID))
                    {
                        db.Vendor_Locations.Remove(loc);
                    }
                }
                foreach (var newLocID in Locations)
                {
                    if (!existingLocations.Select(s => s.LocationID).Contains(newLocID))
                    {
                        Vendor_Locations loc = new Vendor_Locations();
                        loc.VendorID = vendor.VendorID;
                        loc.LocationID = newLocID;
                        db.Vendor_Locations.Add(loc);
                    }
                }
                db.SaveChanges();


                return RedirectToAction("Index");
            }
            ViewBag.CityID = new SelectList(db.Cities, "CityID", "CityName", vendor.CityID);
            ViewBag.LocationID = new SelectList(db.Locations, "LocationID", "LocationName", vendor.LocationID);
            ViewBag.StateID = new SelectList(db.States, "StateID", "State1", vendor.StateID);
            return View(vendor);
        }

        // GET: Vendors/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Vendor vendor = db.Vendors.Find(id);
            if (vendor == null)
            {
                return HttpNotFound();
            }
            return View(vendor);
        }

        // POST: Vendors/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Vendor vendor = db.Vendors.Find(id);
            db.Vendors.Remove(vendor);
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

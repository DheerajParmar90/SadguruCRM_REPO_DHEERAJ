using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using SadguruCRM.EnumClassesAndHelpers;
using SadguruCRM.Helpers;
using SadguruCRM.Models;

namespace SadguruCRM.Controllers
{
    [VerifyUser]
    public class EmployeesController : Controller
    {
        private SadguruCRMEntities db = new SadguruCRMEntities();
        private static TimeZoneInfo INDIAN_ZONE = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");

        // GET: Employees
        public ActionResult Index()
        {
            var employees = db.Employees.Include(e => e.Branch).Include(e => e.City).Include(e => e.City1).Include(e => e.EmployeeDesignation).Include(e => e.UserLogin).Include(e => e.Employees_Status_Master).Include(e => e.Location).Include(e => e.Location1).Include(e => e.State).Include(e => e.Employee1).Include(e => e.State1).Include(e => e.UserLogin1);
            return View(employees.ToList());
        }

        // GET: Employees/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Employee employee = db.Employees.Find(id);
            if (employee == null)
            {
                return HttpNotFound();
            }
            return View(employee);
        }

        // GET: Employees/Create
        public ActionResult Create()
        {
            ViewBag.BranchID = new SelectList(db.Branches, "BranchID", "BranchName");
            ViewBag.CurrentAddressCityID = new SelectList(db.Cities, "CityID", "CityName");
            ViewBag.PermAddressCityID = new SelectList(db.Cities, "CityID", "CityName");
            ViewBag.DesignationID = new SelectList(db.EmployeeDesignations, "EmployeeDesignationID", "Designation");
            ViewBag.DepartmentID = new SelectList(db.Department_Master, "DepartmentID", "Department");
            ViewBag.CreatedByUserID = new SelectList(db.UserLogins, "UserID", "UserName");
            ViewBag.EmployeeStatusID = new SelectList(db.Employees_Status_Master, "EmployeeStatusID", "EmployeeStatus");
            ViewBag.CurrentAddressLocationID = new SelectList(db.Locations, "LocationID", "LocationName");
            ViewBag.PermAddressLocationID = new SelectList(db.Locations, "LocationID", "LocationName");
            ViewBag.PermAddressStateID = new SelectList(db.States, "StateID", "State1", db.States.Where(s => s.State1 == "Maharashtra").FirstOrDefault().StateID);
            ViewBag.Report_To = new SelectList(db.Employees, "EmployeeID", "Name");
            ViewBag.CurrentAddressStateID = new SelectList(db.States, "StateID", "State1", db.States.Where(s => s.State1 == "Maharashtra").FirstOrDefault().StateID);
            ViewBag.UpdatedByUserID = new SelectList(db.UserLogins, "UserID", "UserName");

            var GenderEnums = from EnumClassesAndHelpers.Gender g in Enum.GetValues(typeof(EnumClassesAndHelpers.Gender))
                              select new
                              {
                                  ID = (byte)g,
                                  Name = EnumHelper.GetEnumDescription(g)
                              };

            ViewBag.Gender = new SelectList(GenderEnums, "ID", "Name");
            var MaritalStatusEnums = from EnumClassesAndHelpers.MaritalStatus g in Enum.GetValues(typeof(EnumClassesAndHelpers.MaritalStatus))
                              select new
                              {
                                  ID = (byte)g,
                                  Name = EnumHelper.GetEnumDescription(g)
                              };

            ViewBag.MaritalStatus = new SelectList(MaritalStatusEnums, "ID", "Name");
            var BloodGroupEnums = from EnumClassesAndHelpers.BloodGroup g in Enum.GetValues(typeof(EnumClassesAndHelpers.BloodGroup))
                              select new
                              {
                                  ID = (byte)g,
                                  Name = EnumHelper.GetEnumDescription(g)
                              };

            ViewBag.BloodGroups = new SelectList(BloodGroupEnums, "ID", "Name");
            ViewBag.EmployeeCode = "EMP" + String.Format("{0:D4}", (db.Employees.Count() + 1));
            ViewBag.BanksList = new SelectList(db.Bank_Master, "BankID", "BankName");

            return View();
        }

        // POST: Employees/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Employee employee, HttpPostedFileBase EmployeePhotoPath)
        {
            if (ModelState.IsValid)
            {
                //employee.EmployeeCode = "EMP" + String.Format("{0:D4}", (db.Employees.Count() + 1));
                employee.CreatedByUserID = Convert.ToInt32(Session["UserID"]);
                employee.CreatedDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, INDIAN_ZONE);

                if (EmployeePhotoPath != null)
                {
                    string path = System.IO.Path.Combine(Server.MapPath("~/EmployeePhotos"), System.IO.Path.GetFileName(EmployeePhotoPath.FileName));
                    EmployeePhotoPath.SaveAs(path);
                    employee.EmployeePhotoPath = path;

                }
                else { 
                    
                }
                db.Employees.Add(employee);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.BranchID = new SelectList(db.Branches, "BranchID", "BranchName", employee.BranchID);
            ViewBag.CurrentAddressCityID = new SelectList(db.Cities, "CityID", "CityName", employee.CurrentAddressCityID);
            ViewBag.PermAddressCityID = new SelectList(db.Cities, "CityID", "CityName", employee.PermAddressCityID);
            ViewBag.DesignationID = new SelectList(db.EmployeeDesignations, "EmployeeDesignationID", "Designation", employee.DesignationID);
            ViewBag.CreatedByUserID = new SelectList(db.UserLogins, "UserID", "UserName", employee.CreatedByUserID);
            ViewBag.EmployeeStatusID = new SelectList(db.Employees_Status_Master, "EmployeeStatusID", "EmployeeStatus", employee.EmployeeStatusID);
            ViewBag.CurrentAddressLocationID = new SelectList(db.Locations, "LocationID", "LocationName", employee.CurrentAddressLocationID);
            ViewBag.PermAddressLocationID = new SelectList(db.Locations, "LocationID", "LocationName", employee.PermAddressLocationID);
            ViewBag.PermAddressStateID = new SelectList(db.States, "StateID", "State1", employee.PermAddressStateID);
            ViewBag.Report_To = new SelectList(db.Employees, "EmployeeID", "Name", employee.Report_To);
            ViewBag.CurrentAddressStateID = new SelectList(db.States, "StateID", "State1", employee.CurrentAddressStateID);
            ViewBag.UpdatedByUserID = new SelectList(db.UserLogins, "UserID", "UserName", employee.UpdatedByUserID);
            return View(employee);
        }

        // GET: Employees/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Employee employee = db.Employees.Find(id);
            if (employee == null)
            {
                return HttpNotFound();
            }
            ViewBag.BranchID = new SelectList(db.Branches, "BranchID", "BranchName", employee.BranchID);
            ViewBag.CurrentAddressCityID = new SelectList(db.Cities, "CityID", "CityName", employee.CurrentAddressCityID);
            ViewBag.PermAddressCityID = new SelectList(db.Cities, "CityID", "CityName", employee.PermAddressCityID);
            ViewBag.DesignationID = new SelectList(db.EmployeeDesignations, "EmployeeDesignationID", "Designation", employee.DesignationID);
            ViewBag.CreatedByUserID = new SelectList(db.UserLogins, "UserID", "UserName", employee.CreatedByUserID);
            ViewBag.EmployeeStatusID = new SelectList(db.Employees_Status_Master, "EmployeeStatusID", "EmployeeStatus", employee.EmployeeStatusID);
            ViewBag.CurrentAddressLocationID = new SelectList(db.Locations, "LocationID", "LocationName", employee.CurrentAddressLocationID);
            ViewBag.PermAddressLocationID = new SelectList(db.Locations, "LocationID", "LocationName", employee.PermAddressLocationID);
            ViewBag.PermAddressStateID = new SelectList(db.States, "StateID", "State1", employee.PermAddressStateID);
            ViewBag.Report_To = new SelectList(db.Employees, "EmployeeID", "Name", employee.Report_To);
            ViewBag.CurrentAddressStateID = new SelectList(db.States, "StateID", "State1", employee.CurrentAddressStateID);
            ViewBag.UpdatedByUserID = new SelectList(db.UserLogins, "UserID", "UserName", employee.UpdatedByUserID);

            ViewBag.BanksList = new SelectList(db.Bank_Master, "BankID", "BankName");
            return View(employee);
        }

        // POST: Employees/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "EmployeeID,Name,DesignationID,EnrolmentDate,EmployeePhotoPath,DOB,CellNos,EmailID,EmployeeStatus,Remarks,ContactNumber,CreatedDate,LastUpdatedDate,CreatedByUserID,UpdatedByUserID,LoginID,Password,EmployeeCode,EmployeeStatusID,CurrentAddressAddressLine1,CurrentAddressAddressLine2,CurrentAddressAddressLine3,CurrentAddressLocationID,CurrentAddressCityID,CurrentAddressPincode,CurrentAddressStateID,PermAddressAddressLine1,PermAddressAddressLine2,PermAddressAddressLine3,PermAddressLocationID,PermAddressCityID,PermAddressPincode,PermAddressStateID,Gender_Enum,Marital_Status_Enum,Blood_Group_Enum,Qualification,PrimaryCellNumber,SecondaryCellNumber,PersonalEmailID,CompanyEmailID,BasicSalary,BranchID,DateOfJoining,DateOfProbation,DateOfJConfirmation,DateOfLeaving,Report_To,Pan_Number,AADHAR_Number,ESIC_Number,Other,Work_Experience,Bank_Name,Accounty_Holder_Name,Account_Number,IFSC_Code,Bank_Branch")] Employee employee)
        {
            if (ModelState.IsValid)
            {
                db.Entry(employee).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.BranchID = new SelectList(db.Branches, "BranchID", "BranchName", employee.BranchID);
            ViewBag.CurrentAddressCityID = new SelectList(db.Cities, "CityID", "CityName", employee.CurrentAddressCityID);
            ViewBag.PermAddressCityID = new SelectList(db.Cities, "CityID", "CityName", employee.PermAddressCityID);
            ViewBag.DesignationID = new SelectList(db.EmployeeDesignations, "EmployeeDesignationID", "Designation", employee.DesignationID);
            ViewBag.CreatedByUserID = new SelectList(db.UserLogins, "UserID", "UserName", employee.CreatedByUserID);
            ViewBag.EmployeeStatusID = new SelectList(db.Employees_Status_Master, "EmployeeStatusID", "EmployeeStatus", employee.EmployeeStatusID);
            ViewBag.CurrentAddressLocationID = new SelectList(db.Locations, "LocationID", "LocationName", employee.CurrentAddressLocationID);
            ViewBag.PermAddressLocationID = new SelectList(db.Locations, "LocationID", "LocationName", employee.PermAddressLocationID);
            ViewBag.PermAddressStateID = new SelectList(db.States, "StateID", "State1", employee.PermAddressStateID);
            ViewBag.Report_To = new SelectList(db.Employees, "EmployeeID", "Name", employee.Report_To);
            ViewBag.CurrentAddressStateID = new SelectList(db.States, "StateID", "State1", employee.CurrentAddressStateID);
            ViewBag.UpdatedByUserID = new SelectList(db.UserLogins, "UserID", "UserName", employee.UpdatedByUserID);
            return View(employee);
        }

        // GET: Employees/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Employee employee = db.Employees.Find(id);
            if (employee == null)
            {
                return HttpNotFound();
            }
            return View(employee);
        }

        // POST: Employees/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Employee employee = db.Employees.Find(id);
            db.Employees.Remove(employee);
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

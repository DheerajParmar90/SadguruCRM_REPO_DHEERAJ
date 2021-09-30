using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;
using SadguruCRM.Models;
using SadguruCRM.Models.ViewModels;

namespace SadguruCRM.Controllers
{
    [SessionState(System.Web.SessionState.SessionStateBehavior.ReadOnly)]
    public class DocumentsTemplatesController : Controller
    {
        private SadguruCRMEntities db = new SadguruCRMEntities();
        private static TimeZoneInfo INDIAN_ZONE = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");

        // GET: DocumentsTemplates
        public ActionResult Index()
        {
            var documents_Templates = db.Documents_Templates.Include(d => d.UserLogin).Include(d => d.UserLogin1);
            return View(documents_Templates.ToList());
        }
        [AllowAnonymous]
        public ActionResult Header()
        {
            return View();
        }
        [AllowAnonymous]
        public ActionResult Footer()
        {
            return View();
        }
        // GET: DocumentsTemplates/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Documents_Templates documents_Templates = db.Documents_Templates.Find(id);
            if (documents_Templates == null)
            {
                return HttpNotFound();
            }
            return View(documents_Templates);
        }

        // GET: DocumentsTemplates/Create
        public ActionResult Create()
        {
            ViewBag.CreatedByUserID = new SelectList(db.UserLogins, "UserID", "UserName");
            ViewBag.LastUpdatedByUserID = new SelectList(db.UserLogins, "UserID", "UserName");
            return View();
        }

        // POST: DocumentsTemplates/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost, ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Document_Template_ID,Document_Name,Document_Header,Document_Footer,CreatedOn,CreatedByUserID,LastUpdatedOn,LastUpdatedByUserID")] Documents_Templates documents_Templates)
        {
            if (ModelState.IsValid)
            {
                documents_Templates.CreatedOn = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, INDIAN_ZONE);
                documents_Templates.CreatedByUserID = Convert.ToInt32(Session["UserID"]);
                db.Documents_Templates.Add(documents_Templates);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.CreatedByUserID = new SelectList(db.UserLogins, "UserID", "UserName", documents_Templates.CreatedByUserID);
            ViewBag.LastUpdatedByUserID = new SelectList(db.UserLogins, "UserID", "UserName", documents_Templates.LastUpdatedByUserID);
            return View(documents_Templates);
        }
        public string uploadnow(HttpPostedFileWrapper upload)
        {
            string subPath = "~/Images/UploadedImages"; // your code goes here

            bool exists = Directory.Exists(HostingEnvironment.MapPath(subPath));

            if (!exists)
                Directory.CreateDirectory(HostingEnvironment.MapPath(subPath));
            if (upload != null)
            {
                string ImageName = upload.FileName;
                string path = Path.Combine(HostingEnvironment.MapPath(subPath), ImageName);
                upload.SaveAs(path);

            }
            return "Image Uploaded";
            //var result = new { responseText = "Successed", ID = "32" };
            //return Json(result, JsonRequestBehavior.AllowGet);
        }
        public ActionResult uploadPartial()
        {
            string subPath = "~/Images/UploadedImages"; // your code goes here

            bool exists = System.IO.Directory.Exists(HostingEnvironment.MapPath(subPath));

            if (!exists)
                System.IO.Directory.CreateDirectory(HostingEnvironment.MapPath(subPath));
            var appData = HostingEnvironment.MapPath(subPath);
            var images = Directory.GetFiles(appData).Select(x => new imagesviewmodel
            {
                Url = Url.Content(subPath + "/" + Path.GetFileName(x))
            });
            return View(images);
        }
        // GET: DocumentsTemplates/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Documents_Templates documents_Templates = db.Documents_Templates.Find(id);
            if (documents_Templates == null)
            {
                return HttpNotFound();
            }
            ViewBag.CreatedByUserID = new SelectList(db.UserLogins, "UserID", "UserName", documents_Templates.CreatedByUserID);
            ViewBag.LastUpdatedByUserID = new SelectList(db.UserLogins, "UserID", "UserName", documents_Templates.LastUpdatedByUserID);
            return View(documents_Templates);
        }

        // POST: DocumentsTemplates/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost, ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Document_Template_ID,Document_Name,Document_Header,Document_Footer,CreatedOn,CreatedByUserID,LastUpdatedOn,LastUpdatedByUserID")] Documents_Templates documents_Templates)
        {
            if (ModelState.IsValid)
            {
                documents_Templates.LastUpdatedOn = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, INDIAN_ZONE);
                documents_Templates.LastUpdatedByUserID = Convert.ToInt32(Session["UserID"]);
                db.Entry(documents_Templates).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CreatedByUserID = new SelectList(db.UserLogins, "UserID", "UserName", documents_Templates.CreatedByUserID);
            ViewBag.LastUpdatedByUserID = new SelectList(db.UserLogins, "UserID", "UserName", documents_Templates.LastUpdatedByUserID);
            return View(documents_Templates);
        }

        // GET: DocumentsTemplates/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Documents_Templates documents_Templates = db.Documents_Templates.Find(id);
            if (documents_Templates == null)
            {
                return HttpNotFound();
            }
            return View(documents_Templates);
        }

        // POST: DocumentsTemplates/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Documents_Templates documents_Templates = db.Documents_Templates.Find(id);
            db.Documents_Templates.Remove(documents_Templates);
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

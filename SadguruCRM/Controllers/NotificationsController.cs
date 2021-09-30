using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using SadguruCRM.EnumClassesAndHelpers;
using SadguruCRM.Helpers;
using SadguruCRM.Models;

namespace SadguruCRM.Controllers
{
    public class NotificationsController : Controller
    {
        private SadguruCRMEntities db = new SadguruCRMEntities();
        [HttpPost]
        public void GetNotificationSingleUser(string userID)
        {
            try
            {
                int intUserID;
                Int32.TryParse(userID, out intUserID);
                List<Notification> notifications = db.Notifications.OrderByDescending(x => x.CreatedOn).Take(4).ToList();
                //NotificationHub.BroadcastNotificationSingleUser(intUserID, ref notifications);
            }
            catch (Exception ex)
            {
                ex.ToString();
            }
        }
        [HttpPost]
        public void CreateNotifications(string leadID)
        {
            try
            {
                int intLeadID;
                Int32.TryParse(leadID, out intLeadID);
                var lead = db.Leads.Find(intLeadID);
                    int x;
                    Notification notification = new Notification();
                    notification.Type = (int)NotificationTypes.NewLead;
                    notification.Details = "Lead ID: " + lead.LeadID.ToString();
                    notification.Title = EnumHelper.GetEnumDescription(NotificationTypes.NewLead);
                    using (SadguruCRMEntities db = new SadguruCRMEntities())
                    {
                        List<int> userIDs = db.UserLogins.Select(v => v.UserID).ToList();
                        foreach (int userid in userIDs)
                        {
                            notification.SentTo = userid.ToString();
                            db.Notifications.Add(notification);
                        }
                        x = db.SaveChanges();
                        
                    }
                    NotificationHub.BroadcastLeadsNotification(ref notification);
                    //NotificationHub.BroadcastrNotificationSingleUser(intUserID, ref notifications);
            }
            catch (Exception ex)
            {
                ex.ToString();
            }
        }

        // GET: Notifications
        public ActionResult Index()
        {
            return View(db.Notifications.ToList());
        }

        // GET: Notifications/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Notification notification = db.Notifications.Find(id);
            if (notification == null)
            {
                return HttpNotFound();
            }
            return View(notification);
        }

        // GET: Notifications/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Notifications/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Type,Details,Title,DetailsURL,SentTo,Date,IsRead,IsDeleted,IsReminder,Code,NotificationType")] Notification notification)
        {
            if (ModelState.IsValid)
            {
                db.Notifications.Add(notification);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(notification);
        }

        // GET: Notifications/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Notification notification = db.Notifications.Find(id);
            if (notification == null)
            {
                return HttpNotFound();
            }
            return View(notification);
        }

        // POST: Notifications/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Type,Details,Title,DetailsURL,SentTo,Date,IsRead,IsDeleted,IsReminder,Code,NotificationType")] Notification notification)
        {
            if (ModelState.IsValid)
            {
                db.Entry(notification).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(notification);
        }

        // GET: Notifications/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Notification notification = db.Notifications.Find(id);
            if (notification == null)
            {
                return HttpNotFound();
            }
            return View(notification);
        }

        // POST: Notifications/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Notification notification = db.Notifications.Find(id);
            db.Notifications.Remove(notification);
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

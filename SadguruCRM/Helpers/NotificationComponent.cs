using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SadguruCRM.Models;
using SadguruCRM.EnumClassesAndHelpers;
using System.Threading.Tasks;

namespace SadguruCRM.Helpers
{
    public class NotificationComponent
    {
        private static TimeZoneInfo INDIAN_ZONE = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");

        //public void AddNotification(Objet obj) {

        //}
        public static async Task AddLeadNotificationAsync(Lead lead)
        {
            
            await Task.Run(async () => //This code runs on a new thread, control is returned to the caller on the UI thread.
            {
                try
                {
                    int x;
                    Notification notification = new Notification();
                    notification.Type = (int)NotificationTypes.NewLead;
                    notification.Details = "Lead ID: " + lead.LeadID.ToString();
                    notification.Title = EnumHelper.GetEnumDescription(NotificationTypes.NewLead);
                    notification.CreatedOn = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, INDIAN_ZONE);
                    using (SadguruCRMEntities db = new SadguruCRMEntities())
                    {
                        db.Notifications.Add(notification);
                        x = await db.SaveChangesAsync();
                        NotificationHub.BroadcastLeadsNotification(ref notification);
                    }
                }
                catch (Exception ex) {
                    string msg = ex.Message;
                }
                
               
            });
            
        }

        public void GetNotificationSingleUser(string userID)
        {
            try
            {
                int intUserID;
                Int32.TryParse(userID, out intUserID);
                using (SadguruCRMEntities db = new SadguruCRMEntities())
                {
                    List<Notification> notifications = db.Notifications.OrderByDescending(x => x.Id).Take(4).ToList();
                    //NotificationHub hub = new NotificationHub();
                    NotificationHub.BroadcastNotificationSingleUser(intUserID, ref notifications);
                }                    
            }
            catch (Exception ex)
            {
                ex.ToString();
            }
        }
    }
}
using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SadguruCRM.Models;
using System.Web;
using System.Web.Security;
using Microsoft.Ajax.Utilities;
using Newtonsoft.Json;
using SadguruCRM.Controllers;

namespace SadguruCRM.Helpers
{
    public class NotificationHub: Hub
    {
        private static readonly ConcurrentDictionary<string, ConnectedUser> connectedUsers =
        new ConcurrentDictionary<string, ConnectedUser>(StringComparer.InvariantCultureIgnoreCase);
        private SadguruCRMEntities context = new SadguruCRMEntities();
        //CustomUserIdProvider userIDProvider = new CustomUserIdProvider();
        public void Send(string name, string message)
        {
            // Call the addNewMessageToPage method to update clients.
            Clients.All.addNewMessageToPage(name, message);
        }
        public static void BroadcastNotification(string msg)
        {

            var hubContext = GlobalHost.ConnectionManager.GetHubContext<NotificationHub>();
            hubContext.Clients.All.NotificationReciever(msg);
        }
        public static void BroadcastLeadsNotification(ref Notification noti)
        {

            var hubContext = GlobalHost.ConnectionManager.GetHubContext<NotificationHub>();
            hubContext.Clients.All.NotificationReciever(noti.Details);
        }

        //https://www.c-sharpcorner.com/article/user-specific-notifications-using-asp-net-mvc-and-signalr/


        public override Task OnConnected()
        {
            // string userID = Context.User.Identity.Name; FormsAuthentication.GetAuthCookie;
            string userID = this.Context.QueryString["ForHub"];
            string connectionId = Context.ConnectionId;

            var user = connectedUsers.GetOrAdd(userID, _ => new ConnectedUser
            {
                UserID = userID,
                ConnectionIDs = new HashSet<string>()
            });

            lock (user.ConnectionIDs)
            {
                user.ConnectionIDs.Add(connectionId);
                if (user.ConnectionIDs.Count == 1)
                {
                    Clients.Others.userConnected(userID);
                }
            }
            NotificationComponent notificationsController = new NotificationComponent();
            notificationsController.GetNotificationSingleUser(userID);
            return base.OnConnected();
        }
        public override Task OnDisconnected(bool stopCalled)
        {
            //string userID = userIDProvider.GetUserIdString();
            //string userID = HttpContext.Current.Session["UserID"].ToString();
            string userID = this.Context.QueryString["ForHub"];
            string connectionId = Context.ConnectionId;

            ConnectedUser user;
            connectedUsers.TryGetValue(userID, out user);

            if (user != null)
            {
                lock (user.ConnectionIDs)
                {
                    user.ConnectionIDs.RemoveWhere(cid => cid.Equals(connectionId));
                    if (!user.ConnectionIDs.Any())
                    {
                        ConnectedUser removedUser;
                        connectedUsers.TryRemove(userID, out removedUser);
                        Clients.Others.userDisconnected(userID);
                    }
                }
            }

            return base.OnDisconnected(stopCalled);
        }

        public static void AskClientWhoCeatedLeadToGenerteAsynchRequestToCreateNotifications(int userID, int leadID) {

            string loggedUser = userID.ToString();
            ConnectedUser receiver;
            if (connectedUsers.TryGetValue(loggedUser, out receiver))
            {
                var cid = receiver.ConnectionIDs.FirstOrDefault();
                var context = GlobalHost.ConnectionManager.GetHubContext<NotificationHub>();
                //context.Clients.All.NotificationReciever("reer"); 
                context.Clients.Client(cid).GenerteAsynchRequestToCreateNotifications(leadID);
            }

        }
        //Logged Use Call  
        public static void BroadcastNotificationSingleUser(int userID, ref List<Notification> notifications)
        {
            try
            {
                string loggedUser = userID.ToString();

                //Get TotalNotification  
                //string totalNotif = LoadNotifData(loggedUser);

                //Send To  
                ConnectedUser receiver;
                if (connectedUsers.TryGetValue(loggedUser, out receiver))
                {
                    var cids = receiver.ConnectionIDs.ToList();
                    var context = GlobalHost.ConnectionManager.GetHubContext<NotificationHub>();
                    //context.Clients.All.NotificationReciever("reer"); 
                    //context.Clients.Client(cid).NotificationReciever(JsonConvert.SerializeObject(notifications));
                    foreach (string cid in cids) {
                        context.Clients.Client(cid).NotificationReciever(JsonConvert.SerializeObject(notifications));
                    }
                    
                }
            }
            catch (Exception ex)
            {
                ex.ToString();
            }
        }
        //Specific User Call  
        public void SendNotification(string SentTo)
        {
            try
            {
                //Get TotalNotification  
                string totalNotif = LoadNotifData(SentTo);

                //Send To  
                ConnectedUser receiver;
                if (connectedUsers.TryGetValue(SentTo, out receiver))
                {
                    var cid = receiver.ConnectionIDs.FirstOrDefault();
                    var context = GlobalHost.ConnectionManager.GetHubContext<NotificationHub>();
                    context.Clients.Client(cid).broadcaastNotif(totalNotif);
                }
            }
            catch (Exception ex)
            {
                ex.ToString();
            }
        }

        private string LoadNotifData(string userId)
        {
            int total = 0;
            var query = (from t in context.Notifications
                         where t.SentTo == userId
                         select t)
                        .ToList();
            total = query.Count;
            return total.ToString();
        }
    }

    public class ConnectedUser
    {
        public HashSet<string> ConnectionIDs { get; set; }
        public string UserID { get; set; }

    }
}
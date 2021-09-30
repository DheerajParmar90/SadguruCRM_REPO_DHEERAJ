using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SadguruCRM.Helpers
{
    public class CustomUserIdProvider
    {
        public string GetUserIdString()
        {
            string userID = HttpContext.Current.Session["UserID"].ToString();
            return userID;
        }
    }
}
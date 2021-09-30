using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using SadguruCRM.Helpers;

using SadguruCRM.Models;

namespace SadguruCRM.Controllers
{
    public class LoginController : Controller
    {
        String EntityConnectionString = String.Format("metadata=res://*/Models.SadguruCRMModel.csdl|res://*/Models.SadguruCRMModel.ssdl|res://*/Models.SadguruCRMModel.msl;provider=System.Data.SqlClient;provider connection string=&quot;data source=az1-wsq1.my-hosting-panel.com;initial catalog=thibowco_sadgurucrm;persist security info=True;user id=thibowco_sadgurucrm;MultipleActiveResultSets=True;App=EntityFramework&quot;");

        private SadguruCRMEntities db = new SadguruCRMEntities();
        
        // GET: Login
        public ActionResult Index()
        {
            if (TempData["ErrorMessage"] != null) {
                ViewBag.ErrorMessage = TempData["ErrorMessage"].ToString();
            }
            return View();
        }
        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult Login(UserLogin login)
        {
            try
            {
                using (var context = new SadguruCRMEntities())
                {
                    
                    var getUser = (from s in context.UserLogins where s.UserName == login.UserName select s).FirstOrDefault();
                    if (getUser != null)
                    {
                        var hashCode = "Sadguru@1234";
                        //Password Hasing Process Call Helper Class Method    
                        var encodingPasswordString = Helper.EncodePassword(login.Password, hashCode);
                        //Check Login Detail User Name Or Password    
                        var query = (from s in context.UserLogins where (s.UserName == login.UserName) && s.Password.Equals(encodingPasswordString) select s).FirstOrDefault();
                        if (query != null)
                        {
                            //RedirectToAction("Details/" + id.ToString(), "FullTimeEmployees");    
                            //return View("../Admin/Registration"); url not change in browser    
                            Session["UserID"] = query.UserID;
                            Session["UserName"] = query.UserName;
                            Session["UserLoginTypeID"] = query.UserLoginTypeID;

                            return RedirectToAction("Index", "Home");
                        }
                        TempData["ErrorMessage"] = "Invallid User Name or Password";
                        return RedirectToAction("Index", "Login");
                    }
                    TempData["ErrorMessage"] = "Invallid User Name or Password";
                    return RedirectToAction("Index", "Login");
                }
            }
            catch (Exception exception)
            {
                
                TempData["ErrorMessage"] = exception.Message;
                return RedirectToAction("Index", "Login");
            }
        }
        public ActionResult Logout()
        {
            Session.Clear();
            Session.RemoveAll();
            Session.Abandon();
            return RedirectToAction("Index", "Login");
        }

    }
}
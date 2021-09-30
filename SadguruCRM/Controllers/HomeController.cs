using SadguruCRM.Helpers;
using SadguruCRM.Models;
using SadguruCRM.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Linq;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Threading.Tasks;

namespace SadguruCRM.Controllers
{
    [VerifyUser]
    public class HomeController : Controller
    {
        private SadguruCRMEntities db = new SadguruCRMEntities();
        //private readonly IHubContext<NotificationHub> _hubContext;
        System.Diagnostics.Stopwatch watch = new System.Diagnostics.Stopwatch();
        private static TimeZoneInfo INDIAN_ZONE = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");

        public async Task<ActionResult> Index()
        {
            HomeViewModel homeVM = new HomeViewModel();
            try {
                if (TempData["ErrorMessage"] != null)
                {
                    ViewBag.ErrorMessage = TempData["ErrorMessage"].ToString();
                }
                if (!watch.IsRunning)
                {
                    watch = System.Diagnostics.Stopwatch.StartNew();
                }

                var leads = db.Leads.Include(l => l.Branch).Include(l => l.City).Include(l => l.UserLogin).Include(l => l.LeadStatus).Include(l => l.Source).Include(l => l.Source1).Include(l => l.PremisesType);
                homeVM.lead_reminder = await new HelperNonStatic().GenerateCustomerNameAndServicesShortCodes(leads);

                watch.Stop();
                long totalTime;
                if (TempData["TimeTaken"] != null)
                {
                    totalTime = Int64.Parse(TempData["TimeTaken"].ToString()) + watch.ElapsedMilliseconds;
                }
                else
                {
                    totalTime = watch.ElapsedMilliseconds;
                }
                ViewBag.TimeTaken = totalTime;
            }
            catch (Exception ex)
            {

            }

            return View(homeVM);
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}
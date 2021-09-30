using SadguruCRM.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace SadguruCRM.Helpers
{
    public class HelperNonStatic
    {
        public async Task<List<Lead>> GenerateCustomerNameAndServicesShortCodes(IQueryable<Lead> leads) {

            List<Task<Lead>> tasks = new List<Task<Lead>>();
            using (SadguruCRMEntities db = new SadguruCRMEntities()) {
                //try later
                //var services = db.Services.Select(x => new { x.ServiceID, x.ServiceShortCode }).ToList();
                var services = db.Services.ToList();
                var lead_services_mapping = db.Lead_Services_Mapping.ToList();
                foreach (var item in leads)
                {
                    Lead lead = item;


                    tasks.Add(Task.Run(() => (GenerrateSingleLeadCustomerNameAndServicesShortCodes(ref lead,ref lead_services_mapping, ref services))));
                }
                var results = await Task.WhenAll(tasks);

                return results.ToList();

            }
            

        }
        public Lead GenerrateSingleLeadCustomerNameAndServicesShortCodes(ref Lead item,ref List<Lead_Services_Mapping> lead_services_mapping, ref List<Service> services)
        {
            if (item.CustomerName == null)
            {
                item.CustomerName = item.Title + " " + item.FirstName + " " + item.LastName;
            }
            else {
                item.CustomerName = item.Title + " " + item.CustomerName;
            }
            int LeadID = item.LeadID;
            //var listOfServiceIDs = item.Lead_Services_Mapping.Select(c => c.ServiceID).ToList();
            var listOfServiceIDs = lead_services_mapping.Where(x => x.LeadID == LeadID).Select(c => c.ServiceID).ToList();
            //if (item.LeadID == 190)
            //{
                item.AddressLine1 = String.Join(", ", services.Where(x => listOfServiceIDs.Contains(x.ServiceID)).Select(x => x.ServiceShortCode).ToArray());

            //}
            //else {
            //    item.AddressLine1 = String.Join(", ", services.Where(x => listOfServiceIDs.Contains(x.ServiceID)).Select(x => x.ServiceShortCode).ToArray());
            //}
            
            return item;

        }

        public async Task<List<Estimate>> GenerateServicesShortCodesForEstimates(IQueryable<Estimate> estimates)
        {

            List<Task<Estimate>> tasks = new List<Task<Estimate>>();
            using (SadguruCRMEntities db = new SadguruCRMEntities())
            {
                //try later
                //var services = db.Services.Select(x => new { x.ServiceID, x.ServiceShortCode }).ToList();
                var services = db.Services.ToList();
                var services_mapping = db.Estimate_Services_Mapping.ToList();
                foreach (var item in estimates)
                {
                    Estimate estimate = item;


                    tasks.Add(Task.Run(() => (GenerrateSingleEstimateServicesShortCodes(ref estimate, ref services_mapping, ref services))));
                }
                var results = await Task.WhenAll(tasks);

                return results.ToList();

            }


        }

        public Estimate GenerrateSingleEstimateServicesShortCodes(ref Estimate item, ref List<Estimate_Services_Mapping> estimate_services_mapping, ref List<Service> services)
        {
            int estID = item.EstimateID;
            //var listOfServiceIDs = item.Lead_Services_Mapping.Select(c => c.ServiceID).ToList();
            var listOfServiceIDs = estimate_services_mapping.Where(x => x.EstimateID == estID).Select(c => c.ServiceID).ToList();
            //if (item.LeadID == 190)
            //{
            item.FinalEstimateRate = String.Join(", ", services.Where(x => listOfServiceIDs.Contains(x.ServiceID)).Select(x => x.ServiceShortCode).ToArray());
            if (!item.NewCustomer) {
                if (item.Customer.Customers_Billing_Address_Mapping.First().Location != null)
                    item.KindAttention = item.Customer.Customers_Billing_Address_Mapping.First().Location.LocationName;
            }
                //}
                //else {
                //    item.AddressLine1 = String.Join(", ", services.Where(x => listOfServiceIDs.Contains(x.ServiceID)).Select(x => x.ServiceShortCode).ToArray());
                //}

                return item;

        }
    }
}


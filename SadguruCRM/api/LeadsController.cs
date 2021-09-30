using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using SadguruCRM.Helpers;
using SadguruCRM.Models;

namespace SadguruCRM.api
{
    public class LeadsController : ApiController
    {
        private SadguruCRMEntities db = new SadguruCRMEntities();
        private static TimeZoneInfo INDIAN_ZONE = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");

        //// GET: api/PendingLeads
        //public IQueryable<Lead> GetLeads()
        //{
        //    return db.Leads;
        //}

        //// GET: api/PendingLeads/5
        //[ResponseType(typeof(Lead))]
        //public IHttpActionResult GetLead(int id)
        //{
        //    Lead lead = db.Leads.Find(id);
        //    if (lead == null)
        //    {
        //        return NotFound();
        //    }

        //    return Ok(lead);
        //}

        //// PUT: api/PendingLeads/5
        //[ResponseType(typeof(void))]
        //public IHttpActionResult PuLeads(int id, Lead lead)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }

        //    if (id != lead.LeadID)
        //    {
        //        return BadRequest();
        //    }

        //    db.Entry(lead).State = EntityState.Modified;

        //    try
        //    {
        //        db.SaveChanges();
        //    }
        //    catch (DbUpdateConcurrencyException)
        //    {
        //        if (!LeadExists(id))
        //        {
        //            return NotFound();
        //        }
        //        else
        //        {
        //            throw;
        //        }
        //    }

        //    return StatusCode(HttpStatusCode.NoContent);
        //}


        [ResponseType(typeof(Lead))]
        public IHttpActionResult PostLeadFromOutside(Pending_Leads leadFromOutside)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                int intLocation;
                bool isLocationInt = int.TryParse(leadFromOutside.Location, out intLocation);
                if (!isLocationInt || db.Locations.Count(e => e.LocationID == intLocation) == 0)
                {
                    return BadRequest("Location Not Found");
                }

                int intCity;
                bool isCityInt = int.TryParse(leadFromOutside.City, out intCity);
                if (!isCityInt || db.Cities.Count(e => e.CityID == intCity) == 0)
                {
                    return BadRequest("City Not Found");
                }

                List<Lead_Services_Mapping> servicesList = new List<Lead_Services_Mapping>();
                foreach (var service in leadFromOutside.Pending_Leads_Services)
                {

                    int intService;
                    bool isServiceInt = int.TryParse(service.Service, out intService);
                    if (!isServiceInt || db.ServiceGroups.Count(e => e.ServiceGroupID == intService) == 0)
                    {
                        return BadRequest(service.Service + " Service Not Found" + ", Please send Correct Service ID");
                    }

                    int intFrequency;
                    bool isFrequencyInt = int.TryParse(service.Frequency, out intFrequency);
                    if (!isFrequencyInt || db.FrequencyOfServices.Count(e => e.FrequencyOfServiceID == intFrequency) == 0)
                    {
                        return BadRequest(service.Frequency + " Frequency Not Found" + ", Please send Correct Frequency ID");
                    }

                    decimal deciRate;
                    bool isRateDeci = decimal.TryParse(service.Rate, out deciRate);
                    if (!isRateDeci)
                    {
                        return BadRequest(service.Rate + " Rate Not Correct" + ", Please send Correct Amount");
                    }
                    Lead_Services_Mapping leadService = new Lead_Services_Mapping();
                    leadService.ServiceID = intService;
                    leadService.ServiceGroupID = db.Services.Where(i => i.ServiceID == intService).First().ServiceGroupID;
                    leadService.FrequencyOfServiceID = intFrequency;
                    leadService.Rate = deciRate;
                    leadService.FinalRatePerService = deciRate;
                    servicesList.Add(leadService);
                }
                leadFromOutside.CreatedAt = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, INDIAN_ZONE);
                db.Pending_Leads.Add(leadFromOutside);
                db.SaveChanges();

                Lead lead = new Lead();
                lead.CustomerName = leadFromOutside.Name;
                lead.TelNo = leadFromOutside.ContactNumber;
                lead.EmailID = leadFromOutside.Email;
                lead.OtherDetails = leadFromOutside.Enquiry;
                lead.LocationID = intLocation;
                lead.CityID = intCity;
                lead.FinalRate = leadFromOutside.FinalRate;
                lead.SpecialInstructions = leadFromOutside.AdditionalComment;
                lead.AddressLine1 = leadFromOutside.Address1;
                lead.AddressLine2 = leadFromOutside.Address2;
                lead.CellNo = leadFromOutside.MobileNumber;
                lead.Pincode = leadFromOutside.Pincode;
                if (leadFromOutside.SelectedDateTime != null)
                    lead.ServiceDate = leadFromOutside.SelectedDateTime;
                lead.CreatedDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, INDIAN_ZONE);
                lead.LeadDate = lead.CreatedDate;


                db.Leads.Add(lead);
                db.DbccCheckIdent<Lead>(db.Leads.Max(p => p.LeadID));
                db.SaveChanges();
                foreach (var service in servicesList)
                {
                    service.LeadID = lead.LeadID;
                    db.Lead_Services_Mapping.Add(service);
                    db.DbccCheckIdent<Lead_Services_Mapping>(db.Lead_Services_Mapping.Max(p => p.Lead_Services_Mapping_ID));
                    db.SaveChanges();
                }
                return CreatedAtRoute("DefaultApi", new { id = lead.LeadID }, lead.LeadID);
            }
            catch (Exception ex) {
                return BadRequest(ex.Message);
            }
            
            
        }

        // DELETE: api/PendingLeads/5
        //[ResponseType(typeof(Lead))]
        //public IHttpActionResult DeleteLead(int id)
        //{
        //    Lead lead = db.Leads.Find(id);
        //    if (lead == null)
        //    {
        //        return NotFound();
        //    }

        //    db.Leads.Remove(lead);
        //    db.SaveChanges();

        //    return Ok(lead);
        //}

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool LeadExists(int id)
        {
            return db.Leads.Count(e => e.LeadID == id) > 0;
        }
    }
}
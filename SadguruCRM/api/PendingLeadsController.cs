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
using SadguruCRM.Models;

namespace SadguruCRM.api
{
    public class PendingLeadsController : ApiController
    {
        private SadguruCRMEntities db = new SadguruCRMEntities();

        // GET: api/PendingLeads
        public IQueryable<Pending_Leads> GetPending_Leads()
        {
            return db.Pending_Leads;
        }

        // GET: api/PendingLeads/5
        [ResponseType(typeof(Pending_Leads))]
        public IHttpActionResult GetPending_Leads(int id)
        {
            Pending_Leads pending_Leads = db.Pending_Leads.Find(id);
            if (pending_Leads == null)
            {
                return NotFound();
            }

            return Ok(pending_Leads);
        }

        // PUT: api/PendingLeads/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutPending_Leads(int id, Pending_Leads pending_Leads)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != pending_Leads.PendingLeadID)
            {
                return BadRequest();
            }

            db.Entry(pending_Leads).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!Pending_LeadsExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/PendingLeads
        [ResponseType(typeof(Pending_Leads))]
        public IHttpActionResult PostPending_Leads(Pending_Leads pending_Leads)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Pending_Leads.Add(pending_Leads);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = pending_Leads.PendingLeadID }, pending_Leads);
        }

        // DELETE: api/PendingLeads/5
        [ResponseType(typeof(Pending_Leads))]
        public IHttpActionResult DeletePending_Leads(int id)
        {
            Pending_Leads pending_Leads = db.Pending_Leads.Find(id);
            if (pending_Leads == null)
            {
                return NotFound();
            }

            db.Pending_Leads.Remove(pending_Leads);
            db.SaveChanges();

            return Ok(pending_Leads);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool Pending_LeadsExists(int id)
        {
            return db.Pending_Leads.Count(e => e.PendingLeadID == id) > 0;
        }
    }
}
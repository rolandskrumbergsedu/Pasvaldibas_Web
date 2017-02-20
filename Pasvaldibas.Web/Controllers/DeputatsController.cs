using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Web.Http;
using System.Web.Http.Description;
using Pasvaldibas.Web.Models;

namespace Pasvaldibas.Web.Controllers
{
    public class DeputatsController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: api/Deputats
        public IQueryable<Deputats> GetDeputati()
        {
            return db.Deputati;
        }

        // GET: api/Deputats/5
        [ResponseType(typeof(Deputats))]
        public IHttpActionResult GetDeputats(int id)
        {
            Deputats deputats = db.Deputati.Find(id);
            if (deputats == null)
            {
                return NotFound();
            }

            return Ok(deputats);
        }

        // PUT: api/Deputats/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutDeputats(int id, Deputats deputats)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != deputats.DeputatsId)
            {
                return BadRequest();
            }

            db.Entry(deputats).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DeputatsExists(id))
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

        // POST: api/Deputats
        [ResponseType(typeof(Deputats))]
        public IHttpActionResult PostDeputats(Deputats deputats)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Deputati.Add(deputats);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = deputats.DeputatsId }, deputats);
        }

        // DELETE: api/Deputats/5
        [ResponseType(typeof(Deputats))]
        public IHttpActionResult DeleteDeputats(int id)
        {
            Deputats deputats = db.Deputati.Find(id);
            if (deputats == null)
            {
                return NotFound();
            }

            db.Deputati.Remove(deputats);
            db.SaveChanges();

            return Ok(deputats);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool DeputatsExists(int id)
        {
            return db.Deputati.Count(e => e.DeputatsId == id) > 0;
        }
    }
}
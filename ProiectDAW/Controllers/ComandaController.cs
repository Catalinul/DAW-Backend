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
using ProiectDAW.Models;

namespace ProiectDAW.Controllers
{
    public class ComandaController : ApiController
    {
        private DBModel db = new DBModel();

        // GET: api/Comanda
        public IQueryable<Comanda> GetComandas()
        {
            return db.Comandas;
        }

        // GET: api/Comanda/5
        [ResponseType(typeof(Comanda))]
        public IHttpActionResult GetComanda(long id)
        {
            Comanda comanda = db.Comandas.Find(id);
            if (comanda == null)
            {
                return NotFound();
            }

            return Ok(comanda);
        }

        // PUT: api/Comanda/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutComanda(long id, Comanda comanda)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != comanda.ComandaID)
            {
                return BadRequest();
            }

            db.Entry(comanda).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ComandaExists(id))
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

        // POST: api/Comanda
        [ResponseType(typeof(Comanda))]
        public IHttpActionResult PostComanda(Comanda comanda)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Comandas.Add(comanda);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = comanda.ComandaID }, comanda);
        }

        // DELETE: api/Comanda/5
        [ResponseType(typeof(Comanda))]
        public IHttpActionResult DeleteComanda(long id)
        {
            Comanda comanda = db.Comandas.Find(id);
            if (comanda == null)
            {
                return NotFound();
            }

            db.Comandas.Remove(comanda);
            db.SaveChanges();

            return Ok(comanda);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool ComandaExists(long id)
        {
            return db.Comandas.Count(e => e.ComandaID == id) > 0;
        }
    }
}
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
        public System.Object GetComandas()
        {
			//facem join intre tabela Comanda si tabela Client dupa ClientID
			//expresie LINQ
			var result = (from a in db.Comandas
						  join b in db.Clients on a.ClientID equals b.ClientID

						  select new
						  {
							  a.ComandaID,
							  a.ComandaNr,
							  Client = b.Nume,
							  a.MetPlata,
							  a.Total
						  }).ToList();


            return result;
        }

        // GET: api/Comanda/5
        [ResponseType(typeof(Comanda))]
        public IHttpActionResult GetComanda(long id)
        {
			var comanda = (from a in db.Comandas
						   where a.ComandaID == id

						   select new
						   {
							   a.ComandaID,
							   a.ComandaNr,
							   a.ClientID,
							   a.MetPlata,
							   a.Total
						   }).FirstOrDefault();

			//facem join intre ComandaIteme si Iteme pentru a putea obtine valorile vectorului comandaIteme din comanda.service

			var comandaDetalii = (from a in db.ComandaItemes
								  join b in db.Items on a.ItemID equals b.ItemID
								  where a.ComandaID == id

								  select new
								  {
									  a.ComandaID,
									  a.ComandaItemID,
									  a.ItemID,
									  ItemNume = b.Nume,
									  b.Price,
									  a.Cantitate,
									  Total = a.Cantitate * b.Price
								  }).ToList();

			return Ok(new { comanda, comandaDetalii });
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
			try
			{
				//insert pentru tabela Comanda
				db.Comandas.Add(comanda);

				//insert pentru tabela ComendaIteme
				foreach (var item in comanda.ComandaItemes) //iteram prin colectia din Coamnda.cs
				{
					db.ComandaItemes.Add(item);
				}

				db.SaveChanges();

				//return Ok();
				return CreatedAtRoute("DefaultApi", new { id = comanda.ComandaID }, comanda);
			}
			catch (Exception ex)
			{
				throw ex;
			}
		}

		//// POST: api/Comanda
		//[ResponseType(typeof(Comanda))]
		//public IHttpActionResult PostComanda(Comanda comanda)
		//{
		//	if (!ModelState.IsValid)
		//	{
		//		return BadRequest(ModelState);
		//	}

		//	db.Comandas.Add(comanda);
		//	db.SaveChanges();

		//	return CreatedAtRoute("DefaultApi", new { id = comanda.ComandaID }, comanda);
		//}

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
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
							   a.Total,
							   ItemeComenziSterseID = ""
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

		// POST: api/Comanda
		[ResponseType(typeof(Comanda))]
		public IHttpActionResult PostComanda(Comanda comanda)
		{
			try
			{
				//insert pentru tabela ComendaIteme
				foreach (var item in comanda.ComandaItemes) //iteram prin colectia din Coamnda.cs
				{
					if (item.ComandaItemID == 0) //insert
						db.ComandaItemes.Add(item);
					else
						db.Entry(item).State = EntityState.Modified; ;
				}



				//insert pentru tabela Comanda
				if (comanda.ComandaID == 0) //realizam operatia de insert
					db.Comandas.Add(comanda);
				else //altfel realizam update
					db.Entry(comanda).State = EntityState.Modified; 


				//operatia Delete pentru ComandaIteme 
				// in comanda.service am trimis prin FormData un string care contine lista cu itemele sterse din comanda

				foreach (var id in comanda.ItemeComenziSterseID.Split(',').Where(x => x != "")) //where a fost adaugat ca sa nu avem un item gol la sfarsitul listei
				{
					ComandaIteme x = db.ComandaItemes.Find(Convert.ToInt64(id));
					db.ComandaItemes.Remove(x);
				}

				db.SaveChanges();

				return Ok();
				//return CreatedAtRoute("DefaultApi", new { id = comanda.ComandaID }, comanda);
			}
			catch (Exception ex)
			{
				throw ex;
			}
		}

		// DELETE: api/Comanda/5
		[ResponseType(typeof(Comanda))]
        public IHttpActionResult DeleteComanda(long id)
        {
			Comanda comanda = db.Comandas.Include(y => y.ComandaItemes)
				.SingleOrDefault(x => x.ComandaID == id);

			foreach (var item in comanda.ComandaItemes.ToList())
			{
				db.ComandaItemes.Remove(item);
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
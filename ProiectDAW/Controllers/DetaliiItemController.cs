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
    public class DetaliiItemController : ApiController
    {
        private DBModel db = new DBModel();

        // GET: api/DetaliiItem
        public IQueryable<DetaliiItem> GetDetaliiItems()
        {
            return db.DetaliiItems;
        }
		
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool DetaliiItemExists(int id)
        {
            return db.DetaliiItems.Count(e => e.ItemID == id) > 0;
        }
    }
}
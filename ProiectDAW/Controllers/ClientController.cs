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
    public class ClientController : ApiController
    {
        private DBModel db = new DBModel();

        // GET: api/Client
        public IQueryable<Client> GetClients()
        {
            return db.Clients;
        }
		
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

    }
}
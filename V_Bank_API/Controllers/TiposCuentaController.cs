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
using V_Bank_API.Crypto;
using V_Bank_API.Models;

namespace V_Bank_API.Controllers
{
    public class TiposCuentaController : ApiController
    {
        private V_BankEntities db;
        Crypt c;
        public TiposCuentaController()
        {
            db = new V_BankEntities();
            c = new Crypt();
        }

        // GET: api/TiposCuenta
        public IEnumerable<TipoCuenta> GetTipoCuenta()
        {
            IEnumerable<TipoCuenta> resultado = db.TipoCuenta;

            foreach (TipoCuenta tipoCuenta in resultado)
            {
                tipoCuenta.descripcion = c.desencriptar(tipoCuenta.descripcion);
            }

            return resultado;
        }

        // GET: api/TiposCuenta/5
        [ResponseType(typeof(TipoCuenta))]
        public IHttpActionResult GetTipoCuenta(decimal id)
        {
            TipoCuenta tipoCuenta = db.TipoCuenta.Find(id);
            if (tipoCuenta == null)
            {
                return NotFound();
            }

            tipoCuenta.descripcion = c.desencriptar(tipoCuenta.descripcion);

            return Ok(tipoCuenta);
        }

        // PUT: api/TiposCuenta/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutTipoCuenta(decimal id, TipoCuenta tipoCuenta)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != tipoCuenta.id)
            {
                return BadRequest();
            }

            tipoCuenta.descripcion = c.encriptar(tipoCuenta.descripcion);

            db.Entry(tipoCuenta).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TipoCuentaExists(id))
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

        // POST: api/TiposCuenta
        [ResponseType(typeof(TipoCuenta))]
        public IHttpActionResult PostTipoCuenta(TipoCuenta tipoCuenta)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            tipoCuenta.descripcion = c.encriptar(tipoCuenta.descripcion);

            db.TipoCuenta.Add(tipoCuenta);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = tipoCuenta.id }, tipoCuenta);
        }

        // DELETE: api/TiposCuenta/5
        [ResponseType(typeof(TipoCuenta))]
        public IHttpActionResult DeleteTipoCuenta(decimal id)
        {
            TipoCuenta tipoCuenta = db.TipoCuenta.Find(id);
            if (tipoCuenta == null)
            {
                return NotFound();
            }

            db.TipoCuenta.Remove(tipoCuenta);
            db.SaveChanges();

            return Ok(tipoCuenta);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool TipoCuentaExists(decimal id)
        {
            return db.TipoCuenta.Count(e => e.id == id) > 0;
        }
    }
}
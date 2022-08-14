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
    public class CuentasController : ApiController
    {
        private V_BankEntities db;
        Crypt c;
        public CuentasController()
        {
            db = new V_BankEntities();
            c = new Crypt();
        }

        // GET: api/Cuentas
        public IEnumerable<Cuenta> GetCuenta()
        {
            IEnumerable<Cuenta> resultado = db.Cuenta;

            foreach (Cuenta cuenta in resultado)
            {
                cuenta.numero_cuenta = c.desencriptar(cuenta.numero_cuenta);
                cuenta.cantidad_disponible = c.desencriptar(cuenta.cantidad_disponible);
                cuenta.codigo_seguridad = c.desencriptar(cuenta.codigo_seguridad);
                if (cuenta.tarjeta_asociada != null)
                {
                    cuenta.tarjeta_asociada = c.desencriptar(cuenta.tarjeta_asociada);
                }
            }

            return resultado;
        }

        // GET: api/Cuentas/5
        [ResponseType(typeof(Cuenta))]
        public IHttpActionResult GetCuenta(string id)
        {
            Cuenta cuenta = db.Cuenta.Find(c.encriptar(id));

            if (cuenta == null)
            {
                return NotFound();
            }

            cuenta.numero_cuenta = c.desencriptar(cuenta.numero_cuenta);
            cuenta.cantidad_disponible = c.desencriptar(cuenta.cantidad_disponible);
            cuenta.codigo_seguridad = c.desencriptar(cuenta.codigo_seguridad);
            if (cuenta.tarjeta_asociada != null)
            {
                cuenta.tarjeta_asociada = c.desencriptar(cuenta.tarjeta_asociada);
            }

            return Ok(cuenta);
        }

        // PUT: api/Cuentas/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutCuenta(string id, Cuenta cuenta)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != cuenta.numero_cuenta)
            {
                return BadRequest();
            }

            cuenta.numero_cuenta = c.encriptar(cuenta.numero_cuenta);
            cuenta.cantidad_disponible = c.encriptar(cuenta.cantidad_disponible);
            cuenta.codigo_seguridad = c.encriptar(cuenta.codigo_seguridad);
            
            if (cuenta.tarjeta_asociada!=null)
            {
                cuenta.tarjeta_asociada = c.encriptar(cuenta.tarjeta_asociada);
            }

            db.Entry(cuenta).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CuentaExists(id))
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

        // POST: api/Cuentas
        [ResponseType(typeof(Cuenta))]
        public IHttpActionResult PostCuenta(Cuenta cuenta)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Cuenta cuentaDesc = new Cuenta()
            {
                numero_cuenta = cuenta.numero_cuenta, 
                tipo_cuenta = cuenta.tipo_cuenta, 
                cantidad_disponible = cuenta.cantidad_disponible, 
                tarjeta_asociada = cuenta.tarjeta_asociada, 
                codigo_seguridad = cuenta.codigo_seguridad
            };

            cuenta.numero_cuenta = c.encriptar(cuenta.numero_cuenta);
            cuenta.cantidad_disponible = c.encriptar(cuenta.cantidad_disponible);
            cuenta.codigo_seguridad = c.encriptar(cuenta.codigo_seguridad);
            if (cuenta.tarjeta_asociada != null)
            {
                cuenta.tarjeta_asociada = c.encriptar(cuenta.tarjeta_asociada);
            }

            db.Cuenta.Add(cuenta);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateException)
            {
                if (CuentaExists(cuenta.numero_cuenta))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtRoute("DefaultApi", cuentaDesc, cuenta);
        }

        // DELETE: api/Cuentas/5
        [ResponseType(typeof(Cuenta))]
        public IHttpActionResult DeleteCuenta(string id)
        {
            Cuenta cuenta = db.Cuenta.Find(c.encriptar(id));
            if (cuenta == null)
            {
                return NotFound();
            }

            db.Cuenta.Remove(cuenta);
            db.SaveChanges();

            return Ok(cuenta);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool CuentaExists(string id)
        {
            return db.Cuenta.Count(e => e.numero_cuenta == c.encriptar(id)) > 0;
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using V_Bank_API.Crypto;
using V_Bank_API.Models;

namespace V_Bank_API.Controllers
{
    public class RealizarCompraEasyPayController : ApiController
    {
        private V_BankEntities db;
        Crypt c;
        public RealizarCompraEasyPayController()
        {
            db = new V_BankEntities();
            c = new Crypt();
        }

        // POST: api/RealizarCompras
        [ResponseType(typeof(Cuenta))]
        public IHttpActionResult PostCuenta(RealizarCompraEasyPay compra)
        {
            Cuenta cuenta = db.Cuenta.Find(c.encriptar(compra.Num_Cuenta.ToString()));

            if (cuenta == null)
            {
                return BadRequest("-1; Número de cuenta inválido.");
            }

            if (compra.Codigo_Seguridad.ToString() != c.desencriptar(cuenta.codigo_seguridad))
            {
                return BadRequest("-2; Código de seguridad inválido.");
            }

            var cantidadDisponible = Convert.ToDecimal(c.desencriptar(cuenta.cantidad_disponible));
            
            if (cantidadDisponible < compra.Monto)
            {
                return BadRequest("-4; Fondos Insuficientes.");
            }

            var nuevaCantidadDisponible = cantidadDisponible - compra.Monto;

            cuenta.cantidad_disponible = c.encriptar(nuevaCantidadDisponible.ToString());

            try
            {
                db.SaveChanges();
            }
            catch (Exception)
            {
                throw;
            }

            return Ok("0; Transacción exitosa.");


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

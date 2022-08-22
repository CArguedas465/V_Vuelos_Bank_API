using CreditCardValidator;
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
    public class RealizarCompraTarjetaController : ApiController
    {
        private V_BankEntities db;
        Crypt c;
        public RealizarCompraTarjetaController()
        {
            db = new V_BankEntities();
            c = new Crypt();
        }

        // POST: api/RealizarCompras
        [ResponseType(typeof(Cuenta))]
        public IHttpActionResult PostCuenta(RealizarCompraTarjeta compra)
        {
            var numTarjetaEntrada = c.encriptar(compra.Num_Tarjeta.ToString());
            Tarjeta tarjeta = db.Tarjeta.Find(numTarjetaEntrada);

            if (tarjeta == null)
            {
                return NotFound();
            }

            if (compra.Mes_Exp != Convert.ToInt16(c.desencriptar(tarjeta.mes_expiracion)) || compra.Anio_Exp != Convert.ToInt16(c.desencriptar(tarjeta.anio_expiracion)) || compra.CVV != Convert.ToInt16(c.desencriptar(tarjeta.cvv)))
            {
                return BadRequest("Datos de entrada inválidos (Expiración o CVV incorrecto(s)).");
            }

            /*Revisa Tipo*/
            CreditCardDetector detector = new CreditCardDetector(compra.Num_Tarjeta.ToString());

            if (compra.Tipo == "V")
            {
                if (!detector.IsValid(CardIssuer.Visa))
                {
                    return BadRequest("-1; Número de tarjeta inválido.");
                } ;
            } else
            {
                if (!detector.IsValid(CardIssuer.MasterCard))
                {
                    return BadRequest("-1; Número de tarjeta inválido.");
                };
            }

            /*Revisa que la siga estando activa.*/

            DateTime date = DateTime.Now;
            
            if (date.Year > Convert.ToInt32(compra.Anio_Exp))
            {
                return BadRequest("-2; Fecha de expiración inválida o la tarjeta expiró.");
            } 
            else
            {
                if (date.Year == Convert.ToInt32(compra.Anio_Exp))
                {
                    if (date.Month > Convert.ToInt32(compra.Mes_Exp))
                    {
                        return BadRequest("-2; Fecha de expiración inválida o la tarjeta expiró.");
                    }
                }
            }


            var cuentaQuery = from cuen in db.Cuenta
                              where cuen.tarjeta_asociada == tarjeta.numero_tarjeta
                              select cuen;

            if (!cuentaQuery.Any())
            {
                return BadRequest("Tarjeta no posee ninguna cuenta asociada.");
            }


            var cuenta = cuentaQuery.ToList()[0];
            
            var cantidadDisponible = Convert.ToDecimal(c.desencriptar(cuenta.cantidad_disponible));

            if (cantidadDisponible < compra.Monto)
            {
                return BadRequest("-4; Fondos insuficientes.");
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

            return Ok("0; Transacción Exitosa.");
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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace V_Bank_API.Models
{
    public class RealizarCompraTarjeta
    {
        public decimal Num_Tarjeta { get; set; }
        public int Mes_Exp { get; set; }
        public int Anio_Exp { get; set; }
        public int CVV { get; set; }
        public decimal Monto { get; set; }
        public string Tipo { get; set; }
    }
}
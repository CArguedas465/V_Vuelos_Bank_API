using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace V_Bank_API.Models
{
    public class RealizarCompraEasyPay
    {
        public string Num_Cuenta { get; set; }
        public string Codigo_Seguridad { get; set; }
        public decimal Monto { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace V_Bank_API.Crypto
{
    public class Crypt
    {
        public string encriptar(string cadena)
        {
            string resultado = string.Empty;

            Byte[] encripta = new UnicodeEncoding().GetBytes(cadena);
            resultado = Convert.ToBase64String(encripta);

            return resultado;
        }

        public string desencriptar(string cadena)
        {
            string resultado = string.Empty;

            Byte[] desencripta = Convert.FromBase64String(cadena);
            resultado = new UnicodeEncoding().GetString(desencripta);

            return resultado;
        }
    }
}
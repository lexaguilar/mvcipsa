using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace mvcIpsa.Models
{
    public class AccountDescription
    {
        public int BancoCuenta { get; set; }
        public int BancoId { get; set; }
        public string Banco { get; set; }
        public string Sucursal { get; set; }
        public int MonedaId { get; set; }
        public string Moneda { get; set; }

        public string Cuenta { get; set; }
        public string Descripcion { get; set; }
    }
}

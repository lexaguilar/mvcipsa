using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace mvcIpsa.Models
{
    public class CajaViewModel
    {
        public int Id { get; set; }
        public string Tipomov { get; set; }
        public string Numrecibo { get; set; }
        public short Nestado { get; set; }
        public DateTime FechaProceso { get; set; }
        public string tipopago { get; set; }
        public decimal? Montoefectivo { get; set; }
        public decimal? Montocheque { get; set; }
        public decimal? Montominuta { get; set; }
        public decimal? Montotransferencia { get; set; }
        public decimal Monto { get; set; }
        public string Noreferencia { get; set; }
        public string Cuentabanco { get; set; }
        public string Concepto { get; set; }
        public string Noordenpago { get; set; }
        public string tipoingreso { get; set; }
        public string tipomoneda { get; set; }
        public string Username { get; set; }
        public DateTime Fechacreacion { get; set; }
        public string centrocosto { get; set; }
        public string Identificacioncliente { get; set; }
        public string Cuentacontablebanco { get; set; }
        public decimal Tipocambio { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace mvcIpsa.Models
{
    public class CajaViewModel
    {
        public int Id { get; set; }
        public string TipoMovimiento { get; set; }
        public string NumRecibo { get; set; }
        public short EstadoId { get; set; }
        public string Estado { get; set; }
        public DateTime FechaProceso { get; set; }       
        public decimal Total { get; set; }           
        public string Concepto { get; set; }
        public string NoOrdenPago { get; set; }
        public string TipoIngreso { get; set; }
        public string TipoMoneda { get; set; }
        public string Username { get; set; }
        public DateTime FechaCreacion { get; set; }
        public string CentroCosto { get; set; }       
        public string Beneficiario { get; set; }      
        public int CajaId { get; set; }
        public string Caja { get; set; }
    }
}

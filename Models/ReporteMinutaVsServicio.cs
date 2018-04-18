using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace mvcIpsa.Models
{
    public class ReporteMinutaVsServicio
    {
        public string NumRecibo { get; set; }
        public DateTime FechaProceso { get; set; }
        public int CajaId { get; set; }
        public string Caja { get; set; }
        public string Beneficiario { get; set; }
        public string Moneda { get; set; }
        //Minuta
        public string MinutaReferencia { get; set; }        
        public DateTime MinutaFecha { get; set; }        
        public decimal MontoDolar { get; set; }        
        public decimal MontoCordoba { get; set; }        
        public string Banco { get; set; }        
        public string ServicioNombre { get; set; }
        public decimal ServicioPrecio { get; set; }
        public int ServicioCantidad { get; set; }
        public decimal ServicioTotal { get; set; }
    }
}

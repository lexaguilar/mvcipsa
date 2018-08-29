using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace mvcIpsa.Models
{
    public class ReciboViewModel
    {
        public DateTime FechaProceso { get; set; }
        public string NumRecibo { get; set; }
        public string Beneficiario { get; set; }
        public short EstadoId { get; set; }
        public string MotivoAnulado { get; set; }
        public decimal Total { get; set; }
        public string TipoMoneda { get; set; }
        public int Id { get; set; }
        public  IEnumerable<ReciboDetalle> detalles { get; set; }
    }

    public class ReciboDetalle
    {
        public decimal Cantidad { get; set; }
        public string CtaContable { get; set; }
        public decimal Montodolar { get; set; }
        public decimal Precio { get; set; }
        public string servicio { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace mvcIpsa.Models
{
    public class ConciliacionAnexoViewModel
    {
        public string Titulo { get; set; }
        public string CuentaBancaria { get; set; }

        public string Moneda{ get; set; }
        public IEnumerable<ReporteDetalleViewModel> detalle { get; set; }

        
    }

    public class ParametrosAnexoViewModel
    {
        public string Mensaje { get; set; }
        public bool HashError { get; set; }
        public int ProcesoBancoId { get; set; }
        public string CuentaBancaria { get; set; }
        public string Moneda { get; set; }
    }

}

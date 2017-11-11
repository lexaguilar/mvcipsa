using System;
using System.Collections.Generic;

namespace mvcIpsa.DbModel
{
    public partial class TipoPago
    {
        public TipoPago()
        {
            IngresosEgresosCaja = new HashSet<IngresosEgresosCaja>();
        }

        public int Idtipopago { get; set; }
        public string Descripcion { get; set; }

        public ICollection<IngresosEgresosCaja> IngresosEgresosCaja { get; set; }
    }
}

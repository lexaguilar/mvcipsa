using System;
using System.Collections.Generic;

namespace mvcIpsa.DbModel
{
    public partial class TipoPago
    {
        public TipoPago()
        {
            IngresosEgresosCajaReferencias = new HashSet<IngresosEgresosCajaReferencias>();
        }

        public int Id { get; set; }
        public string Descripcion { get; set; }

        public ICollection<IngresosEgresosCajaReferencias> IngresosEgresosCajaReferencias { get; set; }
    }
}

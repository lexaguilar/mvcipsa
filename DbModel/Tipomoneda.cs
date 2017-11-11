using System;
using System.Collections.Generic;

namespace mvcIpsa.DbModel
{
    public partial class TipoMoneda
    {
        public TipoMoneda()
        {
            IngresosEgresosCaja = new HashSet<IngresosEgresosCaja>();
        }

        public int Idtipomoneda { get; set; }
        public string Descripcion { get; set; }

        public ICollection<IngresosEgresosCaja> IngresosEgresosCaja { get; set; }
    }
}

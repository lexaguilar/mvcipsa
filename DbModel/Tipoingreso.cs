using System;
using System.Collections.Generic;

namespace mvcIpsa.DbModel
{
    public partial class TipoIngreso
    {
        public TipoIngreso()
        {
            IngresosEgresosCaja = new HashSet<IngresosEgresosCaja>();
        }

        public int Idtipoingreso { get; set; }
        public string Descripcion { get; set; }

        public ICollection<IngresosEgresosCaja> IngresosEgresosCaja { get; set; }
    }
}

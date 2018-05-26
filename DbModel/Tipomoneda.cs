using System;
using System.Collections.Generic;

namespace mvcIpsa.DbModel
{
    public partial class TipoMoneda
    {
        public TipoMoneda()
        {
            IngresosEgresosBanco = new HashSet<IngresosEgresosBanco>();
            IngresosEgresosCaja = new HashSet<IngresosEgresosCaja>();
        }

        public int Id { get; set; }
        public string Descripcion { get; set; }

        public ICollection<IngresosEgresosBanco> IngresosEgresosBanco { get; set; }
        public ICollection<IngresosEgresosCaja> IngresosEgresosCaja { get; set; }
    }
}

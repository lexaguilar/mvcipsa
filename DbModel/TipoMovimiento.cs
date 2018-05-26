using System;
using System.Collections.Generic;

namespace mvcIpsa.DbModel
{
    public partial class TipoMovimiento
    {
        public TipoMovimiento()
        {
            IngresosEgresosBanco = new HashSet<IngresosEgresosBanco>();
            IngresosEgresosCaja = new HashSet<IngresosEgresosCaja>();
        }

        public int Id { get; set; }
        public string Descripcion { get; set; }
        public short? TipoDoc { get; set; }

        public ICollection<IngresosEgresosBanco> IngresosEgresosBanco { get; set; }
        public ICollection<IngresosEgresosCaja> IngresosEgresosCaja { get; set; }
    }
}

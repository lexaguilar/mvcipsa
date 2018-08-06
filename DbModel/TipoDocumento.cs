using System;
using System.Collections.Generic;

namespace mvcIpsa.DbModel
{
    public partial class TipoDocumento
    {
        public TipoDocumento()
        {
            IngresosEgresosBanco = new HashSet<IngresosEgresosBanco>();
            TipoMovimiento = new HashSet<TipoMovimiento>();
        }

        public int Id { get; set; }
        public string Descripcion { get; set; }

        public ICollection<IngresosEgresosBanco> IngresosEgresosBanco { get; set; }
        public ICollection<TipoMovimiento> TipoMovimiento { get; set; }
    }
}

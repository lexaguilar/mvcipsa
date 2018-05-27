using System;
using System.Collections.Generic;

namespace mvcIpsa.DbModel
{
    public partial class TipoDocumento
    {
         public TipoDocumento()
        {
            IngresosEgresosBanco = new HashSet<IngresosEgresosBanco>();         
        }
        public int Id { get; set; }
        public string Descripcion { get; set; }

         public ICollection<IngresosEgresosBanco> IngresosEgresosBanco { get; set; }
    }
}

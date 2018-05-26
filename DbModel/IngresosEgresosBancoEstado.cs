using System;
using System.Collections.Generic;

namespace mvcIpsa.DbModel
{
    public partial class IngresosEgresosBancoEstado
    {
        public IngresosEgresosBancoEstado()
        {
            IngresosEgresosBanco = new HashSet<IngresosEgresosBanco>();
        }

        public short Id { get; set; }
        public string Description { get; set; }

        public ICollection<IngresosEgresosBanco> IngresosEgresosBanco { get; set; }
    }
}

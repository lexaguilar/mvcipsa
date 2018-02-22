using System;
using System.Collections.Generic;

namespace mvcIpsa.DbModel
{
    public partial class CajaEstado
    {
        public CajaEstado()
        {
            IngresosEgresosCaja = new HashSet<IngresosEgresosCaja>();
        }

        public short Id { get; set; }
        public string Descripcion { get; set; }

        public ICollection<IngresosEgresosCaja> IngresosEgresosCaja { get; set; }
    }
}

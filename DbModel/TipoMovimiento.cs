using System;
using System.Collections.Generic;

namespace mvcIpsa.DbModel
{
    public partial class TipoMovimiento
    {
        public TipoMovimiento()
        {
            ConciliacionBancaria = new HashSet<ConciliacionBancaria>();
            ConciliacionBancariaAux = new HashSet<ConciliacionBancariaAux>();
            IngresosEgresosBanco = new HashSet<IngresosEgresosBanco>();
            IngresosEgresosCaja = new HashSet<IngresosEgresosCaja>();
        }

        public int Id { get; set; }
        public string Descripcion { get; set; }
        public int TipoDoc { get; set; }
        public string DocName { get; set; }

        public TipoDocumento TipoDocNavigation { get; set; }
        public ICollection<ConciliacionBancaria> ConciliacionBancaria { get; set; }
        public ICollection<ConciliacionBancariaAux> ConciliacionBancariaAux { get; set; }
        public ICollection<IngresosEgresosBanco> IngresosEgresosBanco { get; set; }
        public ICollection<IngresosEgresosCaja> IngresosEgresosCaja { get; set; }
    }
}

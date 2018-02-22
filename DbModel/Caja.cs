using System;
using System.Collections.Generic;

namespace mvcIpsa.DbModel
{
    public partial class Caja
    {
        public Caja()
        {
            CajaCuentaContable = new HashSet<CajaCuentaContable>();
            IngresosEgresosCaja = new HashSet<IngresosEgresosCaja>();
            Profile = new HashSet<Profile>();
        }

        public int Id { get; set; }
        public int NoCaja { get; set; }
        public string Description { get; set; }

        public LoteRecibos LoteRecibos { get; set; }
        public ICollection<CajaCuentaContable> CajaCuentaContable { get; set; }
        public ICollection<IngresosEgresosCaja> IngresosEgresosCaja { get; set; }
        public ICollection<Profile> Profile { get; set; }
    }
}

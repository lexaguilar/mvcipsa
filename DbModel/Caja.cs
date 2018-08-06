using System;
using System.Collections.Generic;

namespace mvcIpsa.DbModel
{
    public partial class Caja
    {
        public Caja()
        {
            CajaCuentaContable = new HashSet<CajaCuentaContable>();
            IngresosEgresosBanco = new HashSet<IngresosEgresosBanco>();
            IngresosEgresosCaja = new HashSet<IngresosEgresosCaja>();
            LoteRecibos = new HashSet<LoteRecibos>();
            Profile = new HashSet<Profile>();
        }

        public int Id { get; set; }
        public int NoCaja { get; set; }
        public string Description { get; set; }

        public ICollection<CajaCuentaContable> CajaCuentaContable { get; set; }
        public ICollection<IngresosEgresosBanco> IngresosEgresosBanco { get; set; }
        public ICollection<IngresosEgresosCaja> IngresosEgresosCaja { get; set; }
        public ICollection<LoteRecibos> LoteRecibos { get; set; }
        public ICollection<Profile> Profile { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

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
        [Display(Name = "No caja")]
        public int NoCaja { get; set; }
        [Display(Name = "Descripción")]
        public string Description { get; set; }

        public ICollection<CajaCuentaContable> CajaCuentaContable { get; set; }
        public ICollection<IngresosEgresosCaja> IngresosEgresosCaja { get; set; }
        public ICollection<Profile> Profile { get; set; }
    }
}

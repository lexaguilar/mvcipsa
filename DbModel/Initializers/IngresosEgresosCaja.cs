using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace mvcIpsa.DbModel
{
    partial class IngresosEgresosCaja
    {
        [Display(Name ="Banco")]
        [NotMapped]
        public string Cuentadescripcionbanco { get; set; }
    }
}

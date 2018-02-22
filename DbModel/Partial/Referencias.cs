using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace mvcIpsa.DbModel
{
    partial class IngresosEgresosCajaReferencias
    {
        [NotMapped]
        public decimal totalD { get; set; }
        [NotMapped]
        public decimal totalC { get; set; }
    }
}

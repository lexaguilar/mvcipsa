﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace mvcIpsa.Models
{
    public class ReporteDetalleViewModel
    {
        public DateTime Fecha { get; set; }
        public string NumDocumento { get; set; }
        public decimal Monto { get; set; }
    }
}

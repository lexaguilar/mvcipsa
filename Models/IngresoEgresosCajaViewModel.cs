﻿using mvcIpsa.DbModel;
using System.Collections.Generic;

namespace mvcIpsa.Models
{
    public class IngresoEgresosCajaViewModel
    {
        public IngresosEgresosCaja master { get; set; }
        public IEnumerable<IngresoEgresosCajaDetalleViewModel> details { get; set; }
    }

    public class IngresoEgresosCajaDetalleViewModel {
        public string cta_cuenta{ get; set; }
        public decimal precio { get; set; }
        public short cantidad { get; set; }
        public decimal montocordoba { get; set; }
        public decimal montodolar { get; set; }
    }
}

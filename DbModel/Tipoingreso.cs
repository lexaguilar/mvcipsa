﻿using System;
using System.Collections.Generic;

namespace mvcIpsa.DbModel
{
    public partial class Tipoingreso
    {
        public Tipoingreso()
        {
            IngresosEgresosCaja = new HashSet<IngresosEgresosCaja>();
        }

        public int Idtipoingreso { get; set; }
        public string Descripcion { get; set; }

        public ICollection<IngresosEgresosCaja> IngresosEgresosCaja { get; set; }
    }
}

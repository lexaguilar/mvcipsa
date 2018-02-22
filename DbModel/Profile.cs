﻿using System;
using System.Collections.Generic;

namespace mvcIpsa.DbModel
{
    public partial class Profile
    {
        public Profile()
        {
            IngresosEgresosCaja = new HashSet<IngresosEgresosCaja>();
            Profilerole = new HashSet<Profilerole>();
        }

        public string Username { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string Correo { get; set; }
        public string Password { get; set; }
        public short Nestado { get; set; }
        public short Ncentrocosto { get; set; }
        public int CajaId { get; set; }

        public Caja Caja { get; set; }
        public ICollection<IngresosEgresosCaja> IngresosEgresosCaja { get; set; }
        public ICollection<Profilerole> Profilerole { get; set; }
    }
}

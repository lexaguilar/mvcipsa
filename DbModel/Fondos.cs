using System;
using System.Collections.Generic;

namespace mvcIpsa.DbModel
{
    public partial class Fondos
    {
        public int Fondoid { get; set; }
        public string Nombre { get; set; }
        public string Cuentaid { get; set; }
        public string UserInserta { get; set; }
        public DateTime? FechaInserta { get; set; }
        public string UserModifica { get; set; }
        public DateTime? FechaModifica { get; set; }
        public string Cuentaidos { get; set; }
    }
}

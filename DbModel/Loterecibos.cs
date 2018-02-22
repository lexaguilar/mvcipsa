using System;
using System.Collections.Generic;

namespace mvcIpsa.DbModel
{
    public partial class LoteRecibos
    {
        public int Id { get; set; }
        public int CajaId { get; set; }
        public int Inicio { get; set; }
        public int Final { get; set; }
        public int Actual { get; set; }

        public Caja Caja { get; set; }
    }
}

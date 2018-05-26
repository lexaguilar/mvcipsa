using System;
using System.Collections.Generic;

namespace mvcIpsa.DbModel
{
    public partial class CambioOficial
    {
        public DateTime FechaCambioOficial { get; set; }
        public decimal Dolares { get; set; }
        public decimal? Euros { get; set; }
        public char? Usuarioid { get; set; }
    }
}

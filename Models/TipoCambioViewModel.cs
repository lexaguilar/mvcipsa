using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace mvcIpsa.Models
{
    public class TipoCambioViewModel
    {
        public string fecha { get; set; }
        public string valor { get; set; }
        public decimal precio { get; set; }
        public int cantidad { get; set; }
        public bool udpated { get; set; }
    }
}

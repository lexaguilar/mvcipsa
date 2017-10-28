using System;
using System.Collections.Generic;

namespace mvcIpsa.DbModel
{
    public partial class TipoCliente
    {
        public TipoCliente()
        {
            Cliente = new HashSet<Cliente>();
        }

        public int Idtipocliente { get; set; }
        public string Tipocliente { get; set; }

        public ICollection<Cliente> Cliente { get; set; }
    }
}

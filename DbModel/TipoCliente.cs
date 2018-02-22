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

        public short Id { get; set; }
        public string Tipocliente { get; set; }

        public ICollection<Cliente> Cliente { get; set; }
    }
}

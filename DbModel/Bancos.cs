using System;
using System.Collections.Generic;

namespace mvcIpsa.DbModel
{
    public partial class Bancos
    {
        public Bancos()
        {
            BancosCuentas = new HashSet<BancosCuentas>();
        }

        public int Bancoid { get; set; }
        public string Descripcion { get; set; }
        public string Siglas { get; set; }
        public int? Orden { get; set; }

        public ICollection<BancosCuentas> BancosCuentas { get; set; }
    }
}

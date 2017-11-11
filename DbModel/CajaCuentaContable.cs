using System;
using System.Collections.Generic;

namespace mvcIpsa.DbModel
{
    public partial class CajaCuentaContable
    {
        public int Id { get; set; }
        public int IdCaja { get; set; }
        public string CtaCuenta { get; set; }

        public MaestroContable CtaCuentaNavigation { get; set; }
        public Caja IdCajaNavigation { get; set; }
    }
}

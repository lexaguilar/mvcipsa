using System;
using System.Collections.Generic;

namespace mvcIpsa.DbModel
{
    public partial class CajaCuentaContable
    {
        public int Id { get; set; }
        public int CajaId { get; set; }
        public string CtaCuenta { get; set; }

        public Caja Caja { get; set; }
        public MaestroContable CtaCuentaNavigation { get; set; }
    }
}

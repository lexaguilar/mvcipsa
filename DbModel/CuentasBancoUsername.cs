using System;
using System.Collections.Generic;

namespace mvcIpsa.DbModel
{
    public partial class CuentasBancoUsername
    {
        public int Id { get; set; }
        public int BancoCuenta { get; set; }
        public string Username { get; set; }

        public Profile UsernameNavigation { get; set; }
    }
}

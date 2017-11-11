using System;
using System.Collections.Generic;

namespace mvcIpsa.DbModel
{
    public partial class Profile
    {
        public string Username { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string Correo { get; set; }
        public string Password { get; set; }
        public short Nestado { get; set; }
        public short Ncentrocosto { get; set; }
        public int? Idcaja { get; set; }

        public Caja IdcajaNavigation { get; set; }
    }
}

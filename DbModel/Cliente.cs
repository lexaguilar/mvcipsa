using System;
using System.Collections.Generic;

namespace mvcIpsa.DbModel
{
    public partial class Cliente
    {
        public string Identificacion { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string Telefono { get; set; }
        public string Correo { get; set; }
        public string Direccion { get; set; }
        public int? Idtipocliente { get; set; }
        public string Numeroruc { get; set; }

        public TipoCliente IdtipoclienteNavigation { get; set; }
    }
}

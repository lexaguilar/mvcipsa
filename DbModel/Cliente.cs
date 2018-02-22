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
        public short? TipoClienteId { get; set; }       
        public int Id { get; set; }

        public TipoCliente TipoCliente { get; set; }
    }
}

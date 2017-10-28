using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace mvcIpsa.Models
{
    public class ProfileViewModel
    {
        public string Username { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string Correo { get; set; }
        public short Nestado { get; set; }
        public short Ncentrocosto { get; set; }
        public string centrocostoDescripcion { get; set; }
        public short Ncaja { get; set; }        
        public string cajaDescripcion { get; set; }
    }
}

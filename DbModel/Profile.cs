using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

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
        [Display(Name = "Caja o delegación")]
        public int? Idcaja { get; set; }

        public Caja IdcajaNavigation { get; set; }
    }
}

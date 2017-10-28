using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace mvcIpsa.DbModel
{
    public partial class Profile
    {
        [Required]
        public string Username { get; set; }
        [Required]
        public string Nombre { get; set; }
        [Required]
        public string Apellido { get; set; }
        [Required]
        public string Correo { get; set; }
        public string Password { get; set; }
        [Display(Name = "Estado")]
        public short Nestado { get; set; }

        public short Ncentrocosto { get; set; }
        [Required]
        [Display(Name = "Caja")]
        public short? Ncaja { get; set; }

        public Caja NcajaNavigation { get; set; }
    }
}

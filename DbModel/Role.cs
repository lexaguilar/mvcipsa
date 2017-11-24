using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace mvcIpsa.DbModel
{
    public partial class Role
    {
        public int Id { get; set; }
        [Display(Name = "Nombre del rol")]
        public string Name { get; set; }
        [Display(Name = "Descripción")]
        public string Description { get; set; }
        public short? Nestado { get; set; }
    }
}

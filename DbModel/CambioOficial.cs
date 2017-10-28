﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
namespace mvcIpsa.DbModel
{
    public partial class CambioOficial
    {
        [Display(Name = "Fecha")]
        public DateTime FechaCambioOficial { get; set; }
        public decimal Dolares { get; set; }
        public decimal? Euros { get; set; }
        public string Usuarioid { get; set; }
    }
}

using System;
using System.Collections.Generic;

namespace mvcIpsa.DbModel
{
    public partial class ReporteFirma
    {
        public string Reporte { get; set; }
        public string UsernameElaborado { get; set; }
        public string UsernameRevisado { get; set; }
        public string UsernameAprobado { get; set; }
        public string UsernameAutorizado { get; set; }
        public bool? MostrarAprobado { get; set; }
        public bool? MostrarAutorizado { get; set; }
        public bool? MostrarElaborado { get; set; }
        public bool? MostrarRevisado { get; set; }
        public string Ubicacion { get; set; }
        public string Nombre { get; set; }
    }
}

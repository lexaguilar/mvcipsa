using System;
using System.Collections.Generic;

namespace mvcIpsa.DbModel
{
    public partial class IngresosEgresosCajaDetalle
    {

        public int Id { get; set; }
        public int Idrecibo { get; set; }
        public decimal Montodolar { get; set; }
        public decimal Montocordoba { get; set; }
        public string CtaContable { get; set; }
        public decimal Precio { get; set; }
        public int Cantidad { get; set; }

        public MaestroContable CtaContableNavigation { get; set; }
        public IngresosEgresosCaja IdreciboNavigation { get; set; }
    }
}

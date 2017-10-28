using System;
using System.Collections.Generic;

namespace mvcIpsa.DbModel
{
    public partial class IngresosEgresosCajaDetalle
    {
        public int Id { get; set; }
        public int Idcaja { get; set; }
        public string Numrecibo { get; set; }
        public short? Numero { get; set; }
        public decimal? Montodolar { get; set; }
        public decimal? Montocordoba { get; set; }
        public char? Ctacentrocostos { get; set; }
        public char? Ctaservicio { get; set; }
        public short? Cantidad { get; set; }
        public char? Ctacontable { get; set; }
        public short? Ncaja { get; set; }
        public decimal? Tipocambio { get; set; }

        public IngresosEgresosCaja IdcajaNavigation { get; set; }
    }
}

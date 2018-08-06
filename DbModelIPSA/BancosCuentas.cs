using System;
using System.Collections.Generic;

namespace mvcIpsa.DbModelIPSA
{
    public partial class BancosCuentas
    {
        public int BancoCuenta { get; set; }
        public int Bancoid { get; set; }
        public string CtaBancaria { get; set; }
        public string NombreSucursal { get; set; }
        public string Descripcion { get; set; }
        public string CtaContable { get; set; }
        public int? Moneda { get; set; }
        public int? Estado { get; set; }
        public bool? Multiminutas { get; set; }
        public decimal? SaldoInicialTemp { get; set; }
        public decimal? IngresosTemp { get; set; }
        public decimal? EgresosTemp { get; set; }
        public decimal? SaldoFinalTemp { get; set; }
        public string Centroid { get; set; }

        public Bancos Banco { get; set; }
    }
}

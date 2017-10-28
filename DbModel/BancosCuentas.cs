using System;
using System.Collections.Generic;

namespace mvcIpsa.DbModel
{
    public partial class BancosCuentas
    {
        public int BancoCuenta { get; set; }
        public int Bancoid { get; set; }
        public char? CtaBancaria { get; set; }
        public char NombreSucursal { get; set; }
        public char? Descripcion { get; set; }
        public char CtaContable { get; set; }
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

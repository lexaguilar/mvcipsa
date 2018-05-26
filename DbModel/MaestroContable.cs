using System;
using System.Collections.Generic;

namespace mvcIpsa.DbModel
{
    public partial class MaestroContable
    {
        public string CtaContable { get; set; }
        public char? Centro { get; set; }
        public string Cuenta { get; set; }
        public string Nombre { get; set; }
        public int TipoDh { get; set; }
        public int TipoCta { get; set; }
        public int NivelCuenta { get; set; }
        public string CtaPadre { get; set; }
        public decimal? SaldoInicialPeriodo { get; set; }
        public decimal? Mes1 { get; set; }
        public decimal? Mes2 { get; set; }
        public decimal? Mes3 { get; set; }
        public decimal? Mes4 { get; set; }
        public decimal? Mes5 { get; set; }
        public decimal? Mes6 { get; set; }
        public decimal? Mes7 { get; set; }
        public decimal? Mes8 { get; set; }
        public decimal? Mes9 { get; set; }
        public decimal? Mes10 { get; set; }
        public decimal? Mes11 { get; set; }
        public decimal? Mes12 { get; set; }
        public decimal? SaldoInicial { get; set; }
        public decimal? MovDebitos { get; set; }
        public decimal? MovCreditos { get; set; }
        public decimal? SaldoFinal { get; set; }
        public decimal? SaldoPreCierre { get; set; }
        public int? Movimiento { get; set; }
        public decimal? SaldoInicialhist { get; set; }
        public decimal? MovDebitoshist { get; set; }
        public decimal? MovCreditoshist { get; set; }
        public decimal? SaldoFinalhist { get; set; }
    }
}

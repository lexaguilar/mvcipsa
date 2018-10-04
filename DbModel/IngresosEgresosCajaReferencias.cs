using System;
using System.Collections.Generic;

namespace mvcIpsa.DbModel
{
    public partial class IngresosEgresosCajaReferencias
    {
        public int Id { get; set; }
        public int ReciboId { get; set; }
        public decimal MontoEfectivo { get; set; }
        public decimal MontoMinu { get; set; }
        public decimal MontoCheq { get; set; }
        public decimal MontoTrans { get; set; }
        public decimal Total { get; set; }
        public DateTime Fecha { get; set; }
        public decimal TipoCambio { get; set; }
        public string Referencia { get; set; }
        public short? IdBanco { get; set; }
        public int TipoPagoId { get; set; }
        public bool Procesado { get; set; }
        public bool? Excluido { get; set; }
        public IngresosEgresosCaja Recibo { get; set; }
        public TipoPago TipoPago { get; set; }
    }
}

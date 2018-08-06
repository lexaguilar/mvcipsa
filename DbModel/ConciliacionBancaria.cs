using System;
using System.Collections.Generic;

namespace mvcIpsa.DbModel
{
    public partial class ConciliacionBancaria
    {
        public int Id { get; set; }
        public DateTime Fecha { get; set; }
        public string Referencia { get; set; }
        public int TipoMovimientoId { get; set; }
        public decimal Credito { get; set; }
        public int EstadoId { get; set; }
        public string Uuid { get; set; }
        public int ProcesoBancoId { get; set; }
        public decimal Debito { get; set; }
        public bool Conciliado { get; set; }


        public ConciliacionBancariaEstado Estado { get; set; }
        public ProcesoBanco ProcesoBanco { get; set; }
        public TipoMovimiento TipoMovimiento { get; set; }
    }
}

using System;
using System.Collections.Generic;

namespace mvcIpsa.DbModel
{
    public partial class IngresosEgresosBanco
    {
        public int Id { get; set; }
        public DateTime FechaRegistro { get; set; }
        public string Referencia { get; set; }
        public decimal Monto { get; set; }
        public int TipoMonedaId { get; set; }
        public string CuentaContableBanco { get; set; }
        public int TipoMovimientoId { get; set; }
        public string Concepto { get; set; }
        public DateTime FechaProceso { get; set; }
        public string Username { get; set; }
        public int CajaId { get; set; }
        public short EstadoId { get; set; }
        public decimal TipoCambio { get; set; }
        public string MotivoAnulado { get; set; }
        
        public int TipoDocumentoId { get; set; }

        public Caja Caja { get; set; }
        public IngresosEgresosBancoEstado Estado { get; set; }
        public TipoDocumento TipoDocumento { get; set; }
        public TipoMoneda TipoMoneda { get; set; }
        public TipoMovimiento TipoMovimiento { get; set; }
        public Profile UsernameNavigation { get; set; }
    }
}

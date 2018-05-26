using System;
using System.Collections.Generic;

namespace mvcIpsa.DbModel
{
    public partial class IngresosEgresosCaja
    {
        public IngresosEgresosCaja()
        {
            IngresosEgresosCajaDetalle = new HashSet<IngresosEgresosCajaDetalle>();
            IngresosEgresosCajaReferencias = new HashSet<IngresosEgresosCajaReferencias>();
        }

        public int Id { get; set; }
        public int TipoMovimientoId { get; set; }
        public string NumRecibo { get; set; }
        public short EstadoId { get; set; }
        public decimal Total { get; set; }
        public string Concepto { get; set; }
        public string NoOrdenPago { get; set; }
        public int? TipoIngresoId { get; set; }
        public int? TipoMonedaId { get; set; }
        public string Username { get; set; }
        public int? ClienteId { get; set; }
        public int CajaId { get; set; }
        public DateTime FechaRegistro { get; set; }
        public DateTime FechaProceso { get; set; }
        public string Muestra { get; set; }
        public string MotivoAnulado { get; set; }
        public string Beneficiario { get; set; }
        public short? TipoCleinteId { get; set; }
        public short Referencias { get; set; }

        public Caja Caja { get; set; }
        public CajaEstado Estado { get; set; }
        public TipoIngreso TipoIngreso { get; set; }
        public TipoMoneda TipoMoneda { get; set; }
        public TipoMovimiento TipoMovimiento { get; set; }
        public Profile UsernameNavigation { get; set; }
        public ICollection<IngresosEgresosCajaDetalle> IngresosEgresosCajaDetalle { get; set; }
        public ICollection<IngresosEgresosCajaReferencias> IngresosEgresosCajaReferencias { get; set; }
    }
}

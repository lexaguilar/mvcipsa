using System;
using System.Collections.Generic;

namespace mvcIpsa.DbModel
{
    public partial class IngresosEgresosCaja
    {
        public IngresosEgresosCaja()
        {
            IngresosEgresosCajaDetalle = new HashSet<IngresosEgresosCajaDetalle>();
        }

        public int Idrecibo { get; set; }
        public int Tipomov { get; set; }
        public string Numrecibo { get; set; }
        public short Nestado { get; set; }
        public int Idtipopago { get; set; }
        public decimal Montoefectivo { get; set; }
        public decimal Montocheque { get; set; }
        public decimal Montominuta { get; set; }
        public decimal Montotransferencia { get; set; }
        public decimal Monto { get; set; }
        public string Noreferencia { get; set; }
        public string Cuentabanco { get; set; }
        public string Concepto { get; set; }
        public string Noordenpago { get; set; }
        public int? Idtipoingreso { get; set; }
        public int? Idtipomoneda { get; set; }
        public string Username { get; set; }
        public int Ncentrocosto { get; set; }
        public string Identificacioncliente { get; set; }
        public string Cuentacontablebanco { get; set; }
        public decimal Tipocambio { get; set; }
        public int IdCaja { get; set; }
        public DateTime Fecharegistro { get; set; }
        public DateTime FechaProceso { get; set; }

        public Caja IdCajaNavigation { get; set; }
        public TipoIngreso IdtipoingresoNavigation { get; set; }
        public TipoMoneda IdtipomonedaNavigation { get; set; }
        public TipoPago IdtipopagoNavigation { get; set; }
        public CajaEstado NestadoNavigation { get; set; }
        public TipoMovimiento TipomovNavigation { get; set; }
        public ICollection<IngresosEgresosCajaDetalle> IngresosEgresosCajaDetalle { get; set; }
    }
}

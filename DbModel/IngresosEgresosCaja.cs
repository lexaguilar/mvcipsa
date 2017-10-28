using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace mvcIpsa.DbModel
{
    public partial class IngresosEgresosCaja
    {
        public IngresosEgresosCaja()
        {
            IngresosEgresosCajaDetalle = new HashSet<IngresosEgresosCajaDetalle>();
        }

      
        public int Id { get; set; }
        [Display(Name = "Tipo movimiento")]
        public int Tipomov { get; set; }
        [Required]
        [Display(Name = "No de recibo")]
        public string Numrecibo { get; set; }
        public short Nestado { get; set; }
        public DateTime FechaProceso { get; set; }
        [Required]
        [Display(Name = "Forma de pago")]
        public int Idtipopago { get; set; }
        [Display(Name = "Monto en efectivo")]
        public decimal? Montoefectivo { get; set; }
        [Display(Name = "Monto en cheque")]
        public decimal? Montocheque { get; set; }
        [Display(Name = "Monto en minuta")]
        public decimal? Montominuta { get; set; }
        [Display(Name = "Monto en tranferencia")]
        public decimal? Montotransferencia { get; set; }
        [Display(Name = "Total")]
        public decimal Monto { get; set; }
        [Display(Name = "No referencia")]
        public string Noreferencia { get; set; }
        [Display(Name = "Cta bancaria")]
        public string Cuentabanco { get; set; }
        public string Concepto { get; set; }
        [Display(Name = "Orden de pago")]
        public string Noordenpago { get; set; }
        [Display(Name = "Tipo ingreso")]
        public int? Idtipoingreso { get; set; }
        [Display(Name = "Moneda")]
        public int? Idtipomoneda { get; set; }
        public string Username { get; set; }
        public DateTime Fechacreacion { get; set; }
        public int Ncentrocosto { get; set; }
        [Display(Name = "Cedula/ RUC")]
        public string Identificacioncliente { get; set; }
        public string Cuentacontablebanco { get; set; }
        public decimal Tipocambio { get; set; }
        public TimeSpan Fecharegistro { get; set; }

        public Tipoingreso IdtipoingresoNavigation { get; set; }
        public Tipomoneda IdtipomonedaNavigation { get; set; }
        public Tipopago IdtipopagoNavigation { get; set; }
        public CajaEstado NestadoNavigation { get; set; }
        public TipoMovimiento TipomovNavigation { get; set; }
        public ICollection<IngresosEgresosCajaDetalle> IngresosEgresosCajaDetalle { get; set; }
    }
}

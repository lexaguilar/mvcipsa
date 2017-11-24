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

        public int Idrecibo { get; set; }
        public int Tipomov { get; set; }
        public string Numrecibo { get; set; }
        public short Nestado { get; set; }
        [Required]
        [Display(Name = "Tipo de pago")]
        public int Idtipopago { get; set; }
        
        [Display(Name = "Monto efectivo")]
        public decimal Montoefectivo { get; set; }
        [Display(Name = "Monto cheque")]
        public decimal Montocheque { get; set; }
        [Display(Name = "Monto minuta")]
        public decimal Montominuta { get; set; }
        [Display(Name = "Monto tranferencia")]
        public decimal Montotransferencia { get; set; }
        [Display(Name = "Monto total")]
        public decimal Monto { get; set; }
        [Display(Name = "Mo. transferencia")]
        public string Noreferencia { get; set; }
        [Display(Name = "Banco")]
        public string Cuentabanco { get; set; }
        public string Concepto { get; set; }
        [Display(Name = "No orden pago")]
        public string Noordenpago { get; set; }
        [Display(Name = "Tipo ingreso")]
        public int? Idtipoingreso { get; set; }
        [Display(Name = "Moneda")]
        public int? Idtipomoneda { get; set; }
        public string Username { get; set; }
        public int? Bancoid { get; set; }
        [Display(Name = "Cliente")]
        public string Identificacioncliente { get; set; }
        public string Cuentacontablebanco { get; set; }
        public decimal Tipocambio { get; set; }
        public int IdCaja { get; set; }
        public DateTime Fecharegistro { get; set; }
        [Display(Name = "Fecha de proceso")]
        public DateTime FechaProceso { get; set; }

        public Bancos Banco { get; set; }
        public Caja IdCajaNavigation { get; set; }
        public TipoIngreso IdtipoingresoNavigation { get; set; }
        public TipoMoneda IdtipomonedaNavigation { get; set; }
        public TipoPago IdtipopagoNavigation { get; set; }
        public CajaEstado NestadoNavigation { get; set; }
        public TipoMovimiento TipomovNavigation { get; set; }
        public ICollection<IngresosEgresosCajaDetalle> IngresosEgresosCajaDetalle { get; set; }
    }
}

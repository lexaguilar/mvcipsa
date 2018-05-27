using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace mvcIpsa.Models
{
    public class BancoViewModel
    {
        public int Id { get; set; }
        public DateTime FechaRegistro { get; set; }
        public string Referencia { get; set; }
        public decimal Monto { get; set; }
        public int TipoMonedaId { get; set; }
        public string TipoMoneda { get; set; }
        public string CuentaContableBanco { get; set; }
        public int TipoMovimientoId { get; set; }
        public string TipoMovimiento { get; set; }
        public string Concepto { get; set; }
        public DateTime FechaProceso { get; set; }
        public string Username { get; set; }
        public int CajaId { get; set; }
        public string Caja { get; set; }
        public int EstadoId { get; set; }
        public string Estado { get; set; }
        public decimal TipoCambio { get; set; }
        public string MotivoAnulado { get; set; }

    }
}

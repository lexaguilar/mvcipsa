using System;
using System.Collections.Generic;

namespace mvcIpsa.DbModel
{
    public partial class ProcesoBanco
    {
        public ProcesoBanco()
        {
            ConciliacionBancaria = new HashSet<ConciliacionBancaria>();
            ConciliacionBancariaAux = new HashSet<ConciliacionBancariaAux>();
        }

        public int Id { get; set; }
        public int BancoCuenta { get; set; }
        public decimal SaldoInicial { get; set; }
        public decimal SaldoFinal { get; set; }
        public DateTime Fecha { get; set; }
        public string Username { get; set; }
        public DateTime FechaRegistrado { get; set; }
        public int TipoProcesoId { get; set; }

        public TipoProceso TipoProceso { get; set; }
        public Profile UsernameNavigation { get; set; }
        public ICollection<ConciliacionBancaria> ConciliacionBancaria { get; set; }
        public ICollection<ConciliacionBancariaAux> ConciliacionBancariaAux { get; set; }
    }
}

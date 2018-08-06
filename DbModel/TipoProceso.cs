using System;
using System.Collections.Generic;

namespace mvcIpsa.DbModel
{
    public partial class TipoProceso
    {
        public TipoProceso()
        {
            ProcesoBanco = new HashSet<ProcesoBanco>();
        }

        public int Id { get; set; }
        public string Descripcion { get; set; }

        public ICollection<ProcesoBanco> ProcesoBanco { get; set; }
    }
}

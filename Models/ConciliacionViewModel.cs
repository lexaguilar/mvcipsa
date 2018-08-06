using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using mvcIpsa.DbModel;
namespace mvcIpsa.Models
{
    public class ConciliacionViewModel
    {
        public int BancoCuenta { get; set; }
        public int Year { get; set; }
        public int Month { get; set; }
        public IEnumerable<ConciliacionBancariaAux> conciliacionBancariaAux { get; set; }
        public IEnumerable<ConciliacionBancaria> conciliacionBancaria { get; set; }
    }
}

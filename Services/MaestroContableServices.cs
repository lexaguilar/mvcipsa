using mvcIpsa.DbModelIPSA;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace mvcIpsa.Services
{
    public class MaestroContableServices
    {
        private readonly DBIPSAContext DbIpsa;
        public MaestroContableServices(DBIPSAContext _DbIpsa)
        {
            DbIpsa = _DbIpsa;
        }

        internal MaestroContable[] ObtenerServicios()
        {
            return DbIpsa.MaestroContable
                .Where(mc => mc.TipoCta == 4 || mc.Cuenta.StartsWith("1101") || mc.Cuenta.StartsWith("1108") || mc.Cuenta.StartsWith("1105"))                
               //.Where(mc => mc.TipoCta == 4 || mc.Cuenta.StartsWith("4201") || mc.Cuenta.StartsWith("4301") || mc.Cuenta.StartsWith("1101"))
               .ToArray();
        }
    }
}

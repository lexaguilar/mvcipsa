using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using mvcIpsa.DbModel;
using mvcIpsa.Models;
namespace mvcIpsa.Services
{
    public class TasaCambioServices
    {
        private readonly IPSAContext db;
        public TasaCambioServices(IPSAContext _db)
        {
            db = _db;
        }
    }
}

using mvcIpsa.DbModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace mvcIpsa.Services
{
    public class ReporteFirmaServices
    {
        private readonly IPSAContext db;
        public ReporteFirmaServices(IPSAContext _db)
        {
            db = _db;
        }

        public IEnumerable<ReporteFirma> GetList()
        {

            var reporteFirma = db.ReporteFirma;
            return reporteFirma;
        }
    }
}

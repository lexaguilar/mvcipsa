using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using mvcIpsa.DbModel;
using mvcIpsa.Models;
namespace mvcIpsa.Services
{
    public class TipoMovimientoServices
    {
        private readonly IPSAContext db;
        public TipoMovimientoServices(IPSAContext _db)
        {
            db = _db;
        }

        public IEnumerable<TipoMovimiento> GetList()
        {          
            return db.TipoMovimiento;
        }

        public IEnumerable<CatalogViewModel> GetViewModelList()
        {

           return this.GetList().Select(tm => new CatalogViewModel{
               Id = tm.Id.ToString(),
               Text = tm.Descripcion.TrimEnd()
           });          
        }
    }
}

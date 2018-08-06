using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using mvcIpsa.DbModel;
using mvcIpsa.Models;
namespace mvcIpsa.Services
{
    public class TipoDocumentoServices
    {
        private readonly IPSAContext db;
        public TipoDocumentoServices(IPSAContext _db)
        {
            db = _db;
        }

        public IEnumerable<TipoDocumento> GetList()
        {          
            return db.TipoDocumento;
        }

        public IEnumerable<CatalogViewModel> GetViewModelList()
        {

           return this.GetList().Select(td => new CatalogViewModel{
               Id = td.Id.ToString(),
               Text = td.Descripcion.TrimEnd()
           });          
        }
    }
}

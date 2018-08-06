using Microsoft.EntityFrameworkCore;
using mvcIpsa.DbModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace mvcIpsa.Services
{
    public class ClienteServices
    {
        private readonly IPSAContext db;
        public ClienteServices(IPSAContext _db)
        {
            db = _db;
        }

        /// <summary>
        /// Obtiene la lista de clientes
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Cliente> GetList()
        {

            var clientes = db.Cliente.Include(c => c.TipoCliente);
            return clientes;
        }

        /// <summary>
        /// Obtiene la lista de clientes filtrados por Lista de clientes 
        /// </summary>
        /// <param name="identificacion"></param>
        /// <returns>Lista de clientes</returns>
        public IEnumerable<Cliente> GetList(string Identificacion, int PerPage, int Page)
        {          
            if (char.IsDigit(Identificacion[0]))
            {
                var clientes = db.Cliente.Include(c => c.TipoCliente).Where(c => c.Identificacion.StartsWith(Identificacion));
                return clientes.Skip((Page - 1) * PerPage).Take(PerPage);
            }
            else
            {
                var clientes = db.Cliente.Include(c => c.TipoCliente).Where(c => c.Nombre.StartsWith(Identificacion) || c.Apellido.StartsWith(Identificacion));
                return clientes.Skip((Page - 1) * PerPage).Take(PerPage);
            }
        }       
    }
}

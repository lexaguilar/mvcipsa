using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using mvcIpsa.DbModelIPSA;
using mvcIpsa.Models;
namespace mvcIpsa.Services
{
    public class BancoCuentasServices
    {
        private readonly DBIPSAContext DbIpsa;
        public BancoCuentasServices(DBIPSAContext _DbIpsa)
        {
            DbIpsa = _DbIpsa;
        }
       
        public IEnumerable<BancosCuentas> GetList()
        {

            var bancosCuentas = DbIpsa.BancosCuentas.Include(bc => bc.Banco);
            return bancosCuentas;
        }

        public IEnumerable<BancoCuentaViewModel> GetViewModelList()
        {

            var bancosCuentas = DbIpsa.BancosCuentas.Include(bc => bc.Banco).Select(bc => new BancoCuentaViewModel
            {
                Id = bc.BancoCuenta,
                Text = bc.Descripcion.TrimEnd(),
                Banco = bc.Banco.Descripcion,
                Moneda = bc.Moneda == 1 ? "Cordobas" : "Dolares"

            });
            return bancosCuentas;
        }
    }
}

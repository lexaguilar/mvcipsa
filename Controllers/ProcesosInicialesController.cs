using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using mvcIpsa.DbModel;
using mvcIpsa.DbModelIPSA;
using mvcIpsa.Extensions;
using mvcIpsa.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace mvcIpsa.Controllers
{
    [Authorize]
    public class ProcesosInicialesController : Controller
    {
        private readonly IPSAContext db;
        private readonly DBIPSAContext DbIpsa;
        public ProcesosInicialesController(IPSAContext _db, DBIPSAContext _DbIpsa)
        {
            db = _db;
            DbIpsa = _DbIpsa;
        }
        public IActionResult Index()
        {
            return View();
        }
        
        public IActionResult getlist()
        {
            var bancoCuentasServices = new BancoCuentasServices(DbIpsa);
            var procesoBancoSesrvices = new ProcesoBancoServices(db);
            var bancos = bancoCuentasServices.GetList().Select(bc => new
            {
                bc.BancoCuenta,
                bc.Descripcion,
                Moneda = bc.Moneda == 1 ? "Cordobas" : "Dolares",
                Banco = bc.Banco.Descripcion,
                SaldoInicial = procesoBancoSesrvices.ExistSaldoIncial(bc.BancoCuenta) ? procesoBancoSesrvices.getSaldoIncial(bc.BancoCuenta).SaldoInicial : 0,
                Fecha = procesoBancoSesrvices.ExistSaldoIncial(bc.BancoCuenta) ? procesoBancoSesrvices.getSaldoIncial(bc.BancoCuenta).Fecha : DateTime.Today
            });

            return Json(bancos);
        }

        [HttpPost]
        public async Task<IActionResult> update(int BancoCuenta, [Bind("BancoCuenta,SaldoInicial,Fecha")] ProcesoBanco procesoBanco)
        {
            var user = this.GetServiceUser();
            var procesoBancoSesrvices = new ProcesoBancoServices(db);

            var Existe = procesoBancoSesrvices.ExistSaldoIncial(BancoCuenta);

            if (Existe)
            {
                var oldProcesoBanco = db.ProcesoBanco.Where(b => b.BancoCuenta == BancoCuenta && b.TipoProcesoId == (int)TipoProcesos.SaldoInicial).FirstOrDefault();
                if (procesoBanco.SaldoInicial != 0)
                {
                    oldProcesoBanco.SaldoInicial = procesoBanco.SaldoInicial;
                }

                if (procesoBanco.Fecha > DateTime.MinValue)
                {
                    oldProcesoBanco.Fecha = new DateTime(procesoBanco.Fecha.Year, procesoBanco.Fecha.Month,1);
                }

                db.SaveChanges();
            }
            else
            {

                var newProcesoBanco = new ProcesoBanco
                {
                    BancoCuenta = BancoCuenta,
                    SaldoInicial = procesoBanco.SaldoInicial,
                    SaldoFinal = 0,
                    Fecha = procesoBanco.Fecha,
                    Username = user.username,
                    FechaRegistrado = DateTime.Now,
                    TipoProcesoId = (int)TipoProcesos.SaldoInicial
                };

                db.ProcesoBanco.Add(newProcesoBanco);
                db.SaveChanges();
            }

            return Ok();
        }

    }
}

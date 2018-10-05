using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using mvcIpsa.DbModel;
using mvcIpsa.DbModelIPSA;
using mvcIpsa.Models;
using mvcIpsa.Services;

namespace mvcIpsa.Controllers
{
    [Produces("application/json")]
    [Route("api/BancoCuentas")]
    public class BancoCuentasController : Controller
    {
        private readonly DBIPSAContext DbIpsa;
        private readonly IPSAContext db;
        public BancoCuentasController(IPSAContext _db, DBIPSAContext _DbIpsa)
        {
            DbIpsa = _DbIpsa;
            db = _db;
        }
        public IActionResult Get()
        {
            var bancoCuentasServices = new BancoCuentasServices(DbIpsa);
            return Json(bancoCuentasServices.GetList().ToArray());
        }

        // [HttpPost]
        // public IActionResult Post(CuentasBancoUsernameViewModel model)
        // {
           

        //     foreach (int bancoCuenta in model.IdsToAdd ?? new int[] { })
        //     {
        //         var cuentasBancoUsername = new CuentasBancoUsername { BancoCuenta = bancoCuenta, Username = model.username };
        //         db.Add(cuentasBancoUsername);
        //     }

        //     foreach (int bancoCuenta in model.IdsToDelete ?? new int[] { })
        //     {
        //         var cuentasBancoUsername = db.CuentasBancoUsername.Where(p => p.Username == model.username && p.BancoCuenta == bancoCuenta).FirstOrDefault();
        //         if (cuentasBancoUsername != null)
        //         {
        //             db.CuentasBancoUsername.Remove(cuentasBancoUsername);
        //         }
        //     }
        //     await db.SaveChangesAsync();
        //     return RedirectToAction("EditAccountBank", new { id = model.username });
            
        //     return View("Error", new string[] { "Role Not Found" });
        // }
    }
}
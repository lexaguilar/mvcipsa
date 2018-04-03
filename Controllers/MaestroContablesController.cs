using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using mvcIpsa.DbModelIPSA;

namespace mvcIpsa.Controllers
{
    public class MaestroContablesController : Controller
    {
        private readonly DBIPSAContext _context;

        public MaestroContablesController(DBIPSAContext context)
        {
            _context = context;
        }

        // GET: MaestroContables
        public async Task<IActionResult> Index()
        {
            return View();
        }
        [HttpGet("MaestroContable/obtenerLista")]
        public async Task<IActionResult> ObtenerLista()
        {
            var cuentas = await _context.MaestroContable
                .Where(mc => mc.TipoCta == 4 || mc.Cuenta.StartsWith("1101") || mc.Cuenta.StartsWith("1108") || mc.Cuenta.StartsWith("1105"))
                .ToArrayAsync();

            var servicios = from mc in cuentas
                            join mcp in cuentas
                            on mc.CtaPadre equals mcp.CtaContable
                            select new
                            {
                                NombrePadre = mcp.Nombre,
                                ctaPadre = mcp.Cuenta,
                                mc.Cuenta,
                                mc.Nombre,                                
                                mc.CtaContable
                            };

            return Json(servicios);
        }
    }
}

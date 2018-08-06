using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using mvcIpsa.DbModel;
using mvcIpsa.Models;
using Microsoft.AspNetCore.Authorization;
using mvcIpsa.DbModelIPSA;

namespace mvcIpsa.Controllers
{
    [Authorize(Policy = "Admin")]
    public class CajasController : Controller
    {
        private readonly IPSAContext _context;
        private readonly DBIPSAContext DbIpsa;

        public CajasController(IPSAContext context, DBIPSAContext _DbIpsa)
        {
            _context = context;
            DbIpsa = _DbIpsa;
        }

        // GET: Cajas
        /// <summary>
        /// Obtiene las cajas
        /// </summary>
        /// <param name="planId">Id del plan</param>
        /// <returns></returns>
        public async Task<IActionResult> Index()
        {
            return View();
        }
        [ActionName("getList")]
        public async Task<IActionResult> getList()
        {
            return Json(await _context.Caja.ToListAsync());
        }

        // GET: Cajas/Details/5
        
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var caja = await _context.Caja
                .SingleOrDefaultAsync(m => m.Id == id);
            if (caja == null)
            {
                return NotFound();
            }

            return View(caja);
        }

        // GET: Cajas/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Cajas/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,NoCaja,Description")] Caja caja)
        {
            if (ModelState.IsValid)
            {
                _context.Add(caja);
                await _context.SaveChangesAsync();

                var loterecibo = new LoteRecibos
                {
                    CajaId = caja.Id,
                    Actual = 0,
                    Final = 0,
                    Inicio = 0
                };

                _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            return View(caja);
        }

        // GET: Cajas/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var caja = await _context.Caja.SingleOrDefaultAsync(m => m.Id == id);
            if (caja == null)
            {
                return NotFound();
            }
            return View(caja);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,NoCaja,Description")] Caja caja)
        {
            if (id != caja.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(caja);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CajaExists(caja.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(caja);
        }

        // GET: Cajas/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var caja = await _context.Caja
                .SingleOrDefaultAsync(m => m.Id == id);
            if (caja == null)
            {
                return NotFound();
            }

            return View(caja);
        }

        // POST: Cajas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var caja = await _context.Caja.SingleOrDefaultAsync(m => m.Id == id);
            _context.Caja.Remove(caja);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CajaExists(int id)
        {
            return _context.Caja.Any(e => e.Id == id);
        }

        public async Task<IActionResult> EditAccountsById(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cuentas = DbIpsa.MaestroContable
                .Where(mc => mc.TipoCta == 4 || mc.Cuenta.StartsWith("1101") || mc.Cuenta.StartsWith("1108") || mc.Cuenta.StartsWith("1105"))
                .ToArray();

             var servicios = from mc in cuentas
                            join mcp in cuentas
                            on mc.CtaPadre equals mcp.CtaContable
                            select new
                            {
                                mc.Cuenta,
                                mc.Nombre,
                                Padre = mcp.Nombre,
                                mc.CtaContable
                            };

            string[] CuentaList = _context.CajaCuentaContable.Where(x => x.CajaId == id.Value).Select(x => x.CtaCuenta).ToArray();
            var members = servicios.Where(x => CuentaList.Any(y => y == x.CtaContable)).ToList();
   
            var _caja = _context.Caja.Find(id);

            var cuentasPorCaja = new CajaCtaContablesViewModel
            {
                idCaja = id.Value,
                description = _caja.Description,
                MaestroContableMembers = members.Select(x => new MaestroContableViewModel
                {
                    Nombre = x.Nombre,
                    CtaContable = x.CtaContable,
                    Cuenta = x.Cuenta,
                    Padre = x.Padre
                }).ToArray()
            };
            return View("ListOfAccounting", cuentasPorCaja);
        }

        
        public async Task<IActionResult> EditLotes(int? id)
        {
            if (id==null)
            {
                return NotFound();
            }

            var lote = _context.LoteRecibos.Where(l => l.CajaId == id.Value).FirstOrDefault();
            ViewBag.Caja = _context.Caja.Find(id).Description;
            if (lote == null)
            {
                var newLote = new LoteRecibos
                {
                    CajaId = id.Value,
                    Actual = 0,
                    Final = 0,
                    Inicio = 0
                };
                _context.LoteRecibos.Add(newLote);
                _context.SaveChanges();
                return View(newLote);
            }

            decimal porcentaje = 0;
            if(Convert.ToInt16(lote.Final - lote.Inicio)!=0)              
                porcentaje = (Convert.ToDecimal(lote.Actual - lote.Inicio) / Convert.ToDecimal(lote.Final - lote.Inicio)) * 100;

            ViewBag.Porcentaje = Math.Round(porcentaje, 2);
            return View(lote);
        }
        [HttpPost]
        [ActionName("saveLote")]
        [Produces("application/json")]
        public async Task<IActionResult> saveLote(LoteRecibos lote)
        {
            var LoteActual = await _context.LoteRecibos.FindAsync(lote.Id);
            if (LoteActual == null)
            {
                return NotFound();
            }

            LoteActual.Inicio = lote.Inicio;
            LoteActual.Final = lote.Final;
            LoteActual.Actual = lote.Actual;
            await _context.SaveChangesAsync();

            return Json(LoteActual);
        }

        [HttpGet("Cajas/GetAccounting/{id}/nomembers")]
        public IActionResult GetAccounting(int id)
        {
            var cuentas = DbIpsa.MaestroContable
                .Where(mc => mc.TipoCta == 4 || mc.Cuenta.StartsWith("1101") || mc.Cuenta.StartsWith("1108") || mc.Cuenta.StartsWith("1105"))
                .ToArray();

            var servicios = from mc in cuentas
                            join mcp in cuentas
                            on mc.CtaPadre equals mcp.CtaContable
                            select new
                            {
                                mc.Cuenta,
                                mc.Nombre,
                                Padre = mcp.Nombre,
                                mc.CtaContable
                            };

            string[] CuentaList = _context.CajaCuentaContable.Where(x => x.CajaId == id).Select(x => x.CtaCuenta).ToArray();

            var NoMembers = servicios.Where(x => !CuentaList.Contains(x.CtaContable)).ToList();

            return Json(NoMembers);
        }

        [HttpPost("Cajas/AddAccounting/{idCaja}/add")]
        public async Task<IActionResult> AddAccounting(string[] CtaContables, int idCaja)
        {
            //var _CtaContables = CtaContables.Split(',');
            foreach (var item in CtaContables)
            {
                var cajaCuentaContable = _context.CajaCuentaContable.Where(c=>c.CtaCuenta == item && c.CajaId == idCaja).FirstOrDefault();
                if (cajaCuentaContable == null)
                {
                    var newCajaCuentaContable = new CajaCuentaContable{
                         CtaCuenta = item, CajaId= idCaja
                    };
                    _context.CajaCuentaContable.Add(newCajaCuentaContable);
                }

            }
            _context.SaveChanges();
            return Ok(0);
        }


        [HttpGet("Cajas/getServices/{id}/members")]
        public IActionResult Execept(int id)
        {
            var cuentas = DbIpsa.MaestroContable
                .Where(mc => mc.TipoCta == 4 || mc.Cuenta.StartsWith("1101") || mc.Cuenta.StartsWith("1108") || mc.Cuenta.StartsWith("1105"))
                .ToArray();

            var servicios = from mc in cuentas
                            join mcp in cuentas
                            on mc.CtaPadre equals mcp.CtaContable
                            select new
                            {
                                mc.Cuenta,
                                mc.Nombre,
                                Padre = mcp.Nombre,
                                mc.CtaContable
                            };

            string[] CuentaList = _context.CajaCuentaContable.Where(x => x.CajaId == id).Select(x => x.CtaCuenta).ToArray();

            var Members = servicios.Where(x => CuentaList.Contains(x.CtaContable)).ToList();

            return Json(Members);
        }

        [HttpPost("Cajas/AddAccounting/{idCaja}/remove")]
        public async Task<IActionResult> removeCaja(string[] CtaContables, int idCaja)
        {           
            foreach (var item in CtaContables)
            {
                var cajaCuentaContable = _context.CajaCuentaContable.Where(c => c.CtaCuenta == item && c.CajaId == idCaja).FirstOrDefault();
                if (cajaCuentaContable != null)
                {
                    _context.CajaCuentaContable.Remove(cajaCuentaContable);
                }
            }
            _context.SaveChanges();
            return Ok(0);
        }
    }
}

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

namespace mvcIpsa.Controllers
{
    [Authorize(Policy = "Admin")]
    public class CajasController : Controller
    {
        private readonly IPSAContext _context;

        public CajasController(IPSAContext context)
        {
            _context = context;
        }

        // GET: Cajas
        public async Task<IActionResult> Index()
        {
            return View(await _context.Caja.ToListAsync());
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
                    IdCaja = caja.Id,
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

        // POST: Cajas/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
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

            var servicios = from mc in _context.MaestroContable
                            join mcp in _context.MaestroContable
                            on mc.CtaPadre equals mcp.CtaContable
                            where mc.TipoCta == 4 || mc.Cuenta.StartsWith("1101") || mc.Cuenta.StartsWith("1108") || mc.Cuenta.StartsWith("1105")
                            select new
                            {
                                mc.Cuenta,
                                mc.Nombre,
                                Padre = mcp.Nombre,
                                mc.CtaContable
                            };

            string[] CuentaList = _context.CajaCuentaContable.Where(x => x.IdCaja == id.Value).Select(x => x.CtaCuenta).ToArray();
            var members = servicios.Where(x => CuentaList.Any(y => y == x.CtaContable)).ToList();
            //var nonMembers = servicios.Except(members);
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

        //EditLotes
        public async Task<IActionResult> EditLotes(int? id)
        {
            if (id==null)
            {
                return NotFound();
            }

            var lote = _context.LoteRecibos.Where(l => l.IdCaja == id.Value).FirstOrDefault();
            if (lote == null)
            {                
                return View(lote);
            }

            ViewBag.Caja = _context.Caja.Find(id).Description;
            var porcentaje = (Convert.ToDecimal(lote.Actual - lote.Inicio) / Convert.ToDecimal(lote.Final - lote.Inicio)) * 100;
            ViewBag.Porcentaje = Math.Round(porcentaje, 2);
            return View(lote);
        }
        [HttpPost]
        [ActionName("saveLote")]
        [Produces("application/json")]
        public async Task<IActionResult> saveLote(LoteRecibos lote)
        {
            var LoteActual = await _context.LoteRecibos.FindAsync(lote.IdCaja);
            if (LoteActual == null)
            {
                return NotFound();
            }

            LoteActual.Inicio = lote.Inicio;
            LoteActual.Final = lote.Final;

            await _context.SaveChangesAsync();

            return Json(LoteActual);
        }
    }
}

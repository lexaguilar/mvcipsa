using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using mvcIpsa.DbModel;

namespace mvcIpsa.Controllers
{
    public class IngresosEgresosCajasController : Controller
    {
        private readonly IPSAContext _context;

        public IngresosEgresosCajasController(IPSAContext context)
        {
            _context = context;
        }

        // GET: IngresosEgresosCajas
        public async Task<IActionResult> Index()
        {
            var iPSAContext = _context.IngresosEgresosCaja.Include(i => i.IdtipoingresoNavigation).Include(i => i.IdtipomonedaNavigation).Include(i => i.IdtipopagoNavigation).Include(i => i.NestadoNavigation).Include(i => i.TipomovNavigation);
            return View(await iPSAContext.ToListAsync());
        }

        // GET: IngresosEgresosCajas/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ingresosEgresosCaja = await _context.IngresosEgresosCaja
                .Include(i => i.IdtipoingresoNavigation)
                .Include(i => i.IdtipomonedaNavigation)
                .Include(i => i.IdtipopagoNavigation)
                .Include(i => i.NestadoNavigation)
                .Include(i => i.TipomovNavigation)
                .SingleOrDefaultAsync(m => m.Id == id);
            if (ingresosEgresosCaja == null)
            {
                return NotFound();
            }

            return View(ingresosEgresosCaja);
        }

        // GET: IngresosEgresosCajas/Create
        public IActionResult Create()
        {
            ViewData["Idtipoingreso"] = new SelectList(_context.Tipoingreso, "Idtipoingreso", "Descripcion");
            ViewData["Idtipomoneda"] = new SelectList(_context.Tipomoneda, "Idtipomoneda", "Descripcion");
            ViewData["Idtipopago"] = new SelectList(_context.Tipopago, "Idtipopago", "Descripcion");
            ViewData["Nestado"] = new SelectList(_context.CajaEstado, "Nestado", "Descripcion");
            ViewData["Tipomov"] = new SelectList(_context.TipoMovimiento, "Idtipomovimiento", "Descripcion");
            return View();
        }

        // POST: IngresosEgresosCajas/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Tipomov,Numrecibo,Nestado,FechaProceso,Idtipopago,Montoefectivo,Montocheque,Montominuta,Montotransferencia,Monto,Noreferencia,Cuentabanco,Concepto,Noordenpago,Idtipoingreso,Idtipomoneda,Username,Fechacreacion,Ncentrocosto,Identificacioncliente,Cuentacontablebanco,Tipocambio")] IngresosEgresosCaja ingresosEgresosCaja)
        {
            if (ModelState.IsValid)
            {
                _context.Add(ingresosEgresosCaja);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["Idtipoingreso"] = new SelectList(_context.Tipoingreso, "Idtipoingreso", "Descripcion", ingresosEgresosCaja.Idtipoingreso);
            ViewData["Idtipomoneda"] = new SelectList(_context.Tipomoneda, "Idtipomoneda", "Descripcion", ingresosEgresosCaja.Idtipomoneda);
            ViewData["Idtipopago"] = new SelectList(_context.Tipopago, "Idtipopago", "Descripcion", ingresosEgresosCaja.Idtipopago);
            ViewData["Nestado"] = new SelectList(_context.CajaEstado, "Nestado", "Descripcion", ingresosEgresosCaja.Nestado);
            ViewData["Tipomov"] = new SelectList(_context.TipoMovimiento, "Idtipomovimiento", "Descripcion", ingresosEgresosCaja.Tipomov);
            return View(ingresosEgresosCaja);
        }

        // GET: IngresosEgresosCajas/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ingresosEgresosCaja = await _context.IngresosEgresosCaja.SingleOrDefaultAsync(m => m.Id == id);
            if (ingresosEgresosCaja == null)
            {
                return NotFound();
            }
            ViewData["Idtipoingreso"] = new SelectList(_context.Tipoingreso, "Idtipoingreso", "Descripcion", ingresosEgresosCaja.Idtipoingreso);
            ViewData["Idtipomoneda"] = new SelectList(_context.Tipomoneda, "Idtipomoneda", "Descripcion", ingresosEgresosCaja.Idtipomoneda);
            ViewData["Idtipopago"] = new SelectList(_context.Tipopago, "Idtipopago", "Descripcion", ingresosEgresosCaja.Idtipopago);
            ViewData["Nestado"] = new SelectList(_context.CajaEstado, "Nestado", "Descripcion", ingresosEgresosCaja.Nestado);
            ViewData["Tipomov"] = new SelectList(_context.TipoMovimiento, "Idtipomovimiento", "Descripcion", ingresosEgresosCaja.Tipomov);
            return View(ingresosEgresosCaja);
        }

        // POST: IngresosEgresosCajas/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Tipomov,Numrecibo,Nestado,FechaProceso,Idtipopago,Montoefectivo,Montocheque,Montominuta,Montotransferencia,Monto,Noreferencia,Cuentabanco,Concepto,Noordenpago,Idtipoingreso,Idtipomoneda,Username,Fechacreacion,Ncentrocosto,Identificacioncliente,Cuentacontablebanco,Tipocambio")] IngresosEgresosCaja ingresosEgresosCaja)
        {
            if (id != ingresosEgresosCaja.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(ingresosEgresosCaja);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!IngresosEgresosCajaExists(ingresosEgresosCaja.Id))
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
            ViewData["Idtipoingreso"] = new SelectList(_context.Tipoingreso, "Idtipoingreso", "Descripcion", ingresosEgresosCaja.Idtipoingreso);
            ViewData["Idtipomoneda"] = new SelectList(_context.Tipomoneda, "Idtipomoneda", "Descripcion", ingresosEgresosCaja.Idtipomoneda);
            ViewData["Idtipopago"] = new SelectList(_context.Tipopago, "Idtipopago", "Descripcion", ingresosEgresosCaja.Idtipopago);
            ViewData["Nestado"] = new SelectList(_context.CajaEstado, "Nestado", "Descripcion", ingresosEgresosCaja.Nestado);
            ViewData["Tipomov"] = new SelectList(_context.TipoMovimiento, "Idtipomovimiento", "Descripcion", ingresosEgresosCaja.Tipomov);
            return View(ingresosEgresosCaja);
        }

        // GET: IngresosEgresosCajas/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ingresosEgresosCaja = await _context.IngresosEgresosCaja
                .Include(i => i.IdtipoingresoNavigation)
                .Include(i => i.IdtipomonedaNavigation)
                .Include(i => i.IdtipopagoNavigation)
                .Include(i => i.NestadoNavigation)
                .Include(i => i.TipomovNavigation)
                .SingleOrDefaultAsync(m => m.Id == id);
            if (ingresosEgresosCaja == null)
            {
                return NotFound();
            }

            return View(ingresosEgresosCaja);
        }

        // POST: IngresosEgresosCajas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var ingresosEgresosCaja = await _context.IngresosEgresosCaja.SingleOrDefaultAsync(m => m.Id == id);
            _context.IngresosEgresosCaja.Remove(ingresosEgresosCaja);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool IngresosEgresosCajaExists(int id)
        {
            return _context.IngresosEgresosCaja.Any(e => e.Id == id);
        }
    }
}

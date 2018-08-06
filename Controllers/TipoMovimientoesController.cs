using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using mvcIpsa.DbModel;
using Newtonsoft.Json;

namespace mvcIpsa.Controllers
{
    [Authorize]
    public class TipoMovimientoesController : Controller
    {
        private readonly IPSAContext _context;

        public TipoMovimientoesController(IPSAContext context)
        {
            _context = context;
        }

        // GET: TipoMovimientoes
        public async Task<IActionResult> Index()
        {
            return View();
        }

        public async Task<IActionResult> getlist()
        {
            
            var iPSAContext = _context.TipoMovimiento.Include(t => t.TipoDocNavigation).ToArray();
           
            return Json(iPSAContext,
            new JsonSerializerSettings { MaxDepth = 1, ReferenceLoopHandling = ReferenceLoopHandling.Ignore });
        }

        //TipoDocumentoLookup
        public async Task<IActionResult> TipoDocumentoLookup()
        {
            var iPSAContext = _context.TipoDocumento;
            return Json(await iPSAContext.ToListAsync());
        }

        // GET: TipoMovimientoes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tipoMovimiento = await _context.TipoMovimiento
                .Include(t => t.TipoDocNavigation)
                .SingleOrDefaultAsync(m => m.Id == id);
            if (tipoMovimiento == null)
            {
                return NotFound();
            }

            return View(tipoMovimiento);
        }

        // GET: TipoMovimientoes/Create
        public IActionResult Create()
        {
            ViewData["TipoDoc"] = new SelectList(_context.TipoDocumento, "Id", "Descripcion");
            return View();
        }

        // POST: TipoMovimientoes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Descripcion,TipoDoc,DocName")] TipoMovimiento tipoMovimiento)
        {
            if (ModelState.IsValid)
            {
                _context.Add(tipoMovimiento);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["TipoDoc"] = new SelectList(_context.TipoDocumento, "Id", "Descripcion", tipoMovimiento.TipoDoc);
            return View(tipoMovimiento);
        }

        // GET: TipoMovimientoes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tipoMovimiento = await _context.TipoMovimiento.SingleOrDefaultAsync(m => m.Id == id);
            if (tipoMovimiento == null)
            {
                return NotFound();
            }
            ViewData["TipoDoc"] = new SelectList(_context.TipoDocumento, "Id", "Descripcion", tipoMovimiento.TipoDoc);
            return View(tipoMovimiento);
        }

        // POST: TipoMovimientoes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> update(int id, [Bind("Id,Descripcion,TipoDoc,DocName")] TipoMovimiento tipoMovimiento)
        {
            var oldTipoMovimiento = _context.TipoMovimiento.Find(id);

            if (tipoMovimiento.Descripcion != null)
            {
                oldTipoMovimiento.Descripcion = tipoMovimiento.Descripcion;
            }

            if (tipoMovimiento.TipoDoc != 0)
            {
                oldTipoMovimiento.TipoDoc = tipoMovimiento.TipoDoc;
            }

            if (tipoMovimiento.DocName!= null)
            {
                oldTipoMovimiento.DocName = tipoMovimiento.DocName;
            }

            if (id != tipoMovimiento.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {                  
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TipoMovimientoExists(tipoMovimiento.Id))
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

            return Ok();
        }

        // GET: TipoMovimientoes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tipoMovimiento = await _context.TipoMovimiento
                .Include(t => t.TipoDocNavigation)
                .SingleOrDefaultAsync(m => m.Id == id);
            if (tipoMovimiento == null)
            {
                return NotFound();
            }

            return View(tipoMovimiento);
        }

        // POST: TipoMovimientoes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var tipoMovimiento = await _context.TipoMovimiento.SingleOrDefaultAsync(m => m.Id == id);
            _context.TipoMovimiento.Remove(tipoMovimiento);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TipoMovimientoExists(int id)
        {
            return _context.TipoMovimiento.Any(e => e.Id == id);
        }
    }

    public class Sort
    {
        public string selector { get; set; }
        public bool desc { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using mvcIpsa.DbModel;

namespace mvcIpsa.Controllers
{
    [Authorize]
    public class ReporteFirmasController : Controller
    {
        private readonly IPSAContext _context;

        public ReporteFirmasController(IPSAContext context)
        {
            _context = context;
        }

        // GET: ReporteFirmas
        public async Task<IActionResult> Index()
        {
            return View();
        }


        // GET: ReporteFirmas/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reporteFirma = await _context.ReporteFirma
                .SingleOrDefaultAsync(m => m.Reporte == id);
            if (reporteFirma == null)
            {
                return NotFound();
            }

            return View(reporteFirma);
        }

        // GET: ReporteFirmas/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: ReporteFirmas/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Reporte,UsernameElaborado,UsernameRevisado,UsernameAprobado,UsernameAutorizado")] ReporteFirma reporteFirma)
        {
            if (ModelState.IsValid)
            {
                _context.Add(reporteFirma);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(reporteFirma);
        }

        // GET: ReporteFirmas/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reporteFirma = await _context.ReporteFirma.SingleOrDefaultAsync(m => m.Reporte == id);
            if (reporteFirma == null)
            {
                return NotFound();
            }
            return View(reporteFirma);
        }

        // POST: ReporteFirmas/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]       
        public async Task<IActionResult> update(string Reporte, [Bind("Reporte,UsernameElaborado,UsernameRevisado,UsernameAprobado,UsernameAutorizado,MostrarElaborado,MostrarRevisado,MostrarAprobado,MostrarAutorizado")] ReporteFirma reporteFirma)
        {
            var oldReporteFirma = _context.ReporteFirma.Find(Reporte);

            if (!string.IsNullOrEmpty(reporteFirma.UsernameAprobado))            
                oldReporteFirma.UsernameAprobado = reporteFirma.UsernameAprobado;

            if (!string.IsNullOrEmpty(reporteFirma.UsernameAutorizado))
                oldReporteFirma.UsernameAutorizado = reporteFirma.UsernameAutorizado;

            if (!string.IsNullOrEmpty(reporteFirma.UsernameElaborado))
                oldReporteFirma.UsernameElaborado = reporteFirma.UsernameElaborado;

            if (!string.IsNullOrEmpty(reporteFirma.UsernameRevisado))
                oldReporteFirma.UsernameRevisado = reporteFirma.UsernameRevisado;

            if (reporteFirma.MostrarAprobado.HasValue)
                oldReporteFirma.MostrarAprobado = reporteFirma.MostrarAprobado;

            if (reporteFirma.MostrarAutorizado.HasValue)
                oldReporteFirma.MostrarAutorizado = reporteFirma.MostrarAutorizado;

            if (reporteFirma.MostrarElaborado.HasValue)
                oldReporteFirma.MostrarElaborado = reporteFirma.MostrarElaborado;

            if (reporteFirma.MostrarRevisado.HasValue)
                oldReporteFirma.MostrarRevisado = reporteFirma.MostrarRevisado;

            await _context.SaveChangesAsync();

            return Ok();
        }

        // GET: ReporteFirmas/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reporteFirma = await _context.ReporteFirma
                .SingleOrDefaultAsync(m => m.Reporte == id);
            if (reporteFirma == null)
            {
                return NotFound();
            }

            return View(reporteFirma);
        }

        // POST: ReporteFirmas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var reporteFirma = await _context.ReporteFirma.SingleOrDefaultAsync(m => m.Reporte == id);
            _context.ReporteFirma.Remove(reporteFirma);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ReporteFirmaExists(string id)
        {
            return _context.ReporteFirma.Any(e => e.Reporte == id);
        }
    }
}

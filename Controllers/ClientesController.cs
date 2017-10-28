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
    using mvcIpsa.Models;
    public class ClientesController : Controller
    {
        private readonly IPSAContext db;

        public ClientesController(IPSAContext _db)
        {
            db = _db;
        }

        // GET: Clientes
        public async Task<IActionResult> Index()
        {
            


            var clients = db.Cliente.Include(c => c.IdtipoclienteNavigation).Select(c=> new ClienteViewModel {
                Identificacion = c.Identificacion,
                Nombre =c.Nombre,
                Apellido=c.Apellido,
                Telefono =c.Telefono,
                Correo =c.Correo,
                Direccion=c.Direccion,
                tipocliente= c.IdtipoclienteNavigation.Tipocliente,
                Numeroruc =c.Numeroruc
            });
            return View(await clients.ToListAsync());
        }

        // GET: Clientes/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cliente = await db.Cliente
                .Include(c => c.IdtipoclienteNavigation)
                .SingleOrDefaultAsync(m => m.Identificacion == id);
            if (cliente == null)
            {
                return NotFound();
            }

            return View(cliente);
        }

        // GET: Clientes/Create
        public IActionResult Create()
        {
            ViewData["Idtipocliente"] = new SelectList(db.TipoCliente, "Idtipocliente", "Idtipocliente");
            return View();
        }

        // POST: Clientes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Identificacion,Nombre,Apellido,Telefono,Correo,Direccion,Idtipocliente,Numeroruc")] Cliente cliente)
        {
            if (ModelState.IsValid)
            {
                db.Add(cliente);
                await db.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["Idtipocliente"] = new SelectList(db.TipoCliente, "Idtipocliente", "Idtipocliente", cliente.Idtipocliente);
            return View(cliente);
        }

        // GET: Clientes/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cliente = await db.Cliente.SingleOrDefaultAsync(m => m.Identificacion == id);
            if (cliente == null)
            {
                return NotFound();
            }
            ViewData["Idtipocliente"] = new SelectList(db.TipoCliente, "Idtipocliente", "Idtipocliente", cliente.Idtipocliente);
            return View(cliente);
        }

        // POST: Clientes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Identificacion,Nombre,Apellido,Telefono,Correo,Direccion,Idtipocliente,Numeroruc")] Cliente cliente)
        {
            if (id != cliente.Identificacion)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    db.Update(cliente);
                    await db.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ClienteExists(cliente.Identificacion))
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
            ViewData["Idtipocliente"] = new SelectList(db.TipoCliente, "Idtipocliente", "Idtipocliente", cliente.Idtipocliente);
            return View(cliente);
        }

        // GET: Clientes/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cliente = await db.Cliente
                .Include(c => c.IdtipoclienteNavigation)
                .SingleOrDefaultAsync(m => m.Identificacion == id);
            if (cliente == null)
            {
                return NotFound();
            }

            return View(cliente);
        }

        // POST: Clientes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var cliente = await db.Cliente.SingleOrDefaultAsync(m => m.Identificacion == id);
            db.Cliente.Remove(cliente);
            await db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ClienteExists(string id)
        {
            return db.Cliente.Any(e => e.Identificacion == id);
        }
    }
}

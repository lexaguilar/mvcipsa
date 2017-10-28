using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using mvcIpsa.DbModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using mvcIpsa.Models;
namespace mvcIpsa.Controllers
{
    [Authorize]
    public class ProfileController : Controller
    {
        private readonly IPSAContext db;
        public ProfileController(IPSAContext _db)
        {
            db = _db;
        }
        public IActionResult Index()
        {
            var perfiles = db.Profile.Include(p => p.NcajaNavigation).ToList().Select(p => new ProfileViewModel
            {
                Username = p.Username,
                Nombre = p.Nombre,
                Apellido = p.Apellido,
                Correo = p.Correo,
                Nestado = p.Nestado,
                cajaDescripcion = p.NcajaNavigation.Descripcion,
                Ncaja = p.Ncaja.Value,
                Ncentrocosto = p.Ncentrocosto,
                centrocostoDescripcion = "IPSA Central"
            }).Where(p => p.Nestado == 1);
            return View(perfiles);
        }

        public IActionResult Create()
        {
            ViewBag.Cajas = db.Caja.ToList();
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(Profile profile)
        {
            if (ModelState.IsValid)
            {
                profile.Nestado = 1;
                profile.Ncentrocosto = 1;
                profile.Password = UrlHelperExtensions.getPasswordHashed("ipsa2017*");
                db.Profile.Add(profile);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.Cajas = db.Caja.ToList();
            return View(profile);
        }

        public IActionResult Edit(string id)
        {
            var profile = db.Profile.Find(id);
            ViewBag.Cajas = db.Caja.ToList();
            return View(profile);
        }
        [HttpPost]
        public async Task<IActionResult> Edit(string id,Profile profileClient)
        {
            if (ModelState.IsValid)
            {
                var profile = db.Profile.Find(profileClient.Username);
                profile.Nombre = profileClient.Nombre;
                profile.Apellido = profileClient.Apellido;
                profile.Correo = profileClient.Correo;
                profile.Ncaja = profileClient.Ncaja;
                //profile.CopyFromExcept(profile, x => new
                //{
                //    x.Username,
                //    x.Nestado,
                //    x.Ncentrocosto
                //});
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.Cajas = db.Caja.ToList();
            return View(profileClient);
        }

        public async Task<IActionResult> Delete(string id)
        {
            var profile = db.Profile.Include(p => p.NcajaNavigation).Where(p => p.Username == id);

            var result = profile.Select(p => new ProfileViewModel
            {
                Username = p.Username,
                Nombre = p.Nombre,
                Apellido = p.Apellido,
                Correo = p.Correo,
                Nestado = p.Nestado,
                cajaDescripcion = p.NcajaNavigation.Descripcion,
                Ncaja = p.Ncaja.Value,
                Ncentrocosto = p.Ncentrocosto,
                centrocostoDescripcion = "IPSA Central"
            }).FirstOrDefault();


            var roles = await db.Profilerole
                .Join(
                    db.Role,
                    profilerole => profilerole.Idrole,
                    role => role.Id,
                    (profilerole, role) => new { profilerole, role }
                )
                .Where(p => p.profilerole.Username == id).Select(p => p.role).ToListAsync();
            ViewBag.roles = roles;

            return View(result);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(ProfileViewModel profileViewModel)
        {
            var profile = await db.Profile.FindAsync(profileViewModel.Username);
            if (profile != null)
            {
                profile.Nestado = 91;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(profileViewModel);
        }        
    }
}
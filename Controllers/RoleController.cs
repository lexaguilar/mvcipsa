using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using mvcIpsa.Models;
using mvcIpsa.DbModel;
using Microsoft.AspNetCore.Authorization;
namespace mvcIpsa.Controllers
{
    [Authorize(Policy = "Admin")]
    public class RoleController : Controller
    {
        
        private readonly IPSAContext db;
        public RoleController(IPSAContext _db)
        {
            db = _db;
        }
        public IActionResult Index()
        {
            var role = db.Role.Where(r=>r.Nestado==1);
            ViewBag.roles = role;
            return View();
        }

        //public IActionResult Create()
        //{            
        //    return View();
        //}
        //[HttpPost]
        //public async Task<IActionResult> Create(Role role)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        role.Nestado = 1;
        //        db.Role.Add(role);
        //        await db.SaveChangesAsync();
        //        return RedirectToAction("Index");
        //    }
           
        //    return View(role);
        //}

        public IActionResult Edit(int id)
        {
            var role = db.Role.Find(id);           
            return View(role);
        }
        [HttpPost]
        public async Task<IActionResult> Edit(int id, Role roleClient)
        {
            if (ModelState.IsValid)
            {
                var role = db.Role.Find(id);

                role.Name = roleClient.Name;
                role.Description = roleClient.Description;

                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.Cajas = db.Caja.ToList();
            return View(roleClient);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var role = db.Role.Find(id);
            return View(role);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id,Role roleClient)
        {
            
                var role = db.Role.Find(id);

                role.Nestado = 91;          

                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
namespace mvcIpsa.Controllers
{
    using mvcIpsa.DbModel;
    using mvcIpsa.Models;
    public class CajaController : Controller
    {
        private readonly IPSAContext db;
        public CajaController(IPSAContext _db)
        {
            db = _db;
        }
        public IActionResult Index()
        {
            var cajaList = db.IngresosEgresosCaja
                .Include(c => c.IdtipoingresoNavigation)
                .Include(c => c.TipomovNavigation)
                .Include(c => c.IdtipopagoNavigation)
                .ToList()
                .Select(c => new CajaViewModel
                {
                    Id = c.Id,
                    Tipomov = c.TipomovNavigation.Descripcion,
                    Numrecibo=c.Numrecibo,
                    Nestado=c.Nestado,
                    FechaProceso=c.FechaProceso,
                    tipopago = c.IdtipopagoNavigation.Descripcion,
                    Montoefectivo =c.Montoefectivo,
                    Montocheque=c.Montocheque,
                    Montominuta=c.Montominuta,
                    Montotransferencia = c.Montotransferencia,
                    Monto=c.Monto,
                    Noreferencia=c.Noreferencia,
                    Cuentabanco =c.Cuentabanco,
                    Concepto=c.Concepto,
                    Noordenpago=c.Noordenpago,
                    tipoingreso = c.IdtipoingresoNavigation.Descripcion,
                    tipomoneda =c.IdtipomonedaNavigation.Descripcion,
                    Username = c.Username,
                    Fechacreacion =c.Fechacreacion,
                    centrocosto= "IPSA Central",
                    Identificacioncliente =c.Identificacioncliente,
                    Cuentacontablebanco=c.Cuentacontablebanco,
                    Tipocambio=c.Tipocambio
                });

            return View(cajaList);
        }

        public IActionResult Create()
        {
            ViewData["Idtipoingreso"] = new SelectList(db.Tipoingreso, "Idtipoingreso", "Descripcion");
            ViewData["Idtipomoneda"] = new SelectList(db.Tipomoneda, "Idtipomoneda", "Descripcion");
            ViewData["Idtipopago"] = new SelectList(db.Tipopago, "Idtipopago", "Descripcion");
            ViewData["Nestado"] = new SelectList(db.CajaEstado, "Nestado", "Descripcion",1);
            ViewData["Tipomov"] = new SelectList(db.TipoMovimiento, "Idtipomovimiento", "Descripcion");

            ViewBag.servicios = from mc in db.MaestroContable join mcp in db.MaestroContable
                                on mc.CtaPadre equals mcp.CtaContable
                                where mc.TipoCta==4 || mc.Cuenta.StartsWith("1101") || mc.Cuenta.StartsWith("1108") || mc.Cuenta.StartsWith("1105")
                                select new {
                                    mc.Cuenta,
                                    mc.Nombre,
                                    padre=mcp.Nombre,
                                    mc.TipoCta
                                }
                                ;
                
                
                
                
            //    db.MaestroContable
            //    .Where(mc=> mc.TipoCta==4 || mc.Cuenta.StartsWith("1101") || mc.Cuenta.StartsWith("1108") || mc.Cuenta.StartsWith("1105"))
            //    .Select(mc => new {
            //    cuenta = mc.Cuenta,
            //    nombre = mc.Nombre
            //}).ToList();

            ViewBag.fondos = db.Fondos.Select(mc => new {
                fondoid = mc.Fondoid,
                nombre = mc.Nombre
            }).ToList();

            ViewBag.clientes = db.Cliente.Include(c=>c.IdtipoclienteNavigation).Select(c=> new { nombre= c.Nombre +" "+c.Apellido, identificacion = c.Identificacion , tipoCliente = c.IdtipoclienteNavigation.Tipocliente}).ToList();
            
            return View();
        }
    }
}
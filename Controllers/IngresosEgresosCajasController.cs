using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
namespace mvcIpsa.Controllers
{
    using Microsoft.AspNetCore.Authorization;
    using mvcIpsa.DbModel;
    using mvcIpsa.Extensions;
    using mvcIpsa.Models;
    using System;
    using System.Threading.Tasks;

    [Authorize(Policy = "Admin,User")]
    public class IngresosEgresosCajasController : Controller
    {
        private readonly IPSAContext db;
        public IngresosEgresosCajasController(IPSAContext _db)
        {
            db = _db;
        }

        public IActionResult Index(CajaParameterModel c)
        {
            ViewData["Nestado"] = new SelectList(db.CajaEstado, "Nestado", "Descripcion", 1);
            return View();
        }

        public IActionResult GetList(CajaParameterModel p)
        {
            var user = this.GetServiceUser();
            PaginationResult<IEnumerable<CajaViewModel>> result = new PaginationResult<IEnumerable<CajaViewModel>>();
            var query = db.IngresosEgresosCaja
                .Include(c => c.IdtipoingresoNavigation)
                .Include(c => c.TipomovNavigation)
                .Include(c => c.IdtipopagoNavigation)
                .Include(c => c.IdtipomonedaNavigation)
                .Where(c => c.IdCaja == user.idcaja)
                .ToList()
                .Select(c => new CajaViewModel
                {
                    Id = c.Idrecibo,
                    Tipomov = c.TipomovNavigation.Descripcion,
                    Numrecibo = c.Numrecibo,
                    Nestado = c.Nestado,
                    FechaProceso = c.FechaProceso,
                    tipopago = c.IdtipopagoNavigation.Descripcion,
                    Montoefectivo = c.Montoefectivo,
                    Montocheque = c.Montocheque,
                    Montominuta = c.Montominuta,
                    Montotransferencia = c.Montotransferencia,
                    Monto = c.Monto,
                    Noreferencia = c.Noreferencia ?? "",
                    Cuentabanco = c.Cuentabanco ?? "",
                    Concepto = c.Concepto,
                    Noordenpago = c.Noordenpago,
                    tipoingreso = c.IdtipoingresoNavigation.Descripcion,
                    tipomoneda = c.IdtipomonedaNavigation.Descripcion,
                    Username = c.Username,
                    Fechacreacion = c.Fecharegistro,
                    centrocosto = "IPSA Central",
                    Identificacioncliente = c.Identificacioncliente,
                    Cuentacontablebanco = c.Cuentacontablebanco ?? "",
                    Tipocambio = c.Tipocambio,
                });

            query = query.Where(x => x.FechaProceso >= p.Desde && x.FechaProceso <= p.Hasta);
            if (p.estado.HasValue)
                query = query.Where(x => x.Nestado == p.estado);

            result.Count = query.Count();
            result.Result = query.OrderByDescending(q => q.FechaProceso)
                .Skip((p.Page - 1) * p.Rows)
                .Take(p.Rows).ToArray();

            return Json(result);
        }

        public IActionResult Create()
        {
            var user = this.GetServiceUser();

            ViewData["Idtipoingreso"] = new SelectList(db.TipoIngreso, "Idtipoingreso", "Descripcion");
            ViewData["Idtipomoneda"] = new SelectList(db.TipoMoneda, "Idtipomoneda", "Descripcion");
            ViewData["Idtipopago"] = new SelectList(db.TipoPago, "Idtipopago", "Descripcion");
            ViewData["Nestado"] = new SelectList(db.CajaEstado, "Nestado", "Descripcion", 1);
            ViewData["Banco"] = new SelectList(db.Bancos, "Bancoid", "Descripcion");

            var lote = db.LoteRecibos.Where(lt => lt.IdCaja == user.idcaja).FirstOrDefault();
            if (lote==null)
            {
                return NotFound(new string[] { "No se encontro un lote de recibos para la caja de " + user.description });
            }

            if (lote.Actual<lote.Inicio)
            {
                return NotFound(new string[] { "No se encontro un lote de recibos para la caja de " + user.description });
            }

            if (lote.Actual > lote.Final)
            {
                return NotFound(new string[] { "No se encontro un lote de recibos para la caja de " + user.description });
            }

            ViewBag.NumRecibo = (lote.Actual + 1).ToString().PadLeft(10, '0');

             var servicios = from mc in db.MaestroContable
                                join mcp in db.MaestroContable on mc.CtaPadre equals mcp.CtaContable
                                join ccc in db.CajaCuentaContable on mc.CtaContable equals ccc.CtaCuenta
                                where ccc.IdCaja == user.idcaja
                                //where mc.TipoCta==4 || mc.Cuenta.StartsWith("1101") || mc.Cuenta.StartsWith("1108") || mc.Cuenta.StartsWith("1105")
                                select new
                                {
                                    mc.Cuenta,
                                    mc.Nombre,
                                    padre = mcp.Nombre,
                                    mc.TipoCta
                                };

            if (servicios == null)
            {
                return NotFound(new string[] { "La caja "+ user.description +" no contiene ninguna cuenta contable asociada"});
            }

            ViewBag.servicios = servicios;

            var cambioOficial = db.CambioOficial.Find(DateTime.Today);
            decimal TasaDelDia = 0;
            if (cambioOficial != null)
            {
                TasaDelDia = cambioOficial.Dolares;
            }
            ViewBag.TasaDelDia = TasaDelDia.ToString().Replace(',','.');



            ViewBag.fondos = db.Fondos.Select(mc => new
            {
                fondoid = mc.Fondoid,
                nombre = mc.Nombre
            }).ToList();

            ViewBag.clientes = db.Cliente.Include(c => c.IdtipoclienteNavigation).Select(c => new { nombre = c.Nombre + " " + c.Apellido, identificacion = c.Identificacion, tipoCliente = c.IdtipoclienteNavigation.Tipocliente }).ToList();

            return View();
        }

        [HttpPost]
        [Produces("application/json")]
        public async Task<IActionResult> Create(IngresoEgresosCajaViewModel iECajaViewModel)
        {
            var user = this.GetServiceUser();

            var CambioOficial = db.CambioOficial.Find(iECajaViewModel.master.FechaProceso);
            if (CambioOficial == null)
                return NotFound(new string[] { string.Format("No se encontro la tasa de cambio para la fecha {0}", iECajaViewModel.master.FechaProceso) });

            var ingresosEgresosCaja = iECajaViewModel.master;

            //Numero de recibo actual
            var lote = db.LoteRecibos.Where(lt => lt.IdCaja == user.idcaja).FirstOrDefault();
            ingresosEgresosCaja.Numrecibo = (lote.Actual + 1).ToString().PadLeft(10, '0');
            lote.Actual = lote.Actual + 1;

            ingresosEgresosCaja.Tipomov = 32;
            ingresosEgresosCaja.Nestado = 1;
            ingresosEgresosCaja.Fecharegistro = DateTime.Now;
            ingresosEgresosCaja.IdCaja = user.idcaja;
            ingresosEgresosCaja.Tipocambio = CambioOficial.Dolares;
            ingresosEgresosCaja.Username = user.username;

            ingresosEgresosCaja.Monto = (ingresosEgresosCaja.Montoefectivo + ingresosEgresosCaja.Montocheque + ingresosEgresosCaja.Montominuta + ingresosEgresosCaja.Montotransferencia);

            foreach (var item in iECajaViewModel.details)
            {
                var _montoDolar = item.precio * item.cantidad;
                ingresosEgresosCaja.IngresosEgresosCajaDetalle.Add(new IngresosEgresosCajaDetalle
                {
                    Cantidad = item.cantidad,
                    CtaContable = $"1000{item.cta_cuenta}",
                    Precio = item.precio,
                    Montodolar = _montoDolar,
                    Montocordoba = _montoDolar * CambioOficial.Dolares,
                    Idrecibo = ingresosEgresosCaja.Idrecibo
                });
            }

            db.Add(ingresosEgresosCaja);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                return BadRequest();
            }
            return Ok(ingresosEgresosCaja.IngresosEgresosCajaDetalle.Count());
        }

        [ActionName("FindTipoCambio")]
        public async Task<IActionResult> FindTipoCambio(DateTime fecha)
        {
            var cambioOficial = await db.CambioOficial.FindAsync(fecha);
            if (cambioOficial == null)
            {
                return NotFound();
            }
            return Json(cambioOficial);
        }

        public IActionResult Edit(int id)
        {
            var user = this.GetServiceUser();

            var recibo = db.IngresosEgresosCaja
                .Include(iec => iec.IngresosEgresosCajaDetalle)
                .Where(iec => iec.Idrecibo == id).FirstOrDefault();

            ViewData["Idtipoingreso"] = new SelectList(db.TipoIngreso, "Idtipoingreso", "Descripcion", recibo.Idtipoingreso);
            ViewData["Idtipomoneda"] = new SelectList(db.TipoMoneda, "Idtipomoneda", "Descripcion", recibo.Idtipomoneda);
            ViewData["Idtipopago"] = new SelectList(db.TipoPago, "Idtipopago", "Descripcion", recibo.Idtipopago);
            ViewData["Nestado"] = new SelectList(db.CajaEstado, "Nestado", "Descripcion", 1);
         
            ViewBag.clientes = db.Cliente.Include(c => c.IdtipoclienteNavigation).Select(c => new { nombre = c.Nombre + " " + c.Apellido, identificacion = c.Identificacion, tipoCliente = c.IdtipoclienteNavigation.Tipocliente }).ToList();

            ViewBag.servicios = from mc in db.MaestroContable
                                join mcp in db.MaestroContable on mc.CtaPadre equals mcp.CtaContable
                                join ccc in db.CajaCuentaContable on mc.CtaContable equals ccc.CtaCuenta
                                where ccc.IdCaja == user.idcaja
                                //where mc.TipoCta==4 || mc.Cuenta.StartsWith("1101") || mc.Cuenta.StartsWith("1108") || mc.Cuenta.StartsWith("1105")
                                select new
                                {
                                    mc.Cuenta,
                                    mc.Nombre,
                                    padre = mcp.Nombre,
                                    mc.TipoCta
                                };

            return View(recibo);
        }
    }
}
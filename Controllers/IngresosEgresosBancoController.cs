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
    using mvcIpsa.Services;
    using System;
    using System.Threading.Tasks; 
    using mvcIpsa.DbModelIPSA;

    using Newtonsoft.Json;
    using Microsoft.Extensions.Options;
    using System.IO;
    /// <summary>
    /// Controlador de movimientos
    /// </summary>
    [Authorize(Policy = "Admin,User")]    
    public class IngresosEgresosBancoController : Controller
    {
        private readonly IPSAContext db;
        private readonly DBIPSAContext DbIpsa;
        private readonly AppSettings settings;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="_db"></param>
        /// <param name="_DbIpsa"></param>
        public IngresosEgresosBancoController(IPSAContext _db, DBIPSAContext _DbIpsa, IOptions<AppSettings> _settings)
        {
            db = _db;
            DbIpsa = _DbIpsa;
            settings = _settings.Value;
        }       

        ///       
        public IActionResult Index()
        {
            var usr = this.GetServiceUser();
            

            var cajas = db.Caja.Select(p => new Caja { Id = p.Id, Description =p.Description });
            if (!usr.roles.Contains((int)Roles.Administrador))
                cajas = cajas.Where(p => p.Id ==  usr.cajaid);

            ViewBag.EstadoId = new SelectList(db.IngresosEgresosBancoEstado, "Id", "Descripcion", 1);
            ViewBag.Caja = new SelectList(cajas, "Id", "Description");           
            ViewBag.TipoDoc = new SelectList(db.TipoDocumento, "Id", "Descripcion");   
            ViewBag.Title = "Movimientos";
            return View("Index");   
        }

        public IActionResult GetList(CajaParameterModel p)
        {
            var user = this.GetServiceUser();

            var CtaContable = DbIpsa.BancosCuentas.Select(bc => new
            {
                bc.BancoCuenta,
                bc.Descripcion
            }).ToArray();

            PaginationResult<IEnumerable<BancoViewModel>> result = new PaginationResult<IEnumerable<BancoViewModel>>();
            var query = db.IngresosEgresosBanco
                .Include(c => c.Estado)
                .Include(c => c.TipoMovimiento)
                .Include(c => c.TipoMoneda)
                .Include(c => c.Caja)
                .Select(c => new BancoViewModel
                {
                    Id = c.Id,
                    FechaRegistro = c.FechaRegistro,
                    Monto = c.Monto,
                    TipoMonedaId = c.TipoMonedaId,
                    TipoMoneda = c.TipoMoneda.Descripcion,
                    CuentaContableBanco = CtaContable.Where(cc => cc.BancoCuenta == c.BancoCuenta).FirstOrDefault().Descripcion,
                    TipoMovimientoId = c.TipoMovimientoId,
                    TipoMovimiento = c.TipoMovimiento.Descripcion,
                    Concepto = c.Concepto,
                    FechaProceso = c.FechaProceso,
                    Username = c.Username,
                    CajaId = c.CajaId,
                    Caja = c.Caja.Description,
                    EstadoId = c.EstadoId,
                    Estado = c.Estado.Descripcion,
                    TipoCambio = c.TipoCambio,
                    MotivoAnulado = c.MotivoAnulado,
                    TipoDocumentoId = c.TipoDocumentoId,
                    TipoDocumento = c.TipoDocumento.Descripcion,
                    Referencia = c.Referencia
                    
                });

            var strim = query.ToString();

            if (p.searchByNum)
            {
                query = query.Where(x => x.Referencia == p.Referencia);
                if (!user.roles.Contains((int)Roles.Administrador))
                    query = query.Where(x => x.CajaId == user.cajaid);

                result.Count = query.Count();               
                result.Result = query.ToArray();
            }
            else
            {
                query = query.Where(x => x.FechaProceso >= p.Desde && x.FechaProceso <= p.Hasta);

                if (!user.roles.Contains((int)Roles.Administrador))
                    query = query.Where(x => x.CajaId == user.cajaid);
                else
                {
                    if (p.caja.HasValue)
                    {
                        query = query.Where(x => x.CajaId == p.caja.Value);
                    }
                }

                if (p.estado.HasValue)
                    query = query.Where(x => x.EstadoId == p.estado);

                if (p.tipoDoc.HasValue)
                    query = query.Where(x => x.TipoDocumentoId == p.tipoDoc);

                //result.Count = query.Count();
                result.Result = query.OrderByDescending(q => q.FechaProceso).ToArray(); //query??[]//.OrderByDescending(q => q.FechaProceso)
                    //.Skip((p.Page - 1) * p.Rows)
                    //.Take(p.Rows)
                    
            }

            return Json(result);
        }
        /// <summary>
        /// Vista para crear un nuevo movimiento
        /// </summary>
        /// <returns></returns>
        public IActionResult Create()
        {
            return View();
        }
        /// <summary>
        /// Insertar un nuevo movimiento
        /// </summary>
        /// <param name="ingresosEgresosBanco"></param>
        /// <returns></returns>
        [HttpPost("api/banco/guardar")]
        public async Task<IActionResult> Post(IngresosEgresosBanco ingresosEgresosBanco)
        {
            var user = this.GetServiceUser();
          
            if (settings.onlyNumber)
                if (ingresosEgresosBanco.Referencia.Any(r => !char.IsNumber(r)))
                    return BadRequest(string.Format($"La referencia {ingresosEgresosBanco.Referencia} debe ser númerica"));

            ingresosEgresosBanco.CajaId = user.cajaid;
            ingresosEgresosBanco.EstadoId = (int)IngresosEgresosBancoEstados.Registrado;

            var cambioOficial = await db.CambioOficial.FindAsync(ingresosEgresosBanco.FechaProceso);
            if (cambioOficial == null)
            {
                return BadRequest("No se encontró la tasa de cambios para la fecha " + ingresosEgresosBanco.FechaProceso.ToShortDateString());
            }
          
            ingresosEgresosBanco.TipoCambio = cambioOficial.Dolares;
            
            var bancoCuenta = DbIpsa.BancosCuentas.Find(ingresosEgresosBanco.BancoCuenta, "1000");
            if (bancoCuenta == null)
            {
                return BadRequest("No se encontró información del banco " + bancoCuenta.Descripcion);
            }

            ingresosEgresosBanco.TipoMonedaId = bancoCuenta.Moneda.Value;
            ingresosEgresosBanco.Username = user.username;
            ingresosEgresosBanco.FechaRegistro = DateTime.Now;
            ingresosEgresosBanco.Procesado = false;

            if (ModelState.IsValid)
            {
                db.Add(ingresosEgresosBanco);
                await db.SaveChangesAsync();                
            }
       
            return Json(ingresosEgresosBanco);
        }
        /// <summary>
        /// Editar un movimiento
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        // GET: IngresosEgresosBancoes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {     
            return View(id);
        }

        [HttpGet("api/banco/cargar/{id}")]
        public async Task<IActionResult> GetData(int id)
        {     
            var movimiento = db.IngresosEgresosBanco.Find(id);
            return Json(movimiento, new Newtonsoft.Json.JsonSerializerSettings { MaxDepth = 1, ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore });
        }

        // POST: IngresosEgresosBancoes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPut("api/banco/guardar")]     
        public async Task<IActionResult> Put(IngresosEgresosBanco ingresosEgresosBanco)
        {
            var user = this.GetServiceUser();
            var oldMovimiento = db.IngresosEgresosBanco.Find(ingresosEgresosBanco.Id);

            if (oldMovimiento.EstadoId == (int)IngresosEgresosBancoEstados.Anulado)
            {
                return BadRequest($"No se puede editar el movimiento {ingresosEgresosBanco.Id} por que esta anulado");
            }
            var cambioOficial = await db.CambioOficial.FindAsync(ingresosEgresosBanco.FechaProceso);
            if (cambioOficial == null)
            {
                return BadRequest("No se encontró la tasa de cambios para la fecha " + ingresosEgresosBanco.FechaProceso.ToShortDateString());
            }
            oldMovimiento.TipoCambio = cambioOficial.Dolares;
            
            var bancoCuenta = DbIpsa.BancosCuentas.Find(ingresosEgresosBanco.BancoCuenta, "1000");
            if (bancoCuenta == null)
            {
                return BadRequest("No se encontró información del banco " + bancoCuenta.Descripcion);
            }

            if (oldMovimiento.Procesado==true)
            {
                return BadRequest($"No se puede editar el movimiento {ingresosEgresosBanco.Id} por que esta conciliado");
            }

            oldMovimiento.TipoMonedaId = bancoCuenta.Moneda.Value;
            oldMovimiento.TipoDocumentoId = ingresosEgresosBanco.TipoDocumentoId;
            oldMovimiento.TipoMovimientoId = ingresosEgresosBanco.TipoMovimientoId;
            oldMovimiento.BancoCuenta = ingresosEgresosBanco.BancoCuenta;
            oldMovimiento.Referencia = ingresosEgresosBanco.Referencia;
            oldMovimiento.Monto = ingresosEgresosBanco.Monto;
            oldMovimiento.Concepto = ingresosEgresosBanco.Concepto;
            oldMovimiento.FechaProceso = ingresosEgresosBanco.FechaProceso;

            oldMovimiento.UsernameEditado = user.username;
            oldMovimiento.FechaEditado = DateTime.Now;

            db.SaveChanges();

            return Json(ingresosEgresosBanco);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id">Codigo del movimiento</param>
        /// <param name="motivo">Motivo para anular</param>
        /// <returns></returns>        
        [HttpPost("api/banco/anular/{id}")]

        public async Task<IActionResult> CancelarMovimiento(int id, string motivo)
        {
            var user = this.GetServiceUser();
            var movimiento = db.IngresosEgresosBanco.Find(id);
            if (movimiento.EstadoId == (int)IngresosEgresosBancoEstados.Anulado)
            {
                return BadRequest($"No se puede anular el movimiento {movimiento.Id} por que ya estaba anulado");
            }

            if (movimiento.Procesado)
            {
                return BadRequest($"No se puede anular el movimiento {movimiento.Id} por que esta conciliado");
            }
            movimiento.EstadoId = (int)IngresosEgresosCajaEstado.Anulado;
            movimiento.MotivoAnulado = motivo;
            movimiento.UsernameAnulado = user.username;
            movimiento.FechaAnulado = DateTime.Now;
            await db.SaveChangesAsync();
            return Ok();
        }

        private bool IngresosEgresosBancoExists(int id)
        {
            return db.IngresosEgresosBanco.Any(e => e.Id == id);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ingresosEgresosBanco = await db.IngresosEgresosBanco
                .Include(i => i.Caja)
                .Include(i => i.Estado)
                .Include(i => i.TipoDocumento)
                .Include(i => i.TipoMoneda)
                .Include(i => i.TipoMovimiento)
                .Include(i => i.UsernameNavigation)
                .SingleOrDefaultAsync(m => m.Id == id);
            if (ingresosEgresosBanco == null)
            {
                return NotFound();
            }

            return View(ingresosEgresosBanco);
        }
    }
}
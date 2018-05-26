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
    using Extensions;
    using mvcIpsa.DbModelIPSA;
    using Newtonsoft.Json;
    using Microsoft.Extensions.Options;
    using System.IO;

    [Authorize(Policy = "Admin,User")]
    public class IngresosEgresosBancoController : Controller
    {
        private readonly IPSAContext db;     

        public IngresosEgresosBancoController(IPSAContext _db)
        {
            db = _db;         
        }

        public IActionResult Index()
        {
            var usr = this.GetServiceUser();
            ViewData["EstadoId"] = new SelectList(db, "Id", "Descripcion", 1);

            var cajas = db.Caja.Select(p => new Caja { Id = p.Id, Description =p.Description });
            if (!usr.roles.Contains((int)Roles.Administrador))
                cajas = cajas.Where(p => p.Id ==  usr.cajaid);


            ViewData["Caja"] = new SelectList(cajas, "Id", "Description");
            return View();
        }

        public IActionResult GetList(CajaParameterModel p)
        {
            var user = this.GetServiceUser();
            PaginationResult<IEnumerable<CajaViewModel>> result = new PaginationResult<IEnumerable<CajaViewModel>>();
            var query = db.IngresosEgresosCaja
                .Include(c => c.TipoIngreso)
                .Include(c => c.TipoMovimiento)                
                .Include(c => c.TipoMoneda)
                .Include(c => c.Estado)                
                .Select(c => new CajaViewModel
                {
                    Id = c.Id,
                    TipoMovimiento = c.TipoMovimiento.Descripcion,                    
                    NumRecibo = c.NumRecibo,
                    EstadoId = c.EstadoId,
                    Estado = c.Estado.Descripcion,
                    FechaProceso = c.FechaProceso,     
                    Total = c.Total,
                    Beneficiario  = c.Beneficiario,
                    Concepto = c.Concepto,
                    NoOrdenPago =c.NoOrdenPago,
                    TipoIngreso = c.TipoIngreso.Descripcion,
                    TipoMoneda = c.TipoMoneda.Descripcion,
                    Username = c.Username,
                    FechaCreacion = c.FechaRegistro,
                    CentroCosto = "IPSA Central",
                    CajaId = c.CajaId
                });

            if (p.searchByNum)
            {
                query = query.Where(x => x.NumRecibo == p.numRecibo.PadLeft(10,'0'));
                if (!user.roles.Contains((int)Roles.Administrador))
                    query = query.Where(x => x.CajaId == user.cajaid);

                result.Count = query.Count();
                var strim  = query.ToString();
                result.Result = query.ToArray();
            }
            else
            {
                query = query.Where(x => x.FechaProceso >= p.Desde && x.FechaProceso <= p.Hasta );

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

                result.Count = query.Count();
                result.Result = query.OrderByDescending(q => q.FechaProceso)
                    .Skip((p.Page - 1) * p.Rows)
                    .Take(p.Rows).ToArray();
            }          

            return Json(result);
        }

        public IActionResult Create()
        {
            var user = this.GetServiceUser();

            ViewData["TipoIngresoId"] = new SelectList(db.TipoIngreso, "Id", "Descripcion");
            ViewData["TipoMonedaId"] = new SelectList(db.TipoMoneda, "Id", "Descripcion");          
            ViewData["EstadoId"] = new SelectList(db.CajaEstado, "Id", "Descripcion", 1);
            ViewData["Banco"] = new SelectList(db.Bancos, "Bancoid", "Descripcion");
            ViewData["TipoCliente"] = new SelectList(db.TipoCliente, "Id", "Tipocliente", 1);

            var lote = db.LoteRecibos.Where(lt => lt.CajaId == user.cajaid).FirstOrDefault();
            if (lote == null)
            {
                return NotFound(new string[] { "No se encontró un lote de recibos para la caja de " + user.description });
            }

            if (lote.Actual < lote.Inicio)
            {
                return NotFound($"El lote actual {lote.Actual} esta fuera del rango de lotes asignados ({lote.Inicio} a {lote.Final})");
            }

            if (lote.Actual >= lote.Final)
            {
                return NotFound($"Ya llego al limite de lotes asignados Max({lote.Final}) para la caja de {user.description}");
            }

            ViewBag.NumRecibo = (lote.Actual + 1).ToString().PadLeft(10, '0');


            var cuentas = DbIpsa.MaestroContable
                .Where(mc => mc.TipoCta == 4 || mc.Cuenta.StartsWith("1101") || mc.Cuenta.StartsWith("1108") || mc.Cuenta.StartsWith("1105"))
                .ToArray();
         
            var servicios = from mc in cuentas
                            join mcp in cuentas on mc.CtaPadre equals mcp.CtaContable
                            join ccc in db.CajaCuentaContable on mc.CtaContable equals ccc.CtaCuenta
                            where ccc.CajaId == user.cajaid 
                            select new
                            {
                                mc.Cuenta,
                                mc.Nombre,
                                padre = mcp.Nombre,
                                mc.TipoCta
                            };

            if (servicios == null)
            {
                return NotFound(new string[] { "La caja " + user.description + " no contiene ninguna cuenta contable asociada" });
            }

            ViewBag.servicios = servicios;
                      
            ViewBag.fondos = db.Fondos.Select(mc => new
            {
                fondoid = mc.Fondoid,
                nombre = mc.Nombre
            }).ToList();

            ViewBag.clientes = db.Cliente
                .Include(c => c.TipoCliente)
                .Select(c => new { c.Id, nombre = c.Nombre + " " + c.Apellido, identificacion = c.Identificacion,idTipo = c.TipoCliente.Id, tipoCliente = c.TipoCliente.Tipocliente })
                .ToList();

            return View();
        }

        [HttpPost]
        [Produces("application/json")]
        public async Task<IActionResult> Create(IngresoEgresosCajaViewModel iECajaViewModel)
        {
            createLogs(iECajaViewModel,"Create");

            var user = this.GetServiceUser();
            if (user.description.Contains("2017") && iECajaViewModel.master.FechaProceso.Year != 2017)
            {
                return BadRequest("El usuario solo puede registrar información para el año 2017");
            }
            var ingresosEgresosCaja = iECajaViewModel.master;

            //Numero de recibo actual
            var lote = db.LoteRecibos.Where(lt => lt.CajaId == user.cajaid).FirstOrDefault();
            ingresosEgresosCaja.NumRecibo = (lote.Actual + 1).ToString().PadLeft(10, '0');
            lote.Actual = lote.Actual + 1;

            ingresosEgresosCaja.TipoMovimientoId = 32;
            ingresosEgresosCaja.EstadoId = 1;
            ingresosEgresosCaja.FechaRegistro = DateTime.Now;
            ingresosEgresosCaja.CajaId = user.cajaid;          
            ingresosEgresosCaja.Username = user.username;


            var totalServicioDolar = iECajaViewModel.details.Sum(s => s.montodolar);
            var totalPagoDolar = iECajaViewModel.referencias.Sum(p => p.totalD);

            if (Math.Round(totalServicioDolar,2) != Math.Round(totalPagoDolar, 2))
            {
                return BadRequest(string.Format($"El total cobrado por los servicios ({Math.Round(totalServicioDolar, 2)}) no conicide con el total pagado {Math.Round(totalPagoDolar, 2)}"));
            }

            if (Math.Round(totalServicioDolar,2) != Math.Round(totalPagoDolar, 2))
            {
                return BadRequest(string.Format($"El total cobrado por los servicios ({Math.Round(totalServicioDolar, 2)}) no conicide con el total pagado {Math.Round(totalPagoDolar, 2)}"));
            }

            var totalPagoCordoba = iECajaViewModel.referencias.Sum(p => p.totalC);

            if (iECajaViewModel.master.TipoMonedaId == (short)TipoMonedaParamFilter.Cordoba)
                ingresosEgresosCaja.Total = totalPagoCordoba;
            if (iECajaViewModel.master.TipoMonedaId == (short)TipoMonedaParamFilter.Dolar)
                ingresosEgresosCaja.Total = totalPagoDolar;

            foreach (var item in iECajaViewModel.details)
            {
                if (item.precio <= 0 || item.cantidad <= 0)
                {
                    return BadRequest(string.Format($"El monto o la cantidad para el servicio de la cuenta 1000{item.cta_cuenta}, no puede ser 0"));
                }
                var _montoDolar = item.precio * item.cantidad;
                ingresosEgresosCaja.IngresosEgresosCajaDetalle.Add(new IngresosEgresosCajaDetalle
                {
                    Cantidad = item.cantidad,
                    CtaContable = $"1000{item.cta_cuenta}",
                    Precio = item.precio,
                    Montodolar = _montoDolar,                  
                    ReciboId = ingresosEgresosCaja.Id
                });
            }


            foreach (var referencia in iECajaViewModel.referencias)
            {
                if (referencia.TipoPagoId == (short)TipoPagoParamFilter.None)                                
                        return BadRequest(string.Format("Debe seleccionar un tipo de pago válido"));  

                if (referencia.TipoPagoId == (short)TipoPagoParamFilter.Minuta || referencia.TipoPagoId == (short)TipoPagoParamFilter.Transferencia || referencia.TipoPagoId == (short)TipoPagoParamFilter.Cheque)
                {
                    if (referencia.Referencia == null || referencia.Referencia.Trim().Length == 0)
                        return BadRequest(string.Format("Debe de ingresar la referencia para la forma de pago cheque, minuta o transferencia"));

                    if (referencia.IdBanco==null || referencia.IdBanco == 0)
                        return BadRequest(string.Format("Debe de ingresar el banco para la forma de pago cheque, minuta o transferencia"));
                }
                    


                var _CambioOficial = db.CambioOficial.Find(referencia.Fecha);
                if (_CambioOficial == null)
                    return NotFound(string.Format("No se encontró la tasa de cambio para la fecha {0} de la referencia {1}", referencia.Fecha, referencia.Referencia));

                ingresosEgresosCaja.IngresosEgresosCajaReferencias.Add(new IngresosEgresosCajaReferencias
                {
                    ReciboId = ingresosEgresosCaja.Id,
                    MontoEfectivo = referencia.MontoEfectivo,
                    MontoMinu = referencia.MontoMinu,
                    MontoCheq = referencia.MontoCheq,
                    MontoTrans = referencia.MontoTrans,
                    Total = (referencia.MontoEfectivo + referencia.MontoMinu + referencia.MontoCheq + referencia.MontoTrans),                    
                    Fecha = referencia.Fecha,
                    TipoCambio = _CambioOficial.Dolares,
                    Referencia = referencia.Referencia,
                    IdBanco = referencia.IdBanco,
                    TipoPagoId = referencia.TipoPagoId
                });
            }
           
            ingresosEgresosCaja.Referencias = (short)ingresosEgresosCaja.IngresosEgresosCajaReferencias.Count();
        
            db.Add(ingresosEgresosCaja);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                return BadRequest();
            }
            return Ok(new {
                ingresosEgresosCaja.NumRecibo,
                ingresosEgresosCaja.Id
            });
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

        [HttpPost]
        [ActionName("CancelRecibo")]
        public async Task<IActionResult> CancelRecibo(int idrecibo,string motivo)
        {
            var recibo = db.IngresosEgresosCaja.Find(idrecibo);
            if (recibo.EstadoId == (int)IngresosEgresosCajaEstado.Anulado)
            {
                return BadRequest($"No se puede anular el recibo {recibo.NumRecibo} por que ya estaba anulado" );
            }
            recibo.EstadoId = (int)IngresosEgresosCajaEstado.Anulado;
            recibo.MotivoAnulado = motivo;
            await db.SaveChangesAsync();
            return Ok();
        }

        public IActionResult Edit(int id)
        {
            var user = this.GetServiceUser();

            var serviciosDetalle = db.IngresosEgresosCajaDetalle.Where(d => d.ReciboId == id).ToArray();
            var cuentasContables = db.CajaCuentaContable.Where(c => c.CajaId == user.cajaid).Select(ccc => ccc.CtaCuenta).ToArray();
            var hasAllAccounting = serviciosDetalle.Select(r => r.CtaContable).All(cta => cuentasContables.Contains(cta));

            if(!hasAllAccounting)
            {
                var cccs = serviciosDetalle.Where(s => !cuentasContables.Contains(s.CtaContable)).Select( p => p.CtaContable);
                return View("Error",$"El usuario {user.username} asignado a la caja {user.description} no contiene la(s) cuenta(s) {string.Join(",",cccs)} para poder editar recibo {db.IngresosEgresosCaja.Find(id).NumRecibo}");
            }

            var recibo = db.IngresosEgresosCaja
                .Include(iec => iec.Estado).FirstOrDefault(iec => iec.Id == id);
                

            ViewData["TipoIngresoId"] = new SelectList(db.TipoIngreso, "Id", "Descripcion", recibo.TipoIngresoId);
            ViewData["TipoMonedaId"] = new SelectList(db.TipoMoneda, "Id", "Descripcion", recibo.TipoMonedaId);
            ViewData["TipoCliente"] = new SelectList(db.TipoCliente, "Id", "Tipocliente");

            ViewBag.clientes = db.Cliente
                .Include(c => c.TipoCliente)
                .Select(c => new { c.Id, nombre = c.Nombre + " " + c.Apellido, identificacion = c.Identificacion, idTipo = c.TipoCliente.Id, tipoCliente = c.TipoCliente.Tipocliente })
                .ToList();

            var cuentas = DbIpsa.MaestroContable
                .Where(mc => mc.TipoCta == 4 || mc.Cuenta.StartsWith("1101") || mc.Cuenta.StartsWith("1108") || mc.Cuenta.StartsWith("1105"))
                .ToArray();
    
            ViewBag.servicios = from mc in cuentas
                                join mcp in cuentas on mc.CtaPadre equals mcp.CtaContable
                                join ccc in db.CajaCuentaContable on mc.CtaContable equals ccc.CtaCuenta
                                where ccc.CajaId == user.cajaid                               
                                select new
                                {
                                    mc.Cuenta,
                                    mc.Nombre,
                                    padre = mcp.Nombre,
                                    mc.TipoCta
                                };


            ViewBag.referencias = db.IngresosEgresosCajaReferencias.Where(r => r.ReciboId == id).ToArray();
            ViewBag.detalle = serviciosDetalle;

            return View(recibo);
        }
       
        [HttpPost]
        [Produces("application/json")]
        public async Task<IActionResult> Edit(IngresoEgresosCajaViewModel iECajaViewModel)
        {
           
            createLogs(iECajaViewModel,"Edit");
            var user = this.GetServiceUser();

            if (user.description.Contains("2017") && iECajaViewModel.master.FechaProceso.Year != 2017)            
                return BadRequest("El usuario solo puede registrar información para el año 2017");            

            var recibos = db.IngresosEgresosCaja.Find(iECajaViewModel.master.Id);

            if (recibos.EstadoId == (short)IngresosEgresosCajaEstado.Anulado)
            {
                return BadRequest($"No se puede editar el recibo {recibos.NumRecibo.PadLeft(10, '0')} ya que está anulado");
            }

            if (recibos.EstadoId == (short)IngresosEgresosCajaEstado.Cerrado)
            {
                return BadRequest($"No se puede editar el recibo {recibos.NumRecibo.PadLeft(10, '0')} ya que está cerrado");
            }

            var ingresosEgresosCaja = iECajaViewModel.master;
            recibos.FechaProceso = ingresosEgresosCaja.FechaProceso;
            recibos.NoOrdenPago = ingresosEgresosCaja.NoOrdenPago;
            recibos.TipoMonedaId = ingresosEgresosCaja.TipoMonedaId;
            recibos.TipoIngresoId = ingresosEgresosCaja.TipoIngresoId;
            recibos.ClienteId = ingresosEgresosCaja.ClienteId;
            recibos.TipoCleinteId = ingresosEgresosCaja.TipoCleinteId;
            recibos.Beneficiario = ingresosEgresosCaja.Beneficiario;
            recibos.Muestra = ingresosEgresosCaja.Muestra;
            recibos.Concepto = ingresosEgresosCaja.Concepto;

            var totalServicioDolar = iECajaViewModel.details.Sum(s => s.montodolar);
            var totalPagoDolar = iECajaViewModel.referencias.Sum(p => p.totalD);
            if (Math.Round(totalServicioDolar, 2) != Math.Round(totalPagoDolar, 2))
            {
                return BadRequest(string.Format($"El total cobrado por los servicios ({Math.Round(totalServicioDolar, 2)}) no conicide con el total pagado {Math.Round(totalPagoDolar, 2)}"));
            }

            var oldDetalles = db.IngresosEgresosCajaDetalle.Where(d => d.ReciboId == recibos.Id);
            db.IngresosEgresosCajaDetalle.RemoveRange(oldDetalles);

            foreach (var item in iECajaViewModel.details)
            {
                if (item.precio <= 0 || item.cantidad <= 0)
                {
                    return BadRequest(string.Format($"El monto o la cantidad para el servicio de la cuenta 1000{item.cta_cuenta}, no puede ser 0"));
                }
                var _montoDolar = item.precio * item.cantidad;
                recibos.IngresosEgresosCajaDetalle.Add(new IngresosEgresosCajaDetalle
                {
                    Cantidad = item.cantidad,
                    CtaContable = $"1000{item.cta_cuenta}",
                    Precio = item.precio,
                    Montodolar = _montoDolar,
                    ReciboId = recibos.Id
                });
            }



            var oldReferencias = db.IngresosEgresosCajaReferencias.Where(d => d.ReciboId == recibos.Id);
            db.IngresosEgresosCajaReferencias.RemoveRange(oldReferencias);

            var totalPagoCordoba = iECajaViewModel.referencias.Sum(p => p.totalC);

            if (recibos.TipoMonedaId == (short)TipoMonedaParamFilter.Cordoba)
                recibos.Total = totalPagoCordoba;
            if (recibos.TipoMonedaId == (short)TipoMonedaParamFilter.Dolar)
                recibos.Total = totalPagoDolar;

            foreach (var referencia in iECajaViewModel.referencias)
            {
                if (referencia.TipoPagoId == (short)TipoPagoParamFilter.None)
                    return BadRequest(string.Format("Debe seleccionar un tipo de pago válido"));
                
                if (referencia.TipoPagoId == (short)TipoPagoParamFilter.Minuta || referencia.TipoPagoId == (short)TipoPagoParamFilter.Transferencia || referencia.TipoPagoId == (short)TipoPagoParamFilter.Cheque)
                {
                    if (referencia.Referencia.Trim().Length == 0)
                        return BadRequest(string.Format("Debe de ingresar la referencia para la forma de pago cheque, minuta o transferencia"));

                    if (referencia.IdBanco == null || referencia.IdBanco == 0)
                        return BadRequest(string.Format("Debe de ingresar el banco para la forma de pago cheque, minuta o transferencia"));
                }


                var _CambioOficial = db.CambioOficial.Find(referencia.Fecha);
                if (_CambioOficial == null)
                    return NotFound(string.Format("No se encontró la tasa de cambio para la fecha {0} de la referencia {1}", referencia.Fecha, referencia.Referencia));

                recibos.IngresosEgresosCajaReferencias.Add(new IngresosEgresosCajaReferencias
                {
                    ReciboId = recibos.Id,
                    MontoEfectivo = referencia.MontoEfectivo,
                    MontoMinu = referencia.MontoMinu,
                    MontoCheq = referencia.MontoCheq,
                    MontoTrans = referencia.MontoTrans,
                    Total = (referencia.MontoEfectivo + referencia.MontoMinu + referencia.MontoCheq + referencia.MontoTrans),
                    Fecha = referencia.Fecha,
                    TipoCambio = _CambioOficial.Dolares,
                    Referencia = referencia.Referencia,
                    IdBanco = referencia.IdBanco,
                    TipoPagoId = referencia.TipoPagoId
                });
            }

            recibos.Referencias = (short)iECajaViewModel.referencias.Count();//  recibos.IngresosEgresosCajaReferencias.Count();            

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                return BadRequest();
            }
            return Ok(ingresosEgresosCaja.NumRecibo);
        }

    }
}
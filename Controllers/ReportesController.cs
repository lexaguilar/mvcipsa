using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using mvcIpsa.DbModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using mvcIpsa.Models;
using mvcIpsa.DbModelIPSA;
using mvcIpsa.Extensions;
using Microsoft.AspNetCore.Mvc.Rendering;
using mvcIpsa.Services;
using Microsoft.Extensions.Options;
namespace mvcIpsa.Controllers
{   
    [Authorize(Policy = "Admin,Reportes")]
    public class ReportesController : Controller
    {
        private readonly IPSAContext db;
        private readonly DBIPSAContext DbIpsa;
        public ReportesController(IPSAContext _db, DBIPSAContext _DbIpsa)
        {
            db = _db;
            DbIpsa = _DbIpsa;
        }
        public IActionResult RecibosCaja_Detelle1()
        {
            var usr = this.GetServiceUser();
            var cajas = db.Caja.Select(p => new Caja { Id = p.Id, Description = p.Description });
            if (!usr.roles.Contains((int)Roles.Administrador))
                cajas = cajas.Where(p => p.Id == usr.cajaid);

            ViewData["Caja"] = new SelectList(cajas, "Id", "Description");

            return View("RecibosCaja");
        }

        public IActionResult RecibosCaja_Detelle1_GetList(DateTime desde,DateTime hasta, int? cajaId)
        {
            var usr = this.GetServiceUser();

            var reporte = db.IngresosEgresosCaja
            .Include(i => i.Caja)
            //.Include(i=>i.IngresosEgresosCajaDetalle)
            //.Include(i=>i.IngresosEgresosCajaReferencias).ThenInclude(c => c.TipoPago)      
            .Include(i => i.TipoMoneda)
            .Include(i => i.Estado)
            .Where(i => i.FechaProceso >= desde && i.FechaProceso <= hasta);

            if (cajaId.HasValue)
            {
                if (usr.roles.Contains((int)Roles.Administrador))
                    reporte = reporte.Where(iec => iec.CajaId == cajaId);
                else
                    reporte = reporte.Where(iec => iec.CajaId == usr.cajaid);
            }
            else
            {
                if (!usr.roles.Contains((int)Roles.Administrador))
                    reporte = reporte.Where(iec => iec.CajaId == usr.cajaid);
            }

            var data = reporte.ToArray();
            var servicios = new MaestroContableServices(DbIpsa).ObtenerServicios();

            var ingresosEgresosCajaReferencias = db.IngresosEgresosCajaReferencias.Include(x => x.TipoPago).Where(r => data.Select(x=>x.Id).Contains(r.ReciboId)).ToArray();
            var ingresosEgresosCajaDetalle = db.IngresosEgresosCajaDetalle.Where(r => data.Select(x => x.Id).Contains(r.ReciboId)).ToArray();

            var result = data.Select(rep => new
            {
                rep.Id,
                Estado = rep.Estado.Descripcion,
                Caja = rep.Caja.Description,
                rep.Beneficiario,
                rep.FechaProceso,
                rep.Username,
                rep.Total,
                TipoMoneda = rep.TipoMoneda.Descripcion,
                rep.NumRecibo,
                Dolar = rep.EstadoId == (int)IngresosEgresosCajaEstado.Anulado ? 0 : Math.Round((rep.TipoMonedaId == (short)TipoMonedaParamFilter.Dolar ? rep.Total : 0), 2),
                Cordoba = rep.EstadoId == (int)IngresosEgresosCajaEstado.Anulado ? 0 : Math.Round((rep.TipoMonedaId == (short)TipoMonedaParamFilter.Cordoba ? rep.Total : 0), 2),
                rep.Referencias,
                IngresosEgresosCajaReferencias = ingresosEgresosCajaReferencias.Where(x => x.ReciboId == rep.Id).Select(c => new
                {
                    c.Fecha,
                    TipoPago = c.TipoPago.Descripcion,
                    c.TipoCambio,
                    Dolar = Math.Round((rep.TipoMonedaId == (short)TipoMonedaParamFilter.Dolar ? c.Total : c.Total / c.TipoCambio), 2),
                    Cordoba = Math.Round((rep.TipoMonedaId == (short)TipoMonedaParamFilter.Cordoba ? c.Total : c.Total * c.TipoCambio), 2),
                    c.Referencia
                }),
                IngresosEgresosCajaDetalle = ingresosEgresosCajaDetalle.Where(x => x.ReciboId == rep.Id).Select(c => new
                {
                    c.Cantidad,
                    c.CtaContable,
                    c.Montodolar,
                    c.Precio,
                    servicio = servicios.Where(s => s.CtaContable == c.CtaContable).FirstOrDefault().Nombre
                })
            }).ToArray();

            return Json(result,new Newtonsoft.Json.JsonSerializerSettings { MaxDepth = 1, ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore });
        }

        public IActionResult RecibosCaja_Detelle2()
        {         
            var usr = this.GetServiceUser();
            var cajas = db.Caja.Select(p => new Caja { Id = p.Id, Description =p.Description });
            if (!usr.roles.Contains((int)Roles.Administrador))
                cajas = cajas.Where(p => p.Id ==  usr.cajaid);


            ViewData["Caja"] = new SelectList(cajas, "Id", "Description");         
                 
            return View("PagosPorServicio");
        }

        public IActionResult RecibosCaja_Detelle2_GetList(CajaParameterModel p)
        {
            var usr = this.GetServiceUser();

            var servicios = new MaestroContableServices(DbIpsa).ObtenerServicios();

            //var servicios = DbIpsa.MaestroContable
            //    .Where(mc => mc.TipoCta == 4 || mc.Cuenta.StartsWith("1101") || mc.Cuenta.StartsWith("1108") || mc.Cuenta.StartsWith("1105"))
            //    .ToArray();

            
            var reporteMinutas = from iec in db.IngresosEgresosCaja
                        join iecr in db.IngresosEgresosCajaReferencias on iec.Id equals iecr.ReciboId
                        join c in db.Caja on iec.CajaId equals c.Id
                        join m in db.TipoMoneda on iec.TipoMonedaId.Value equals m.Id
                        where iec.FechaProceso >= p.Desde && iec.FechaProceso <= p.Hasta
                        && iec.EstadoId == (int)IngresosEgresosBancoEstados.Registrado
                        select new ReporteMinutaVsServicio{
                            NumRecibo = iec.NumRecibo,
                            FechaProceso = iec.FechaProceso,
                            Caja = c.Description,
                            CajaId = c.Id,
                            Beneficiario = iec.Beneficiario,
                            Moneda = m.Descripcion,
                            MinutaReferencia = iecr.Referencia,
                            MinutaFecha = iecr.Fecha,
                            MontoDolar = Math.Round((iec.TipoMonedaId == (short)TipoMonedaParamFilter.Dolar?iecr.Total:iecr.Total/iecr.TipoCambio),2),
                            MontoCordoba = Math.Round((iec.TipoMonedaId == (short)TipoMonedaParamFilter.Cordoba?iecr.Total:iecr.Total*iecr.TipoCambio),2),                     
            };
             var reporteServicios = from iec in db.IngresosEgresosCaja
                        join iecd in db.IngresosEgresosCajaDetalle on iec.Id equals iecd.ReciboId
                        join c in db.Caja on iec.CajaId equals c.Id
                        join m in db.TipoMoneda on iec.TipoMonedaId.Value equals m.Id 
                        where iec.FechaProceso >= p.Desde && iec.FechaProceso <= p.Hasta
                        && iec.EstadoId == (int)IngresosEgresosBancoEstados.Registrado
                        select new ReporteMinutaVsServicio{
                            NumRecibo = iec.NumRecibo,
                            FechaProceso = iec.FechaProceso,
                            Caja = c.Description,
                            CajaId = c.Id,
                            Beneficiario = iec.Beneficiario,
                            Moneda = m.Descripcion,
                            ServicioNombre =    servicios.Where(s=>s.CtaContable == iecd.CtaContable).FirstOrDefault().Nombre,
                            ServicioPrecio = iecd.Precio,
                            ServicioCantidad = iecd.Cantidad,
                            ServicioTotal = iecd.Montodolar,
                            MinutaFecha = iec.FechaProceso
                        };

            if(p.caja.Value > 0)
            {
                reporteMinutas = from rm in reporteMinutas 
                where rm.CajaId == p.caja.Value 
                select rm;

                reporteServicios = from rs in reporteServicios 
                where rs.CajaId == p.caja.Value 
                select rs;
            }
            

            var allReporteMinutas = reporteMinutas.ToArray();
            var allReporteServicios = reporteServicios.ToArray();

            var MinutasYServicios =allReporteMinutas.Concat(allReporteServicios);

            //var reporteAgrupado = from t in MinutasYServicios
            //            group t by t.NumRecibo into g
            //            select new ReporteMinutaVsServicio
            //            {
            //                    NumRecibo = g.Key,
            //                    MontoCordoba = g.Sum(x => x.MontoCordoba),
            //                    MontoDolar = g.Sum(x => x.MontoDolar),
            //                    ServicioTotal = g.Sum(x => x.ServicioTotal)
            //            };

            //var all = reporteAgrupado.ToArray().Concat(MinutasYServicios);

            //return Json(all);

            //Quitar la primera columna el resumen
            return Json(MinutasYServicios);
        }

        public IActionResult RecibosCaja_Detelle3()
        {  
            return View("ResumenRecibosPorCaja");
        }

        public IActionResult RecibosCaja_Detelle3_GetList(CajaParameterModel p)
        {
            var usr = this.GetServiceUser();

            var reporte = from iec in db.IngresosEgresosCaja
                    join c in db.Caja on iec.CajaId equals c.Id
                    join pf in db.Profile on iec.Username equals pf.Username
                    where iec.EstadoId == (int)IngresosEgresosBancoEstados.Registrado
                    select new {
                        Usuario = pf.Nombre + " " + pf.Apellido,
                        Caja = c.Description,
                        Fecha = iec.FechaProceso,
                        iec.Id
                    };

                                      
            if(p.all)
                return Json(reporte.ToArray());
            else
                return Json(reporte.Where(r => r.Fecha >= p.Desde && r.Fecha <= p.Hasta).ToArray());
        }

        public IActionResult CaratulaConciliacion(string bancoCuenta, int mes, int anio)
        {
            var usr = this.GetServiceUser();
            var reporteFirma = db.ReporteFirma.Find("CaratulaConciliacion");
            var bancosCuentas = DbIpsa.BancosCuentas.Include(bc => bc.Banco).ToArray();

            var bancosCuentasOnlyCode = bancosCuentas.Select(b => b.BancoCuenta).ToArray();

            var _bancoCuenta = bancosCuentasOnlyCode.Where(b => HelperExtensions.HashSHA1(b.ToString()) == bancoCuenta).FirstOrDefault();

            if (_bancoCuenta == 0)            
                return View("Error", "No se encontr� la cuenta bancaria");            

            if (reporteFirma == null)            
                return View("Error", $"No se encontr� la configuraci�n de las firmas del reporte CaratulaConciliacion");

            var infoProcesoBanco = db.ProcesoBanco.Where(b => b.BancoCuenta == _bancoCuenta && b.Fecha.Month == mes && b.Fecha.Year == anio).FirstOrDefault();
            if (infoProcesoBanco == null)
                return View("Error", "No se encontr� la cuenta bancaria con la fecha de proceso indicada");

            var bancoCuentaInfo = bancosCuentas.Where(c => c.BancoCuenta == _bancoCuenta).FirstOrDefault();
            if (!bancoCuentaInfo.Moneda.HasValue)
                return View("Error", $"No esta definida la moneda para la cuenta {bancoCuentaInfo.Descripcion}");

            var conciliacionBancaria = db.ConciliacionBancaria.Include(cb => cb.TipoMovimiento).Where(cb => cb.ProcesoBancoId == infoProcesoBanco.Id).ToArray();
            var conciliacionBancariaAux = db.ConciliacionBancariaAux.Include(cb => cb.TipoMovimiento).Where(cb => cb.ProcesoBancoId == infoProcesoBanco.Id).ToArray();

            var AuxCheques = conciliacionBancaria.Where(cba => cba.TipoMovimientoId == (int)TipoMovimientos.Cheques)
                                                    .Join(conciliacionBancaria
                                                            .Where(cba => cba.TipoMovimientoId == (int)TipoMovimientos.Cheques), cb => cb.Uuid,
                                                    cbaux => cbaux.Uuid,
                                                    (cb, cbaux) => new { cb, cbaux })
                                                    .Select(x => new { total = x.cb.Debito - x.cbaux.Credito });

            var profile = db.Profile.Find(infoProcesoBanco.Username);
            reporteFirma.UsernameElaborado = profile.Nombre + " " + profile.Apellido;

            var caratula = new CaratulaViewModel();

            caratula.ReporteFirmas = reporteFirma;
            caratula.MonedaSimbol = bancoCuentaInfo.Moneda.Value==1?"C$":"$";
            caratula.Titulo = $"Movimientos Bancarios ({bancoCuentaInfo.Banco.Descripcion}) Del Mes De {HelperExtensions.NombreDelMes(mes)} De {anio}";
            caratula.Cuenta = $"{bancoCuentaInfo.Descripcion.ToUpper()}";
            caratula.SaldoSegunBanco = infoProcesoBanco.SaldoInicial; // + (conciliacionBancaria.Sum(x=>x.Credito) - conciliacionBancaria.Sum(x => x.Debito));
            caratula.ChequeFlotantes = conciliacionBancariaAux.Where(cba => cba.TipoMovimientoId == (int)TipoMovimientos.Cheques && cba.Conciliado).Sum(x => x.Credito);
            caratula.DifChequesDeMenosBanco = AuxCheques.Where(ac => ac.total > 0).Sum(s => s.total);
            caratula.DifChequesDeMasBanco =   AuxCheques.Where(ac => ac.total < 0).Sum(s => s.total);

            caratula.NCNoRegistradasEnLibro = conciliacionBancaria.ObtenerNCNoRegistradasEnLibro().Sum(x => x.Debito);
            caratula.NDNoRegistradasEnLibro = conciliacionBancaria.ObtenerNDNoRegistradasEnLibro().Sum(x => x.Credito);
            caratula.DPNoRegistradasEnLibro = conciliacionBancaria.ObtenerDPNoRegistradasEnLibro().Sum(x => x.Credito);

            caratula.DPNoRegistradasEnBanco = conciliacionBancariaAux.ObtenerDPNoRegistradasEnBanco().Sum(x => x.Debito);
            caratula.NCNoRegistradasEnBanco = conciliacionBancariaAux.ObtenerNCNoRegistradaEnBanco().Sum(x => x.Credito);
            caratula.NDNoRegistradasEnBanco = conciliacionBancariaAux.ObtenerNDNoRegistradaEnBanco().Sum(x => x.Debito);

            caratula.CreditosPorCorreccionesIntMas = conciliacionBancariaAux.ObtenerCreditosPorCorreccionesIntMas().Sum(x => x.Credito);
            caratula.DeditosPorCorreccionesIntMenos = conciliacionBancariaAux.ObtenerCreditosPorCorreccionesIntMenos().Sum(x => x.Debito);

            caratula.SaldoSegunLibro -= caratula.ChequeFlotantes;
            caratula.SaldoSegunLibro += caratula.DifChequesDeMenosBanco;
            caratula.SaldoSegunLibro += caratula.DifChequesDeMasBanco;
            caratula.SaldoSegunLibro -= caratula.NCNoRegistradasEnLibro;
            caratula.SaldoSegunLibro += caratula.NDNoRegistradasEnLibro;
            caratula.SaldoSegunLibro += caratula.DPNoRegistradasEnLibro;
            caratula.SaldoSegunLibro -= caratula.DPNoRegistradasEnBanco;
            caratula.SaldoSegunLibro += caratula.NCNoRegistradasEnBanco;
            caratula.SaldoSegunLibro += caratula.NDNoRegistradasEnBanco;
            caratula.SaldoSegunLibro += caratula.CreditosPorCorreccionesIntMas;
            caratula.SaldoSegunLibro -= caratula.DeditosPorCorreccionesIntMenos;


            //caratula.SaldoSegunLibro = new decimal[] 
            //{
            //    -caratula.ChequeFlotantes==0?0:-caratula.ChequeFlotantes,
            //    caratula.DifChequesDeMenosBanco,
            //    caratula.DifChequesDeMasBanco,
            //    -caratula.NCNoRegistradasEnLibro==0?0:-caratula.NCNoRegistradasEnLibro,
            //    caratula.NDNoRegistradasEnLibro,
            //    -caratula.DPNoRegistradasEnLibro==0?0:-caratula.DPNoRegistradasEnLibro,
            //    -caratula.DPNoRegistradasEnBanco==0?0:-caratula.DPNoRegistradasEnBanco,
            //    caratula.NCNoRegistradasEnBanco,
            //    caratula.NDNoRegistradasEnBanco,
            //    caratula.CreditosPorCorreccionesIntMas,
            //    -caratula.DeditosPorCorreccionesIntMenos==0?0:-caratula.DeditosPorCorreccionesIntMenos,
            //}.Sum();

            return View(caratula);
        }
        /// <summary>
        /// Obtiene los Depositos no registrados en nuestro libro
        /// </summary>
        /// <param name="bancoCuenta"></param>
        /// <param name="mes"></param>
        /// <param name="anio"></param>
        /// <returns></returns>
        public IActionResult DPNoRegistradaEnLibro(string bancoCuenta, int mes, int anio)
        {
            var reporteServices = new ReporteServices(db, DbIpsa);
            var result = reporteServices.conciliacionAnexoViewModel(bancoCuenta, mes, anio, "DPNoRegistradaEnLibro");

            if (result.HashError)            
                return View("Error", result.Mensaje);            

            var reporteDetalle = db.ConciliacionBancaria.Include(cb => cb.TipoMovimiento).Where(cb => cb.ProcesoBancoId == result.ProcesoBancoId).ToArray()
                .ObtenerDPNoRegistradasEnLibro()
                .Select(x => new ReporteDetalleViewModel {
                     Fecha = x.Fecha,
                     Monto = x.Credito,
                     NumDocumento = x.Referencia
                }).ToArray();

            var conciliacionAnexoViewModel = new ConciliacionAnexoViewModel {
                Titulo = $"Depositos no registradas en nuestro Libros Al Mes De {HelperExtensions.NombreDelMes(mes)} De {anio}",
                CuentaBancaria = $"{result.CuentaBancaria}",
                detalle = reporteDetalle,
                Moneda = result.Moneda
            };

            return View(conciliacionAnexoViewModel);
        }

        /// <summary>
        /// Obtiene las Notas de Debito no registradas en nuestro libros
        /// </summary>
        /// <param name="bancoCuenta"></param>
        /// <param name="mes"></param>
        /// <param name="anio"></param>
        /// <returns></returns>
        public IActionResult NDNoRegistradaEnLibro(string bancoCuenta, int mes, int anio)
        {
            var reporteServices = new ReporteServices(db, DbIpsa);
            var result = reporteServices.conciliacionAnexoViewModel(bancoCuenta, mes, anio, "NDNoRegistradaEnLibro");

            if (result.HashError)
                return View("Error", result.Mensaje);
            
            var reporteDetalle = db.ConciliacionBancaria.Include(cb => cb.TipoMovimiento).Where(cb => cb.ProcesoBancoId == result.ProcesoBancoId).ToArray()
                .ObtenerNDNoRegistradasEnLibro()
                .Select(x => new ReporteDetalleViewModel
                {
                    Fecha = x.Fecha,
                    Monto = x.Credito,
                    NumDocumento = x.Referencia
                });

            var conciliacionAnexoViewModel = new ConciliacionAnexoViewModel
            {
                Titulo = $"Notas De Debito No Registradas En Libro Al Mes De {HelperExtensions.NombreDelMes(mes)} De {anio}",
                CuentaBancaria = $"{result.CuentaBancaria}",
                detalle = reporteDetalle,
                Moneda = result.Moneda
            };

            return View(conciliacionAnexoViewModel);
        }

        /// <summary>
        /// Obtiene las Notas de credito no registrada en libro
        /// </summary>
        /// <param name="bancoCuenta"></param>
        /// <param name="mes"></param>
        /// <param name="anio"></param>
        /// <returns></returns>
        public IActionResult NCNoRegistradaEnLibro(string bancoCuenta, int mes, int anio)
        {
            var reporteServices = new ReporteServices(db, DbIpsa);
            var result = reporteServices.conciliacionAnexoViewModel(bancoCuenta, mes, anio, "NCNoRegistradaEnLibro");

            if (result.HashError)
                return View("Error", result.Mensaje);

            var reporteDetalle = db.ConciliacionBancaria.Include(cb => cb.TipoMovimiento).Where(cb => cb.ProcesoBancoId == result.ProcesoBancoId).ToArray()               
                .ObtenerNCNoRegistradasEnLibro()
                .Select(x => new ReporteDetalleViewModel
                {
                    Fecha = x.Fecha,
                    Monto = x.Debito,
                    NumDocumento = x.Referencia
                }).ToArray();

            var conciliacionAnexoViewModel = new ConciliacionAnexoViewModel
            {
                Titulo = $"Notas de credito no registrada en Libro Al Mes De {HelperExtensions.NombreDelMes(mes)} De {anio}",
                CuentaBancaria = $"{result.CuentaBancaria}",
                detalle = reporteDetalle,
                Moneda = result.Moneda
            };

            return View(conciliacionAnexoViewModel);
        }


        /// <summary>
        /// Obtiene los Depositos no registrados en el banco
        /// </summary>
        /// <param name="bancoCuenta"></param>
        /// <param name="mes"></param>
        /// <param name="anio"></param>
        /// <returns></returns>
        public IActionResult DPNoRegistradaEnBanco(string bancoCuenta, int mes, int anio)
        {
            var reporteServices = new ReporteServices(db, DbIpsa);
            var result = reporteServices.conciliacionAnexoViewModel(bancoCuenta, mes, anio, "DPNoRegistradaEnBanco");

            if (result.HashError)
                return View("Error", result.Mensaje);           

            var reporteDetalle = db.ConciliacionBancariaAux
                .Include(cb => cb.TipoMovimiento).Where(cb => cb.ProcesoBancoId == result.ProcesoBancoId).ToArray()
                .ObtenerDPNoRegistradasEnBanco()
                .Select(x => new ReporteDetalleViewModel
                {
                    TableInfo = x.TableInfo,
                    Fecha = x.Fecha,
                    Monto = x.Debito,
                    NumDocumento = x.Referencia,
                    NumRecibo = x.IdRef.ToString().PadLeft(10, '0'),
                    Caja = x.TableInfo == 1 ? db.IngresosEgresosCaja.Include(i => i.Caja).Single(i=> i.Id == x.IdRef).Caja.Description : "Movimientos"
                }).ToArray();

            var conciliacionAnexoViewModel = new ConciliacionAnexoViewModel
            {
                Titulo = $"Depositos no registrados en el Banco Al Mes De {HelperExtensions.NombreDelMes(mes)} De {anio}",
                CuentaBancaria = result.CuentaBancaria,
                detalle = reporteDetalle,
                Moneda = result.Moneda
            };

            return View(conciliacionAnexoViewModel);
        }

        /// <summary>
        /// Obtiene las Notas de debito no registrada en Banco
        /// </summary>
        /// <param name="bancoCuenta"></param>
        /// <param name="mes"></param>
        /// <param name="anio"></param>
        /// <returns></returns>
        public IActionResult NDNoRegistradaEnBanco(string bancoCuenta, int mes, int anio)
        {
            var reporteServices = new ReporteServices(db, DbIpsa);
            var result = reporteServices.conciliacionAnexoViewModel(bancoCuenta, mes, anio, "NDNoRegistradaEnBanco");

            if (result.HashError)
                return View("Error", result.Mensaje);

            var reporteDetalle = db.ConciliacionBancariaAux
                .Include(cb => cb.TipoMovimiento).Where(cb => cb.ProcesoBancoId == result.ProcesoBancoId).ToArray()                
                .ObtenerNDNoRegistradaEnBanco()
                .Select(x => new ReporteDetalleViewModel
                {
                    TableInfo = x.TableInfo,
                    Fecha = x.Fecha,
                    Monto = x.Debito,
                    NumDocumento = x.Referencia,
                    NumRecibo = x.IdRef.ToString().PadLeft(10, '0'),
                    Caja = x.TableInfo == 1 ? db.IngresosEgresosCaja.Include(i => i.Caja).Single(i => i.Id == x.IdRef).Caja.Description : "Movimientos"
                }).ToArray();

            var conciliacionAnexoViewModel = new ConciliacionAnexoViewModel
            {
                Titulo = $"Notas de debito no registrada en Banco Al Mes De {HelperExtensions.NombreDelMes(mes)} De {anio}",
                CuentaBancaria = result.CuentaBancaria,
                detalle = reporteDetalle,
                Moneda = result.Moneda
            };

            return View(conciliacionAnexoViewModel);
        }


        /// <summary>
        /// Obtiene las Notas de credito no registrada en Banco
        /// </summary>
        /// <param name="bancoCuenta"></param>
        /// <param name="mes"></param>
        /// <param name="anio"></param>
        /// <returns></returns>
        public IActionResult NCNoRegistradaEnBanco(string bancoCuenta, int mes, int anio)
        {
            var reporteServices = new ReporteServices(db, DbIpsa);
            var result = reporteServices.conciliacionAnexoViewModel(bancoCuenta, mes, anio, "NCNoRegistradaEnBanco");

            if (result.HashError)
                return View("Error", result.Mensaje);

            var reporteDetalle = db.ConciliacionBancariaAux
                .Include(cb => cb.TipoMovimiento).Where(cb => cb.ProcesoBancoId == result.ProcesoBancoId).ToArray()
                .ObtenerNCNoRegistradaEnBanco()
                .Select(x => new ReporteDetalleViewModel
                {
                    TableInfo = x.TableInfo,
                    Fecha = x.Fecha,
                    Monto = x.Credito,
                    NumDocumento = x.Referencia,
                    NumRecibo = x.IdRef.ToString().PadLeft(10, '0'),
                    Caja = x.TableInfo == 1 ? db.IngresosEgresosCaja.Include(i => i.Caja).Single(i => i.Id == x.IdRef).Caja.Description : "Movimientos"
                }).ToArray();

            var conciliacionAnexoViewModel = new ConciliacionAnexoViewModel
            {
                Titulo = $"Notas De Credito No Registradas En Banco Al Mes De {HelperExtensions.NombreDelMes(mes)} De {anio}",
                CuentaBancaria = result.CuentaBancaria,
                detalle = reporteDetalle,
                Moneda = result.Moneda
            };

            return View(conciliacionAnexoViewModel);
        }

        /// <summary>
        /// Obtiene los Debitos por correciones internas de mas
        /// </summary>
        /// <param name="bancoCuenta"></param>
        /// <param name="mes"></param>
        /// <param name="anio"></param>
        /// <returns></returns>
        public IActionResult CreditosPorCorreccionesIntMas(string bancoCuenta, int mes, int anio)
        {
            var reporteServices = new ReporteServices(db, DbIpsa);
            var result = reporteServices.conciliacionAnexoViewModel(bancoCuenta, mes, anio, "CreditosPorCorreccionesIntMas");

            if (result.HashError)
                return View("Error", result.Mensaje);

            var reporteDetalle = db.ConciliacionBancariaAux
                .Include(cb => cb.TipoMovimiento).Where(cb => cb.ProcesoBancoId == result.ProcesoBancoId).ToArray()
                .ObtenerCreditosPorCorreccionesIntMas()
                .Select(x => new ReporteDetalleViewModel
                {
                    TableInfo = x.TableInfo,
                    Fecha = x.Fecha,
                    Monto = x.Credito,
                    NumDocumento = x.Referencia,
                    NumRecibo = x.IdRef.ToString().PadLeft(10, '0'),
                    Caja = x.TableInfo == 1 ? db.IngresosEgresosCaja.Include(i => i.Caja).Single(i => i.Id == x.IdRef).Caja.Description : "Movimientos"
                }).ToArray();

            var conciliacionAnexoViewModel = new ConciliacionAnexoViewModel
            {
                Titulo = $"Notas De Credito No Registradas En Banco Al Mes De {HelperExtensions.NombreDelMes(mes)} De {anio}",
                CuentaBancaria = result.CuentaBancaria,
                detalle = reporteDetalle,
                Moneda = result.Moneda
            };

            return View(conciliacionAnexoViewModel);
        }

        /// <summary>
        /// Obtiene los Debitos por correciones internas de menos
        /// </summary>
        /// <param name="bancoCuenta"></param>
        /// <param name="mes"></param>
        /// <param name="anio"></param>
        /// <returns></returns>
        public IActionResult CreditosPorCorreccionesIntMenos(string bancoCuenta, int mes, int anio)
        {
            var reporteServices = new ReporteServices(db, DbIpsa);
            var result = reporteServices.conciliacionAnexoViewModel(bancoCuenta, mes, anio, "CreditosPorCorreccionesIntMenos");

            if (result.HashError)
                return View("Error", result.Mensaje);

            var reporteDetalle = db.ConciliacionBancariaAux
                .Include(cb => cb.TipoMovimiento).Where(cb => cb.ProcesoBancoId == result.ProcesoBancoId).ToArray()
                .ObtenerCreditosPorCorreccionesIntMenos()
                .Select(x => new ReporteDetalleViewModel
                {
                    TableInfo = x.TableInfo,
                    Fecha = x.Fecha,
                    Monto = x.Credito,
                    NumDocumento = x.Referencia,
                    NumRecibo = x.IdRef.ToString().PadLeft(10, '0'),
                    Caja = x.TableInfo == 1 ? db.IngresosEgresosCaja.Include(i => i.Caja).Single(i => i.Id == x.IdRef).Caja.Description : "Movimientos"
                }).ToArray();

            var conciliacionAnexoViewModel = new ConciliacionAnexoViewModel
            {
                Titulo = $"Notas De Credito No Registradas En Banco Al Mes De {HelperExtensions.NombreDelMes(mes)} De {anio}",
                CuentaBancaria = result.CuentaBancaria,
                detalle = reporteDetalle,
                Moneda = result.Moneda
            };

            return View(conciliacionAnexoViewModel);
        }

        /// <summary>
        /// Obtiene los Debitos por correciones internas de menos
        /// </summary>
        /// <param name="bancoCuenta"></param>
        /// <param name="mes"></param>
        /// <param name="anio"></param>
        /// <returns></returns>
        [HttpGet("reportes/recibos-mal-digitados")]
        public IActionResult RecibosMalDigitados(){            
            return View();
        }

        [HttpGet("reportes/recibos-mal-digitados-obtener")]
        public IActionResult RecibosMalDigitadosObtener()
        {  
            var fechaMin = new DateTime(2017,1,1);

            var results = from iec in db.IngresosEgresosCaja                       
                          join iecr in db.IngresosEgresosCajaReferencias on iec.Id equals iecr.ReciboId                        
                          where iec.EstadoId == (short)IngresosEgresosCajaEstado.Registrado
                          //&& (iec.FechaProceso < fechaMin || iecr.Fecha < fechaMin) && (iecr.Excluido ?? false) == false
                          && (iecr.Fecha < fechaMin && (iecr.Excluido ?? false) == false)
                          select iec.Id;

            var result = db.IngresosEgresosCaja
                        .Include(i => i.TipoMoneda)
                        .Include(i => i.IngresosEgresosCajaReferencias)
                        .Include(i => i.Caja)
                        .Where(i => results.Contains(i.Id)).ToArray();

            return Json(result.Select(x=> new
            {
                x.Id,
                x.NumRecibo,
                x.FechaProceso,
                x.FechaRegistro,
                x.Beneficiario,
                caja = x.Caja.Description,
                moneda = x.TipoMoneda.Descripcion,
                x.Total,
                x.Username,
                detalle = x.IngresosEgresosCajaReferencias.Select(p => new {
                    p.Id,
                    p.Total,
                    p.Referencia,
                    Excluido = (p.Excluido??false),
                    p.Fecha,
                }).Where(i=> i.Excluido == false)                              
            }));
               
        }

        [HttpPost("reportes/corregir-fecha")]
        public IActionResult CorregirFecha(int Id, DateTime FechaProceso)
        {
            var user = this.GetServiceUser();

            var oldRecibo = db.IngresosEgresosCaja.Find(Id);

            if(oldRecibo == null)
                return NotFound("No se encontro el recibo con id: " + Id);

            oldRecibo.FechaProceso = FechaProceso;
            oldRecibo.UsernameEditado = user.username;
            oldRecibo.FechaEditado = DateTime.Now;

            db.SaveChanges();
            return Json(FechaProceso);
        }

        [HttpPost("reportesDetalle/corregir-o-excluir")]
        public IActionResult CorregirDetalle(int Id, DateTime? Fecha, bool? Excluido)
        {
            var user = this.GetServiceUser();

            var oldReferencia = db.IngresosEgresosCajaReferencias.Find(Id);

            if (oldReferencia == null)
                return NotFound("No se encontro el recibo con id: " + Id);

            if (Excluido.HasValue)
            {
                if (Excluido.Value)
                {
                    oldReferencia.Excluido = Excluido.Value;
                }
            }

            if (Fecha.HasValue)
            {
                oldReferencia.Fecha = Fecha.Value;
            }

            db.SaveChanges();
            return Json(oldReferencia);
        }

        [HttpGet("reportesDetalle/recibos-excluidos")]
        public IActionResult RecibosExcluidos(int Id, DateTime? Fecha, bool? Excluido)
        {
            var user = this.GetServiceUser();

            var oldReferencia = db.IngresosEgresosCajaReferencias.Find(Id);

            if (oldReferencia == null)
                return NotFound("No se encontro el recibo con id: " + Id);

            if (Excluido.HasValue)
            {
                if (Excluido.Value)
                {
                    oldReferencia.Excluido = Excluido.Value;
                }
            }

            if (Fecha.HasValue)
            {
                oldReferencia.Fecha = Fecha.Value;
            }

            db.SaveChanges();
            return Json(oldReferencia);
        }

    }

}
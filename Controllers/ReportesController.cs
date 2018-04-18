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
            return View("RecibosCaja");
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

             var servicios = DbIpsa.MaestroContable
                .Where(mc => mc.TipoCta == 4 || mc.Cuenta.StartsWith("1101") || mc.Cuenta.StartsWith("1108") || mc.Cuenta.StartsWith("1105"))
                .ToArray();

            
            var reporteMinutas = from iec in db.IngresosEgresosCaja
                        join iecr in db.IngresosEgresosCajaReferencias on iec.Id equals iecr.ReciboId
                        join c in db.Caja on iec.CajaId equals c.Id
                        join m in db.TipoMoneda on iec.TipoMonedaId.Value equals m.Id
                        where iec.FechaProceso >= p.Desde && iec.FechaProceso <= p.Hasta
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
                        select new ReporteMinutaVsServicio{
                            NumRecibo = iec.NumRecibo,
                            FechaProceso = iec.FechaProceso,
                            Caja = c.Description,
                            CajaId = c.Id,
                            Beneficiario = iec.Beneficiario,
                            Moneda = m.Descripcion,
                            ServicioNombre =    servicios.Where(s=>s.CtaContable == iecd.CtaContable).FirstOrDefault().Nombre           ,
                            ServicioPrecio = iecd.Precio,
                            ServicioCantidad = iecd.Cantidad,
                            ServicioTotal = iecd.Montodolar
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

            var reporteAgrupado = from t in MinutasYServicios
                        group t by t.NumRecibo into g
                        select new ReporteMinutaVsServicio
                        {
                                NumRecibo = g.Key,
                                MontoCordoba = g.Sum(x => x.MontoCordoba),
                                MontoDolar = g.Sum(x => x.MontoDolar),
                                ServicioTotal = g.Sum(x => x.ServicioTotal)
                        };

            var all = reporteAgrupado.ToArray().Concat(MinutasYServicios);

            return Json(all);
        }

        public IActionResult RecibosCaja_Detelle1_GetList(DateTime desde,DateTime hasta)
        {
            var usr = this.GetServiceUser();
            //var reporte = 
            var reporte = db.IngresosEgresosCaja
            .Include(i=>i.Caja)
            .Include(i=>i.IngresosEgresosCajaDetalle)
            .Include(i=>i.IngresosEgresosCajaReferencias).ThenInclude(c => c.TipoPago)      
            .Include(i=>i.TipoMoneda)
            .Where(i => i.FechaProceso >= desde && i.FechaProceso <= hasta);

            if(usr.roles.Contains((int)Roles.Reportes))
                reporte = reporte.Where(iec => iec.CajaId == usr.cajaid);
            else
                reporte = reporte.Where(iec => iec.CajaId > 0);
           
            var servicios = DbIpsa.MaestroContable
                .Where(mc => mc.TipoCta == 4 || mc.Cuenta.StartsWith("1101") || mc.Cuenta.StartsWith("1108") || mc.Cuenta.StartsWith("1105"))
                .ToArray();

            var result = reporte.Select(rep => new {
                rep.Id,
                Caja = rep.Caja.Description,
                rep.Beneficiario,
                rep.FechaProceso,
                rep.Username,
                Total =  rep.Total,
                TipoMoneda=rep.TipoMoneda.Descripcion,
                rep.NumRecibo,
                Dolar = Math.Round((rep.TipoMonedaId == (short)TipoMonedaParamFilter.Dolar?rep.Total:0),2),
                Cordoba = Math.Round((rep.TipoMonedaId == (short)TipoMonedaParamFilter.Cordoba?rep.Total:0),2),
                rep.Referencias,
                IngresosEgresosCajaReferencias =rep.IngresosEgresosCajaReferencias.Select(c=> new {
                    c.Fecha,
                    TipoPago=c.TipoPago.Descripcion,
                    c.TipoCambio,
                    Dolar = Math.Round((rep.TipoMonedaId == (short)TipoMonedaParamFilter.Dolar?c.Total:c.Total/c.TipoCambio),2),
                    Cordoba = Math.Round((rep.TipoMonedaId == (short)TipoMonedaParamFilter.Cordoba?c.Total:c.Total*c.TipoCambio),2),
                    c.Referencia    
                }), 
                IngresosEgresosCajaDetalle = rep.IngresosEgresosCajaDetalle.Select(c => new{
                    c.Cantidad,
                    c.CtaContable,
                    c.Montodolar,
                    c.Precio,
                    servicio = servicios.Where(s=>s.CtaContable == c.CtaContable).FirstOrDefault().Nombre
                })
            }).ToArray();

            return Json(result,new Newtonsoft.Json.JsonSerializerSettings { MaxDepth = 1, ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore });
        }
    }
}
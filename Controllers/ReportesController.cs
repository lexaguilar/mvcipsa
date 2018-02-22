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
    [Authorize(Policy = "Admin")]
    public class ReportesController : Controller
    {
        private readonly IPSAContext db;
        public ReportesController(IPSAContext _db)
        {
            db = _db;
        }
        public IActionResult RecibosCaja_Detelle1()
        {                       
            return View("RecibosCaja");
        }

        public IActionResult RecibosCaja_Detelle1_GetList(DateTime desde,DateTime hasta){
            // var reporte = db.IngresosEgresosCaja
            // .Include(i=>i.Caja)
            // .Include(i=>i.IngresosEgresosCajaDetalle)
            // .Include(i=>i.IngresosEgresosCajaReferencias.Select(ii=> new {
            //     ii.Fecha,
            //     TipoPago=ii.TipoPago.Descripcion,
            //     ii.totalC,
            //     ii.totalD,
            //     ii.Referencia,
            //     ii.TipoCambio
            // }))          
            // .Include(i=>i.TipoMoneda)
            // .Where(i => i.FechaProceso >= desde && i.FechaProceso <= hasta).ToArray();

              var reporte = db.IngresosEgresosCaja
            .Include(i=>i.Caja)
            .Include(i=>i.IngresosEgresosCajaDetalle)
            .Include(i=>i.IngresosEgresosCajaReferencias).ThenInclude(c => c.TipoPago)      
            .Include(i=>i.TipoMoneda)
            .Where(i => i.FechaProceso >= desde && i.FechaProceso <= hasta).ToArray();

            //var detalle = from dt in db.IngresosEgresosCajaDetalle


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
                    c.Cantidad,c.CtaContable,c.Montodolar,c.Precio
                })
            });           

            return Json(result,new Newtonsoft.Json.JsonSerializerSettings { MaxDepth = 1, ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore });
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using mvcIpsa.DbModel;
using mvcIpsa.DbModelIPSA;
using mvcIpsa.Models;
using mvcIpsa.Services;

namespace mvcIpsa.Controllers
{
    public class ComprobantesController : Controller
    {
        private readonly IPSAContext db;
        private readonly DBIPSAContext DbIpsa;
        public ComprobantesController(DBIPSAContext _DbIpsa, IPSAContext _db)
        {
            db = _db;
            DbIpsa = _DbIpsa;
        }
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost("comprobantes/obtener-previo")]
        public IActionResult obtenerPrevio(ParameterCaja parameter)
        {
            if (parameter.SearchType == SearchType.None)
            {
                return BadRequest("Seleccione un tipo de busqueda, mensual o diario");
            }

            var bancoService = new BancoCuentasServices(DbIpsa);
            var bancoInfo = bancoService.GetList().ToArray();

            var servicios = new MaestroContableServices(DbIpsa).ObtenerServicios();

            var resultCaja = from iec in db.IngresosEgresosCaja
                             join iecd in db.IngresosEgresosCajaDetalle on iec.Id equals iecd.ReciboId
                             join c in db.Caja on iec.CajaId equals c.Id
                             where parameter.CajaIds.Contains(c.Id) && iec.FechaProceso >= parameter.desde && iec.FechaProceso <= parameter.hasta                            
                             select new
                             {
                                 Fecha = iec.FechaProceso,
                                 _Caja = c.Description,
                                 iec.Total,
                                 iecd.CtaContable,
                                 iecd.Montodolar,
                                 iec.Id,
                                 iec.TipoMonedaId
                             };

            var result = resultCaja.ToArray();

            var bancosReferencias = from iec in db.IngresosEgresosCaja
                                    join iecd in db.IngresosEgresosCajaReferencias on iec.Id equals iecd.ReciboId
                                    join c in db.Caja on iec.CajaId equals c.Id
                                    where parameter.CajaIds.Contains(c.Id) && iec.FechaProceso >= parameter.desde && iec.FechaProceso <= parameter.hasta
                                    select new
                                    {
                                        Id = iec.Id,
                                        BancoId = iecd.IdBanco
                                    };


            var bancosReferenciasArr = bancosReferencias.ToArray();

            var resultFinal = result.Select(x => new
            {
                x.Fecha,
                x._Caja,
                x.Total,
                Servicio = servicios.Where(c=>c.CtaContable == x.CtaContable).FirstOrDefault().Nombre,
                x.CtaContable,
                x.Montodolar,
                x.TipoMonedaId,
                x.Id,
                BancoId = bancosReferenciasArr.Where(h=>h.Id == x.Id).First().BancoId
            });

            var resultados = new List<Resultados>();
            foreach (var x in resultFinal)
            {
                try
                {
                    resultados.Add(new Resultados
                    {
                        Fecha = x.Fecha,
                        _Caja = x._Caja,
                        Total = x.Total,
                        Servicio = x.Servicio,
                        CtaContable = x.CtaContable,
                        Montodolar = x.Montodolar,                      
                        Banco = bancoInfo.Where(b => b.Bancoid == x.BancoId && (b.Moneda??0) == x.TipoMonedaId).FirstOrDefault().Banco.Descripcion,
                        CuentaBancaria = bancoInfo.Where(b => b.Bancoid == x.BancoId && (b.Moneda??0) == x.TipoMonedaId).FirstOrDefault().Descripcion
                    });
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                
            }

            //var resultFinal2 = resultFinal.Select(x => new
            //{
            //    x.Fecha,
            //    x._Caja,
            //    x.Total,
            //    x.Servicio,
            //    x.CtaContable,
            //    x.Montodolar,
            //    x.BancoId,
            //    Banco = bancoInfo.Where(b => b.Bancoid == x.BancoId && (b.Moneda??0) == x.TipoMonedaId).FirstOrDefault().Banco.Descripcion,
            //    CuentaBancaria = bancoInfo.Where(b => b.Bancoid == x.BancoId && (b.Moneda??0) == x.TipoMonedaId).FirstOrDefault().Descripcion
            //});

            return Json(resultados);
        }
    }

    public class Resultados
    {
        public DateTime Fecha { get; set; }
        public string _Caja { get; set; }
        public decimal Total { get; set; }
        public string Servicio { get; set; }
        public string CtaContable { get; set; }
        public string Banco { get; set; }
        public string CuentaBancaria { get; set; }
        public decimal Montodolar { get; set; }
    }
    
}
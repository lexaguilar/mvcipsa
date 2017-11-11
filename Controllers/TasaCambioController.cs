using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using mvcIpsa.DbModel;
using mvcIpsa.Models;
using Microsoft.AspNetCore.Authorization;

namespace mvcIpsa.Controllers
{
    [Authorize(Policy = "Admin")]
    public class TasaCambioController : Controller
    {
        private readonly IPSAContext db;
        public TasaCambioController(IPSAContext _db)
        {
            db = _db;
        }
        public IActionResult Index()
        {
            var tasaCambio = db.CambioOficial               
                .OrderByDescending(x=>x.FechaCambioOficial)
                .ToList();
            return View(tasaCambio);
        }
        public async Task<IActionResult> Create()
        {
            List<int> anios = new List<int>();
            var currentDay = DateTime.Today.Year; 
            for (int i = currentDay; i <= currentDay+1; i++)            
                anios.Add(currentDay);            

            ViewBag.anios = anios;
           
            return View();         
        }

        [HttpGet("TasaCambio/obtenerCambioOficial")]
        public async Task<IActionResult> obtenerCambioOficial(int anio,int mes)
        {
            WS.BC.TipoCambio.Tipo_Cambio_BCNSoapClient tc = new WS.BC.TipoCambio.Tipo_Cambio_BCNSoapClient(new WS.BC.TipoCambio.Tipo_Cambio_BCNSoapClient.EndpointConfiguration());
            var result = await tc.RecuperaTC_MesAsync(anio, mes);

            var response = result.Body.RecuperaTC_MesResult;
            var TipoCambio = new List<TipoCambioViewModel>();


            foreach (System.Xml.Linq.XElement item in response.Descendants())
            {
                if (item.HasElements)
                {
                    TipoCambio.Add(new TipoCambioViewModel
                    {
                        valor = item.Element("Valor").Value,
                        fecha = item.Element("Fecha").Value,
                        cantidad = 5 ,
                        precio = 10.0M
                    });
                }
                
            }
            

            return Json(TipoCambio.OrderBy(x=>x.fecha));
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using mvcIpsa.DbModel;
using mvcIpsa.Models;
using Microsoft.AspNetCore.Authorization;
using System.Net.Http;

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
                .OrderByDescending(x => x.FechaCambioOficial)
                .ToList();
            return View(tasaCambio);
        }
        public async Task<IActionResult> Create()
        {
            List<int> anios = new List<int>();
            var currentDay = DateTime.Today.Year;
            for (int i = currentDay; i <= currentDay + 1; i++)
                anios.Add(currentDay);

            ViewBag.anios = anios;

            return View();
        }

        [HttpGet("TasaCambio/obtenerCambioOficial")]
        public async Task<IActionResult> obtenerCambioOficial(int anio, int mes)
        {
            var TipoCambio = getTipoCambio(anio, mes);

            return Json(TipoCambio);
        }

        internal string getTipoCambio(int anio, int mes)
        {
            var clientBC = new HttpClient();
            clientBC.BaseAddress = new Uri("http://www.bcn.gob.ni/");

            var response =  clientBC.GetAsync("estadisticas/mercados_cambiarios/tipo_cambio/cordoba_dolar/mes.php?mes=01&anio=2018").Result;
            if (response.IsSuccessStatusCode)
            {
                return response.Content.ReadAsStringAsync().Result;
            }
            return null;

        }

        //[HttpGet("TasaCambio/Guardar")]
        //public async Task<IActionResult> Guardar(IEnumerable<CambioOficial> tasaCambio)
        //{
        //    var tacaCambio = await getTipoCambio(anio, mes);
        //    foreach (var item in tacaCambio)
        //    {
        //        var existe = db.CambioOficial.Find(Convert.ToDateTime(item.fecha));
        //        if (existe == null)
        //        {
        //            var newCambioOficial = new CambioOficial
        //            {
        //                FechaCambioOficial = Convert.ToDateTime(item.fecha),
        //                Dolares = Convert.ToDecimal(item.valor)
        //            };
        //            item.udpated = true;

        //            db.CambioOficial.Add(newCambioOficial);
        //        }
        //    }
        //    await db.SaveChangesAsync();

        //    foreach (var item in tacaCambio.Where(t => t.udpated == false))
        //    {
        //        var existe = db.CambioOficial.Find(Convert.ToDateTime(item.fecha));
        //        existe.Dolares = Convert.ToDecimal(item.valor);
        //        await db.SaveChangesAsync();
        //    }

        //    return Json(tacaCambio);
           
        //}
    }
}
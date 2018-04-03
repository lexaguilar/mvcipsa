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
using System.Xml.Linq;

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
            return View();
        }
        [HttpGet("TasaCambio/obtenerLista")]
        public IActionResult ObtenerLista()
        {
            var tasaCambio = db.CambioOficial
                .OrderByDescending(x => x.FechaCambioOficial)
                .ToList();
            return Json(tasaCambio);
        }

        public async Task<IActionResult> Create()
        {
            List<int> anios = new List<int>();
            var currentDay = DateTime.Today.Year;            
            anios.Add(currentDay);
            anios.Add(currentDay+1);

            ViewBag.anios = anios;

            return View();
        }

        [HttpGet("TasaCambio/obtenerCambioOficial")]
        public async Task<IActionResult> obtenerCambioOficial(int anio, int mes)
        {
            var tasaCambio = new WS.BC.TipoCambio.Tipo_Cambio_BCNSoapClient(WS.BC.TipoCambio.Tipo_Cambio_BCNSoapClient.EndpointConfiguration.Tipo_Cambio_BCNSoap);

            var result = await tasaCambio.RecuperaTC_MesAsync(anio, mes);
            XElement root = result.Body.RecuperaTC_MesResult;
            var tasa = root.Descendants("Tc");
            var result2 = tasa.Select(x => new { id = x.Element("Fecha").Value, valor = x.Element("Valor").Value }).ToArray();

            return Ok(result2);
        }

        //internal string getTipoCambio(int anio, int mes)
        //{


        //var clientBC = new HttpClient();
        //clientBC.BaseAddress = new Uri("http://www.bcn.gob.ni/");

        //var response =  clientBC.GetAsync($"estadisticas/mercados_cambiarios/tipo_cambio/cordoba_dolar/mes.php?mes={mes.PadLeft(2,'0')}&anio={anio}").Result;
        //if (response.IsSuccessStatusCode)
        //{
        //    return response.Content.ReadAsStringAsync().Result;
        //}
        //return null;

        //  }

        [HttpPost("TasaCambio/Guardar")]
        public async Task<IActionResult> Guardar(IEnumerable<tasaCambio> tasaCambio)
        {                      
            foreach (var item in tasaCambio)
            {
                var fecha = Convert.ToDateTime(item.id);
                var existe = db.CambioOficial.Find(fecha);
                if (existe == null)
                {
                    var newCambioOficial = new CambioOficial
                    {
                        FechaCambioOficial = Convert.ToDateTime(item.id),
                        Dolares = Convert.ToDecimal(item.valor)
                    };                   

                    db.CambioOficial.Add(newCambioOficial);
                }
                else
                {
                    existe.Dolares = Convert.ToDecimal(item.valor);
                }
            }
            await db.SaveChangesAsync();

            return Ok();

        }
    }

    public class tasaCambio
    {
        public string id { get; set; }
        public decimal valor { get; set; }
    }
}
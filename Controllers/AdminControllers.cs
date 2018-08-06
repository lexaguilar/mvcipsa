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

    [Authorize(Policy = "Admin")]
    public class AdminController : Controller
    {
        private readonly IPSAContext db;
        private readonly DBIPSAContext DbIpsa;

        private readonly AppSettings settings;

        public AdminController(IPSAContext _db, DBIPSAContext _DbIpsa,IOptions<AppSettings> _settings)
        {
            db = _db;
            DbIpsa = _DbIpsa;
            settings = _settings.Value;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult GetList()
        {
            var r = new List<recibos>();            
            using (var command = db.Database.GetDbConnection().CreateCommand())// . context.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = "SELECT * FROM ( " +
                "SELECT recibo_id,sum( " +
                    "CASE WHEN c.tipo_moneda_id = 1 THEN (d.total / d.tipo_cambio) " +
                            "WHEN c.tipo_moneda_id = 2 THEN d.total " +
                    "END " +
                ") FROM ingresos_egresos_caja_referencias d join ingresos_egresos_caja c on c.id = d.recibo_id  " +
                "GROUP BY recibo_id) as t " +
                "JOIN ( " +
                "SELECT recibo_id,sum(montodolar) FROM ingresos_egresos_caja_detalle " +
                "GROUP BY recibo_id)as t2 on t.recibo_id = t2.recibo_id";
                db.Database.OpenConnection();
                db.Database.SetCommandTimeout(1000000000);
                using (var reader = command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            r.Add(new recibos{
                                ReciboId = reader.GetInt32(0),
                                detelles = reader.GetDecimal(1),
                                referencias = reader.GetDecimal(2)
                            });
                        }                        
                    }; 
                }
            }
            return Json(r);
        }

        [HttpPost]
        [ActionName("AjustarRecibo")]
        public async Task<IActionResult> AjustarRecibo(int idrecibo,decimal monto)
        {            
            var i = new IngresosEgresosCajaDetalle
                {
                    Cantidad = 1,
                    CtaContable = $"1000{0}",
                    Precio =monto,
                    Montodolar = monto,                  
                    ReciboId = idrecibo
                };
            db.IngresosEgresosCajaDetalle.Add(i);
            db.SaveChanges();
            return Ok();
        }
    }

    public class recibos{
        public int ReciboId { get; set; }
        public decimal detelles { get; set; }
        public decimal referencias { get; set; }
    }
}
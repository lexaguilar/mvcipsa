using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using mvcIpsa.Models;
using Microsoft.AspNetCore.Authorization;
using mvcIpsa.Extensions;
using mvcIpsa.DbModel;
using mvcIpsa.Services;
using mvcIpsa.DbModelIPSA;
using Microsoft.EntityFrameworkCore;

namespace mvcIpsa.Controllers
{
    [Produces("application/json")]
    [Route("api/catalogs")]
    [Authorize]
    public class CatalogsController : Controller
    {        
        private readonly IPSAContext db;
        private readonly Newtonsoft.Json.JsonSerializerSettings confi = new Newtonsoft.Json.JsonSerializerSettings { MaxDepth = 1, ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore };
        private readonly DBIPSAContext DbIpsa;
        public CatalogsController(IPSAContext _db, DBIPSAContext _DbIpsa)
        {
            db = _db;
            DbIpsa = _DbIpsa;
        }
        [HttpGet("ReporteFirma")]
        public IActionResult GetReporteFirma()
        {
            var reporteFirma = new ReporteFirmaServices(db);
            return Json(reporteFirma.GetList().Select(r => new ReporteFirma
            {
                MostrarAprobado = r.MostrarAprobado ?? false,
                MostrarAutorizado = r.MostrarAutorizado ?? false,
                MostrarElaborado = r.MostrarElaborado ?? false,
                MostrarRevisado = r.MostrarRevisado ?? false,
                UsernameAprobado = r.UsernameAprobado,
                UsernameAutorizado = r.UsernameAutorizado,
                UsernameElaborado = r.UsernameElaborado,
                UsernameRevisado = r.UsernameRevisado,
                Reporte = r.Reporte,
                Ubicacion = r.Ubicacion,
            }));
        }
        
        [HttpGet("bancoCuentas")]
        public IActionResult GetBancoCuenta()
        {
            var bancosCuentas = new BancoCuentasServices(DbIpsa);
            return Json(bancosCuentas.GetViewModelList(), confi);
        }

        [HttpGet("tipoMovimientos")]
        public IActionResult GetTipoMovimientos()
        {           
            var tipoMovimientos = new TipoMovimientoServices(db); 
            return Json(tipoMovimientos.GetViewModelList(), confi);           
        }
        [HttpGet("tipoDocumentos")]
        public IActionResult GetTipoDocumentos()
        {           
            var tipoDocumentos = new TipoDocumentoServices(db); 
            return Json(tipoDocumentos.GetViewModelList(), confi);           
        }

        [HttpGet("procesoBanco/bank/{bancoCuenta}")]
        public IActionResult procesoBanco(int bancoCuenta)
        {
            var procesoBancoServices = new ProcesoBancoServices(db);
            return Json(procesoBancoServices.ExistSaldoIncial(bancoCuenta));
        }

        [HttpGet("clientes/buscar/{id}")]
        public IActionResult clienteFind(int id)
        {
            return Json(db.Cliente.Where(c => c.Id == id).Select(c => new ClienteViewModel
            {
                Id = c.Id,
                NombreCompleto = c.Nombre + " " + c.Apellido,
                TipoCliente = c.TipoCliente.Tipocliente,
                TipoClienteId = c.TipoCliente.Id,
                Identificacion = c.Identificacion
            }));
        }

        [HttpGet("cliente/buscar/{identificacion}/per_page/{perPage}/page/{page}")]
        public IActionResult cliente(string identificacion,int perPage, int page)
        {
            PaginationResult<IEnumerable<ClienteViewModel>> clientes = new PaginationResult<IEnumerable<ClienteViewModel>>();
            var query = db.Cliente.Include(c => c.TipoCliente).Select(c => new ClienteViewModel
            {
                Id = c.Id,
                NombreCompleto = c.Nombre + " " + c.Apellido,               
                TipoCliente = c.TipoCliente.Tipocliente,
                TipoClienteId = c.TipoCliente.Id,
                Identificacion = c.Identificacion
            });

            if (char.IsDigit(identificacion[0]))
            {
                query = query.Where(c => c.Identificacion.ToLower().StartsWith(identificacion.ToLower()));               
            }
            else
            {
                query = query.Where(c => c.NombreCompleto.ToLower().Contains(identificacion.ToLower()));               
            }


            clientes.Count = query.Count();
            clientes.Result = query.Skip((page - 1) * perPage).Take(perPage).ToArray();
            return Json(clientes, confi);           
        }

       
    }
}
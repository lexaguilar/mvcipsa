using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using mvcIpsa.DbModel;

namespace mvcIpsa.Controllers
{
    using System.IO;
    using System.Text;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Http;
    using mvcIpsa.Models;
    using NPOI.HSSF.UserModel;
    using NPOI.SS.UserModel;
    using NPOI.XSSF.UserModel;
    using mvcIpsa.DbModelIPSA;
    using mvcIpsa.Extensions;
    using mvcIpsa.Services;

    [Authorize(Policy = "Admin")]
     public class ConciliacionController : Controller
    {
        private readonly IPSAContext db;
        private readonly DBIPSAContext DbIpsa;
        private IHostingEnvironment _hostingEnvironment;        
        public ConciliacionController(IHostingEnvironment hostingEnvironment,IPSAContext _db, DBIPSAContext _DbIpsa)
        {
            _hostingEnvironment = hostingEnvironment;
            db = _db;
            DbIpsa = _DbIpsa;
        }

        public IActionResult Index()
        {
            
        }                       

        [HttpPost]
        [ActionName("upLoadFile")]
        public async Task<IActionResult> upLoadFile()
        {            
            var result = new List<AccountStatusOfBank>();
            IFormFile file = Request.Form.Files[0];
            string folderName = "Upload";
            string webRootPath = _hostingEnvironment.WebRootPath;
            string newPath = Path.Combine(webRootPath, folderName);
            //StringBuilder sb = new StringBuilder();
            if (!Directory.Exists(newPath))
            {
                Directory.CreateDirectory(newPath);
            }
            if (file.Length > 0)
            {
                string sFileExtension = Path.GetExtension(file.FileName).ToLower();
                ISheet sheet;
                string fullPath = Path.Combine(newPath, file.FileName);
                using (var stream = new FileStream(fullPath, FileMode.Create))
                {
                    file.CopyTo(stream);
                    stream.Position = 0;
                    if (sFileExtension == ".xls")
                    {
                        HSSFWorkbook hssfwb = new HSSFWorkbook(stream); //This will read the Excel 97-2000 formats  
                        sheet = hssfwb.GetSheetAt(0); //get first sheet from workbook  
                    }
                    else
                    {
                        XSSFWorkbook hssfwb = new XSSFWorkbook(stream); //This will read 2007 Excel format  
                        sheet = hssfwb.GetSheetAt(0); //get first sheet from workbook   
                    }
                    IRow headerRow = sheet.GetRow(0); //Get Header Row
                    int cellCount = headerRow.LastCellNum;
                   
                    for (int i = (sheet.FirstRowNum + 1); i <= sheet.LastRowNum; i++) //Read Excel File
                    {
                        
                        IRow row = sheet.GetRow(i);
                        if (row == null) continue;
                        if (row.Cells.All(d => d.CellType == CellType.Blank)) continue;
                        

                        result.Add(new AccountStatusOfBank{
                                Fecha = row.GetCell(0).ToString(),
                                Referencia = row.GetCell(1).ToString(),
                                TipoMovimiento = row.GetCell(2).ToString(),
                                Debito = row.GetCell(3).ToString().Replace(",",""),
                                Credito = row.GetCell(4).ToString().Replace(",",""),
                                Estado = "Pendiente",
                                EstadoId = -1,
                                ck = false,
                                UUID = ""
                        });
                    }
                }
            }



            return Json(result);
        }

        [ActionName("GetRecibosAndTransferencies")]
        public IActionResult GetRecibosAndTransferencies(int BancosCuenta, int Year, int Month)
        {
            if (BancosCuenta == 0 || Year == 0 || Month == 0)
                return BadRequest("Por favor seleccione la fecha,mes y año");

            var info = DbIpsa.BancosCuentas.Where(bc => bc.BancoCuenta == BancosCuenta);

            if (info == null)
            {
                return BadRequest("No se encontro el banco");
            }
            //TODO revisar el join
            var accountInfo = from i in info
                              join m in db.TipoMoneda on i.Moneda equals m.Id
                              join b in DbIpsa.Bancos on i.Bancoid equals b.Bancoid
                              select new AccountDescription
                              {
                                  BancoId = i.Bancoid,
                                  Banco = b.Descripcion,
                                  MonedaId = i.Moneda.Value,
                                  Moneda = m.Descripcion,
                                  Sucursal = i.NombreSucursal,
                                  Cuenta = i.CtaContable,
                                  Descripcion = i.Descripcion
                              };

            var _BancosCuentas = accountInfo.FirstOrDefault();


            var result = from iec in db.IngresosEgresosCaja
                         join iecr in db.IngresosEgresosCajaReferencias on iec.Id equals iecr.ReciboId
                         where iec.TipoMonedaId == _BancosCuentas.MonedaId && iecr.IdBanco == _BancosCuentas.BancoId && iec.EstadoId == (short)IngresosEgresosCajaEstado.Registrado &&
                         iec.FechaProceso.Year == Year && iec.FechaProceso.Month == Month
                         select new Auxiliar
                         {
                             Fecha = iec.FechaProceso,
                             Referencia = iecr.Referencia,
                             Debito = iecr.Total,
                             Credito = 0,
                             CajaId = iec.CajaId,
                             NumRecibo = iec.NumRecibo,
                             Id = iec.Id,
                             UUID = "",
                             TipoMovimiento = (int)TipoDocumentos.Desposito
                         };

            // var resultBanco = from ieb in db.IngresosEgresosBanco
            //                 where ieb.TipoMonedaId == _BancosCuentas.MonedaId && ieb.BancoCuenta == _BancosCuentas.BancoId && ieb.EstadoId == (short)IngresosEgresosBancoEstados.Registrado &&
            //                 ieb.FechaProceso.Year == Year && ieb.FechaProceso.Month == Month && ieb.TipoDocumentoId != (int)TipoDocumentos.Cheque
            //                 select new Auxiliar{
            //                     Fecha = ieb.FechaProceso,
            //                     Referencia = ieb.Referencia,
            //                     Total = ieb.Monto,
            //                     CajaId = ieb.CajaId,
            //                     NumRecibo = ieb.Id.ToString(),
            //                     Id = ieb.Id
            //                 };

            return Json(new
            {
                info = _BancosCuentas,
                data = result
            });
        }

        [ActionName("SaveAuxiliarAndIngresosEgresos")]      
        public IActionResult SaveAuxiliarAndIngresosEgresos(ConciliacionViewModel conciliacionViewModel)
        {
            var procesoBancoServices = new ProcesoBancoServices(db);
            var procesoBanco = procesoBancoServices.Find(conciliacionViewModel.BancoCuenta, conciliacionViewModel.Year, conciliacionViewModel.Month);

            if (procesoBanco == null)
            {
                return BadRequest($"No se encontro el proceso para el banco {conciliacionViewModel.BancoCuenta} del año {conciliacionViewModel.Year} y mes {conciliacionViewModel.Month}");
            }

            var id = procesoBanco.Id;

            var auxs = new List<ConciliacionBancariaAux>();
            foreach (var aux in conciliacionViewModel.conciliacionBancariaAux)
            {
                auxs.Add(new ConciliacionBancariaAux
                {
                    Fecha = aux.Fecha,
                    Referencia = aux.Referencia,
                    TipoMovimientoId = aux.TipoMovimientoId,
                    Debito = aux.Debito,
                    Credito = aux.Credito,
                    EstadoId = aux.EstadoId,
                    Uuid = aux.Uuid,
                    ProcesoBancoId = id
                });
            }

            var ecs = new List<ConciliacionBancaria>();
            foreach (var ec in conciliacionViewModel.conciliacionBancaria)
            {
                ecs.Add(new ConciliacionBancaria
                {
                    Fecha = ec.Fecha,
                    Referencia = ec.Referencia,
                    TipoMovimientoId = ec.TipoMovimientoId,
                    Debito = ec.Debito,
                    Credito = ec.Credito,
                    EstadoId = ec.EstadoId,
                    Uuid = ec.Uuid,
                    ProcesoBancoId = id
                });
            }

            var oldProcesoBanco = db.ProcesoBanco.Find(id);

            oldProcesoBanco.ConciliacionBancaria = ecs;
            oldProcesoBanco.ConciliacionBancariaAux = auxs;
            db.SaveChanges();
            return Ok();
        }
    }


}

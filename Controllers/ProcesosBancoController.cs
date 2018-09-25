using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using mvcIpsa.DbModel;
using mvcIpsa.DbModelIPSA;
using mvcIpsa.Extensions;
using mvcIpsa.Models;
using mvcIpsa.Services;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;

namespace mvcIpsa.Controllers
{
    [Authorize]
    public class ProcesosBancoController : Controller
    {
        private readonly IPSAContext db;
        private readonly DBIPSAContext DbIpsa;
        private IHostingEnvironment _hostingEnvironment;
        private DateTime MinDate = new DateTime(2017,1,1);
        public ProcesosBancoController(IHostingEnvironment hostingEnvironment, IPSAContext _db, DBIPSAContext _DbIpsa)
        {
            _hostingEnvironment = hostingEnvironment;
            db = _db;
            DbIpsa = _DbIpsa;
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult GetList()
        {
            var procesos = db.ProcesoBanco.Where(x => x.SaldoFinal != 0).ToList();
            var bancosCuenta = DbIpsa.BancosCuentas.Include(x => x.Banco);

            var procesoBancoServices = new ProcesoBancoServices(db);
            var listaCuentas = procesoBancoServices.GetLastCuentas();

             

            var documentos = db.ReporteFirma; //new List<String> { "Caratula", "Cheques flotantes", "Notas de credito no registradas en libro", "Notas de debito no registradas en el Banco", "Notas de debito no registradas en Libro" };

            return Json(procesos.Select(x => new {
                Banco = bancosCuenta.Where(c => c.BancoCuenta == x.BancoCuenta).FirstOrDefault().Banco.Descripcion,
                Cuenta = bancosCuenta.Where(c => c.BancoCuenta == x.BancoCuenta).FirstOrDefault().Descripcion,
                bancosCuenta.Where(c => c.BancoCuenta == x.BancoCuenta).FirstOrDefault().Moneda,
                x.SaldoInicial,
                x.SaldoFinal,
                x.Fecha,
                x.Username,
                x.BancoCuenta,
                x.Id,
                xIsLastAccount = listaCuentas.Contains(x.Id)?true:false,
                Documentos = documentos.Select(d => new
                {
                    x.Fecha,
                    Nombre = string.IsNullOrEmpty(d.Nombre) ? d.Reporte : d.Nombre ,
                    d.Reporte,
                    BancoCuenta = HelperExtensions.HashSHA1(x.BancoCuenta.ToString()) })

            }));            
        }

        public IActionResult Create()
        {
            var usr = this.GetServiceUser();
            var anios = new List<int> { 2015, 2016, 2017 };

            for (int i = 0; i <= DateTime.Today.Year - anios.Last(); i++)
            {
                anios.Add(anios.Last() + 1);
            }

            ViewBag.Anios = new SelectList(anios);

            var cuentasAsigandas = db.CuentasBancoUsername.Where(x => x.Username == usr.username).Select(x => x.BancoCuenta).ToArray();

            if (cuentasAsigandas.Length == 0)            
                return View("Error", $"El usuario {usr.username} no tiene cuentas bancarias asignadas");            

            ViewBag.Cuentas = new SelectList(DbIpsa.BancosCuentas.Select(bc => new
            {
                bc.BancoCuenta,
                bc.Descripcion
            }).Where(x => cuentasAsigandas.Contains(x.BancoCuenta)), "BancoCuenta", "Descripcion");

            return View();
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
            var _tipoMovimientos = db.TipoMovimiento.ToArray();

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
                         
                        var tipoMovimiento = _tipoMovimientos.Where(t => t.DocName == row.GetCell(2).ToString()).FirstOrDefault();
                        if(tipoMovimiento == null){
                            return BadRequest("El tipo de movimiento " + row.GetCell(2).ToString() +" no se encontro en catalogo, por favor agreguelo si no existe");
                        }

                        result.Add(new AccountStatusOfBank
                        {
                            _Fecha = row.GetCell(0).ToString(),
                            Referencia = row.GetCell(1).ToString(),
                            TipoMovimientoId = tipoMovimiento.Id,
                            TipoMovimiento = row.GetCell(2).ToString(),
                            Debito = row.GetCell(3).ToString().Replace(",", ""),
                            Credito = row.GetCell(4).ToString().Replace(",", ""),
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
                return BadRequest("Por favor seleccione el banco,mes y año");

            var info = DbIpsa.BancosCuentas.Include(b => b.Banco).Where(bc => bc.BancoCuenta == BancosCuenta).ToList();

            if (info == null)            
                return BadRequest("No se encontró el banco");

            var accountInfo = from i in info
                              join m in db.TipoMoneda on i.Moneda equals m.Id
                              //join b in DbIpsa.Bancos on i.Bancoid equals b.Bancoid
                              select new AccountDescription
                              {
                                  BancoId = i.Bancoid,
                                  Banco = i.Banco.Descripcion,
                                  MonedaId = i.Moneda.Value,
                                  Moneda = m.Descripcion,
                                  Sucursal = i.NombreSucursal,
                                  Cuenta = i.CtaContable,
                                  Descripcion = i.Descripcion
                              };

            var _BancosCuentas = accountInfo.FirstOrDefault();

            var procesoBancoServices = new ProcesoBancoServices(db);
            var existeSaldoInicial = procesoBancoServices.ExistSaldoIncial(BancosCuenta);
            if (!existeSaldoInicial)
                return BadRequest($"No se encontró el proceso inicial para el cuenta {_BancosCuentas.Descripcion} del año {Year} y mes {HelperExtensions.NombreDelMes(Month)}");

            var dataSaldoInicial = procesoBancoServices.getSaldoIncial(BancosCuenta);

            if (dataSaldoInicial.Fecha > new DateTime(Year, Month, 1))
                return BadRequest($"El año {Year} y mes {HelperExtensions.NombreDelMes(Month)} son menor a la fecha del saldo inicial del proceso");

            var procesoBancoMayor = db.ProcesoBanco.Where(pb => pb.BancoCuenta == BancosCuenta && pb.Fecha > new DateTime(Year, Month, 1)).ToArray();
            if (procesoBancoMayor.HasData())
                return BadRequest($"Ya se concilio el mes de {HelperExtensions.NombreDelMes(Month)} para el año {Year} de la cuenta {_BancosCuentas.Descripcion}");

            var procesoBancoActual = db.ProcesoBanco.Where(pb => pb.BancoCuenta == BancosCuenta && pb.Fecha.Year == Year && pb.Fecha.Month == Month && pb.TipoProcesoId != (int)TipoProcesos.SaldoInicial).ToArray();
            if (procesoBancoActual.HasData())
                return BadRequest($"Ya se concilio el mes de {HelperExtensions.NombreDelMes(Month)} para el año {Year} de la cuenta {_BancosCuentas.Descripcion}");

            var resultCaja = from iec in db.IngresosEgresosCaja
                         join c in db.Caja on iec.CajaId equals c.Id
                         join tm in db.TipoMovimiento on iec.TipoMovimientoId equals tm.Id
                         join iecr in db.IngresosEgresosCajaReferencias on iec.Id equals iecr.ReciboId
                         where iec.TipoMonedaId == _BancosCuentas.MonedaId 
                         && iecr.IdBanco == _BancosCuentas.BancoId 
                         && iec.EstadoId == (short)IngresosEgresosCajaEstado.Registrado 
                         && iec.FechaProceso.Year <= Year 
                         && iec.FechaProceso.Month <= Month
                         && iec.FechaProceso >= MinDate
                         && iecr.Procesado == false
                         && iecr.TipoPagoId > 2 //solo 3 y 4 minuta y transferencia
                         select new Auxiliar
                         {
                             Fecha = iec.FechaProceso,
                             Referencia = iecr.Referencia,
                             Debito = iecr.Total,
                             Credito = 0,
                             CajaId = iec.CajaId,
                             NumRecibo = iec.NumRecibo,
                             IdOrigen = iecr.Id,
                             IdRef = iec.Id,
                             UUID = "",
                             TipoDocumento = (int)TipoDocumentos.Desposito,
                             TipoMovimientoId = (int)TipoMovimientos.DepositosDeCajaUnica,
                             TipoMovimiento = tm.DocName,
                             TableInfo = 1,
                             Caja = c.Description
                         };

            var resultBanco = from ieb in db.IngresosEgresosBanco
                              join c in db.Caja on ieb.CajaId equals c.Id
                              join tm in db.TipoMovimiento on ieb.TipoMovimientoId equals tm.Id
                              where ieb.TipoMonedaId == _BancosCuentas.MonedaId 
                              && ieb.BancoCuenta == BancosCuenta
                              && ieb.EstadoId == (short)IngresosEgresosBancoEstados.Registrado 
                              && ieb.FechaProceso.Year <= Year 
                              && ieb.FechaProceso.Month <= Month
                              && ieb.Procesado == false
                              select new Auxiliar
                              {
                                  Fecha = ieb.FechaProceso,
                                  Referencia = ieb.Referencia,
                                  Debito = ieb.TipoDocumentoId != (int)TipoDocumentos.NotaDeCredito ? ieb.Monto : 0,
                                  Credito = ieb.TipoDocumentoId == (int)TipoDocumentos.NotaDeCredito ? ieb.Monto : 0,
                                  CajaId = ieb.CajaId,
                                  NumRecibo = ieb.Id.ToString(),
                                  IdOrigen = ieb.Id,                                  
                                  UUID = "",
                                  TipoDocumento = ieb.TipoDocumentoId,
                                  TipoMovimientoId = ieb.TipoMovimientoId,
                                  TipoMovimiento = tm.DocName,
                                  TableInfo = 2,
                                  IdRef = ieb.Id,
                                  Caja = c.Description
                              };

            var rc = resultCaja.ToArray();
            var rb = resultBanco.ToArray();

           
            return Json(new
            {
                info = _BancosCuentas,
                data = rc.Concat(rb)
            });
        }

        /// <summary>
        /// Guarda un proceso de conciliacion
        /// </summary>
        /// <param name="conciliacionViewModel"></param>
        /// <returns></returns>
        [ActionName("SaveAuxiliarAndIngresosEgresos")]
        public IActionResult SaveAuxiliarAndIngresosEgresos(ConciliacionViewModel conciliacionViewModel)
        {
            var usr = this.GetServiceUser();

            var procesoBancoServices = new ProcesoBancoServices(db);
            var existeSaldoInicial = procesoBancoServices.ExistSaldoIncial(conciliacionViewModel.BancoCuenta);
            if (!existeSaldoInicial)
                return BadRequest($"No se encontró el proceso inicial para el banco {conciliacionViewModel.BancoCuenta} del año {conciliacionViewModel.Year} y mes {conciliacionViewModel.Month}");

            var procesoBanco = procesoBancoServices.Find(conciliacionViewModel.BancoCuenta, conciliacionViewModel.Year, conciliacionViewModel.Month);

            //Si el mes y anio que intentamos guardar no existe, lo mandamos a crear
            if (procesoBanco == null)
            {
                var January = 1;
                var December = 12;
                //Verificar que existe un preceso en el mes anterior
                var _beforeMonth = conciliacionViewModel.Month == January ? December : conciliacionViewModel.Month - 1;
                var _beforeYear = _beforeMonth == 12 ? conciliacionViewModel.Year - 1 : conciliacionViewModel.Year;
                var procesoBancoAnterior = procesoBancoServices.Find(conciliacionViewModel.BancoCuenta, _beforeYear, _beforeMonth);
                if (procesoBancoAnterior == null)
                {
                    return BadRequest($"No se encontró el proceso en el mes {HelperExtensions.NombreDelMes(_beforeMonth)} para el banco {conciliacionViewModel.BancoCuenta}");
                }

                var newProcesoBanco = new ProcesoBanco
                {
                    BancoCuenta = conciliacionViewModel.BancoCuenta,
                    Fecha = new DateTime(conciliacionViewModel.Year, conciliacionViewModel.Month, 1),
                    SaldoInicial = procesoBancoAnterior.SaldoFinal,
                    Username = usr.username,
                    FechaRegistrado = DateTime.Now,
                    TipoProcesoId = (int)TipoProcesos.Movimientos
                };

                procesoBanco = procesoBancoServices.Create(newProcesoBanco);

            }
            else
            {
                return BadRequest($"Ya se concilio el mes de {HelperExtensions.NombreDelMes(conciliacionViewModel.Month)} para el año {conciliacionViewModel.Year} de la cuenta {conciliacionViewModel.BancoCuenta}");
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
                    ProcesoBancoId = id,
                    Conciliado = string.IsNullOrEmpty(aux.Uuid) ? false : true,
                    TableInfo = aux.TableInfo,
                    IdOrigen = aux.IdOrigen,
                    IdRef = aux.IdRef
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
                    ProcesoBancoId = id,
                    Conciliado = string.IsNullOrEmpty(ec.Uuid) ? false : true
                });
            }

            var oldProcesoBanco = db.ProcesoBanco.Find(id);

            oldProcesoBanco.ConciliacionBancaria = ecs;
            oldProcesoBanco.ConciliacionBancariaAux = auxs;
            oldProcesoBanco.SaldoFinal = oldProcesoBanco.SaldoInicial +  ecs.Sum(x => x.Credito) - ecs.Sum(x => x.Debito);

            var ingresosCajaReferencias = db.IngresosEgresosCajaReferencias.Where(f => conciliacionViewModel.conciliacionBancariaAux.Where(x=>x.TableInfo == 1).Select(x=>x.IdOrigen).Contains(f.Id)).ToList();
            if (ingresosCajaReferencias.Count > 0)
            {
                ingresosCajaReferencias.ForEach(a => a.Procesado = string.IsNullOrEmpty(conciliacionViewModel.conciliacionBancariaAux.Where(x => x.IdOrigen == a.Id && x.TableInfo == 1).FirstOrDefault().Uuid) ? false : true);
            }

            var ingresosBanco = db.IngresosEgresosBanco.Where(f => conciliacionViewModel.conciliacionBancariaAux.Where(x => x.TableInfo == 2).Select(x => x.IdOrigen).Contains(f.Id)).ToList();
            if (ingresosBanco.Count > 0)
            {
                ingresosBanco.ForEach(a => a.Procesado = string.IsNullOrEmpty(conciliacionViewModel.conciliacionBancariaAux.Where(x => x.IdOrigen == a.Id && x.TableInfo == 2).FirstOrDefault().Uuid) ? false : true);
            }
            

            db.SaveChanges();

            return Ok();
        }

        /// <summary>
        /// Guarda un proceso de conciliacion
        /// </summary>
        /// <param name="conciliacionViewModel"></param>
        /// <returns></returns>
        [ActionName("delete")]
        public IActionResult delete(int id)
        {
            var usr = this.GetServiceUser();

            var cAuxBanco = db.ConciliacionBancariaAux.Where(x => x.ProcesoBancoId == id && x.TableInfo == 2).ToArray();
            var cAuxCaja  = db.ConciliacionBancariaAux.Where(x => x.ProcesoBancoId == id && x.TableInfo == 1).ToArray();

            var iecb = db.IngresosEgresosBanco.Where(x => cAuxBanco.Select(p => p.IdOrigen).Contains(x.Id)).ToList();
            iecb.ForEach(x => x.Procesado = false);

            var iecr = db.IngresosEgresosCajaReferencias.Where(x => cAuxCaja.Select(p => p.IdOrigen).Contains(x.Id)).ToList();
            iecr.ForEach(x => x.Procesado = false);

            db.ConciliacionBancariaAux.RemoveRange(cAuxBanco);
            db.ConciliacionBancariaAux.RemoveRange(cAuxCaja);

            var cBanco = db.ConciliacionBancaria.Where(x => x.ProcesoBancoId == id);
            db.ConciliacionBancaria.RemoveRange(cBanco);

            var procesoBanco = db.ProcesoBanco.Find(id);
            //si es saldo inicial solo actualizar a 0 el saldo final
            if (procesoBanco.TipoProcesoId == 1)
            {
                procesoBanco.SaldoFinal = 0;
            }else
            {
                //sino, eliminar
                db.ProcesoBanco.Remove(procesoBanco);
            }            

            db.SaveChanges();

            return Ok();
        }
    }

    public class AccountDescription
    {
        public int BancoId { get; set; }
        public string Banco { get; set; }
        public string Sucursal { get; set; }
        public int MonedaId { get; set; }
        public string Moneda { get; set; }

        public string Cuenta { get; set; }
        public string Descripcion { get; set; }

    }

    public class AccountStatusOfBank
    {
        public bool ck { get; set; }
        public string _Fecha { get; set; }
        public string Referencia { get; set; }
        public int TipoMovimientoId { get; set; }
        public string TipoMovimiento { get; set; }
        public string Debito { get; set; }

        public string Credito { get; set; }
        public int EstadoId { get; set; }
        public string Estado { get; set; }

        public string UUID { get; set; }
        public string TipoDocumento { get; set; }

    }

    public class Auxiliar
    {
        public Auxiliar()
        {
            Estado = "Pendiente";
            EstadoId = -1;
            ck = false;
        }
        public DateTime Fecha { get; set; }
        public string Referencia { get; set; }
        public decimal Credito { get; set; }
        public decimal Debito { get; set; }
        public int CajaId { get; set; }
        public string NumRecibo { get; set; }
        public int IdOrigen { get; set; }
        public int IdRef { get; set; }
        public int EstadoId { get; set; }
        public string Estado { get; set; }
        public bool ck { get; set; }
        public int TipoDocumento { get; set; }
        public int TipoMovimientoId { get; set; }
        public string TipoMovimiento { get; set; }
        public string UUID { get; set; }
        /// <summary>
        /// 1 si es de caja y 2 si es de banco para saber donde actualizaremos el regostro cuando se guarde la conciliacion
        /// </summary>
        public int TableInfo { get; set; }
        public string Caja { get; set; }
    }
}
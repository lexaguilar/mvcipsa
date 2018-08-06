using Microsoft.EntityFrameworkCore;
using mvcIpsa.DbModel;
using mvcIpsa.DbModelIPSA;
using mvcIpsa.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace mvcIpsa.Services
{
    public class ReporteServices
    {
        private readonly IPSAContext db;
        private readonly DBIPSAContext DbIpsa;
        public ReporteServices(IPSAContext _db, DBIPSAContext _DbIpsa)
        {
            db = _db;
            DbIpsa = _DbIpsa;
        }

        internal ParametrosAnexoViewModel conciliacionAnexoViewModel(string bancoCuenta, int mes, int anio, string NombreReporte) {
            var reporteFirma = db.ReporteFirma.Find(NombreReporte);
            var bancosCuentas = DbIpsa.BancosCuentas.Include(bc => bc.Banco).ToArray();

            var bancosCuentasOnlyCode = bancosCuentas.Select(b => b.BancoCuenta).ToArray();

            var _bancoCuenta = bancosCuentasOnlyCode.Where(b => HelperExtensions.HashSHA1(b.ToString()) == bancoCuenta).FirstOrDefault();

            var conciliacionAnexoViewModel = new ParametrosAnexoViewModel();
            if (_bancoCuenta == 0)
            {
                conciliacionAnexoViewModel.HashError = true;
                conciliacionAnexoViewModel.Mensaje = $"No se encontró la cuenta bancaria";
                return conciliacionAnexoViewModel;
            }            

            if (reporteFirma == null)
            {
                conciliacionAnexoViewModel.HashError = true;
                conciliacionAnexoViewModel.Mensaje = $"No se encontró la configuración de las firmas del reporte {NombreReporte}";
                return conciliacionAnexoViewModel;
            }               

            var infoProcesoBanco = db.ProcesoBanco.Where(b => b.BancoCuenta == _bancoCuenta && b.Fecha.Month == mes && b.Fecha.Year == anio).FirstOrDefault();
            if (infoProcesoBanco == null)
            {
                conciliacionAnexoViewModel.HashError = true;
                conciliacionAnexoViewModel.Mensaje = $"No se encontró la cuenta bancaria con la fecha de proceso indicada";
                return conciliacionAnexoViewModel;
            }               

            var bancoCuentaInfo = bancosCuentas.Where(c => c.BancoCuenta == _bancoCuenta).FirstOrDefault();
            if (!bancoCuentaInfo.Moneda.HasValue)
            {
                conciliacionAnexoViewModel.HashError = true;
                conciliacionAnexoViewModel.Mensaje = $"No esta definida la moneda para la cuenta {bancoCuentaInfo.Descripcion}";
                return conciliacionAnexoViewModel;
            }

            conciliacionAnexoViewModel.ProcesoBancoId = infoProcesoBanco.Id;
            conciliacionAnexoViewModel.CuentaBancaria = bancoCuentaInfo.Descripcion.ToUpper();
            conciliacionAnexoViewModel.Moneda = (bancoCuentaInfo.Moneda ?? 1) == 1 ? "C$" : "$";

            return conciliacionAnexoViewModel;
        }
    }
}

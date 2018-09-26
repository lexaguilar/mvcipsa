using mvcIpsa.DbModel;
using mvcIpsa.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using mvcIpsa.Models;

namespace mvcIpsa.Services
{
    public class ProcesoBancoServices
    {
        private readonly IPSAContext db;
        public ProcesoBancoServices(IPSAContext _db)
        {
            db = _db;
        }

        public ProcesoBanco Find(int bancoCuenta,int year, int month)
        {
            var result = db.ProcesoBanco.FirstOrDefault(b=> b.BancoCuenta == bancoCuenta && b.Fecha.ToString("YYYY-MM") == new DateTime(year,month,1).ToString("YYYY-MM"));
            if (result == null)            
                return new ProcesoBanco();
            
            return result;
        }
        /// <summary>
        /// Retorna las true si el banco tiene una cuenta inicial
        /// </summary>
        /// <param name="bancoCuenta">codigo de la cuenta banco</param>
        /// <returns></returns>
        public bool ExistSaldoIncial(int bancoCuenta)
        {
            return db.ProcesoBanco.Any(b => b.BancoCuenta == bancoCuenta && b.TipoProcesoId == (int)TipoProcesos.SaldoInicial);            
        }

        public ProcesoBanco getSaldoIncial(int bancoCuenta)
        {
            return db.ProcesoBanco.Where(b => b.BancoCuenta == bancoCuenta).FirstOrDefault();
        }        

        /// <summary>
        /// returna los bancos que movimienntos
        /// </summary>
        /// <returns></returns>
        public int[] GetCuentas()
        {
            return db.ProcesoBanco.Select(x => x.BancoCuenta).Distinct().ToArray();
        }

        /// <summary>
        /// returna los bancos que movimienntos
        /// </summary>
        /// <returns></returns>
        public int[] GetLastCuentas()
        {
            var t =  db.ProcesoBanco.GroupBy(x => x.BancoCuenta, (key, xs) => xs.OrderByDescending(x => x.Id).First().Id);
            return t.ToArray();
            //return db.ProcesoBanco.Select(x => x.BancoCuenta).Distinct().ToArray();
        }

        internal ValidateProcesoBanco VerificarProcesoAnterio(AccountDescription info, int Year, int Month)
        {
            var January = 1;
            var December = 12;
            var _beforeMonth = Month == January ? December : Month - 1;
            var _beforeYear = _beforeMonth == 12 ? Year - 1 : Year;

            var procesoBanco = Find(info.BancoCuenta, Year, Month);
            if (!procesoBanco.Exist())
            {
                var procesoBancoAnterior = Find(info.BancoCuenta, _beforeYear, _beforeMonth);

                if (!procesoBancoAnterior.Exist())
                    return new ValidateProcesoBanco
                    {
                        Error = $"No se encontró el proceso en el mes {HelperExtensions.NombreDelMes(_beforeMonth)} para el banco {info.Descripcion}"
                    };


                if (procesoBancoAnterior.Exist() && procesoBancoAnterior.IsInitialBalance() && !procesoBancoAnterior.IsClosed())
                    return new ValidateProcesoBanco
                    {
                        Error = $"El proceso en el mes {HelperExtensions.NombreDelMes(_beforeMonth)} para el banco {info.Descripcion} no esta procesado"
                    };    
            }
            return new ValidateProcesoBanco { successed = true,saldoAnterior = procesoBanco.SaldoFinal };
        }
    }

    internal class ValidateProcesoBanco
    {
        public bool successed { get; set; }
        public string Error { get; set; }
        public decimal saldoAnterior { get; set; }
    }
}

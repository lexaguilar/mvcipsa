using mvcIpsa.DbModel;
using mvcIpsa.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
            var result = db.ProcesoBanco.Where(b=> b.BancoCuenta == bancoCuenta && b.Fecha.ToString("YYYY-MM") == new DateTime(year,month,1).ToString("YYYY-MM"));
            return result.FirstOrDefault();
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
        /// Crea una entidad de procesoBanco
        /// </summary>
        /// <param name="procesoBanco">ProcesoBanco</param>
        /// <returns></returns>
        public ProcesoBanco Create(ProcesoBanco procesoBanco)
        {
            db.ProcesoBanco.Add(procesoBanco);
            db.SaveChanges();
            return procesoBanco;
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
    }
}

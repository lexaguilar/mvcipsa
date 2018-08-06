using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using mvcIpsa.DbModel;
namespace mvcIpsa.Extensions
{
    static class IpsaContextExtensions
    {
        /// <summary>
        /// Obtiene las notas de debito no registrados en libro
        /// </summary>
        /// <param name="Array"></param>
        /// <returns></returns>
        internal static IEnumerable<ConciliacionBancaria> ObtenerNDNoRegistradasEnLibro(this ConciliacionBancaria[] Array)
        {
            return Array
                .Where(cb => (cb.TipoMovimiento.TipoDoc == (int)TipoDocumentos.Desposito || cb.TipoMovimiento.TipoDoc == (int)TipoDocumentos.NotaDeDebito)
                && cb.TipoMovimientoId != (int)TipoMovimientos.DepositosDeCajaUnica && cb.TipoMovimientoId != (int)TipoMovimientos.CorreccionIntMas
                && cb.TipoMovimientoId != (int)TipoMovimientos.CorreccionIntMenos && !cb.Conciliado);
        }
        /// <summary>
        /// Obtiene los depositos no registrados en libro
        /// </summary>
        /// <param name="Array">Arreglo de los datos de origen</param>
        /// <returns></returns>
        internal static IEnumerable<ConciliacionBancaria> ObtenerDPNoRegistradasEnLibro(this ConciliacionBancaria[] Array)
        {
            return Array.Where(cb => (cb.TipoMovimiento.TipoDoc == (int)TipoDocumentos.Desposito || cb.TipoMovimiento.TipoDoc == (int)TipoDocumentos.NotaDeDebito)
                && cb.TipoMovimientoId == (int)TipoMovimientos.DepositosDeCajaUnica
                && !cb.Conciliado);
        }

        /// <summary>
        /// Obtiene las notas de crebito no registrados en libro
        /// </summary>
        /// <param name="Array">Arreglo de los datos de origen</param>
        /// <returns></returns>
        internal static IEnumerable<ConciliacionBancaria> ObtenerNCNoRegistradasEnLibro(this ConciliacionBancaria[] Array)
        {
            return Array.Where(cb => cb.TipoMovimientoId == (int)TipoMovimientos.NotaDeCredito
                && !cb.Conciliado);
        }

        /// <summary>
        /// Obtiene los Debitos por correciones internas de mas
        /// </summary>
        /// <param name="Array">Arreglo de los datos de origen</param>
        /// <returns></returns>
        internal static IEnumerable<ConciliacionBancariaAux> ObtenerCreditosPorCorreccionesIntMas(this ConciliacionBancariaAux[] Array)
        {
            return Array.Where(cb => cb.TipoMovimientoId == (int)TipoMovimientos.CorreccionIntMas
                && !cb.Conciliado);
        }

        /// <summary>
        /// Obtiene los Debitos por correciones internas de menos
        /// </summary>
        /// <param name="Array">Arreglo de los datos de origen</param>
        /// <returns></returns>
        internal static IEnumerable<ConciliacionBancariaAux> ObtenerCreditosPorCorreccionesIntMenos(this ConciliacionBancariaAux[] Array)
        {
            return Array.Where(cb => cb.TipoMovimientoId == (int)TipoMovimientos.CorreccionIntMenos
                && !cb.Conciliado);
        }

        /// <summary>
        /// Obtiene los depositos no registrados en banco
        /// </summary>
        /// <param name="Array">Arreglo de los datos de origen</param>
        /// <returns></returns>
        internal static IEnumerable<ConciliacionBancariaAux> ObtenerDPNoRegistradasEnBanco(this ConciliacionBancariaAux[] Array)
        {
            return Array.Where(cb => (cb.TipoMovimiento.TipoDoc == (int)TipoDocumentos.Desposito || cb.TipoMovimiento.TipoDoc == (int)TipoDocumentos.NotaDeDebito)
                && cb.TipoMovimientoId == (int)TipoMovimientos.DepositosDeCajaUnica
                && !cb.Conciliado);
        }

        /// <summary>
        /// Obtiene las notas de debito no registrados en banco
        /// </summary>
        /// <param name="Array"></param>
        /// <returns></returns>
        internal static IEnumerable<ConciliacionBancariaAux> ObtenerNDNoRegistradaEnBanco(this ConciliacionBancariaAux[] Array)
        {
            return Array.Where(cb => (cb.TipoMovimiento.TipoDoc == (int)TipoDocumentos.Desposito || cb.TipoMovimiento.TipoDoc == (int)TipoDocumentos.NotaDeDebito)
                && cb.TipoMovimientoId != (int)TipoMovimientos.DepositosDeCajaUnica
                && cb.TipoMovimientoId != (int)TipoMovimientos.CorreccionIntMas
                && cb.TipoMovimientoId != (int)TipoMovimientos.CorreccionIntMenos && !cb.Conciliado);
        }

        /// <summary>
        /// Obtiene las notas de credito no registrados en banco
        /// </summary>
        /// <param name="Array"></param>
        /// <returns></returns>
        internal static IEnumerable<ConciliacionBancariaAux> ObtenerNCNoRegistradaEnBanco(this ConciliacionBancariaAux[] Array)
        {
            return Array.Where(cb => cb.TipoMovimientoId == (int)TipoMovimientos.NotaDeCredito && !cb.Conciliado);
        }

        internal static decimal Sum(this decimal[] array)
        {
            return array.Sum();
        }

        /// <summary>
        /// Obtiene las notas de crebito no registrados en libro
        /// </summary>
        /// <param name="procesoBanco">Arreglo de los datos de origen</param>
        /// <returns></returns>
        internal static bool HasData(this ProcesoBanco[] procesoBanco)
        {
            return procesoBanco.Count() > 0;
        }
    }
}

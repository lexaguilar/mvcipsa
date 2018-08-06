using mvcIpsa.DbModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace mvcIpsa.Models
{
    public class CaratulaViewModel
    {
        public ReporteFirma ReporteFirmas { get; set; }
        public String Titulo { get; set; }
        public String Banco { get; set; }
        public String MonedaSimbol { get; set; }
        public string Cuenta { get; set; }
        public decimal SaldoSegunBanco { get; set; }
        /// <summary>
        /// (-) Cheques Flotantes
        /// </summary>
        public decimal ChequeFlotantes { get; set; }
        /// <summary>
        /// (+) Diferencias en Cheques de Menos en el Banco
        /// </summary>
        public decimal DifChequesDeMenosBanco{ get; set; }
        /// <summary>
        /// (-) Diferencias en Cheques de Mas en el banco
        /// </summary>
        public decimal DifChequesDeMasBanco { get; set; }
        /// <summary>
        /// (+) Cheques no perteneciente a nuestra cuenta
        /// </summary>
        public decimal ChequesNoSonDelaCuenta { get; set; }
        /// <summary>
        /// (+) Notas de Debito no registradas en nuestros libros
        /// </summary>
        public decimal NDNoRegistradasEnLibro{ get; set; }
        /// <summary>
        /// (-) Notas de Credito no registradas en nuestro libro
        /// </summary>
        public decimal NCNoRegistradasEnLibro { get; set; }
        /// <summary>
        /// (-) Notas de Debito no Registradas por el Banco
        /// </summary>
        public decimal NDNoRegistradasEnBanco { get; set; }
        /// <summary>
        /// (+) Notas de Credito no Registradas por el Banco
        /// </summary>
        public decimal NCNoRegistradasEnBanco { get; set; }
        /// <summary>
        /// (-) Depositos no registradas en nuestros libros
        /// </summary>
        public decimal DPNoRegistradasEnLibro { get; set; }
        /// <summary>
        /// (+) Depositos no registrado por el banco
        /// </summary>
        public decimal DPNoRegistradasEnBanco { get; set; }
        /// <summary>
        /// (-) Debitos por Correciones internas
        /// </summary>
        public decimal DeditosPorCorreccionesIntMenos { get; set; }
        /// <summary>
        /// (+) Creditos por correcciones internas
        /// </summary>
        public decimal CreditosPorCorreccionesIntMas { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal SaldoSegunLibro { get; set; }
    }
}

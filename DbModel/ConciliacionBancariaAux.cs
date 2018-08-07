using System;
using System.Collections.Generic;

namespace mvcIpsa.DbModel
{
    public partial class ConciliacionBancariaAux
    {
        public int Id { get; set; }
        public DateTime Fecha { get; set; }
        public string Referencia { get; set; }
        public int TipoMovimientoId { get; set; }
        public decimal Debito { get; set; }
        public decimal Credito { get; set; }
        public int EstadoId { get; set; }
        public string Uuid { get; set; }
        public int ProcesoBancoId { get; set; }
        public bool Conciliado { get; set; }
        /// <summary>
        /// 1 si es de caja y 2 si es de banco para saber donde actualizaremos el regostro cuando se guarde la conciliacion
        /// </summary>            
        public int TableInfo { get; set; }
        /// <summary>
        /// El Id pertenecienta a la tabla de origen, el id de ingresos_egresos_banco y para caja la de detalle ingresos_ingresos_caja_referencias
        /// </summary>            
        public int IdOrigen { get; set; }
        /// <summary>
        /// Id de la tabla maestra, si es para banco es la misma, si es par caja es la de ingresos_egresos_caja_id
        /// </summary>
        public int IdRef { get; set; }

        public ConciliacionBancariaEstado Estado { get; set; }
        public ProcesoBanco ProcesoBanco { get; set; }
        public TipoMovimiento TipoMovimiento { get; set; }
    }
}

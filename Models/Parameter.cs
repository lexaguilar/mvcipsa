using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace mvcIpsa.Models
{
    /// <summary>
    /// Clase base
    /// </summary>
    public class Parameter
    {
        /// <summary>
        /// parametro desde
        /// </summary>
        public DateTime desde { get; set; }

        /// <summary>
        /// parametro hasta
        /// </summary>
        public DateTime hasta { get; set; }
    }
    /// <summary>
    /// 
    /// </summary>
    public class ParameterCaja : Parameter
    {
        /// <summary>
        /// parametro codigo de la caja
        /// </summary>
        public int[] CajaIds { get; set; }
        public SearchType SearchType { get; set; }
    }

    public enum SearchType { None, Diario, Mes }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace mvcIpsa
{
    ///
    ///
    public class AppSettings
    {
        /// <summary>
        /// url donde se guardaran los logs
        /// </summary>
        public string url { get; set; }
        /// <summary>
        /// Variable bandera para saber si las referencias ingresadas seran unicamente de tipo numerico
        /// </summary>
        public bool onlyNumber { get; set; }
        /// <summary>
        /// Variable para saber si vamos a registros los datos de que se envian en la peticion
        /// </summary>
        public bool readLog { get; set; }
    }
}
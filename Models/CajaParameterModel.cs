using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace mvcIpsa.Models
{
    public class PaginationParamModel
    {
        public int Page { get; set; }
        public int Count { get; set; }
        public int Rows { get; set; }
    }

    public class CajaParameterModel : PaginationParamModel
    {
        // public int PolicyId { get; set; }
        public int? estado { get; set; }
        public DateTime Desde { get; set; }
        public DateTime Hasta { get; set; }
       
    }

    public class PaginationResult<T> : PaginationParamModel where T : class
    {
        public T Result { get; set; }


    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace mvcIpsa.Models
{
    public class BancoCuentaViewModel
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public string Banco { get; set; }
        public string Moneda { get; set; }
    }
}

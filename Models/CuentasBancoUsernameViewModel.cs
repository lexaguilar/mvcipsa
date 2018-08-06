using mvcIpsa.DbModel;
using mvcIpsa.DbModelIPSA;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace mvcIpsa.Models
{
    public class CuentasBancoUsernameViewModel
    {
        [Required]
        public string username { get; set; }
        public int[] IdsToAdd { get; set; }
        public int[] IdsToDelete { get; set; }
    }

    public class CuentasBancoEditModel
    {
        public Profile profile { get; set; }
        public IEnumerable<BancosCuentas> Members { get; set; }
        public IEnumerable<BancosCuentas> NonMembers { get; set; }
    }
}

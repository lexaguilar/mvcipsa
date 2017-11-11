using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace mvcIpsa.Models
{
    public class CajaCtaContablesViewModel
    {
        public int idCaja { get; set; }
        public string description { get; set; }
        public IEnumerable<MaestroContableViewModel> MaestroContableMembers { get; set; }
        //public IEnumerable<MaestroContableViewModel> MaestroContableNoMenbers { get; set; }

    }
    public class CajaCtaContableModificationModel
    {
        [Required]
        public int id { get; set; }
        public string[] AccountsToAdd { get; set; }
        public string[] AccountsToDelete { get; set; }
    }
    public class MaestroContableViewModel
    {
        public string Padre { get; set; }
        public string Cuenta { get; set; }
        public string Nombre { get; set; }
        public string CtaContable { get; set; }

    }
}

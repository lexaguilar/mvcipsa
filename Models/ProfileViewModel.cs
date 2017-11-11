using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace mvcIpsa.Models
{
    using mvcIpsa.DbModel;
    using System.ComponentModel.DataAnnotations;

    public class ProfileViewModel
    {
        public string Username { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string Correo { get; set; }
        public short Nestado { get; set; }
        public short Ncentrocosto { get; set; }
        public string centrocostoDescripcion { get; set; }
        public int idCaja { get; set; }        
        public string cajaDescripcion { get; set; }
    }

    public class ProfileEditModel
    {
        public Profile profile { get; set; }
        public IEnumerable<Role> Members { get; set; }
        public IEnumerable<Role> NonMembers { get; set; }
    }

    public class ProfileModificationModel
    {
        [Required]
        public string username { get; set; }
        public int[] IdsToAdd { get; set; }
        public int[] IdsToDelete { get; set; }
    }
}

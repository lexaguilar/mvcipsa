using System;
using System.Collections.Generic;

namespace mvcIpsa.DbModel
{
    public partial class Caja
    {
        public Caja()
        {
            Profile = new HashSet<Profile>();
        }

        public short Ncaja { get; set; }
        public string Descripcion { get; set; }

        public ICollection<Profile> Profile { get; set; }
    }
}

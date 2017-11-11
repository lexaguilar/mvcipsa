using System;
using System.Collections.Generic;

namespace mvcIpsa.DbModel
{
    public partial class Role
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public short? Nestado { get; set; }
    }
}

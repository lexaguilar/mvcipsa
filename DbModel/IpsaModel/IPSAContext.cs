using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
namespace mvcIpsa.DbModel
{
    partial class IPSAContext
    {
        public IPSAContext(DbContextOptions<IPSAContext> options) : base(options)
        {
        }
    }
}

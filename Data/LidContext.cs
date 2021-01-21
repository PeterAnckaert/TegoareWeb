using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TegoareWeb.Models;

namespace TegoareWeb.Data
{
    public class LidContext : DbContext
    {
        public LidContext(DbContextOptions<LidContext> options)
            : base(options)
        {
        }

        public DbSet<Lid> Lid { get; set; }
    }
}

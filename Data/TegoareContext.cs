using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TegoareWeb.Models;

namespace TegoareWeb.Data
{
    public class TegoareContext : DbContext
    {
        public TegoareContext(DbContextOptions<TegoareContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Lid>().ToTable("Lid");

            modelBuilder.Entity<Groep>().ToTable("Groep");

            modelBuilder.Entity<Ontmoetingsplaats>().ToTable("Ontmoetingsplaats");

            modelBuilder.Entity<Tijdstip>().ToTable("Tijdstip");

            modelBuilder.Entity<Activiteit>()
                .HasOne<Ontmoetingsplaats>(s => s.Ontmoetingsplaats)
                .WithMany(g => g.Activiteiten)
                .HasForeignKey(s => s.Id_ontmoetingsplaats);
        }

        public DbSet<Lid> Leden { get; set; }
        public DbSet<Groep> Groepen { get; set; }
        public DbSet<Ontmoetingsplaats> Ontmoetingsplaatsen { get; set; }
        public DbSet<Tijdstip> Tijdstippen { get; set; }
        public DbSet<Activiteit> Activiteit { get; set; }
    }
}

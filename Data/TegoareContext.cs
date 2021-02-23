﻿using Microsoft.EntityFrameworkCore;
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

            modelBuilder.Entity<Activiteit>().ToTable("Activiteit");

            modelBuilder.Entity<Relatie>().ToTable("Relatie");

            modelBuilder.Entity<Activiteit>()
               .HasOne<Ontmoetingsplaats>(o => o.Ontmoetingsplaats)
               .WithMany(a => a.Activiteiten)
               .HasForeignKey(o => o.Id_ontmoetingsplaats);

            modelBuilder.Entity<Relatie>()
                .HasOne<Lid>(r => r.Lid1)
                .WithMany(l => l.Relaties1)
                .HasForeignKey(r => r.Id_Lid1);

            modelBuilder.Entity<Relatie>()
                .HasOne<Groep>(r => r.Groep)
                .WithMany(g => g.Relaties)
                .HasForeignKey(r => r.Id_Groep);

            modelBuilder.Entity<Relatie>()
                .HasOne<Lid>(r => r.Lid2)
                .WithMany(l => l.Relaties2)
                .HasForeignKey(r => r.Id_Lid2);

        }

        public DbSet<Lid> Leden { get; set; }
        public DbSet<Groep> Groepen { get; set; }
        public DbSet<Ontmoetingsplaats> Ontmoetingsplaatsen { get; set; }
        public DbSet<Activiteit> Activiteiten { get; set; }
        public DbSet<Relatie> Relaties { get; set; }
    }
}

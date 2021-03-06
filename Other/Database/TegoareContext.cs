﻿using Microsoft.EntityFrameworkCore;
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

            modelBuilder.Entity<Inschrijving>().ToTable("Inschrijving");

            // een activiteit heeft één ontmoetingsplaats
            // maar meerdere activiteiten kunnen doorgaan in dezelfde ontmoetingsplaats
            modelBuilder.Entity<Activiteit>()
               .HasOne(o => o.Ontmoetingsplaats)
               .WithMany(a => a.Activiteiten)
               .HasForeignKey(o => o.Id_ontmoetingsplaats);

            // een relatie heeft één eerste lid
            // maar meerdere relaties kunnen hetzelfde eerste lid hebben
            modelBuilder.Entity<Relatie>()
                .HasOne(r => r.Lid1)
                .WithMany(l => l.Relaties)
                .HasForeignKey(r => r.Id_Lid1);

            // een relatie heeft een groep (rol)
            // maar meerdere relaties kunnen dezelfde groep hebben
            modelBuilder.Entity<Relatie>()
                .HasOne(r => r.Groep)
                .WithMany(g => g.Relaties)
                .HasForeignKey(r => r.Id_Groep);

        }

        public DbSet<Lid> Leden { get; set; }
        public DbSet<Groep> Groepen { get; set; }
        public DbSet<Ontmoetingsplaats> Ontmoetingsplaatsen { get; set; }
        public DbSet<Activiteit> Activiteiten { get; set; }
        public DbSet<Relatie> Relaties { get; set; }
        public DbSet<Inschrijving> Inschrijvingen { get; set; }
    }
}

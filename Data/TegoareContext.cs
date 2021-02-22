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

            modelBuilder.Entity<Activiteit>().ToTable("Activiteit");

            modelBuilder.Entity<KalenderItem>().ToTable("KalenderItem");

            modelBuilder.Entity<Relatie>().ToTable("Relatie");

            modelBuilder.Entity<Activiteit>()
                .HasOne<Tijdstip>(t => t.Publicatiedatum)
                .WithMany(a => a.Activiteiten_Publicatie)
                .HasForeignKey(t => t.Id_publicatiedatum);

            modelBuilder.Entity<Activiteit>()
                .HasOne<Tijdstip>(t => t.Uiterste_inschrijfdatum)
                .WithMany(a => a.Activiteiten_Inschrijf)
                .HasForeignKey(t => t.Id_uiterste_inschrijfdatum);

            modelBuilder.Entity<Activiteit>()
               .HasOne<Ontmoetingsplaats>(o => o.Ontmoetingsplaats)
               .WithMany(a => a.Activiteiten)
               .HasForeignKey(o => o.Id_ontmoetingsplaats);

            modelBuilder.Entity<KalenderItem>().HasKey(ki => new { ki.Id_activiteit, ki.Id_tijdstip });

            modelBuilder.Entity<KalenderItem>()
                .HasOne<Activiteit>(ki => ki.Activiteit)
                .WithMany(a => a.KalenderItems)
                .HasForeignKey(ki => ki.Id_activiteit);

            modelBuilder.Entity<KalenderItem>()
                .HasOne<Tijdstip>(ki => ki.Tijdstip)
                .WithMany(t => t.KalenderItems)
                .HasForeignKey(ki => ki.Id_tijdstip);

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
        public DbSet<Tijdstip> Tijdstippen { get; set; }
        public DbSet<Activiteit> Activiteiten { get; set; }
        public DbSet<KalenderItem> KalenderItems { get; set; }
        public DbSet<Relatie> Relaties { get; set; }
    }
}

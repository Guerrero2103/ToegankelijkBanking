using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace BankApp_Models
{
    public class AppDbContext : DbContext
    {
        // 🔹 Tabellen (DbSets)
        public DbSet<Gebruiker> Gebruikers { get; set; }
        public DbSet<Rol> Rollen { get; set; }
        public DbSet<Rekening> Rekeningen { get; set; }
        public DbSet<Transactie> Transacties { get; set; }
        public DbSet<Investering> Investeringen { get; set; }
        public DbSet<PortefeuilleItem> Portefeuilles { get; set; }
        public DbSet<Kaart> Kaarten { get; set; }
        public DbSet<Afspraak> Afspraken { get; set; }

        // 🔹 Constructor
        public AppDbContext()
        {
            try
            {
                Database.EnsureCreated();
                Console.WriteLine("✅ Database succesvol aangemaakt of al aanwezig.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Fout bij aanmaken database: {ex.Message}");
            }
        }

        // 🔹 SQLite configuratie
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                string basePath = AppDomain.CurrentDomain.BaseDirectory;
                string solutionPath = Path.GetFullPath(Path.Combine(basePath, @"..\..\..\..\")); // Ga terug naar solution root
                string dbPath = Path.Combine(solutionPath, "BankApp_Models", "bankapp.db");

                Console.WriteLine($"[DB PATH] {dbPath}");
                Directory.CreateDirectory(Path.GetDirectoryName(dbPath)!);

                optionsBuilder.UseSqlite($"Data Source={dbPath}");
            }
        }

        // 🔹 Modelconfiguratie + seeding
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // === RELATIES ===
            modelBuilder.Entity<Rekening>()
                .HasOne(r => r.Gebruiker)
                .WithMany(g => g.Rekeningen)
                .HasForeignKey(r => r.GebruikerId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Kaart>()
                .HasOne(k => k.Gebruiker)
                .WithMany(g => g.Kaarten)
                .HasForeignKey(k => k.GebruikerId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Afspraak>()
                .HasOne(a => a.Gebruiker)
                .WithMany(g => g.Afspraken)
                .HasForeignKey(a => a.GebruikerId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<PortefeuilleItem>()
                .HasOne(p => p.Gebruiker)
                .WithMany(g => g.Portefeuille)
                .HasForeignKey(p => p.GebruikerId);

            modelBuilder.Entity<PortefeuilleItem>()
                .HasOne(p => p.Investering)
                .WithMany(i => i.Portefeuilles)
                .HasForeignKey(p => p.InvesteringId);

            // === ROLLEN SEEDEN ===
            modelBuilder.Entity<Rol>().HasData(
                new Rol { Id = 1, Naam = "Klant", Beschrijving = "Standaard bankgebruiker" },
                new Rol { Id = 2, Naam = "Medewerker", Beschrijving = "Bankmedewerker met extra rechten" },
                new Rol { Id = 3, Naam = "Beheerder", Beschrijving = "Volledige toegang tot het systeem" }
            );

            // === GEBRUIKERS SEEDEN ===
            modelBuilder.Entity<Gebruiker>().HasData(
                new Gebruiker
                {
                    Id = 1,
                    Email = "jan.peeters@example.com",
                    WachtwoordHash = "hashed_pw_123",
                    Telefoonnummer = "0478123456",
                    Geboortedatum = new DateTime(1990, 4, 15),
                    Straatnaam = "Kerkstraat",
                    Huisnummer = "12",
                    Bus = "A",
                    Postcode = "2000",
                    Gemeente = "Antwerpen",
                    Land = "België",
                    RolId = 1
                },
                new Gebruiker
                {
                    Id = 2,
                    Email = "sarah.janssens@example.com",
                    WachtwoordHash = "hashed_pw_456",
                    Telefoonnummer = "0498765432",
                    Geboortedatum = new DateTime(1985, 10, 2),
                    Straatnaam = "Stationslaan",
                    Huisnummer = "45",
                    Bus = null,
                    Postcode = "3000",
                    Gemeente = "Leuven",
                    Land = "België",
                    RolId = 1
                },
                new Gebruiker
                {
                    Id = 3,
                    Email = "beheerder@bankapp.local",
                    WachtwoordHash = "admin_pw_789",
                    Telefoonnummer = "0412345678",
                    Geboortedatum = new DateTime(1975, 6, 25),
                    Straatnaam = "Marktplein",
                    Huisnummer = "1",
                    Postcode = "1000",
                    Gemeente = "Brussel",
                    Land = "België",
                    RolId = 3
                }
            );

            // === REKENINGEN SEEDEN ===
            modelBuilder.Entity<Rekening>().HasData(
                new Rekening { Id = 1, Iban = "BE12345678901234", Type = RekeningType.Zicht, Saldo = 1500.50m, GebruikerId = 1 },
                new Rekening { Id = 2, Iban = "BE98765432109876", Type = RekeningType.Spaar, Saldo = 8000m, GebruikerId = 1 },
                new Rekening { Id = 3, Iban = "BE11223344556677", Type = RekeningType.Zicht, Saldo = 2300.75m, GebruikerId = 2 }
            );

            // === TRANSACTIES SEEDEN ===
            modelBuilder.Entity<Transactie>().HasData(
                new Transactie
                {
                    Id = 1,
                    VanIban = "BE12345678901234",
                    NaarIban = "BE11223344556677",
                    NaamOntvanger = "Sarah Janssens",
                    Bedrag = 50.00m,
                    Omschrijving = "Cadeau",
                    Datum = new DateTime(2024, 5, 10),
                    GebruikerId = 1
                },
                new Transactie
                {
                    Id = 2,
                    VanIban = "BE98765432109876",
                    NaarIban = "BE12345678901234",
                    NaamOntvanger = "Jan Peeters",
                    Bedrag = 200.00m,
                    Omschrijving = "Spaargeld overboeking",
                    Datum = new DateTime(2024, 6, 2),
                    GebruikerId = 1
                }
            );

            // === KAARTEN SEEDEN ===
            modelBuilder.Entity<Kaart>().HasData(
                new Kaart { Id = 1, KaartNummer = "1111-2222-3333-4444", Status = KaartStatus.Actief, GebruikerId = 1 },
                new Kaart { Id = 2, KaartNummer = "5555-6666-7777-8888", Status = KaartStatus.Bevroren, GebruikerId = 2 }
            );

            // === INVESTERINGEN SEEDEN ===
            modelBuilder.Entity<Investering>().HasData(
                new Investering { Id = 1, Bedrijfsnaam = "TechNova", PrijsPerAandeel = 120.5m, Hoog = 130.0m, Laag = 100.0m },
                new Investering { Id = 2, Bedrijfsnaam = "GreenPower", PrijsPerAandeel = 45.8m, Hoog = 52.0m, Laag = 40.0m }
            );

            // === PORTEFEUILLE SEEDEN ===
            modelBuilder.Entity<PortefeuilleItem>().HasData(
                new PortefeuilleItem { Id = 1, GebruikerId = 1, InvesteringId = 1, Aantal = 10, TotalePrijs = 1205.0m },
                new PortefeuilleItem { Id = 2, GebruikerId = 1, InvesteringId = 2, Aantal = 20, TotalePrijs = 916.0m }
            );

            // === AFSPRAKEN SEEDEN ===
            modelBuilder.Entity<Afspraak>().HasData(
                new Afspraak { Id = 1, GebruikerId = 1, Datum = new DateTime(2024, 11, 5, 14, 0, 0), Onderwerp = "Hypotheekadvies", Status = AfspraakStatus.Goedgekeurd },
                new Afspraak { Id = 2, GebruikerId = 2, Datum = new DateTime(2024, 11, 7, 10, 30, 0), Onderwerp = "Investering bespreken", Status = AfspraakStatus.InAfwachting }
            );
        }
    }
}


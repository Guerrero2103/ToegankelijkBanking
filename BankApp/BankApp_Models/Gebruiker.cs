using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BankApp_Models
{
    public class Gebruiker
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required, EmailAddress, MaxLength(255)]
        public string Email { get; set; }

        [Required, MaxLength(255)]
        public string WachtwoordHash { get; set; }

        [MaxLength(20)]
        public string Telefoonnummer { get; set; }

        [Required]
        public DateTime Geboortedatum { get; set; }

        [MaxLength(100)]
        public string Straatnaam { get; set; }

        [MaxLength(10)]
        public string Huisnummer { get; set; }

        [MaxLength(10)]
        public string? Bus { get; set; }

        [MaxLength(10)]
        public string Postcode { get; set; }

        [MaxLength(100)]
        public string Gemeente { get; set; }

        [MaxLength(100)]
        public string Land { get; set; }

        // 🔗 Relatie met Rol
        public int? RolId { get; set; }

        [ForeignKey(nameof(RolId))]
        public Rol? Rol { get; set; }

        public bool IsActief { get; set; } = true;

        // 🔹 Navigatie-eigenschappen (relaties met andere tabellen)
        public ICollection<Rekening> Rekeningen { get; set; } = new List<Rekening>();
        public ICollection<Kaart> Kaarten { get; set; } = new List<Kaart>();
        public ICollection<Afspraak> Afspraken { get; set; } = new List<Afspraak>();
        public ICollection<PortefeuilleItem> Portefeuille { get; set; } = new List<PortefeuilleItem>();
        public ICollection<Transactie> Transacties { get; set; } = new List<Transactie>();
    }
}
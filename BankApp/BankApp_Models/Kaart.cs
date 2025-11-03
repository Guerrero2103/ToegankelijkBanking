using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.ComponentModel.DataAnnotations;

namespace BankApp_Models
{
    public enum KaartStatus
    {
        Actief,
        Bevroren,
        Geblokkeerd
    }

    public class Kaart
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string KaartNummer { get; set; } = string.Empty;

        [Required]
        public KaartStatus Status { get; set; }

        // 🔹 Foreign key
        public int GebruikerId { get; set; }
        public Gebruiker Gebruiker { get; set; }
    }
}


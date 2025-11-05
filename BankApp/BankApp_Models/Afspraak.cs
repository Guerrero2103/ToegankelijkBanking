using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.ComponentModel.DataAnnotations;

namespace BankApp_Models
{
    public enum AfspraakStatus
    {
        InAfwachting,
        Goedgekeurd,
        Geannuleerd
    }

    public class Afspraak
    {
        [Key]
        public int Id { get; set; }

        public int GebruikerId { get; set; }
        public Gebruiker Gebruiker { get; set; }

        public DateTime Datum { get; set; }

        [Required]
        public string Onderwerp { get; set; } = string.Empty;

        [Required]
        public AfspraakStatus Status { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BankApp_Models
{
    public class Transactie
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string VanIban { get; set; } = string.Empty;

        [Required]
        public string NaarIban { get; set; } = string.Empty;

        public string NaamOntvanger { get; set; } = string.Empty;

        [Column(TypeName = "decimal(18,2)")]
        public decimal Bedrag { get; set; }

        public string Omschrijving { get; set; } = string.Empty;

        public DateTime Datum { get; set; }

        // 🔹 Optionele FK naar Gebruiker (wie voerde uit)
        public int? GebruikerId { get; set; }
        public Gebruiker? Gebruiker { get; set; }
    }
}


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BankApp_Models
{
    public enum RekeningType
    {
        Zicht,
        Spaar
    }

    public class Rekening
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(34)]
        public string Iban { get; set; } = string.Empty;

        [Required]
        public RekeningType Type { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Saldo { get; set; }

        // 🔹 Foreign key
        public int GebruikerId { get; set; }
        public Gebruiker Gebruiker { get; set; }

        // 🔹 Relatie met Transacties
        public ICollection<Transactie> TransactiesVan { get; set; } = new List<Transactie>();
        public ICollection<Transactie> TransactiesNaar { get; set; } = new List<Transactie>();
    }
}

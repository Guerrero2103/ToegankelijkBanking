using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankApp_Models
{
    public class Rol
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Naam { get; set; }

        [MaxLength(255)]
        public string? Beschrijving { get; set; }

        // 🔁 Eén rol kan meerdere gebruikers hebben
        public ICollection<Gebruiker> Gebruikers { get; set; } = new List<Gebruiker>();
    }
}

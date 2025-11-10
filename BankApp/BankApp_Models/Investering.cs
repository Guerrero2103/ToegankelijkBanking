using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BankApp_Models
{
    public class Investering
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Bedrijfsnaam { get; set; } = string.Empty;

        [Column(TypeName = "decimal(18,2)")]
        public decimal PrijsPerAandeel { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Hoog { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Laag { get; set; }

        public ICollection<PortefeuilleItem> Portefeuilles { get; set; } = new List<PortefeuilleItem>();
    }
}


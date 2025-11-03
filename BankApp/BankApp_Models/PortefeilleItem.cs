using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BankApp_Models
{
    public class PortefeuilleItem
    {
        [Key]
        public int Id { get; set; }

        public int GebruikerId { get; set; }
        public Gebruiker Gebruiker { get; set; }

        public int InvesteringId { get; set; }
        public Investering Investering { get; set; }

        public int Aantal { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalePrijs { get; set; }
    }
}


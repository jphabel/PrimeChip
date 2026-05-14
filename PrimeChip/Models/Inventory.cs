using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace PrimeChip.Models
{
    public class Inventory
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string SKU { get; set; }
        [Required]
        public string Category { get; set; }
        [Required]
        public int InitialStock { get; set; }

        [Required]
        public int Stock { get; set; }

        [Required]
        public int ReorderLevel { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        [Required]
        public decimal UnitPrice { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;


    }
}

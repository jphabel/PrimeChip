using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace PrimeChip.Models
{
    public class Sale
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public int InventoryId { get; set; }
        [Required]
        public Inventory? Inventory { get; set; }
        [Required]
        public string ProductName { get; set; }
        [Required]
        public string SKU { get; set; }
        [Required]
        public int Quantity { get; set; }
        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal UnitPrice { get; set; }
        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalAmount { get; set; }
        [Required]
        public string Type { get; set; }
        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}

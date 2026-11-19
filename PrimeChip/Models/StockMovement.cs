using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PrimeChip.Models
{
    public class StockMovement
    {
        public int Id { get; set; }

        [Required]
        public int InventoryId { get; set; }
        public Inventory Inventory { get; set; } = null!;

        [Required]
        public string ProductName { get; set; } = string.Empty;

        [Required]
        public string Category { get; set; } = string.Empty;

        public int? VendorId { get; set; }
        public Vendor? Vendor { get; set; }

        [Range(1, int.MaxValue)]
        public int Quantity { get; set; }

        [Required, StringLength(20)]
        public string MovementType { get; set; } = string.Empty;

        [StringLength(40)]
        public string? Reason { get; set; }

        [StringLength(500)]
        public string? Note { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal UnitPrice { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public bool IsVoided { get; set; }
        public DateTime? VoidedAt { get; set; }
    }
}

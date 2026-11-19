using System.ComponentModel.DataAnnotations;

namespace PrimeChip.Models
{
    public class StockInViewModel
    {
        [Required]
        public string Category { get; set; } = string.Empty;

        [Required, StringLength(100)]
        public string ProductName { get; set; } = string.Empty;

        [StringLength(100)]
        public string? SKU { get; set; }

        [Range(typeof(decimal), "0", "999999999")]
        public decimal? UnitPrice { get; set; }

        [Range(0, int.MaxValue)]
        public int ReorderLevel { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Select a vendor.")]
        public int VendorId { get; set; }

        [Range(1, int.MaxValue)]
        public int Quantity { get; set; }

        [DataType(DataType.Date)]
        public DateTime Date { get; set; } = DateTime.Today;

        [StringLength(500)]
        public string? Note { get; set; }

        public List<string> Categories { get; set; } = [];
        public List<Vendor> Vendors { get; set; } = [];
        public List<StockMovement> RecentMovements { get; set; } = [];
    }

    public class StockOutViewModel
    {
        [Required]
        public string Category { get; set; } = string.Empty;

        [Required, StringLength(100)]
        public string ProductName { get; set; } = string.Empty;

        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be greater than zero.")]
        public int Quantity { get; set; }

        [Required]
        public string Reason { get; set; } = "Sold";

        [DataType(DataType.Date)]
        public DateTime Date { get; set; } = DateTime.Today;

        [StringLength(500)]
        public string? Note { get; set; }

        public DateTime FilterDate { get; set; } = DateTime.Today;
        public int UnitsSoldToday { get; set; }
        public decimal EstimatedSalesValueToday { get; set; }
        public int TransactionsToday { get; set; }
        public List<string> Categories { get; set; } = [];
        public List<StockMovement> Movements { get; set; } = [];
    }

    public class StockOutReportViewModel
    {
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }

        [DataType(DataType.Date)]
        public DateTime EndDate { get; set; }

        public int UnitsSold { get; set; }
        public decimal EstimatedSalesValue { get; set; }
        public int Transactions { get; set; }
        public List<StockMovement> Movements { get; set; } = [];
    }

    public static class InventoryCategories
    {
        public static readonly string[] Defaults = ["Storage", "Memory", "Processor", "VideoCard", "Motherboard", "Peripherals"];
    }

    public class DashboardViewModel
    {
        public int TotalProducts { get; set; }
        public int TotalStock { get; set; }
        public int LowStock { get; set; }
        public int OutOfStock { get; set; }
        public decimal TotalInventoryValue { get; set; }
        public int UnitsSoldToday { get; set; }
        public decimal SalesValueToday { get; set; }
        public int StockOutsToday { get; set; }
        public List<string> ChartLabels { get; set; } = [];
        public List<int> ChartQty { get; set; } = [];
        public List<decimal> ChartValue { get; set; } = [];
        public List<Inventory> Alerts { get; set; } = [];
        public List<StockMovement> RecentMovements { get; set; } = [];
        public List<Inventory> TopStockItems { get; set; } = [];
    }
}

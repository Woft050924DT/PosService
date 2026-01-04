using System;

namespace PosService.DTO
{
    public class InventoryDTO
    {
        public int ProductId { get; set; }
        public string ProductCode { get; set; } = null!;
        public string? Barcode { get; set; }
        public string ProductName { get; set; } = null!;
        public int? CategoryId { get; set; }
        public string? CategoryName { get; set; }
        public int? SupplierId { get; set; }
        public string? SupplierName { get; set; }
        public string? Unit { get; set; }
        public decimal? CostPrice { get; set; }
        public decimal SellingPrice { get; set; }
        public int? StockQuantity { get; set; }
        public int? MinStock { get; set; }
        public string? ImageUrl { get; set; }
        public bool? IsActive { get; set; }
        public DateTime? CreatedAt { get; set; }
    }
    public class ProductStockDto
    {
        public int ProductId { get; set; }
        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        public int StockQuantity { get; set; }
        public int MinStock { get; set; }
        public string Status { get; set; }
        public DateTime LastUpdated { get; set; }
    }
}


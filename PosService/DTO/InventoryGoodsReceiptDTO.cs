using System.Collections.Generic;

namespace PosService.DTO
{
    public class InventoryGoodsReceiptItemDTO
    {
        public int? ProductId { get; set; }
        public string? ProductCode { get; set; }
        public string? ProductName { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal? LineTotal { get; set; }
    }

    public class InventoryGoodsReceiptDTO
    {
        public string? ReferenceType { get; set; }
        public int? ReferenceId { get; set; }
        public int? UserId { get; set; }
        public string? Notes { get; set; }
        public decimal? TotalAmount { get; set; }
        public List<InventoryGoodsReceiptItemDTO> Items { get; set; } = new List<InventoryGoodsReceiptItemDTO>();
    }

    public class InventoryGoodsReceiptItemResultDTO
    {
        public int ProductId { get; set; }
        public string ProductCode { get; set; } = null!;
        public string ProductName { get; set; } = null!;
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal LineTotal { get; set; }
        public int QuantityBefore { get; set; }
        public int QuantityAfter { get; set; }
    }

    public class InventoryGoodsReceiptResultDTO
    {
        public string? ReferenceType { get; set; }
        public int? ReferenceId { get; set; }
        public int? UserId { get; set; }
        public decimal TotalAmount { get; set; }
        public List<InventoryGoodsReceiptItemResultDTO> Items { get; set; } = new List<InventoryGoodsReceiptItemResultDTO>();
    }
}


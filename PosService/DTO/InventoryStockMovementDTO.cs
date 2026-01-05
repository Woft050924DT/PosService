using System.Collections.Generic;

namespace PosService.DTO
{
    public class InventoryStockItemDTO
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
    }

    public class InventoryStockMovementDTO
    {
        public string? ReferenceType { get; set; }
        public int? ReferenceId { get; set; }
        public int? UserId { get; set; }
        public string? Notes { get; set; }
        public List<InventoryStockItemDTO> Items { get; set; } = new List<InventoryStockItemDTO>();
    }

    public class InventoryStockMovementItemResultDTO
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public int QuantityBefore { get; set; }
        public int QuantityAfter { get; set; }
    }
}


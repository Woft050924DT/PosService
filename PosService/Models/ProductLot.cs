using System;
using System.Collections.Generic;

namespace PosService.Models;

public partial class ProductLot
{
    public int LotId { get; set; }

    public int ProductId { get; set; }

    public int? PurchaseDetailId { get; set; }

    public string? LotNumber { get; set; }

    public DateOnly? ExpiryDate { get; set; }

    public int QuantityReceived { get; set; }

    public int QuantityRemaining { get; set; }

    public decimal CostPrice { get; set; }

    public DateOnly? ManufacturedDate { get; set; }

    public virtual Product Product { get; set; } = null!;

    public virtual PurchaseOrderDetail? PurchaseDetail { get; set; }
}

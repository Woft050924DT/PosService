using System;
using System.Collections.Generic;

namespace PosService.Models;

public partial class InventoryAdjustmentDetail
{
    public int DetailId { get; set; }

    public int AdjustmentId { get; set; }

    public int ProductId { get; set; }

    public int QuantityBefore { get; set; }

    public int QuantityChange { get; set; }

    public int QuantityAfter { get; set; }

    public decimal UnitCost { get; set; }

    public decimal? ValueDifference { get; set; }

    public virtual InventoryAdjustment Adjustment { get; set; } = null!;

    public virtual Product Product { get; set; } = null!;
}

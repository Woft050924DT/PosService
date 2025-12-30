using System;
using System.Collections.Generic;

namespace PosService.Models;

public partial class InventoryAdjustment
{
    public int AdjustmentId { get; set; }

    public string AdjustmentNumber { get; set; } = null!;

    public DateTime? AdjustmentDate { get; set; }

    public string Reason { get; set; } = null!;

    public decimal? TotalValueDifference { get; set; }

    public int CreatedBy { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual User CreatedByNavigation { get; set; } = null!;

    public virtual ICollection<InventoryAdjustmentDetail> InventoryAdjustmentDetails { get; set; } = new List<InventoryAdjustmentDetail>();
}

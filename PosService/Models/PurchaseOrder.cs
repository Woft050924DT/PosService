using System;
using System.Collections.Generic;

namespace PosService.Models;

public partial class PurchaseOrder
{
    public int PurchaseId { get; set; }

    public string PurchaseNumber { get; set; } = null!;

    public DateTime? PurchaseDate { get; set; }

    public int? SupplierId { get; set; }

    public int? UserId { get; set; }

    public decimal? TotalAmount { get; set; }

    public decimal? PaidAmount { get; set; }

    public string? Status { get; set; }

    public string? Notes { get; set; }

    public virtual ICollection<PurchaseOrderDetail> PurchaseOrderDetails { get; set; } = new List<PurchaseOrderDetail>();

    public virtual Supplier? Supplier { get; set; }

    public virtual User? User { get; set; }
}

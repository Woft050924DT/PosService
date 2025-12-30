using System;
using System.Collections.Generic;

namespace PosService.Models;

public partial class PurchaseReturn
{
    public int ReturnId { get; set; }

    public string ReturnNumber { get; set; } = null!;

    public DateTime? ReturnDate { get; set; }

    public int? OriginalPurchaseId { get; set; }

    public int SupplierId { get; set; }

    public int UserId { get; set; }

    public decimal? TotalReturnAmount { get; set; }

    public string? Reason { get; set; }

    public string? Status { get; set; }

    public string? Notes { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual PurchaseOrder? OriginalPurchase { get; set; }

    public virtual ICollection<PurchaseReturnDetail> PurchaseReturnDetails { get; set; } = new List<PurchaseReturnDetail>();

    public virtual Supplier Supplier { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}

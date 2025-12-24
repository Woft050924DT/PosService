using System;
using System.Collections.Generic;

namespace DAL.Models;

public partial class InventoryTransaction
{
    public int TransactionId { get; set; }

    public int? ProductId { get; set; }

    public string? TransactionType { get; set; }

    public string? ReferenceType { get; set; }

    public int? ReferenceId { get; set; }

    public int Quantity { get; set; }

    public int? QuantityBefore { get; set; }

    public int? QuantityAfter { get; set; }

    public string? Notes { get; set; }

    public DateTime? CreatedAt { get; set; }

    public int? CreatedBy { get; set; }

    public virtual User? CreatedByNavigation { get; set; }

    public virtual Product? Product { get; set; }
}

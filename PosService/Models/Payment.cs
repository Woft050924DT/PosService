using System;
using System.Collections.Generic;

namespace PosService.Models;

public partial class Payment
{
    public int PaymentId { get; set; }

    public DateTime? PaymentDate { get; set; }

    public string PaymentType { get; set; } = null!;

    public int? CustomerId { get; set; }

    public int? SupplierId { get; set; }

    public int? RelatedInvoiceId { get; set; }

    public int? RelatedPurchaseId { get; set; }

    public decimal Amount { get; set; }

    public string PaymentMethod { get; set; } = null!;

    public string? ReferenceNumber { get; set; }

    public string? Notes { get; set; }

    public int CreatedBy { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual User CreatedByNavigation { get; set; } = null!;

    public virtual Customer? Customer { get; set; }

    public virtual Supplier? Supplier { get; set; }
}

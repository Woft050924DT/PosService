using System;
using System.Collections.Generic;

namespace PosService.Models;

public partial class SalesInvoice
{
    public int InvoiceId { get; set; }

    public string InvoiceNumber { get; set; } = null!;

    public DateTime? InvoiceDate { get; set; }

    public int? CustomerId { get; set; }

    public int? UserId { get; set; }

    public decimal? SubTotal { get; set; }

    public decimal? Discount { get; set; }

    public decimal? TotalAmount { get; set; }

    public decimal? PaidAmount { get; set; }

    public string? PaymentMethod { get; set; }

    public string? Status { get; set; }

    public string? Notes { get; set; }

    public int? PromotionId { get; set; }

    public decimal? PromotionDiscount { get; set; }

    public virtual Customer? Customer { get; set; }

    public virtual Promotion? Promotion { get; set; }

    public virtual ICollection<SalesInvoiceDetail> SalesInvoiceDetails { get; set; } = new List<SalesInvoiceDetail>();

    public virtual ICollection<SalesReturn> SalesReturns { get; set; } = new List<SalesReturn>();

    public virtual User? User { get; set; }
}

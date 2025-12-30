using System;
using System.Collections.Generic;

namespace PosService.Models;

public partial class SalesReturn
{
    public int ReturnId { get; set; }

    public string ReturnNumber { get; set; } = null!;

    public DateTime? ReturnDate { get; set; }

    public int? OriginalInvoiceId { get; set; }

    public int? CustomerId { get; set; }

    public int UserId { get; set; }

    public decimal? TotalReturnAmount { get; set; }

    public decimal? RefundAmount { get; set; }

    public string? Reason { get; set; }

    public string? Status { get; set; }

    public string? Notes { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual Customer? Customer { get; set; }

    public virtual SalesInvoice? OriginalInvoice { get; set; }

    public virtual ICollection<SalesReturnDetail> SalesReturnDetails { get; set; } = new List<SalesReturnDetail>();

    public virtual User User { get; set; } = null!;
}

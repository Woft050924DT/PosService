using System;
using System.Collections.Generic;

namespace PosService.Models;

public partial class Taxis
{
    public int TaxId { get; set; }

    public string TaxCode { get; set; } = null!;

    public string TaxName { get; set; } = null!;

    public decimal TaxRate { get; set; }

    public string? Description { get; set; }

    public bool? IsActive { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual ICollection<SalesInvoiceDetail> SalesInvoiceDetails { get; set; } = new List<SalesInvoiceDetail>();
}

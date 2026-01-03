using System;
using System.Collections.Generic;

namespace PosService.Models;

public partial class SalesInvoiceDetail
{
    public int DetailId { get; set; }

    public int? InvoiceId { get; set; }

    public int? ProductId { get; set; }

    public int Quantity { get; set; }

    public decimal UnitPrice { get; set; }

    public decimal? Discount { get; set; }

    public decimal LineTotal { get; set; }

    public int? TaxId { get; set; }

    public decimal? TaxAmount { get; set; }

    public decimal? LinePromotionDiscount { get; set; }

    public virtual SalesInvoice? Invoice { get; set; }

    public virtual Product? Product { get; set; }

    public virtual Taxis? Tax { get; set; }
}

using System;
using System.Collections.Generic;

namespace DAL.Models;

public partial class SalesInvoiceDetail
{
    public int DetailId { get; set; }

    public int? InvoiceId { get; set; }

    public int? ProductId { get; set; }

    public int Quantity { get; set; }

    public decimal UnitPrice { get; set; }

    public decimal? Discount { get; set; }

    public decimal LineTotal { get; set; }

    public virtual SalesInvoice? Invoice { get; set; }

    public virtual Product? Product { get; set; }
}

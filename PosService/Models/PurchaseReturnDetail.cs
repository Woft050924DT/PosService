using System;
using System.Collections.Generic;

namespace PosService.Models;

public partial class PurchaseReturnDetail
{
    public int DetailId { get; set; }

    public int ReturnId { get; set; }

    public int ProductId { get; set; }

    public int Quantity { get; set; }

    public decimal UnitPrice { get; set; }

    public decimal? LineTotal { get; set; }

    public virtual Product Product { get; set; } = null!;

    public virtual PurchaseReturn Return { get; set; } = null!;
}

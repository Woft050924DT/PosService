using System;
using System.Collections.Generic;

namespace DAL.Models;

public partial class PurchaseOrderDetail
{
    public int DetailId { get; set; }

    public int? PurchaseId { get; set; }

    public int? ProductId { get; set; }

    public int Quantity { get; set; }

    public decimal UnitPrice { get; set; }

    public decimal LineTotal { get; set; }

    public virtual Product? Product { get; set; }

    public virtual PurchaseOrder? Purchase { get; set; }
}

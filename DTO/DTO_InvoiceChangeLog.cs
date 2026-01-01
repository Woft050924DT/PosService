using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO
{
    public class DTO_InvoiceChangeLog
    {
            public long id { get; set; }
            public int invoiceId { get; set; }
            public string actionType { get; set; } = null!;
            public DateTime invoiceDate { get; set; }
            public DateTime changed_at { get; set; }
            public bool processed { get; set; }
    }
}

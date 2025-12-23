using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO
{
    public class dto_InventoryTransaction
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


    }
}

using System;
using System.Collections.Generic;

namespace PosService.DTO
{
    public class SalesInvoiceDetailDTO
    {
        public int DetailId { get; set; }
        public int? ProductId { get; set; }
        public string? ProductName { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal? Discount { get; set; }
        public decimal? LinePromotionDiscount { get; set; }
        public decimal LineTotal { get; set; }
    }

    public class SalesInvoiceDTO
    {
        public int InvoiceId { get; set; }
        public string InvoiceNumber { get; set; } = null!;
        public DateTime? InvoiceDate { get; set; }
        public int? CustomerId { get; set; }
        public string? CustomerName { get; set; }
        public int? UserId { get; set; }
        public decimal? SubTotal { get; set; }
        public decimal? Discount { get; set; }
        public int? PromotionId { get; set; }
        public string? PromotionCode { get; set; }
        public string? PromotionName { get; set; }
        public decimal? PromotionDiscount { get; set; }
        public decimal? TotalAmount { get; set; }
        public decimal? PaidAmount { get; set; }
        public string? PaymentMethod { get; set; }
        public string? Status { get; set; }
        public string? Notes { get; set; }

        public List<SalesInvoiceDetailDTO> Details { get; set; } = new List<SalesInvoiceDetailDTO>();
    }

    // DTOs used for creating an invoice
    public class CreateSalesInvoiceDetailDTO
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal? Discount { get; set; }
    }

    public class CreateSalesInvoiceDTO
    {
        public string? InvoiceNumber { get; set; } // optional - will be generated if null
        public int? CustomerId { get; set; }
        public int? UserId { get; set; }
        public decimal? Discount { get; set; } // overall invoice discount
        public int? PromotionId { get; set; }
        public string? PromotionCode { get; set; }
        public decimal? PaidAmount { get; set; }
        public string? PaymentMethod { get; set; }
        public string? Notes { get; set; }
        public List<CreateSalesInvoiceDetailDTO> Details { get; set; } = new List<CreateSalesInvoiceDetailDTO>();
    }
}

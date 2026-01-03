using System;

namespace PosService.DTO
{
    public class TaxDTO
    {
        public int TaxId { get; set; }
        public string TaxCode { get; set; } = null!;
        public string TaxName { get; set; } = null!;
        public decimal TaxRate { get; set; }
        public string? Description { get; set; }
        public bool? IsActive { get; set; }
        public DateTime? CreatedAt { get; set; }
    }

    public class CreateTaxDTO
    {
        public string TaxCode { get; set; } = null!;
        public string TaxName { get; set; } = null!;
        public decimal TaxRate { get; set; }
        public string? Description { get; set; }
        public bool? IsActive { get; set; }
    }

    public class UpdateTaxDTO
    {
        public string? TaxCode { get; set; }
        public string? TaxName { get; set; }
        public decimal? TaxRate { get; set; }
        public string? Description { get; set; }
        public bool? IsActive { get; set; }
    }
}


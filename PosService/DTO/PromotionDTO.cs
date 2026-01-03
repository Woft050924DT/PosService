using System;
using System.Collections.Generic;

namespace PosService.DTO
{
    public class PromotionDTO
    {
        public int PromotionId { get; set; }
        public string PromotionCode { get; set; } = null!;
        public string PromotionName { get; set; } = null!;
        public string? Description { get; set; }
        public string DiscountType { get; set; } = null!;
        public decimal DiscountValue { get; set; }
        public decimal? MinOrderAmount { get; set; }
        public string ApplyTo { get; set; } = null!;
        public DateOnly StartDate { get; set; }
        public DateOnly? EndDate { get; set; }
        public bool? IsActive { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedAt { get; set; }
        public List<int> CategoryIds { get; set; } = new List<int>();
        public List<int> ProductIds { get; set; } = new List<int>();
    }

    public class CreatePromotionDTO
    {
        public string PromotionCode { get; set; } = null!;
        public string PromotionName { get; set; } = null!;
        public string? Description { get; set; }
        public string DiscountType { get; set; } = null!;
        public decimal DiscountValue { get; set; }
        public decimal? MinOrderAmount { get; set; }
        public string ApplyTo { get; set; } = null!;
        public DateOnly StartDate { get; set; }
        public DateOnly? EndDate { get; set; }
        public bool? IsActive { get; set; }
        public int? CreatedBy { get; set; }
        public List<int> CategoryIds { get; set; } = new List<int>();
        public List<int> ProductIds { get; set; } = new List<int>();
    }
}


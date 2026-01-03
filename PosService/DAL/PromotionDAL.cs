using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PosService.DTO;
using PosService.Models;

namespace PosService.DAL
{
    public class PromotionDAL
    {
        private readonly HDVContext _db;

        public PromotionDAL(HDVContext db)
        {
            _db = db;
        }

        public async Task<List<PromotionDTO>> GetAllAsync(bool? isActive = null)
        {
            var query = _db.Promotions
                .AsNoTracking()
                .Include(p => p.Categories)
                .Include(p => p.Products)
                .AsQueryable();

            if (isActive.HasValue)
            {
                query = query.Where(p => p.IsActive == isActive.Value);
            }

            var list = await query.ToListAsync();

            return list.Select(p => new PromotionDTO
            {
                PromotionId = p.PromotionId,
                PromotionCode = p.PromotionCode,
                PromotionName = p.PromotionName,
                Description = p.Description,
                DiscountType = p.DiscountType,
                DiscountValue = p.DiscountValue,
                MinOrderAmount = p.MinOrderAmount,
                ApplyTo = p.ApplyTo,
                StartDate = p.StartDate,
                EndDate = p.EndDate,
                IsActive = p.IsActive,
                CreatedBy = p.CreatedBy,
                CreatedAt = p.CreatedAt,
                CategoryIds = p.Categories.Select(c => c.CategoryId).ToList(),
                ProductIds = p.Products.Select(pr => pr.ProductId).ToList()
            }).ToList();
        }

        public async Task<PromotionDTO?> GetByIdAsync(int id)
        {
            var p = await _db.Promotions
                .AsNoTracking()
                .Include(x => x.Categories)
                .Include(x => x.Products)
                .FirstOrDefaultAsync(x => x.PromotionId == id);

            if (p == null) return null;

            return new PromotionDTO
            {
                PromotionId = p.PromotionId,
                PromotionCode = p.PromotionCode,
                PromotionName = p.PromotionName,
                Description = p.Description,
                DiscountType = p.DiscountType,
                DiscountValue = p.DiscountValue,
                MinOrderAmount = p.MinOrderAmount,
                ApplyTo = p.ApplyTo,
                StartDate = p.StartDate,
                EndDate = p.EndDate,
                IsActive = p.IsActive,
                CreatedBy = p.CreatedBy,
                CreatedAt = p.CreatedAt,
                CategoryIds = p.Categories.Select(c => c.CategoryId).ToList(),
                ProductIds = p.Products.Select(pr => pr.ProductId).ToList()
            };
        }

        public async Task<PromotionDTO> CreateAsync(CreatePromotionDTO dto)
        {
            if (dto == null) throw new ArgumentNullException(nameof(dto));

            var exists = await _db.Promotions.AnyAsync(p => p.PromotionCode == dto.PromotionCode);
            if (exists) throw new InvalidOperationException($"PromotionCode '{dto.PromotionCode}' already exists.");

            var entity = new Promotion
            {
                PromotionCode = dto.PromotionCode,
                PromotionName = dto.PromotionName,
                Description = dto.Description,
                DiscountType = dto.DiscountType,
                DiscountValue = dto.DiscountValue,
                MinOrderAmount = dto.MinOrderAmount,
                ApplyTo = dto.ApplyTo,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate,
                IsActive = dto.IsActive ?? true,
                CreatedBy = dto.CreatedBy
            };

            if (dto.CategoryIds != null && dto.CategoryIds.Any())
            {
                var categories = await _db.Categories
                    .Where(c => dto.CategoryIds.Contains(c.CategoryId))
                    .ToListAsync();

                foreach (var c in categories)
                {
                    entity.Categories.Add(c);
                }
            }

            if (dto.ProductIds != null && dto.ProductIds.Any())
            {
                var products = await _db.Products
                    .Where(p => dto.ProductIds.Contains(p.ProductId))
                    .ToListAsync();

                foreach (var p in products)
                {
                    entity.Products.Add(p);
                }
            }

            _db.Promotions.Add(entity);
            await _db.SaveChangesAsync();

            return new PromotionDTO
            {
                PromotionId = entity.PromotionId,
                PromotionCode = entity.PromotionCode,
                PromotionName = entity.PromotionName,
                Description = entity.Description,
                DiscountType = entity.DiscountType,
                DiscountValue = entity.DiscountValue,
                MinOrderAmount = entity.MinOrderAmount,
                ApplyTo = entity.ApplyTo,
                StartDate = entity.StartDate,
                EndDate = entity.EndDate,
                IsActive = entity.IsActive,
                CreatedBy = entity.CreatedBy,
                CreatedAt = entity.CreatedAt,
                CategoryIds = entity.Categories.Select(c => c.CategoryId).ToList(),
                ProductIds = entity.Products.Select(p => p.ProductId).ToList()
            };
        }
    }
}


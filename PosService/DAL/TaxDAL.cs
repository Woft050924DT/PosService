using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PosService.DTO;
using PosService.Models;

namespace PosService.DAL
{
    public class TaxDAL
    {
        private readonly HDVContext _db;

        public TaxDAL(HDVContext db)
        {
            _db = db;
        }

        public async Task<List<TaxDTO>> GetAllAsync(bool? isActive = null, string? q = null)
        {
            var query = _db.Taxes.AsNoTracking().AsQueryable();

            if (isActive.HasValue)
                query = query.Where(t => t.IsActive == isActive.Value);

            if (!string.IsNullOrWhiteSpace(q))
            {
                q = q.Trim();
                query = query.Where(t =>
                    t.TaxCode.Contains(q) ||
                    t.TaxName.Contains(q));
            }

            return await query
                .OrderBy(t => t.TaxCode)
                .Select(t => new TaxDTO
                {
                    TaxId = t.TaxId,
                    TaxCode = t.TaxCode,
                    TaxName = t.TaxName,
                    TaxRate = t.TaxRate,
                    Description = t.Description,
                    IsActive = t.IsActive,
                    CreatedAt = t.CreatedAt
                })
                .ToListAsync();
        }

        public async Task<TaxDTO?> GetByIdAsync(int id)
        {
            var t = await _db.Taxes
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.TaxId == id);

            if (t == null) return null;

            return new TaxDTO
            {
                TaxId = t.TaxId,
                TaxCode = t.TaxCode,
                TaxName = t.TaxName,
                TaxRate = t.TaxRate,
                Description = t.Description,
                IsActive = t.IsActive,
                CreatedAt = t.CreatedAt
            };
        }

        public async Task<TaxDTO> CreateAsync(CreateTaxDTO dto)
        {
            if (dto == null) throw new ArgumentNullException(nameof(dto));

            var entity = new Taxis
            {
                TaxCode = dto.TaxCode,
                TaxName = dto.TaxName,
                TaxRate = dto.TaxRate,
                Description = dto.Description,
                IsActive = dto.IsActive ?? true
            };

            _db.Taxes.Add(entity);
            await _db.SaveChangesAsync();

            return new TaxDTO
            {
                TaxId = entity.TaxId,
                TaxCode = entity.TaxCode,
                TaxName = entity.TaxName,
                TaxRate = entity.TaxRate,
                Description = entity.Description,
                IsActive = entity.IsActive,
                CreatedAt = entity.CreatedAt
            };
        }

        public async Task<TaxDTO?> UpdateAsync(int id, UpdateTaxDTO dto)
        {
            if (dto == null) throw new ArgumentNullException(nameof(dto));

            var entity = await _db.Taxes.FirstOrDefaultAsync(t => t.TaxId == id);
            if (entity == null) return null;

            if (dto.TaxCode != null) entity.TaxCode = dto.TaxCode;
            if (dto.TaxName != null) entity.TaxName = dto.TaxName;
            if (dto.TaxRate.HasValue) entity.TaxRate = dto.TaxRate.Value;
            if (dto.Description != null) entity.Description = dto.Description;
            if (dto.IsActive.HasValue) entity.IsActive = dto.IsActive.Value;

            await _db.SaveChangesAsync();

            return new TaxDTO
            {
                TaxId = entity.TaxId,
                TaxCode = entity.TaxCode,
                TaxName = entity.TaxName,
                TaxRate = entity.TaxRate,
                Description = entity.Description,
                IsActive = entity.IsActive,
                CreatedAt = entity.CreatedAt
            };
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var entity = await _db.Taxes.FirstOrDefaultAsync(t => t.TaxId == id);
            if (entity == null) return false;

            entity.IsActive = false;
            await _db.SaveChangesAsync();
            return true;
        }
    }
}


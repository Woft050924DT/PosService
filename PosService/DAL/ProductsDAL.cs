using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PosService.DTO;
using PosService.Models;
namespace PosService.DAL
{
    public class ProductDAL
    {
        private readonly HDVContext _db;

        public ProductDAL(HDVContext db)
        {
            _db = db;
        }

        public async Task<List<ProductDTO>> GetAllAsync(
            bool? isActive = null,
            int? categoryId = null,
            int? supplierId = null,
            string? q = null)
        {
            var query = _db.Products
                .AsNoTracking()
                .Include(p => p.Category)
                .Include(p => p.Supplier)
                .AsQueryable();

            if (isActive.HasValue)
                query = query.Where(p => p.IsActive == isActive.Value);

            if (categoryId.HasValue)
                query = query.Where(p => p.CategoryId == categoryId.Value);

            if (supplierId.HasValue)
                query = query.Where(p => p.SupplierId == supplierId.Value);

            if (!string.IsNullOrWhiteSpace(q))
            {
                q = q.Trim();
                query = query.Where(p =>
                    p.ProductCode.Contains(q) ||
                    (p.Barcode != null && p.Barcode.Contains(q)) ||
                    p.ProductName.Contains(q));
            }

            return await query
                .Select(p => new ProductDTO
                {
                    ProductId = p.ProductId,
                    ProductCode = p.ProductCode,
                    Barcode = p.Barcode,
                    ProductName = p.ProductName,
                    CategoryId = p.CategoryId,
                    CategoryName = p.Category != null ? p.Category.CategoryName : null,
                    SupplierId = p.SupplierId,
                    SupplierName = p.Supplier != null ? p.Supplier.SupplierName : null,
                    Unit = p.Unit,
                    CostPrice = p.CostPrice,
                    SellingPrice = p.SellingPrice,
                    StockQuantity = p.StockQuantity,
                    MinStock = p.MinStock,
                    ImageUrl = p.ImageUrl,
                    IsActive = p.IsActive,
                    CreatedAt = p.CreatedAt
                })
                .ToListAsync();
        }

        public async Task<ProductDTO?> GetByIdAsync(int id)
        {
            var p = await _db.Products
                .AsNoTracking()
                .Include(x => x.Category)
                .Include(x => x.Supplier)
                .FirstOrDefaultAsync(x => x.ProductId == id);

            if (p == null) return null;

            return new ProductDTO
            {
                ProductId = p.ProductId,
                ProductCode = p.ProductCode,
                Barcode = p.Barcode,
                ProductName = p.ProductName,
                CategoryId = p.CategoryId,
                CategoryName = p.Category?.CategoryName,
                SupplierId = p.SupplierId,
                SupplierName = p.Supplier?.SupplierName,
                Unit = p.Unit,
                CostPrice = p.CostPrice,
                SellingPrice = p.SellingPrice,
                StockQuantity = p.StockQuantity,
                MinStock = p.MinStock,
                ImageUrl = p.ImageUrl,
                IsActive = p.IsActive,
                CreatedAt = p.CreatedAt
            };
        }

        public async Task<ProductDTO> CreateAsync(ProductDTO dto)
        {
            if (dto == null) throw new ArgumentNullException(nameof(dto));

            // enforce unique product code
            var exists = await _db.Products.AnyAsync(p => p.ProductCode == dto.ProductCode);
            if (exists) throw new InvalidOperationException($"ProductCode '{dto.ProductCode}' already exists.");

            var entity = new Product
            {
                ProductCode = dto.ProductCode,
                Barcode = dto.Barcode,
                ProductName = dto.ProductName,
                CategoryId = dto.CategoryId,
                SupplierId = dto.SupplierId,
                Unit = dto.Unit,
                CostPrice = dto.CostPrice,
                SellingPrice = dto.SellingPrice,
                StockQuantity = dto.StockQuantity,
                MinStock = dto.MinStock,
                ImageUrl = dto.ImageUrl,
                IsActive = dto.IsActive ?? true,
                // CreatedAt will be set by DB default
            };

            _db.Products.Add(entity);
            await _db.SaveChangesAsync();

            dto.ProductId = entity.ProductId;
            dto.CreatedAt = entity.CreatedAt;

            return dto;
        }

        public async Task<ProductDTO?> UpdateAsync(int id, ProductDTO dto)
        {
            if (dto == null) throw new ArgumentNullException(nameof(dto));

            var entity = await _db.Products.FirstOrDefaultAsync(p => p.ProductId == id);
            if (entity == null) return null;

            // if product code changed, ensure uniqueness
            if (!string.Equals(entity.ProductCode, dto.ProductCode, StringComparison.OrdinalIgnoreCase))
            {
                var codeExists = await _db.Products.AnyAsync(p => p.ProductCode == dto.ProductCode && p.ProductId != id);
                if (codeExists) throw new InvalidOperationException($"ProductCode '{dto.ProductCode}' already exists.");
            }

            entity.ProductCode = dto.ProductCode;
            entity.Barcode = dto.Barcode;
            entity.ProductName = dto.ProductName;
            entity.CategoryId = dto.CategoryId;
            entity.SupplierId = dto.SupplierId;
            entity.Unit = dto.Unit;
            entity.CostPrice = dto.CostPrice;
            entity.SellingPrice = dto.SellingPrice;
            entity.StockQuantity = dto.StockQuantity;
            entity.MinStock = dto.MinStock;
            entity.ImageUrl = dto.ImageUrl;
            entity.IsActive = dto.IsActive;

            await _db.SaveChangesAsync();

            // return updated DTO (refresh createdAt from entity to be safe)
            dto.ProductId = entity.ProductId;
            dto.CreatedAt = entity.CreatedAt;

            return dto;
        }


        public async Task<bool> DeleteAsync(int id)
        {
            var entity = await _db.Products.FirstOrDefaultAsync(p => p.ProductId == id);
            if (entity == null) return false;

            entity.IsActive = false;
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ExistsByCodeAsync(string productCode)
        {
            if (string.IsNullOrWhiteSpace(productCode)) return false;
            return await _db.Products.AnyAsync(p => p.ProductCode == productCode.Trim());
        }
    }
}

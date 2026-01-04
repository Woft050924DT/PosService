using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PosService.DTO;
using PosService.Models;
using Microsoft.Data.SqlClient;
namespace PosService.DAL
{
    public class InventoryDAL
    {
        private readonly HDVContext _db;

        public InventoryDAL(HDVContext db)
        {
            _db = db;
        }

        public async Task<List<InventoryDTO>> GetAllAsync(
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
                .Select(p => new InventoryDTO
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

        public async Task<InventoryDTO?> GetByIdAsync(int id)
        {
            var p = await _db.Products
                .AsNoTracking()
                .Include(x => x.Category)
                .Include(x => x.Supplier)
                .FirstOrDefaultAsync(x => x.ProductId == id);

            if (p == null) return null;

            return new InventoryDTO
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

        public async Task<InventoryDTO> CreateAsync(InventoryDTO dto)
        {
            if (dto == null) throw new ArgumentNullException(nameof(dto));

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
                IsActive = dto.IsActive ?? true
            };

            _db.Products.Add(entity);
            await _db.SaveChangesAsync();

            dto.ProductId = entity.ProductId;
            dto.CreatedAt = entity.CreatedAt;

            return dto;
        }

        public async Task<InventoryDTO?> UpdateAsync(int id, InventoryDTO dto)
        {
            if (dto == null) throw new ArgumentNullException(nameof(dto));

            var entity = await _db.Products.FirstOrDefaultAsync(p => p.ProductId == id);
            if (entity == null) return null;

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
        const string _conn = "Server=DESKTOP-TG67AE9;Database=HDV;Trusted_Connection=True;TrustServerCertificate=True;";
        public List<ProductStockDto> GetProductStock(
      int? categoryId,
      bool lowStock,
      string search)
        {
            var list = new List<ProductStockDto>();

            using (SqlConnection conn = new SqlConnection(_conn))
            {
                string sql = @"
                SELECT
                    p.ProductID,
                    p.ProductCode,
                    p.ProductName,
                    p.StockQuantity,
                    p.MinStock,
                    CASE
                        WHEN p.StockQuantity <= p.MinStock THEN 'LowStock'
                        ELSE 'InStock'
                    END AS Status,
                    p.CreatedAt
                FROM Products p
                WHERE p.IsActive = 1
                  AND (@CategoryID IS NULL OR p.CategoryID = @CategoryID)
                  AND (@LowStock = 0 OR p.StockQuantity <= p.MinStock)
                  AND (
                        @Search IS NULL
                        OR p.ProductName LIKE N'%' + @Search + '%'
                        OR p.ProductCode LIKE N'%' + @Search + '%'
                        OR p.Barcode LIKE N'%' + @Search + '%'
                  )
                ORDER BY p.ProductName";

                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@CategoryID", (object)categoryId ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@LowStock", lowStock ? 1 : 0);
                cmd.Parameters.AddWithValue("@Search", string.IsNullOrEmpty(search) ? (object)DBNull.Value : search);

                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    list.Add(new ProductStockDto
                    {
                        ProductId = Convert.ToInt32(reader["ProductID"]),
                        ProductCode = reader["ProductCode"].ToString(),
                        ProductName = reader["ProductName"].ToString(),
                        StockQuantity = Convert.ToInt32(reader["StockQuantity"]),
                        MinStock = Convert.ToInt32(reader["MinStock"]),
                        Status = reader["Status"].ToString(),
                        LastUpdated = Convert.ToDateTime(reader["CreatedAt"])
                    });
                }
            }

            return list;
        }
    }
}


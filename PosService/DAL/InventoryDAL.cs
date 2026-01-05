using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using PosService.DTO;

namespace PosService.DAL
{
    public class InventoryDAL
    {
        private readonly string _conn;

        public InventoryDAL(IConfiguration configuration)
        {
            _conn = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task<List<InventoryDTO>> GetAllAsync(
            bool? isActive = null,
            int? categoryId = null,
            int? supplierId = null,
            string? q = null)
        {
            var list = new List<InventoryDTO>();

            var sql = new StringBuilder();
            sql.Append(@"
                SELECT 
                    p.ProductId,
                    p.ProductCode,
                    p.Barcode,
                    p.ProductName,
                    p.CategoryId,
                    c.CategoryName,
                    p.SupplierId,
                    s.SupplierName,
                    p.Unit,
                    p.CostPrice,
                    p.SellingPrice,
                    p.StockQuantity,
                    p.MinStock,
                    p.ImageUrl,
                    p.IsActive,
                    p.CreatedAt
                FROM Products p
                LEFT JOIN Categories c ON p.CategoryId = c.CategoryId
                LEFT JOIN Suppliers s ON p.SupplierId = s.SupplierId
                WHERE 1 = 1");

            var parameters = new List<SqlParameter>();

            if (isActive.HasValue)
            {
                sql.Append(" AND p.IsActive = @IsActive");
                parameters.Add(new SqlParameter("@IsActive", SqlDbType.Bit) { Value = isActive.Value });
            }

            if (categoryId.HasValue)
            {
                sql.Append(" AND p.CategoryId = @CategoryId");
                parameters.Add(new SqlParameter("@CategoryId", SqlDbType.Int) { Value = categoryId.Value });
            }

            if (supplierId.HasValue)
            {
                sql.Append(" AND p.SupplierId = @SupplierId");
                parameters.Add(new SqlParameter("@SupplierId", SqlDbType.Int) { Value = supplierId.Value });
            }

            if (!string.IsNullOrWhiteSpace(q))
            {
                var keyword = q.Trim();
                sql.Append(@" AND (
                        p.ProductCode LIKE @Q
                        OR p.Barcode LIKE @Q
                        OR p.ProductName LIKE @Q
                    )");
                parameters.Add(new SqlParameter("@Q", SqlDbType.NVarChar, 255) { Value = "%" + keyword + "%" });
            }

            using var conn = new SqlConnection(_conn);
            using var cmd = new SqlCommand(sql.ToString(), conn);

            if (parameters.Count > 0)
            {
                cmd.Parameters.AddRange(parameters.ToArray());
            }

            await conn.OpenAsync();
            using var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                var item = new InventoryDTO
                {
                    ProductId = reader["ProductId"] != DBNull.Value ? Convert.ToInt32(reader["ProductId"]) : 0,
                    ProductCode = reader["ProductCode"]?.ToString(),
                    Barcode = reader["Barcode"]?.ToString(),
                    ProductName = reader["ProductName"]?.ToString(),
                    CategoryId = reader["CategoryId"] != DBNull.Value ? Convert.ToInt32(reader["CategoryId"]) : 0,
                    CategoryName = reader["CategoryName"]?.ToString(),
                    SupplierId = reader["SupplierId"] != DBNull.Value ? Convert.ToInt32(reader["SupplierId"]) : 0,
                    SupplierName = reader["SupplierName"]?.ToString(),
                    Unit = reader["Unit"]?.ToString(),
                    CostPrice = reader["CostPrice"] != DBNull.Value ? Convert.ToDecimal(reader["CostPrice"]) : 0,
                    SellingPrice = reader["SellingPrice"] != DBNull.Value ? Convert.ToDecimal(reader["SellingPrice"]) : 0,
                    StockQuantity = reader["StockQuantity"] != DBNull.Value ? Convert.ToInt32(reader["StockQuantity"]) : 0,
                    MinStock = reader["MinStock"] != DBNull.Value ? Convert.ToInt32(reader["MinStock"]) : 0,
                    ImageUrl = reader["ImageUrl"]?.ToString(),
                    IsActive = reader["IsActive"] != DBNull.Value && Convert.ToBoolean(reader["IsActive"]),
                    CreatedAt = reader["CreatedAt"] != DBNull.Value ? Convert.ToDateTime(reader["CreatedAt"]) : DateTime.MinValue
                };

                list.Add(item);
            }

            return list;
        }

        public async Task<List<InventoryDTO>> GetLowStockAsync(
            int? categoryId = null,
            int? supplierId = null,
            string? q = null)
        {
            var list = new List<InventoryDTO>();

            var sql = new StringBuilder();
            sql.Append(@"
                SELECT 
                    p.ProductId,
                    p.ProductCode,
                    p.Barcode,
                    p.ProductName,
                    p.CategoryId,
                    c.CategoryName,
                    p.SupplierId,
                    s.SupplierName,
                    p.Unit,
                    p.CostPrice,
                    p.SellingPrice,
                    p.StockQuantity,
                    p.MinStock,
                    p.ImageUrl,
                    p.IsActive,
                    p.CreatedAt
                FROM Products p
                LEFT JOIN Categories c ON p.CategoryId = c.CategoryId
                LEFT JOIN Suppliers s ON p.SupplierId = s.SupplierId
                WHERE 1 = 1
                  AND p.IsActive = 1
                  AND p.MinStock IS NOT NULL
                  AND p.StockQuantity IS NOT NULL
                  AND p.StockQuantity <= p.MinStock");

            var parameters = new List<SqlParameter>();

            if (categoryId.HasValue)
            {
                sql.Append(" AND p.CategoryId = @CategoryId");
                parameters.Add(new SqlParameter("@CategoryId", SqlDbType.Int) { Value = categoryId.Value });
            }

            if (supplierId.HasValue)
            {
                sql.Append(" AND p.SupplierId = @SupplierId");
                parameters.Add(new SqlParameter("@SupplierId", SqlDbType.Int) { Value = supplierId.Value });
            }

            if (!string.IsNullOrWhiteSpace(q))
            {
                var keyword = q.Trim();
                sql.Append(@" AND (
                        p.ProductCode LIKE @Q
                        OR p.Barcode LIKE @Q
                        OR p.ProductName LIKE @Q
                    )");
                parameters.Add(new SqlParameter("@Q", SqlDbType.NVarChar, 255) { Value = "%" + keyword + "%" });
            }

            using var conn = new SqlConnection(_conn);
            using var cmd = new SqlCommand(sql.ToString(), conn);

            if (parameters.Count > 0)
            {
                cmd.Parameters.AddRange(parameters.ToArray());
            }

            await conn.OpenAsync();
            using var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                var item = new InventoryDTO
                {
                    ProductId = reader["ProductId"] != DBNull.Value ? Convert.ToInt32(reader["ProductId"]) : 0,
                    ProductCode = reader["ProductCode"]?.ToString(),
                    Barcode = reader["Barcode"]?.ToString(),
                    ProductName = reader["ProductName"]?.ToString(),
                    CategoryId = reader["CategoryId"] != DBNull.Value ? Convert.ToInt32(reader["CategoryId"]) : 0,
                    CategoryName = reader["CategoryName"]?.ToString(),
                    SupplierId = reader["SupplierId"] != DBNull.Value ? Convert.ToInt32(reader["SupplierId"]) : 0,
                    SupplierName = reader["SupplierName"]?.ToString(),
                    Unit = reader["Unit"]?.ToString(),
                    CostPrice = reader["CostPrice"] != DBNull.Value ? Convert.ToDecimal(reader["CostPrice"]) : 0,
                    SellingPrice = reader["SellingPrice"] != DBNull.Value ? Convert.ToDecimal(reader["SellingPrice"]) : 0,
                    StockQuantity = reader["StockQuantity"] != DBNull.Value ? Convert.ToInt32(reader["StockQuantity"]) : 0,
                    MinStock = reader["MinStock"] != DBNull.Value ? Convert.ToInt32(reader["MinStock"]) : 0,
                    ImageUrl = reader["ImageUrl"]?.ToString(),
                    IsActive = reader["IsActive"] != DBNull.Value && Convert.ToBoolean(reader["IsActive"]),
                    CreatedAt = reader["CreatedAt"] != DBNull.Value ? Convert.ToDateTime(reader["CreatedAt"]) : DateTime.MinValue
                };

                list.Add(item);
            }

            return list;
        }

        public async Task<InventoryDTO?> GetByIdAsync(int id)
        {
            const string sql = @"
                SELECT 
                    p.ProductId,
                    p.ProductCode,
                    p.Barcode,
                    p.ProductName,
                    p.CategoryId,
                    c.CategoryName,
                    p.SupplierId,
                    s.SupplierName,
                    p.Unit,
                    p.CostPrice,
                    p.SellingPrice,
                    p.StockQuantity,
                    p.MinStock,
                    p.ImageUrl,
                    p.IsActive,
                    p.CreatedAt
                FROM Products p
                LEFT JOIN Categories c ON p.CategoryId = c.CategoryId
                LEFT JOIN Suppliers s ON p.SupplierId = s.SupplierId
                WHERE p.ProductId = @ProductId";

            using var conn = new SqlConnection(_conn);
            using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.Add(new SqlParameter("@ProductId", SqlDbType.Int) { Value = id });

            await conn.OpenAsync();
            using var reader = await cmd.ExecuteReaderAsync();

            if (!await reader.ReadAsync())
            {
                return null;
            }

            var item = new InventoryDTO
            {
                ProductId = reader["ProductId"] != DBNull.Value ? Convert.ToInt32(reader["ProductId"]) : 0,
                ProductCode = reader["ProductCode"]?.ToString(),
                Barcode = reader["Barcode"]?.ToString(),
                ProductName = reader["ProductName"]?.ToString(),
                CategoryId = reader["CategoryId"] != DBNull.Value ? Convert.ToInt32(reader["CategoryId"]) : 0,
                CategoryName = reader["CategoryName"]?.ToString(),
                SupplierId = reader["SupplierId"] != DBNull.Value ? Convert.ToInt32(reader["SupplierId"]) : 0,
                SupplierName = reader["SupplierName"]?.ToString(),
                Unit = reader["Unit"]?.ToString(),
                CostPrice = reader["CostPrice"] != DBNull.Value ? Convert.ToDecimal(reader["CostPrice"]) : 0,
                SellingPrice = reader["SellingPrice"] != DBNull.Value ? Convert.ToDecimal(reader["SellingPrice"]) : 0,
                StockQuantity = reader["StockQuantity"] != DBNull.Value ? Convert.ToInt32(reader["StockQuantity"]) : 0,
                MinStock = reader["MinStock"] != DBNull.Value ? Convert.ToInt32(reader["MinStock"]) : 0,
                ImageUrl = reader["ImageUrl"]?.ToString(),
                IsActive = reader["IsActive"] != DBNull.Value && Convert.ToBoolean(reader["IsActive"]),
                CreatedAt = reader["CreatedAt"] != DBNull.Value ? Convert.ToDateTime(reader["CreatedAt"]) : DateTime.MinValue
            };

            return item;
        }

        public async Task<InventoryDTO> CreateAsync(InventoryDTO dto)
        {
            if (dto == null) throw new ArgumentNullException(nameof(dto));

            const string checkSql = "SELECT COUNT(1) FROM Products WHERE ProductCode = @ProductCode";

            using (var conn = new SqlConnection(_conn))
            using (var checkCmd = new SqlCommand(checkSql, conn))
            {
                checkCmd.Parameters.Add(new SqlParameter("@ProductCode", SqlDbType.NVarChar, 50)
                {
                    Value = dto.ProductCode ?? string.Empty
                });

                await conn.OpenAsync();
                var exists = (int)await checkCmd.ExecuteScalarAsync() > 0;
                if (exists)
                {
                    throw new InvalidOperationException($"ProductCode '{dto.ProductCode}' already exists.");
                }
            }

            const string insertSql = @"
                INSERT INTO Products
                (
                    ProductCode,
                    Barcode,
                    ProductName,
                    CategoryId,
                    SupplierId,
                    Unit,
                    CostPrice,
                    SellingPrice,
                    StockQuantity,
                    MinStock,
                    ImageUrl,
                    IsActive
                )
                OUTPUT INSERTED.ProductId, INSERTED.CreatedAt
                VALUES
                (
                    @ProductCode,
                    @Barcode,
                    @ProductName,
                    @CategoryId,
                    @SupplierId,
                    @Unit,
                    @CostPrice,
                    @SellingPrice,
                    @StockQuantity,
                    @MinStock,
                    @ImageUrl,
                    @IsActive
                )";

            using (var conn = new SqlConnection(_conn))
            using (var cmd = new SqlCommand(insertSql, conn))
            {
                cmd.Parameters.AddWithValue("@ProductCode", (object?)dto.ProductCode ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Barcode", (object?)dto.Barcode ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@ProductName", (object?)dto.ProductName ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@CategoryId", dto.CategoryId.HasValue ? (object)dto.CategoryId.Value : DBNull.Value);
                cmd.Parameters.AddWithValue("@SupplierId", dto.SupplierId.HasValue ? (object)dto.SupplierId.Value : DBNull.Value);
                cmd.Parameters.AddWithValue("@Unit", (object?)dto.Unit ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@CostPrice", dto.CostPrice.HasValue ? (object)dto.CostPrice.Value : DBNull.Value);
                cmd.Parameters.AddWithValue("@SellingPrice", dto.SellingPrice);
                cmd.Parameters.AddWithValue("@StockQuantity", dto.StockQuantity.HasValue ? (object)dto.StockQuantity.Value : DBNull.Value);
                cmd.Parameters.AddWithValue("@MinStock", dto.MinStock.HasValue ? (object)dto.MinStock.Value : DBNull.Value);
                cmd.Parameters.AddWithValue("@ImageUrl", (object?)dto.ImageUrl ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@IsActive", dto.IsActive ?? true);

                await conn.OpenAsync();
                using var reader = await cmd.ExecuteReaderAsync();
                if (await reader.ReadAsync())
                {
                    dto.ProductId = reader["ProductId"] != DBNull.Value ? Convert.ToInt32(reader["ProductId"]) : 0;
                    dto.CreatedAt = reader["CreatedAt"] != DBNull.Value ? Convert.ToDateTime(reader["CreatedAt"]) : DateTime.MinValue;
                }
            }

            return dto;
        }

        public async Task<InventoryDTO?> UpdateAsync(int id, InventoryDTO dto)
        {
            if (dto == null) throw new ArgumentNullException(nameof(dto));

            const string existsSql = "SELECT COUNT(1) FROM Products WHERE ProductId = @ProductId";
            using (var conn = new SqlConnection(_conn))
            using (var existsCmd = new SqlCommand(existsSql, conn))
            {
                existsCmd.Parameters.Add(new SqlParameter("@ProductId", SqlDbType.Int) { Value = id });
                await conn.OpenAsync();
                var exists = (int)await existsCmd.ExecuteScalarAsync() > 0;
                if (!exists)
                {
                    return null;
                }
            }

            if (!string.IsNullOrWhiteSpace(dto.ProductCode))
            {
                const string codeSql = "SELECT COUNT(1) FROM Products WHERE ProductCode = @ProductCode AND ProductId <> @ProductId";
                using var conn = new SqlConnection(_conn);
                using var codeCmd = new SqlCommand(codeSql, conn);
                codeCmd.Parameters.Add(new SqlParameter("@ProductCode", SqlDbType.NVarChar, 50)
                {
                    Value = dto.ProductCode
                });
                codeCmd.Parameters.Add(new SqlParameter("@ProductId", SqlDbType.Int) { Value = id });

                await conn.OpenAsync();
                var codeExists = (int)await codeCmd.ExecuteScalarAsync() > 0;
                if (codeExists)
                {
                    throw new InvalidOperationException($"ProductCode '{dto.ProductCode}' already exists.");
                }
            }

            const string updateSql = @"
                UPDATE Products
                SET
                    ProductCode = @ProductCode,
                    Barcode = @Barcode,
                    ProductName = @ProductName,
                    CategoryId = @CategoryId,
                    SupplierId = @SupplierId,
                    Unit = @Unit,
                    CostPrice = @CostPrice,
                    SellingPrice = @SellingPrice,
                    StockQuantity = @StockQuantity,
                    MinStock = @MinStock,
                    ImageUrl = @ImageUrl,
                    IsActive = @IsActive
                WHERE ProductId = @ProductId";

            using (var conn = new SqlConnection(_conn))
            using (var cmd = new SqlCommand(updateSql, conn))
            {
                cmd.Parameters.AddWithValue("@ProductId", id);
                cmd.Parameters.AddWithValue("@ProductCode", (object?)dto.ProductCode ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Barcode", (object?)dto.Barcode ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@ProductName", (object?)dto.ProductName ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@CategoryId", dto.CategoryId.HasValue ? (object)dto.CategoryId.Value : DBNull.Value);
                cmd.Parameters.AddWithValue("@SupplierId", dto.SupplierId.HasValue ? (object)dto.SupplierId.Value : DBNull.Value);
                cmd.Parameters.AddWithValue("@Unit", (object?)dto.Unit ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@CostPrice", dto.CostPrice.HasValue ? (object)dto.CostPrice.Value : DBNull.Value);
                cmd.Parameters.AddWithValue("@SellingPrice", dto.SellingPrice);
                cmd.Parameters.AddWithValue("@StockQuantity", dto.StockQuantity.HasValue ? (object)dto.StockQuantity.Value : DBNull.Value);
                cmd.Parameters.AddWithValue("@MinStock", dto.MinStock.HasValue ? (object)dto.MinStock.Value : DBNull.Value);
                cmd.Parameters.AddWithValue("@ImageUrl", (object?)dto.ImageUrl ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@IsActive", dto.IsActive ?? true);

                await conn.OpenAsync();
                var rows = await cmd.ExecuteNonQueryAsync();
                if (rows <= 0)
                {
                    return null;
                }
            }

            var updated = await GetByIdAsync(id);
            return updated;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            const string sql = @"
                UPDATE Products
                SET IsActive = 0
                WHERE ProductId = @ProductId";

            using var conn = new SqlConnection(_conn);
            using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.Add(new SqlParameter("@ProductId", SqlDbType.Int) { Value = id });

            await conn.OpenAsync();
            var rows = await cmd.ExecuteNonQueryAsync();
            return rows > 0;
        }
    }
}

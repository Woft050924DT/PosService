using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
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

        public async Task<List<InventoryStockMovementItemResultDTO>> StockInAsync(InventoryStockMovementDTO dto)
        {
            return await ApplyStockMovementAsync(dto, "IN");
        }

        public async Task<List<InventoryStockMovementItemResultDTO>> StockOutAsync(InventoryStockMovementDTO dto)
        {
            return await ApplyStockMovementAsync(dto, "OUT");
        }

        private async Task<List<InventoryStockMovementItemResultDTO>> ApplyStockMovementAsync(InventoryStockMovementDTO dto, string transactionType)
        {
            if (dto == null) throw new ArgumentNullException(nameof(dto));
            if (dto.Items == null || dto.Items.Count == 0) throw new InvalidOperationException("Danh sách hàng hóa không được để trống.");

            var movementSign = string.Equals(transactionType, "IN", StringComparison.OrdinalIgnoreCase) ? 1 : -1;

            var normalizedItems = dto.Items
                .GroupBy(x => x.ProductId)
                .Select(g => new InventoryStockItemDTO
                {
                    ProductId = g.Key,
                    Quantity = g.Sum(x => x.Quantity)
                })
                .ToList();

            foreach (var item in normalizedItems)
            {
                if (item.ProductId <= 0) throw new InvalidOperationException("ProductId không hợp lệ.");
                if (item.Quantity <= 0) throw new InvalidOperationException("Quantity phải lớn hơn 0.");
            }

            const string selectQtySql = @"
                SELECT ISNULL(StockQuantity, 0)
                FROM Products WITH (UPDLOCK, ROWLOCK)
                WHERE ProductId = @ProductId";

            const string updateQtySql = @"
                UPDATE Products
                SET StockQuantity = @NewQuantity
                WHERE ProductId = @ProductId";

            const string insertTxnSql = @"
                INSERT INTO InventoryTransactions
                (
                    ProductId,
                    TransactionType,
                    ReferenceType,
                    ReferenceId,
                    Quantity,
                    QuantityBefore,
                    QuantityAfter,
                    Notes,
                    CreatedAt,
                    CreatedBy
                )
                VALUES
                (
                    @ProductId,
                    @TransactionType,
                    @ReferenceType,
                    @ReferenceId,
                    @Quantity,
                    @QuantityBefore,
                    @QuantityAfter,
                    @Notes,
                    GETDATE(),
                    @CreatedBy
                )";

            var results = new List<InventoryStockMovementItemResultDTO>();

            using var conn = new SqlConnection(_conn);
            await conn.OpenAsync();
            using var tx = await conn.BeginTransactionAsync();

            try
            {
                using var selectCmd = new SqlCommand(selectQtySql, conn, (SqlTransaction)tx);
                selectCmd.Parameters.Add("@ProductId", SqlDbType.Int);

                using var updateCmd = new SqlCommand(updateQtySql, conn, (SqlTransaction)tx);
                updateCmd.Parameters.Add("@ProductId", SqlDbType.Int);
                updateCmd.Parameters.Add("@NewQuantity", SqlDbType.Int);

                using var insertCmd = new SqlCommand(insertTxnSql, conn, (SqlTransaction)tx);
                insertCmd.Parameters.Add("@ProductId", SqlDbType.Int);
                insertCmd.Parameters.Add("@TransactionType", SqlDbType.NVarChar, 50);
                insertCmd.Parameters.Add("@ReferenceType", SqlDbType.NVarChar, 50);
                insertCmd.Parameters.Add("@ReferenceId", SqlDbType.Int);
                insertCmd.Parameters.Add("@Quantity", SqlDbType.Int);
                insertCmd.Parameters.Add("@QuantityBefore", SqlDbType.Int);
                insertCmd.Parameters.Add("@QuantityAfter", SqlDbType.Int);
                insertCmd.Parameters.Add("@Notes", SqlDbType.NVarChar, -1);
                insertCmd.Parameters.Add("@CreatedBy", SqlDbType.Int);

                foreach (var item in normalizedItems)
                {
                    selectCmd.Parameters["@ProductId"].Value = item.ProductId;
                    var scalar = await selectCmd.ExecuteScalarAsync();
                    if (scalar == null || scalar == DBNull.Value)
                    {
                        throw new InvalidOperationException($"Không tìm thấy sản phẩm ProductId={item.ProductId}.");
                    }

                    var before = Convert.ToInt32(scalar);
                    var after = before + (movementSign * item.Quantity);

                    if (after < 0)
                    {
                        throw new InvalidOperationException($"Tồn kho không đủ cho ProductId={item.ProductId}. Hiện có {before}, cần xuất {item.Quantity}.");
                    }

                    updateCmd.Parameters["@ProductId"].Value = item.ProductId;
                    updateCmd.Parameters["@NewQuantity"].Value = after;
                    var rows = await updateCmd.ExecuteNonQueryAsync();
                    if (rows <= 0)
                    {
                        throw new InvalidOperationException($"Cập nhật tồn kho thất bại cho ProductId={item.ProductId}.");
                    }

                    insertCmd.Parameters["@ProductId"].Value = item.ProductId;
                    insertCmd.Parameters["@TransactionType"].Value = transactionType;
                    insertCmd.Parameters["@ReferenceType"].Value = (object?)dto.ReferenceType ?? DBNull.Value;
                    insertCmd.Parameters["@ReferenceId"].Value = dto.ReferenceId.HasValue ? (object)dto.ReferenceId.Value : DBNull.Value;
                    insertCmd.Parameters["@Quantity"].Value = item.Quantity;
                    insertCmd.Parameters["@QuantityBefore"].Value = before;
                    insertCmd.Parameters["@QuantityAfter"].Value = after;
                    insertCmd.Parameters["@Notes"].Value = (object?)dto.Notes ?? DBNull.Value;
                    insertCmd.Parameters["@CreatedBy"].Value = dto.UserId.HasValue ? (object)dto.UserId.Value : DBNull.Value;
                    await insertCmd.ExecuteNonQueryAsync();

                    results.Add(new InventoryStockMovementItemResultDTO
                    {
                        ProductId = item.ProductId,
                        Quantity = item.Quantity,
                        QuantityBefore = before,
                        QuantityAfter = after
                    });
                }

                await tx.CommitAsync();
                return results;
            }
            catch
            {
                await tx.RollbackAsync();
                throw;
            }
        }

        public async Task<InventoryGoodsReceiptResultDTO> ReceiveGoodsAsync(InventoryGoodsReceiptDTO dto)
        {
            if (dto == null) throw new ArgumentNullException(nameof(dto));
            if (dto.Items == null || dto.Items.Count == 0) throw new InvalidOperationException("Danh sách hàng hóa không được để trống.");

            foreach (var item in dto.Items)
            {
                if (item == null) throw new InvalidOperationException("Dòng hàng hóa không hợp lệ.");
                if ((item.ProductId == null || item.ProductId <= 0) && string.IsNullOrWhiteSpace(item.ProductCode))
                    throw new InvalidOperationException("Cần cung cấp ProductId hoặc ProductCode.");
                if (item.Quantity <= 0) throw new InvalidOperationException("Quantity phải lớn hơn 0.");
                if (item.UnitPrice < 0) throw new InvalidOperationException("UnitPrice không được âm.");
                if (item.LineTotal.HasValue && item.LineTotal.Value < 0) throw new InvalidOperationException("LineTotal không được âm.");
            }

            const string selectProductSql = @"
                SELECT TOP 1
                    ProductId,
                    ProductCode,
                    ProductName,
                    ISNULL(StockQuantity, 0) AS StockQuantity
                FROM Products WITH (UPDLOCK, ROWLOCK)
                WHERE
                    (@ProductId IS NOT NULL AND ProductId = @ProductId)
                    OR (@ProductCode IS NOT NULL AND ProductCode = @ProductCode)";

            const string updateQtySql = @"
                UPDATE Products
                SET StockQuantity = @NewQuantity
                WHERE ProductId = @ProductId";

            const string insertTxnSql = @"
                INSERT INTO InventoryTransactions
                (
                    ProductId,
                    TransactionType,
                    ReferenceType,
                    ReferenceId,
                    Quantity,
                    QuantityBefore,
                    QuantityAfter,
                    Notes,
                    CreatedAt,
                    CreatedBy
                )
                VALUES
                (
                    @ProductId,
                    @TransactionType,
                    @ReferenceType,
                    @ReferenceId,
                    @Quantity,
                    @QuantityBefore,
                    @QuantityAfter,
                    @Notes,
                    GETDATE(),
                    @CreatedBy
                )";

            var result = new InventoryGoodsReceiptResultDTO
            {
                ReferenceType = dto.ReferenceType,
                ReferenceId = dto.ReferenceId,
                UserId = dto.UserId
            };

            using var conn = new SqlConnection(_conn);
            await conn.OpenAsync();
            using var tx = await conn.BeginTransactionAsync();

            try
            {
                using var selectCmd = new SqlCommand(selectProductSql, conn, (SqlTransaction)tx);
                selectCmd.Parameters.Add("@ProductId", SqlDbType.Int);
                selectCmd.Parameters.Add("@ProductCode", SqlDbType.NVarChar, 50);

                using var updateCmd = new SqlCommand(updateQtySql, conn, (SqlTransaction)tx);
                updateCmd.Parameters.Add("@ProductId", SqlDbType.Int);
                updateCmd.Parameters.Add("@NewQuantity", SqlDbType.Int);

                using var insertCmd = new SqlCommand(insertTxnSql, conn, (SqlTransaction)tx);
                insertCmd.Parameters.Add("@ProductId", SqlDbType.Int);
                insertCmd.Parameters.Add("@TransactionType", SqlDbType.NVarChar, 50);
                insertCmd.Parameters.Add("@ReferenceType", SqlDbType.NVarChar, 50);
                insertCmd.Parameters.Add("@ReferenceId", SqlDbType.Int);
                insertCmd.Parameters.Add("@Quantity", SqlDbType.Int);
                insertCmd.Parameters.Add("@QuantityBefore", SqlDbType.Int);
                insertCmd.Parameters.Add("@QuantityAfter", SqlDbType.Int);
                insertCmd.Parameters.Add("@Notes", SqlDbType.NVarChar, -1);
                insertCmd.Parameters.Add("@CreatedBy", SqlDbType.Int);

                decimal total = 0m;

                foreach (var item in dto.Items)
                {
                    var productCode = item.ProductId.HasValue
                        ? null
                        : (string.IsNullOrWhiteSpace(item.ProductCode) ? null : item.ProductCode.Trim());

                    selectCmd.Parameters["@ProductId"].Value = item.ProductId.HasValue ? (object)item.ProductId.Value : DBNull.Value;
                    selectCmd.Parameters["@ProductCode"].Value = (object?)productCode ?? DBNull.Value;

                    using var reader = await selectCmd.ExecuteReaderAsync();
                    if (!await reader.ReadAsync())
                    {
                        throw new InvalidOperationException($"Không tìm thấy sản phẩm ({(item.ProductId.HasValue ? "ProductId=" + item.ProductId.Value : "ProductCode=" + productCode)}).");
                    }

                    var productId = reader["ProductId"] != DBNull.Value ? Convert.ToInt32(reader["ProductId"]) : 0;
                    var dbCode = reader["ProductCode"]?.ToString() ?? string.Empty;
                    var dbName = reader["ProductName"]?.ToString() ?? (item.ProductName ?? string.Empty);
                    var before = reader["StockQuantity"] != DBNull.Value ? Convert.ToInt32(reader["StockQuantity"]) : 0;
                    await reader.CloseAsync();

                    var after = before + item.Quantity;

                    var computedLineTotal = item.Quantity * item.UnitPrice;
                    var lineTotal = item.LineTotal ?? computedLineTotal;
                    if (item.LineTotal.HasValue && Math.Abs(item.LineTotal.Value - computedLineTotal) > 0.01m)
                    {
                        throw new InvalidOperationException($"LineTotal không khớp cho {dbCode}. Kỳ vọng {computedLineTotal}.");
                    }

                    updateCmd.Parameters["@ProductId"].Value = productId;
                    updateCmd.Parameters["@NewQuantity"].Value = after;
                    var rows = await updateCmd.ExecuteNonQueryAsync();
                    if (rows <= 0) throw new InvalidOperationException($"Cập nhật tồn kho thất bại cho {dbCode}.");

                    insertCmd.Parameters["@ProductId"].Value = productId;
                    insertCmd.Parameters["@TransactionType"].Value = "IN";
                    insertCmd.Parameters["@ReferenceType"].Value = (object?)dto.ReferenceType ?? DBNull.Value;
                    insertCmd.Parameters["@ReferenceId"].Value = dto.ReferenceId.HasValue ? (object)dto.ReferenceId.Value : DBNull.Value;
                    insertCmd.Parameters["@Quantity"].Value = item.Quantity;
                    insertCmd.Parameters["@QuantityBefore"].Value = before;
                    insertCmd.Parameters["@QuantityAfter"].Value = after;
                    insertCmd.Parameters["@Notes"].Value = (object?)dto.Notes ?? DBNull.Value;
                    insertCmd.Parameters["@CreatedBy"].Value = dto.UserId.HasValue ? (object)dto.UserId.Value : DBNull.Value;
                    await insertCmd.ExecuteNonQueryAsync();

                    total += lineTotal;

                    result.Items.Add(new InventoryGoodsReceiptItemResultDTO
                    {
                        ProductId = productId,
                        ProductCode = dbCode,
                        ProductName = dbName,
                        Quantity = item.Quantity,
                        UnitPrice = item.UnitPrice,
                        LineTotal = lineTotal,
                        QuantityBefore = before,
                        QuantityAfter = after
                    });
                }

                if (dto.TotalAmount.HasValue && Math.Abs(dto.TotalAmount.Value - total) > 0.01m)
                {
                    throw new InvalidOperationException($"TotalAmount không khớp. Kỳ vọng {total}.");
                }

                result.TotalAmount = total;

                await tx.CommitAsync();
                return result;
            }
            catch
            {
                await tx.RollbackAsync();
                throw;
            }
        }
    }
}

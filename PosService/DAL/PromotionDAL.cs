using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using PosService.DTO;

namespace PosService.DAL
{
    public class PromotionDAL
    {
        private readonly string _conn;

        public PromotionDAL(IConfiguration configuration)
        {
            _conn = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task<List<PromotionDTO>> GetAllAsync(bool? isActive = null)
        {
            const string baseSql = @"
                SELECT
                    PromotionID,
                    PromotionCode,
                    PromotionName,
                    Description,
                    DiscountType,
                    DiscountValue,
                    MinOrderAmount,
                    ApplyTo,
                    StartDate,
                    EndDate,
                    IsActive,
                    CreatedBy,
                    CreatedAt
                FROM Promotions
                WHERE 1 = 1";

            var sql = baseSql;
            var parameters = new List<SqlParameter>();

            if (isActive.HasValue)
            {
                sql += " AND IsActive = @IsActive";
                parameters.Add(new SqlParameter("@IsActive", SqlDbType.Bit) { Value = isActive.Value });
            }

            var list = new List<PromotionDTO>();

            using (var conn = new SqlConnection(_conn))
            using (var cmd = new SqlCommand(sql, conn))
            {
                if (parameters.Count > 0)
                {
                    cmd.Parameters.AddRange(parameters.ToArray());
                }

                await conn.OpenAsync();
                using var reader = await cmd.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    var dto = new PromotionDTO
                    {
                        PromotionId = reader["PromotionID"] != DBNull.Value ? Convert.ToInt32(reader["PromotionID"]) : 0,
                        PromotionCode = reader["PromotionCode"]?.ToString() ?? string.Empty,
                        PromotionName = reader["PromotionName"]?.ToString() ?? string.Empty,
                        Description = reader["Description"]?.ToString(),
                        DiscountType = reader["DiscountType"]?.ToString() ?? string.Empty,
                        DiscountValue = reader["DiscountValue"] != DBNull.Value ? Convert.ToDecimal(reader["DiscountValue"]) : 0m,
                        MinOrderAmount = reader["MinOrderAmount"] != DBNull.Value ? Convert.ToDecimal(reader["MinOrderAmount"]) : (decimal?)null,
                        ApplyTo = reader["ApplyTo"]?.ToString() ?? string.Empty,
                        StartDate = DateOnly.FromDateTime(reader["StartDate"] is DateTime sd ? sd : DateTime.MinValue),
                        EndDate = reader["EndDate"] != DBNull.Value
                            ? DateOnly.FromDateTime((DateTime)reader["EndDate"])
                            : (DateOnly?)null,
                        IsActive = reader["IsActive"] != DBNull.Value ? Convert.ToBoolean(reader["IsActive"]) : (bool?)null,
                        CreatedBy = reader["CreatedBy"] != DBNull.Value ? Convert.ToInt32(reader["CreatedBy"]) : (int?)null,
                        CreatedAt = reader["CreatedAt"] != DBNull.Value ? Convert.ToDateTime(reader["CreatedAt"]) : (DateTime?)null
                    };

                    list.Add(dto);
                }
            }

            if (list.Count == 0)
            {
                return list;
            }

            var dict = new Dictionary<int, PromotionDTO>();
            foreach (var p in list)
            {
                dict[p.PromotionId] = p;
            }

            using (var conn = new SqlConnection(_conn))
            {
                await conn.OpenAsync();

                const string catSql = @"
                    SELECT PromotionID, CategoryID
                    FROM PromotionCategories";

                using (var catCmd = new SqlCommand(catSql, conn))
                using (var catReader = await catCmd.ExecuteReaderAsync())
                {
                    while (await catReader.ReadAsync())
                    {
                        var pid = catReader["PromotionID"] != DBNull.Value ? Convert.ToInt32(catReader["PromotionID"]) : 0;
                        if (!dict.TryGetValue(pid, out var dto))
                        {
                            continue;
                        }

                        var cid = catReader["CategoryID"] != DBNull.Value ? Convert.ToInt32(catReader["CategoryID"]) : 0;
                        dto.CategoryIds.Add(cid);
                    }
                }

                const string prodSql = @"
                    SELECT PromotionID, ProductID
                    FROM PromotionProducts";

                using var prodCmd = new SqlCommand(prodSql, conn);
                using var prodReader = await prodCmd.ExecuteReaderAsync();

                while (await prodReader.ReadAsync())
                {
                    var pid = prodReader["PromotionID"] != DBNull.Value ? Convert.ToInt32(prodReader["PromotionID"]) : 0;
                    if (!dict.TryGetValue(pid, out var dto))
                    {
                        continue;
                    }

                    var prid = prodReader["ProductID"] != DBNull.Value ? Convert.ToInt32(prodReader["ProductID"]) : 0;
                    dto.ProductIds.Add(prid);
                }
            }

            return list;
        }

        public async Task<PromotionDTO?> GetByIdAsync(int id)
        {
            const string sql = @"
                SELECT
                    PromotionID,
                    PromotionCode,
                    PromotionName,
                    Description,
                    DiscountType,
                    DiscountValue,
                    MinOrderAmount,
                    ApplyTo,
                    StartDate,
                    EndDate,
                    IsActive,
                    CreatedBy,
                    CreatedAt
                FROM Promotions
                WHERE PromotionID = @PromotionID";

            PromotionDTO? dto = null;

            using (var conn = new SqlConnection(_conn))
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.Add(new SqlParameter("@PromotionID", SqlDbType.Int) { Value = id });

                await conn.OpenAsync();
                using var reader = await cmd.ExecuteReaderAsync();

                if (!await reader.ReadAsync())
                {
                    return null;
                }

                dto = new PromotionDTO
                {
                    PromotionId = reader["PromotionID"] != DBNull.Value ? Convert.ToInt32(reader["PromotionID"]) : 0,
                    PromotionCode = reader["PromotionCode"]?.ToString() ?? string.Empty,
                    PromotionName = reader["PromotionName"]?.ToString() ?? string.Empty,
                    Description = reader["Description"]?.ToString(),
                    DiscountType = reader["DiscountType"]?.ToString() ?? string.Empty,
                    DiscountValue = reader["DiscountValue"] != DBNull.Value ? Convert.ToDecimal(reader["DiscountValue"]) : 0m,
                    MinOrderAmount = reader["MinOrderAmount"] != DBNull.Value ? Convert.ToDecimal(reader["MinOrderAmount"]) : (decimal?)null,
                    ApplyTo = reader["ApplyTo"]?.ToString() ?? string.Empty,
                    StartDate = DateOnly.FromDateTime(reader["StartDate"] is DateTime sd ? sd : DateTime.MinValue),
                    EndDate = reader["EndDate"] != DBNull.Value
                        ? DateOnly.FromDateTime((DateTime)reader["EndDate"])
                        : (DateOnly?)null,
                    IsActive = reader["IsActive"] != DBNull.Value ? Convert.ToBoolean(reader["IsActive"]) : (bool?)null,
                    CreatedBy = reader["CreatedBy"] != DBNull.Value ? Convert.ToInt32(reader["CreatedBy"]) : (int?)null,
                    CreatedAt = reader["CreatedAt"] != DBNull.Value ? Convert.ToDateTime(reader["CreatedAt"]) : (DateTime?)null
                };
            }

            if (dto == null)
            {
                return null;
            }

            using (var conn = new SqlConnection(_conn))
            {
                await conn.OpenAsync();

                const string catSql = @"
                    SELECT CategoryID
                    FROM PromotionCategories
                    WHERE PromotionID = @PromotionID";

                using (var catCmd = new SqlCommand(catSql, conn))
                {
                    catCmd.Parameters.Add(new SqlParameter("@PromotionID", SqlDbType.Int) { Value = id });
                    using var catReader = await catCmd.ExecuteReaderAsync();
                    while (await catReader.ReadAsync())
                    {
                        var cid = catReader["CategoryID"] != DBNull.Value ? Convert.ToInt32(catReader["CategoryID"]) : 0;
                        dto.CategoryIds.Add(cid);
                    }
                }

                const string prodSql = @"
                    SELECT ProductID
                    FROM PromotionProducts
                    WHERE PromotionID = @PromotionID";

                using var prodCmd = new SqlCommand(prodSql, conn);
                prodCmd.Parameters.Add(new SqlParameter("@PromotionID", SqlDbType.Int) { Value = id });
                using var prodReader = await prodCmd.ExecuteReaderAsync();
                while (await prodReader.ReadAsync())
                {
                    var prid = prodReader["ProductID"] != DBNull.Value ? Convert.ToInt32(prodReader["ProductID"]) : 0;
                    dto.ProductIds.Add(prid);
                }
            }

            return dto;
        }

        public async Task<PromotionDTO> CreateAsync(CreatePromotionDTO dto)
        {
            if (dto == null) throw new ArgumentNullException(nameof(dto));

            const string checkSql = "SELECT COUNT(1) FROM Promotions WHERE PromotionCode = @PromotionCode";

            using (var conn = new SqlConnection(_conn))
            using (var checkCmd = new SqlCommand(checkSql, conn))
            {
                checkCmd.Parameters.Add(new SqlParameter("@PromotionCode", SqlDbType.NVarChar, 50)
                {
                    Value = dto.PromotionCode
                });

                await conn.OpenAsync();
                var exists = (int)await checkCmd.ExecuteScalarAsync() > 0;
                if (exists)
                {
                    throw new InvalidOperationException($"PromotionCode '{dto.PromotionCode}' already exists.");
                }
            }

            using var connection = new SqlConnection(_conn);
            await connection.OpenAsync();
            using var tx = await connection.BeginTransactionAsync();

            int promotionId = 0;
            DateTime? createdAt = null;
            bool? isActive = null;

            const string insertSql = @"
                INSERT INTO Promotions
                (
                    PromotionCode,
                    PromotionName,
                    Description,
                    DiscountType,
                    DiscountValue,
                    MinOrderAmount,
                    ApplyTo,
                    StartDate,
                    EndDate,
                    IsActive,
                    CreatedBy
                )
                OUTPUT INSERTED.PromotionID, INSERTED.CreatedAt, INSERTED.IsActive
                VALUES
                (
                    @PromotionCode,
                    @PromotionName,
                    @Description,
                    @DiscountType,
                    @DiscountValue,
                    @MinOrderAmount,
                    @ApplyTo,
                    @StartDate,
                    @EndDate,
                    @IsActive,
                    @CreatedBy
                )";

            using (var cmd = new SqlCommand(insertSql, connection, (SqlTransaction)tx))
            {
                cmd.Parameters.AddWithValue("@PromotionCode", dto.PromotionCode);
                cmd.Parameters.AddWithValue("@PromotionName", dto.PromotionName);
                cmd.Parameters.AddWithValue("@Description", (object?)dto.Description ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@DiscountType", dto.DiscountType);
                cmd.Parameters.AddWithValue("@DiscountValue", dto.DiscountValue);
                cmd.Parameters.AddWithValue("@MinOrderAmount", dto.MinOrderAmount.HasValue ? (object)dto.MinOrderAmount.Value : DBNull.Value);
                cmd.Parameters.AddWithValue("@ApplyTo", dto.ApplyTo);
                cmd.Parameters.AddWithValue("@StartDate", dto.StartDate.ToDateTime(TimeOnly.MinValue));
                cmd.Parameters.AddWithValue("@EndDate", dto.EndDate.HasValue ? (object)dto.EndDate.Value.ToDateTime(TimeOnly.MinValue) : DBNull.Value);
                cmd.Parameters.AddWithValue("@IsActive", dto.IsActive ?? true);
                cmd.Parameters.AddWithValue("@CreatedBy", dto.CreatedBy.HasValue ? (object)dto.CreatedBy.Value : DBNull.Value);

                using var reader = await cmd.ExecuteReaderAsync();
                if (await reader.ReadAsync())
                {
                    promotionId = reader["PromotionID"] != DBNull.Value ? Convert.ToInt32(reader["PromotionID"]) : 0;
                    createdAt = reader["CreatedAt"] != DBNull.Value ? Convert.ToDateTime(reader["CreatedAt"]) : (DateTime?)null;
                    isActive = reader["IsActive"] != DBNull.Value ? Convert.ToBoolean(reader["IsActive"]) : (bool?)null;
                }
            }

            if (promotionId <= 0)
            {
                await tx.RollbackAsync();
                throw new InvalidOperationException("Failed to create promotion.");
            }

            if (dto.CategoryIds != null && dto.CategoryIds.Count > 0)
            {
                const string catInsertSql = @"
                    INSERT INTO PromotionCategories
                    (
                        PromotionID,
                        CategoryID
                    )
                    VALUES
                    (
                        @PromotionID,
                        @CategoryID
                    )";

                using var catCmd = new SqlCommand(catInsertSql, connection, (SqlTransaction)tx);
                catCmd.Parameters.Add(new SqlParameter("@PromotionID", SqlDbType.Int) { Value = promotionId });
                var catParam = catCmd.Parameters.Add("@CategoryID", SqlDbType.Int);

                foreach (var cid in dto.CategoryIds)
                {
                    catParam.Value = cid;
                    await catCmd.ExecuteNonQueryAsync();
                }
            }

            if (dto.ProductIds != null && dto.ProductIds.Count > 0)
            {
                const string prodInsertSql = @"
                    INSERT INTO PromotionProducts
                    (
                        PromotionID,
                        ProductID
                    )
                    VALUES
                    (
                        @PromotionID,
                        @ProductID
                    )";

                using var prodCmd = new SqlCommand(prodInsertSql, connection, (SqlTransaction)tx);
                prodCmd.Parameters.Add(new SqlParameter("@PromotionID", SqlDbType.Int) { Value = promotionId });
                var prodParam = prodCmd.Parameters.Add("@ProductID", SqlDbType.Int);

                foreach (var pid in dto.ProductIds)
                {
                    prodParam.Value = pid;
                    await prodCmd.ExecuteNonQueryAsync();
                }
            }

            await tx.CommitAsync();

            var result = new PromotionDTO
            {
                PromotionId = promotionId,
                PromotionCode = dto.PromotionCode,
                PromotionName = dto.PromotionName,
                Description = dto.Description,
                DiscountType = dto.DiscountType,
                DiscountValue = dto.DiscountValue,
                MinOrderAmount = dto.MinOrderAmount,
                ApplyTo = dto.ApplyTo,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate,
                IsActive = isActive,
                CreatedBy = dto.CreatedBy,
                CreatedAt = createdAt,
                CategoryIds = dto.CategoryIds ?? new List<int>(),
                ProductIds = dto.ProductIds ?? new List<int>()
            };

            return result;
        }
    }
}


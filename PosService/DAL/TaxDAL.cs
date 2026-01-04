using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using PosService.DTO;

namespace PosService.DAL
{
    public class TaxDAL
    {
        private readonly string _conn;

        public TaxDAL(IConfiguration configuration)
        {
            _conn = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task<List<TaxDTO>> GetAllAsync(bool? isActive = null, string? q = null)
        {
            const string baseSql = @"
                SELECT 
                    TaxID,
                    TaxCode,
                    TaxName,
                    TaxRate,
                    Description,
                    IsActive,
                    CreatedAt
                FROM Taxes
                WHERE 1 = 1";

            var sql = baseSql;
            var parameters = new List<SqlParameter>();

            if (isActive.HasValue)
            {
                sql += " AND IsActive = @IsActive";
                parameters.Add(new SqlParameter("@IsActive", SqlDbType.Bit) { Value = isActive.Value });
            }

            if (!string.IsNullOrWhiteSpace(q))
            {
                var keyword = q.Trim();
                sql += " AND (TaxCode LIKE @Q OR TaxName LIKE @Q)";
                parameters.Add(new SqlParameter("@Q", SqlDbType.NVarChar, 255) { Value = "%" + keyword + "%" });
            }

            sql += " ORDER BY TaxCode";

            var list = new List<TaxDTO>();

            using var conn = new SqlConnection(_conn);
            using var cmd = new SqlCommand(sql, conn);
            if (parameters.Count > 0)
            {
                cmd.Parameters.AddRange(parameters.ToArray());
            }

            await conn.OpenAsync();
            using var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                var item = new TaxDTO
                {
                    TaxId = reader["TaxID"] != DBNull.Value ? Convert.ToInt32(reader["TaxID"]) : 0,
                    TaxCode = reader["TaxCode"]?.ToString() ?? string.Empty,
                    TaxName = reader["TaxName"]?.ToString() ?? string.Empty,
                    TaxRate = reader["TaxRate"] != DBNull.Value ? Convert.ToDecimal(reader["TaxRate"]) : 0m,
                    Description = reader["Description"]?.ToString(),
                    IsActive = reader["IsActive"] != DBNull.Value ? Convert.ToBoolean(reader["IsActive"]) : (bool?)null,
                    CreatedAt = reader["CreatedAt"] != DBNull.Value ? Convert.ToDateTime(reader["CreatedAt"]) : (DateTime?)null
                };

                list.Add(item);
            }

            return list;
        }

        public async Task<TaxDTO?> GetByIdAsync(int id)
        {
            const string sql = @"
                SELECT 
                    TaxID,
                    TaxCode,
                    TaxName,
                    TaxRate,
                    Description,
                    IsActive,
                    CreatedAt
                FROM Taxes
                WHERE TaxID = @TaxID";

            using var conn = new SqlConnection(_conn);
            using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.Add(new SqlParameter("@TaxID", SqlDbType.Int) { Value = id });

            await conn.OpenAsync();
            using var reader = await cmd.ExecuteReaderAsync();

            if (!await reader.ReadAsync())
            {
                return null;
            }

            var item = new TaxDTO
            {
                TaxId = reader["TaxID"] != DBNull.Value ? Convert.ToInt32(reader["TaxID"]) : 0,
                TaxCode = reader["TaxCode"]?.ToString() ?? string.Empty,
                TaxName = reader["TaxName"]?.ToString() ?? string.Empty,
                TaxRate = reader["TaxRate"] != DBNull.Value ? Convert.ToDecimal(reader["TaxRate"]) : 0m,
                Description = reader["Description"]?.ToString(),
                IsActive = reader["IsActive"] != DBNull.Value ? Convert.ToBoolean(reader["IsActive"]) : (bool?)null,
                CreatedAt = reader["CreatedAt"] != DBNull.Value ? Convert.ToDateTime(reader["CreatedAt"]) : (DateTime?)null
            };

            return item;
        }

        public async Task<TaxDTO> CreateAsync(CreateTaxDTO dto)
        {
            if (dto == null) throw new ArgumentNullException(nameof(dto));

            const string insertSql = @"
                INSERT INTO Taxes
                (
                    TaxCode,
                    TaxName,
                    TaxRate,
                    Description,
                    IsActive
                )
                OUTPUT INSERTED.TaxID, INSERTED.CreatedAt, INSERTED.IsActive
                VALUES
                (
                    @TaxCode,
                    @TaxName,
                    @TaxRate,
                    @Description,
                    @IsActive
                )";

            using var conn = new SqlConnection(_conn);
            using var cmd = new SqlCommand(insertSql, conn);
            cmd.Parameters.AddWithValue("@TaxCode", dto.TaxCode);
            cmd.Parameters.AddWithValue("@TaxName", dto.TaxName);
            cmd.Parameters.AddWithValue("@TaxRate", dto.TaxRate);
            cmd.Parameters.AddWithValue("@Description", (object?)dto.Description ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@IsActive", dto.IsActive ?? true);

            await conn.OpenAsync();
            using var reader = await cmd.ExecuteReaderAsync();

            var result = new TaxDTO();

            if (await reader.ReadAsync())
            {
                result.TaxId = reader["TaxID"] != DBNull.Value ? Convert.ToInt32(reader["TaxID"]) : 0;
                result.TaxCode = dto.TaxCode;
                result.TaxName = dto.TaxName;
                result.TaxRate = dto.TaxRate;
                result.Description = dto.Description;
                result.IsActive = reader["IsActive"] != DBNull.Value ? Convert.ToBoolean(reader["IsActive"]) : (bool?)null;
                result.CreatedAt = reader["CreatedAt"] != DBNull.Value ? Convert.ToDateTime(reader["CreatedAt"]) : (DateTime?)null;
            }

            return result;
        }

        public async Task<TaxDTO?> UpdateAsync(int id, UpdateTaxDTO dto)
        {
            if (dto == null) throw new ArgumentNullException(nameof(dto));

            var existing = await GetByIdAsync(id);
            if (existing == null) return null;

            var taxCode = dto.TaxCode ?? existing.TaxCode;
            var taxName = dto.TaxName ?? existing.TaxName;
            var taxRate = dto.TaxRate ?? existing.TaxRate;
            var description = dto.Description ?? existing.Description;
            var isActive = dto.IsActive ?? existing.IsActive ?? true;

            const string updateSql = @"
                UPDATE Taxes
                SET
                    TaxCode = @TaxCode,
                    TaxName = @TaxName,
                    TaxRate = @TaxRate,
                    Description = @Description,
                    IsActive = @IsActive
                WHERE TaxID = @TaxID";

            using var conn = new SqlConnection(_conn);
            using var cmd = new SqlCommand(updateSql, conn);
            cmd.Parameters.AddWithValue("@TaxID", id);
            cmd.Parameters.AddWithValue("@TaxCode", taxCode);
            cmd.Parameters.AddWithValue("@TaxName", taxName);
            cmd.Parameters.AddWithValue("@TaxRate", taxRate);
            cmd.Parameters.AddWithValue("@Description", (object?)description ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@IsActive", isActive);

            await conn.OpenAsync();
            var rows = await cmd.ExecuteNonQueryAsync();
            if (rows <= 0)
            {
                return null;
            }

            var updated = await GetByIdAsync(id);
            return updated;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            const string sql = @"
                UPDATE Taxes
                SET IsActive = 0
                WHERE TaxID = @TaxID";

            using var conn = new SqlConnection(_conn);
            using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.Add(new SqlParameter("@TaxID", SqlDbType.Int) { Value = id });

            await conn.OpenAsync();
            var rows = await cmd.ExecuteNonQueryAsync();
            return rows > 0;
        }
    }
}


using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using PosService.DTO;

namespace PosService.DAL
{
    public class CategoryDAL
    {
        private readonly string _conn;

        public CategoryDAL(IConfiguration configuration)
        {
            _conn = configuration.GetConnectionString("DefaultConnection");
        }


        public async Task<List<CategoryDTO>> GetAllAsync(bool? isActive = null)
        {
            const string baseSql = @"
                SELECT 
                    CategoryID,
                    CategoryName,
                    Description,
                    IsActive
                FROM Categories
                WHERE 1 = 1";

            var sql = baseSql;
            var parameters = new List<SqlParameter>();

            if (isActive.HasValue)
            {
                sql += " AND IsActive = @IsActive";
                parameters.Add(new SqlParameter("@IsActive", SqlDbType.Bit) { Value = isActive.Value });
            }

            var list = new List<CategoryDTO>();

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
                var item = new CategoryDTO
                {
                    CategoryId = reader["CategoryID"] != DBNull.Value ? Convert.ToInt32(reader["CategoryID"]) : 0,
                    CategoryName = reader["CategoryName"]?.ToString() ?? string.Empty,
                    Description = reader["Description"]?.ToString(),
                    IsActive = reader["IsActive"] != DBNull.Value ? Convert.ToBoolean(reader["IsActive"]) : (bool?)null
                };

                list.Add(item);
            }

            return list;
        }

        public async Task<CategoryDTO?> GetByIdAsync(int id)
        {
            const string sql = @"
                SELECT 
                    CategoryID,
                    CategoryName,
                    Description,
                    IsActive
                FROM Categories
                WHERE CategoryID = @CategoryID";

            using var conn = new SqlConnection(_conn);
            using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.Add(new SqlParameter("@CategoryID", SqlDbType.Int) { Value = id });

            await conn.OpenAsync();
            using var reader = await cmd.ExecuteReaderAsync();

            if (!await reader.ReadAsync())
            {
                return null;
            }

            var item = new CategoryDTO
            {
                CategoryId = reader["CategoryID"] != DBNull.Value ? Convert.ToInt32(reader["CategoryID"]) : 0,
                CategoryName = reader["CategoryName"]?.ToString() ?? string.Empty,
                Description = reader["Description"]?.ToString(),
                IsActive = reader["IsActive"] != DBNull.Value ? Convert.ToBoolean(reader["IsActive"]) : (bool?)null
            };

            return item;
        }

        public async Task<CategoryDTO> CreateAsync(CreateCategoryDTO dto)
        {
            if (dto == null) throw new ArgumentNullException(nameof(dto));

            const string insertSql = @"
                INSERT INTO Categories
                (
                    CategoryName,
                    Description,
                    IsActive
                )
                OUTPUT INSERTED.CategoryID, INSERTED.IsActive
                VALUES
                (
                    @CategoryName,
                    @Description,
                    @IsActive
                )";

            var result = new CategoryDTO();

            using var conn = new SqlConnection(_conn);
            using var cmd = new SqlCommand(insertSql, conn);
            cmd.Parameters.AddWithValue("@CategoryName", dto.CategoryName);
            cmd.Parameters.AddWithValue("@Description", (object?)dto.Description ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@IsActive", dto.IsActive ?? true);

            await conn.OpenAsync();
            using var reader = await cmd.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                result.CategoryId = reader["CategoryID"] != DBNull.Value ? Convert.ToInt32(reader["CategoryID"]) : 0;
                result.CategoryName = dto.CategoryName;
                result.Description = dto.Description;
                result.IsActive = reader["IsActive"] != DBNull.Value ? Convert.ToBoolean(reader["IsActive"]) : (bool?)null;
            }

            return result;
        }

        public async Task<CategoryDTO?> UpdateAsync(int id, UpdateCategoryDTO dto)
        {
            if (dto == null) throw new ArgumentNullException(nameof(dto));

            var existing = await GetByIdAsync(id);
            if (existing == null) return null;

            var name = dto.CategoryName ?? existing.CategoryName;
            var description = dto.Description ?? existing.Description;
            var isActive = dto.IsActive ?? existing.IsActive ?? true;

            const string updateSql = @"
                UPDATE Categories
                SET
                    CategoryName = @CategoryName,
                    Description = @Description,
                    IsActive = @IsActive
                WHERE CategoryID = @CategoryID";

            using var conn = new SqlConnection(_conn);
            using var cmd = new SqlCommand(updateSql, conn);
            cmd.Parameters.AddWithValue("@CategoryID", id);
            cmd.Parameters.AddWithValue("@CategoryName", name);
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
            const string sql = "DELETE FROM Categories WHERE CategoryID = @CategoryID";

            using var conn = new SqlConnection(_conn);
            using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.Add(new SqlParameter("@CategoryID", SqlDbType.Int) { Value = id });

            await conn.OpenAsync();
            var rows = await cmd.ExecuteNonQueryAsync();
            return rows > 0;
        }
    }
}

using Microsoft.Data.SqlClient;
using DTO;

namespace DAL
{
    public class dal_category
    {
        private string _conn = "Server=DESKTOP-TG67AE9;Database=HDV;Trusted_Connection=True;TrustServerCertificate=True;";
        public List<dto_Categories> GetAll()
        {
            var list = new List<dto_Categories>();

            SqlConnection sqlConnection = new(_conn);
            using var conn = sqlConnection;
            conn.Open();

            var cmd = new SqlCommand("SELECT * FROM Categories", conn);

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                var category = new dto_Categories
                {
                    CategoryId = reader["CategoryId"] != DBNull.Value ? Convert.ToInt32(reader["CategoryId"]) : 0,
                    CategoryName = reader["CategoryName"]?.ToString(),
                    Description = reader["Description"]?.ToString(),
                    IsActive = reader["IsActive"] != DBNull.Value && Convert.ToBoolean(reader["IsActive"])
                };

                list.Add(category);
            }

            return list;
        }
        public bool CreateCategory(dto_Categories category)
        {
            using var conn = new SqlConnection(_conn);
            conn.Open();
            var cmd = new SqlCommand(@"
                INSERT INTO Categories
                (CategoryName, Description, IsActive)
                VALUES(@name, @description, @isActive)", conn);
            cmd.Parameters.AddWithValue("@name", category.CategoryName);
            cmd.Parameters.AddWithValue("@description", category.Description ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@isActive", category.IsActive ?? true);
            int row = cmd.ExecuteNonQuery();
            return row > 0;
        }
        public bool DeleteCaregori(int CategoryID)
        {
            using var conn = new SqlConnection(_conn);
            conn.Open();

            var cmd = new SqlCommand(
                "DELETE FROM Categories WHERE CategoryID = @CategoryID",
                conn
            );

            cmd.Parameters.AddWithValue("@CategoryID", CategoryID);

            int row = cmd.ExecuteNonQuery();
            return row > 0;
        }



    }
}

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


    }
}

using Microsoft.Data.SqlClient;
using DTO;
namespace DAL
{
    public class dal_Products
    {
        private string _conn = "Server=DESKTOP-TG67AE9;Database=HDV;Trusted_Connection=True;TrustServerCertificate=True;";
        public List<dto_Products> GetAllProducts()
        {
            var list = new List<dto_Products>();

            SqlConnection sqlConnection = new(_conn);
            using var conn = sqlConnection;
            conn.Open();

            var cmd = new SqlCommand("SELECT * FROM Products", conn);

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                var product = new dto_Products
                {
                    ProductID = reader["ProductID"] != DBNull.Value
                                ? Convert.ToInt32(reader["ProductID"]) : 0,

                    ProductCode = reader["ProductCode"]?.ToString(),
                    Barcode = reader["Barcode"]?.ToString(),
                    ProductName = reader["ProductName"]?.ToString(),

                    CategoryID = reader["CategoryID"] != DBNull.Value
                                ? Convert.ToInt32(reader["CategoryID"]) : 0,

                    SupplierID = reader["SupplierID"] != DBNull.Value
                                ? Convert.ToInt32(reader["SupplierID"]) : 0,

                    Unit = reader["Unit"]?.ToString(),

                    CostPrice = reader["CostPrice"] != DBNull.Value
                                ? Convert.ToDecimal(reader["CostPrice"]) : 0,

                    SellingPrice = reader["SellingPrice"] != DBNull.Value
                                ? Convert.ToDecimal(reader["SellingPrice"]) : 0,

                    StockQuantity = reader["StockQuantity"] != DBNull.Value
                                ? Convert.ToInt32(reader["StockQuantity"]) : 0,

                    MinStock = reader["MinStock"] != DBNull.Value
                                ? Convert.ToInt32(reader["MinStock"]) : 0,

                    ImageURL = reader["ImageURL"]?.ToString(),

                    IsActive = reader["IsActive"] != DBNull.Value
                                && Convert.ToBoolean(reader["IsActive"]),

                    CreatedAt = reader["CreatedAt"] != DBNull.Value
                                ? Convert.ToDateTime(reader["CreatedAt"])
                                : DateTime.MinValue
                };

                list.Add(product);
            }

            return list;
        }

    }
}

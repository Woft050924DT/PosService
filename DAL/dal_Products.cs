using DTO;
using Microsoft.Data.SqlClient;
using System.Data;
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
        public bool UpdateProduct(dto_Products p)
        {
            using var conn = new SqlConnection(_conn);
            conn.Open();

            using var cmd = new SqlCommand("sp_UpdateProduct", conn);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@ProductID", p.ProductID);
            cmd.Parameters.AddWithValue("@ProductCode", p.ProductCode);
            cmd.Parameters.AddWithValue("@Barcode", p.Barcode);
            cmd.Parameters.AddWithValue("@ProductName", p.ProductName);
            cmd.Parameters.AddWithValue("@CategoryID", p.CategoryID);
            cmd.Parameters.AddWithValue("@SupplierID", p.SupplierID);
            cmd.Parameters.AddWithValue("@Unit", p.Unit);
            cmd.Parameters.AddWithValue("@CostPrice", p.CostPrice);
            cmd.Parameters.AddWithValue("@SellingPrice", p.SellingPrice);
            cmd.Parameters.AddWithValue("@StockQuantity", p.StockQuantity);
            cmd.Parameters.AddWithValue("@MinStock", p.MinStock);
            cmd.Parameters.AddWithValue("@ImageURL", p.ImageURL);
            cmd.Parameters.AddWithValue("@IsActive", p.IsActive);
            int row = cmd.ExecuteNonQuery();
            return row > 0;
        }
        public bool DeleteProduct(int productId)
        {
            using var conn = new SqlConnection(_conn);
            conn.Open();

            using var cmd = new SqlCommand("sp_DeleteProduct", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@ProductID", productId);

            return cmd.ExecuteNonQuery() > 0;
        }



    }
}

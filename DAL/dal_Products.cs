using DTO;
using Microsoft.Data.SqlClient;
using System.Data;
using Microsoft.Extensions.Options;
namespace DAL
{
    public class dal_Products
    {
        private readonly appSetting _appSettings;
        private string _conn;

        public dal_Products(IOptions<ConnectionStrings> options)
        {
            _conn = options.Value.DefaultConnection;
        }
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

            int row = cmd.ExecuteNonQuery();
            return row > 0;
        }
        public bool AddProduct(dto_Products p)
        {
            using var conn = new SqlConnection(_conn);
            using var cmd = new SqlCommand("sp_AddProduct", conn);

            cmd.CommandType = CommandType.StoredProcedure;
            
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

            conn.Open();
            int rows = cmd.ExecuteNonQuery();

            return rows > 0;

        }

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

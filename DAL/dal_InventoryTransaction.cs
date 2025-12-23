using DTO;
using Microsoft.Data.SqlClient;
using System.Data;
using Microsoft.Extensions.Options;
namespace DAL
{
    public class dal_InventoryTransaction
    {
        private readonly appSetting _appSettings;
        private string _conn;

        public dal_InventoryTransaction(IOptions<ConnectionStrings> options)
        {
            _conn = options.Value.DefaultConnection;
        }

        public List<dto_InventoryTransaction> GetAll()
        {
            var list = new List<dto_InventoryTransaction>();

            using var conn = new SqlConnection(_conn);
            using var cmd = new SqlCommand("sp_GetAllInventoryTransaction", conn);
            cmd.CommandType = CommandType.StoredProcedure;

            conn.Open();
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                list.Add(new dto_InventoryTransaction
                {
                    TransactionId = (int)reader["TransactionId"],
                    ProductId = reader["ProductId"] as int?,
                    TransactionType = reader["TransactionType"].ToString(),
                    ReferenceType = reader["ReferenceType"].ToString(),
                    ReferenceId = reader["ReferenceId"] as int?,
                    Quantity = (int)reader["Quantity"],
                    QuantityBefore = reader["QuantityBefore"] as int?,
                    QuantityAfter = reader["QuantityAfter"] as int?,
                    Notes = reader["Notes"].ToString(),
                    CreatedAt = reader["CreatedAt"] as DateTime?,
                    CreatedBy = reader["CreatedBy"] as int?
                });
            }
            return list;
        }
        public bool Add(dto_InventoryTransaction t)
        {
            using var conn = new SqlConnection(_conn);
            using var cmd = new SqlCommand("sp_AddInventoryTransaction", conn);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@ProductId", t.ProductId);
            cmd.Parameters.AddWithValue("@TransactionType", t.TransactionType);
            cmd.Parameters.AddWithValue("@ReferenceType", t.ReferenceType);
            cmd.Parameters.AddWithValue("@ReferenceId", t.ReferenceId);
            cmd.Parameters.AddWithValue("@Quantity", t.Quantity);
            cmd.Parameters.AddWithValue("@QuantityBefore", t.QuantityBefore);
            cmd.Parameters.AddWithValue("@QuantityAfter", t.QuantityAfter);
            cmd.Parameters.AddWithValue("@Notes", t.Notes);
            cmd.Parameters.AddWithValue("@CreatedBy", t.CreatedBy);

            conn.Open();
            int row = cmd.ExecuteNonQuery();
            return row > 0;
        }
    }
}

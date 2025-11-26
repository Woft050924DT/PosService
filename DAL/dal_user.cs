using Microsoft.Data.SqlClient;
using DTO;

namespace DAL
{
    public class dal_user
    {
        private string _conn = "Server=DESKTOP-EU1MAAC\\SQLEXPRESS;Database=HDV;Trusted_Connection=True;TrustServerCertificate=True;";

        public dto_user GetUser(string username)
        {
            using var conn = new SqlConnection(_conn);
            conn.Open();

            var cmd = new SqlCommand(@"
                SELECT u.*, r.RoleName, r.Description
                FROM Users u
                LEFT JOIN Roles r ON u.RoleID = r.RoleID
                WHERE u.Username=@u", conn);

            cmd.Parameters.AddWithValue("@u", username);
            var reader = cmd.ExecuteReader();

            if (reader.Read())
            {
                return new dto_user
                {
                    UserID = (int)reader["UserID"],
                    Username = reader["Username"].ToString(),
                    Password = reader["PasswordHash"].ToString(),
                    FullName = reader["FullName"].ToString(),
                    Phone = reader["Phone"].ToString(),
                    RoleID = (int)reader["RoleID"],
                    IsActive = (bool)reader["IsActive"],
                    CreatedAt = (DateTime)reader["CreatedAt"],
                    Role = new dto_role
                    {
                        RoleID = (int)reader["RoleID"],
                        RoleName = reader["RoleName"].ToString(),
                        Description = reader["Description"].ToString()
                    }
                };
            }
            return null;
        }

        public bool CreateUser(dto_user user)
        {
            using var conn = new SqlConnection(_conn);
            conn.Open();

            var cmd = new SqlCommand(@"
                INSERT INTO Users
                (Username, PasswordHash, FullName, Phone, RoleID, IsActive, CreatedAt)
                VALUES(@username,@password,@fullname,@phone,@role,@isActive,@createdAt)", conn);

            cmd.Parameters.AddWithValue("@username", user.Username);
            cmd.Parameters.AddWithValue("@password", user.Password);
            cmd.Parameters.AddWithValue("@fullname", user.FullName ?? "");
            cmd.Parameters.AddWithValue("@phone", user.Phone ?? "");
            cmd.Parameters.AddWithValue("@role", user.RoleID);
            cmd.Parameters.AddWithValue("@isActive", user.IsActive);
            cmd.Parameters.AddWithValue("@createdAt", DateTime.Now);

            return cmd.ExecuteNonQuery() > 0;
        }

    }
}

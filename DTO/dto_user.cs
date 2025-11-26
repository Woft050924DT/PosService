using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO
{
    public class dto_user
    {
        public int UserID { get; set; }
        public string Username { get; set; }
        public string Password { get; set; } // input password
        public string FullName { get; set; }
        public string Phone { get; set; }
        public int RoleID { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }

        public dto_role Role { get; set; }
    }
}

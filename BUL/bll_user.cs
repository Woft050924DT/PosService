using System.Text;
using DAL;
using DTO;

using System.Security.Cryptography;
using System.Text;
namespace BLL
{
    public class bll_user
    {
        private dal_user dal = new dal_user();

        private string HashPassword(string password)
        {
            using var sha = SHA256.Create();
            return Convert.ToBase64String(sha.ComputeHash(Encoding.UTF8.GetBytes(password)));
        }

        public bool Register(dto_user user)
        {
            if (dal.GetUser(user.Username) != null) return false;

            user.Password = HashPassword(user.Password);
            user.IsActive = true;
            user.CreatedAt = DateTime.Now;
            return dal.CreateUser(user);
        }

        public dto_user Login(string username, string password)
        {
            var user = dal.GetUser(username);
            if (user == null) return null;

            if (user.Password == HashPassword(password) && user.IsActive)
                return user;

            return null;

        }
    }
}

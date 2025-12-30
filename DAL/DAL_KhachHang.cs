using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL.Models;
using DTO;

namespace DAL
{
    public class DAL_KhachHang
    {
        private DTO_I_DBHelper dbHelper;
        public  DAL_KhachHang(DTO_I_DBHelper _dbHelper)
        {
            dbHelper = _dbHelper;
        }
        public bool createKhachHang(Customer model)
        {
            string msgError = string.Empty;
            try
            {
                DataTable result = dbHelper.ExecuteSProcedureReturnDataTable(
                    out msgError,
                    "createKhachHang",
                    "@CustomerCode", model.CustomerCode,
                    "@FullName", model.FullName,
                    "@Phone", model.Phone,
                    "@Email", model.Email,
                    "@Address", model.Address,
                    "@Points", model.Points,
                    "@CreatedAt", model.CreatedAt
                );

                if ((result != null && !string.IsNullOrEmpty(result.ToString())) || !string.IsNullOrEmpty(msgError))
                {
                    throw new Exception(Convert.ToString(result) + msgError);
                }

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception("Error in DAL: " + ex.Message, ex);
            }
        }
    }
}

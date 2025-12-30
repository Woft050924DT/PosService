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
        public DataTable selectKhachHangWithID(Customer model) {
           string msgError = string.Empty;
            try
            {
                DataTable result = dbHelper.ExecuteSProcedureReturnDataTable(
                    out msgError, "selectKhachHangWithID", "@CustomerId", model.CustomerId);
                if ((result != null && !string.IsNullOrEmpty(result.ToString())) || !string.IsNullOrEmpty(msgError))
                {
                    throw new Exception(Convert.ToString(result) + msgError);
                }
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception("Error in DAL: " + ex.Message, ex);
            }
        }
        //chọn sửa thì sẽ dùng api lấy dữ liệu và thêm vào các trường( có api get-by-id gưi json lên client), sau đó dùng api sửa để lưu lại
        public DataTable updateKhachHang(Customer model)
        {
            string msgError = string.Empty;
            try
            {
                DataTable result = dbHelper.ExecuteSProcedureReturnDataTable(
                    out msgError,
                    "updateKhachHang",
                    "@CustomerId", model.CustomerId,
                    "@CustomerCode", model.CustomerCode,
                    "@FullName", model.FullName,
                    "@Phone", model.Phone,
                    "@Email", model.Email,
                    "@Address", model.Address,
                    "@Points", model.Points
                );
                if ((result != null && !string.IsNullOrEmpty(result.ToString())) || !string.IsNullOrEmpty(msgError))
                {
                    throw new Exception(Convert.ToString(result) + msgError);
                }
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception("Error in DAL: " + ex.Message, ex);
            }
        }

        public bool deleteKhachHang(Customer model)
        {
            string msgError = string.Empty;
            try
            {
                DataTable result = dbHelper.ExecuteSProcedureReturnDataTable(
                    out msgError,
                    "deleteKhachHang",
                    "@CustomerId", model.CustomerId
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

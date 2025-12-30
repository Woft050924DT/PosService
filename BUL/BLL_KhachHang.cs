using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL;
using DAL.Models;
using DTO;

namespace BLL
{
    public class BLL_KhachHang
    {
        private DAL_KhachHang DAL_KhachHang;
        public BLL_KhachHang(DAL_KhachHang _DAL_KhachHang) {
            DAL_KhachHang= _DAL_KhachHang;
        }
        public bool createKhachHang(Customer model)
        {
            try
            {
                return DAL_KhachHang.createKhachHang(model);
            }
            catch (Exception ex)
            {
                throw new Exception("Error in BLL: " + ex.Message, ex);
            }
        }
    }
}

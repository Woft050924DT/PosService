using DTO;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;


namespace BLL
{
    public class bll_Products
    {
        private DAL.dal_Products dal = new DAL.dal_Products();
        public List<DTO.dto_Products> GetAllProducts()
        {
            return dal.GetAllProducts();
        }
        public bool UpdateProduct(dto_Products p)
        {
            if (p.ProductID <= 0)
                throw new Exception("ProductID không hợp lệ");

            if (string.IsNullOrEmpty(p.ProductName))
                throw new Exception("ProductName không được để trống");

            return dal.UpdateProduct(p);
        }

        public bool DeleteProduct(int productId)
        {
            if (productId <= 0)
                throw new Exception("ProductID không hợp lệ");

            return dal.DeleteProduct(productId);
        }

        

    }
}
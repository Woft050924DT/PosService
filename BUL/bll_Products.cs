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
    }
}

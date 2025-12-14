using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace BLL
{
    public class bll_Categories
    {
        private DAL.dal_category dal = new DAL.dal_category();
        public List<DTO.dto_Categories> GetAllCategories()
        {
            return dal.GetAll();
        }
        public bool CreateCategory(DTO.dto_Categories category)
        {
            return dal.CreateCategory(category);
        }
        public bool DeleteCategory(int CategoryID)
        {
            return dal.DeleteCaregori(CategoryID);
        }
        public bool UpdateCategory(DTO.dto_Categories category)
        {
            if (category.CategoryId <= 0)
                throw new Exception("CategoryId không hợp lệ");

            if (string.IsNullOrEmpty(category.CategoryName))
                throw new Exception("CategoryName không được để trống");
            return dal.UpdateCategori(category);
        }
    }
}

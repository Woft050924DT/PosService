using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }
}

using System;
using System.Collections.Generic;
using PosService.DAL;
using PosService.DTO;

namespace BLL
{
    public class bll_Category
    {
        private readonly CategoryDAL dal;

        public bll_Category(CategoryDAL categoryDal)
        {
            dal = categoryDal;
        }

        public List<CategoryDTO> GetAllCategory(bool? isActive = null)
        {
            return dal.GetAllAsync(isActive).GetAwaiter().GetResult();
        }

        public CategoryDTO? GetCategoryById(int id)
        {
            if (id <= 0)
                throw new Exception("CategoryId không hợp lệ");

            return dal.GetByIdAsync(id).GetAwaiter().GetResult();
        }

        public bool CreateCategory(CreateCategoryDTO dto)
        {
            if (dto == null)
                throw new Exception("Dữ liệu danh mục không được để trống");

            if (string.IsNullOrEmpty(dto.CategoryName))
                throw new Exception("CategoryName không được để trống");

            var created = dal.CreateAsync(dto).GetAwaiter().GetResult();
            return created != null && created.CategoryId > 0;
        }

        public bool UpdateCategory(int id, UpdateCategoryDTO dto)
        {
            if (id <= 0)
                throw new Exception("CategoryId không hợp lệ");

            if (dto == null)
                throw new Exception("Dữ liệu danh mục không được để trống");

            var updated = dal.UpdateAsync(id, dto).GetAwaiter().GetResult();
            return updated != null;
        }

        public bool DeleteCategory(int id)
        {
            if (id <= 0)
                throw new Exception("CategoryId không hợp lệ");

            return dal.DeleteAsync(id).GetAwaiter().GetResult();
        }
    }
}


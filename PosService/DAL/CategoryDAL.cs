using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PosService.DTO;
using PosService.Models;

namespace PosService.DAL
{
    public class CategoryDAL
    {
        private readonly HdvContext _db;

        public CategoryDAL(HdvContext db)
        {
            _db = db;
        }


        public async Task<List<CategoryDTO>> GetAllAsync(bool? isActive = null)
        {
            var query = _db.Categories.AsNoTracking().AsQueryable();

            if (isActive.HasValue)
            {
                query = query.Where(c => c.IsActive == isActive.Value);
            }

            return await query
                .Select(c => new CategoryDTO
                {
                    CategoryId = c.CategoryId,
                    CategoryName = c.CategoryName,
                    Description = c.Description,
                    IsActive = c.IsActive
                })
                .ToListAsync();
        }

        public async Task<CategoryDTO?> GetByIdAsync(int id)
        {
            var c = await _db.Categories.AsNoTracking().FirstOrDefaultAsync(x => x.CategoryId == id);
            if (c == null) return null;

            return new CategoryDTO
            {
                CategoryId = c.CategoryId,
                CategoryName = c.CategoryName,
                Description = c.Description,
                IsActive = c.IsActive
            };
        }
    }
}
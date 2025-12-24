namespace PosService.DTO
{
    public class CategoryDTO
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = null!;
        public string? Description { get; set; }
        public bool? IsActive { get; set; }
    }

    public class CreateCategoryDTO
    {
        public string CategoryName { get; set; } = null!;
        public string? Description { get; set; }
        public bool? IsActive { get; set; }
    }

    public class UpdateCategoryDTO
    {
        public string? CategoryName { get; set; }
        public string? Description { get; set; }
        public bool? IsActive { get; set; }
    }


}

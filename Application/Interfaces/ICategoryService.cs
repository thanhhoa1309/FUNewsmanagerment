using Repository.DTOs.Category;

namespace Application.Interfaces
{
    public interface ICategoryService
    {
        Task<List<CategoryResponseDTO>> GetAllCategoriesAsync();
        Task<CategoryResponseDTO?> GetCategoryByIdAsync(int categoryId);
        Task<(bool Success, string Message)> CreateCategoryAsync(CategoryCreateDTO categoryDto);
        Task<(bool Success, string Message)> UpdateCategoryAsync(int categoryId, CategoryUpdateDTO categoryDto);
        Task<(bool Success, string Message)> DeleteCategoryAsync(int categoryId);
        Task<List<CategoryResponseDTO>> SearchCategoriesAsync(string searchTerm);
        Task<bool> CanDeleteCategoryAsync(int categoryId);
        Task<List<CategoryResponseDTO>> GetSubCategoriesAsync(int parentCategoryId);
        Task<List<CategoryResponseDTO>> GetActiveCategoriesAsync();
    }
}

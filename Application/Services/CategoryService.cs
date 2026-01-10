using Application.Interfaces;
using Microsoft.EntityFrameworkCore;
using Repository.DTOs.Category;
using Repository.Entities;
using Repository.Interfaces;

namespace Application.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly IUnitOfWork _unitOfWork;

        public CategoryService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<CategoryResponseDTO>> GetAllCategoriesAsync()
        {
            try
            {
                var categories = await _unitOfWork.CategoryRepository
                    .GetAllAsQueryable()
                    .Where(c => !c.IsDeleted)
                    .ToListAsync();

                return categories.Select(c => new CategoryResponseDTO
                {
                    CategoryId = c.CategoryId,
                    CategoryName = c.CategoryName,
                    CategoryDescription = c.CategoryDescription,
                    ParentCategoryId = c.ParentCategoryId,
                    IsActive = c.IsActive,
                    CreatedAt = c.CreatedAt,
                    UpdatedAt = c.UpdatedAt
                }).ToList();
            }
            catch
            {
                return new List<CategoryResponseDTO>();
            }
        }

        public async Task<CategoryResponseDTO?> GetCategoryByIdAsync(int categoryId)
        {
            try
            {
                var category = await _unitOfWork.CategoryRepository.GetByIdAsync(categoryId);

                if (category == null || category.IsDeleted)
                {
                    return null;
                }

                return new CategoryResponseDTO
                {
                    CategoryId = category.CategoryId,
                    CategoryName = category.CategoryName,
                    CategoryDescription = category.CategoryDescription,
                    ParentCategoryId = category.ParentCategoryId,
                    IsActive = category.IsActive,
                    CreatedAt = category.CreatedAt,
                    UpdatedAt = category.UpdatedAt
                };
            }
            catch
            {
                return null;
            }
        }

        public async Task<(bool Success, string Message)> CreateCategoryAsync(CategoryCreateDTO categoryDto)
        {
            try
            {
                var existingCategory = await _unitOfWork.CategoryRepository.FirstOrDefaultAsync(
                    c => c.CategoryName == categoryDto.CategoryName && !c.IsDeleted
                );

                if (existingCategory != null)
                {
                    return (false, "Category name already exists.");
                }

                if (categoryDto.ParentCategoryId.HasValue)
                {
                    var parentCategory = await _unitOfWork.CategoryRepository.GetByIdAsync(categoryDto.ParentCategoryId.Value);
                    if (parentCategory == null || parentCategory.IsDeleted)
                    {
                        return (false, "Parent category not found.");
                    }
                }

                var category = new Category
                {
                    CategoryName = categoryDto.CategoryName,
                    CategoryDescription = categoryDto.CategoryDescription,
                    ParentCategoryId = categoryDto.ParentCategoryId,
                    IsActive = categoryDto.IsActive,
                    CreatedAt = DateTime.UtcNow,
                    IsDeleted = false
                };

                var created = await _unitOfWork.CategoryRepository.CreateAsync(category);
                if (created)
                {
                    await _unitOfWork.SaveChangesAsync();
                    return (true, "Category created successfully.");
                }

                return (false, "Failed to create category.");
            }
            catch (Exception ex)
            {
                return (false, $"Create failed: {ex.Message}");
            }
        }

        public async Task<(bool Success, string Message)> UpdateCategoryAsync(int categoryId, CategoryUpdateDTO categoryDto)
        {
            try
            {
                var category = await _unitOfWork.CategoryRepository.GetByIdAsync(categoryId);

                if (category == null || category.IsDeleted)
                {
                    return (false, "Category not found.");
                }

                if (!string.IsNullOrEmpty(categoryDto.CategoryName))
                {
                    var existingCategory = await _unitOfWork.CategoryRepository.FirstOrDefaultAsync(
                        c => c.CategoryName == categoryDto.CategoryName && c.CategoryId != categoryId && !c.IsDeleted
                    );

                    if (existingCategory != null)
                    {
                        return (false, "Category name already exists.");
                    }

                    category.CategoryName = categoryDto.CategoryName;
                }

                if (categoryDto.CategoryDescription != null)
                    category.CategoryDescription = categoryDto.CategoryDescription;

                if (categoryDto.ParentCategoryId.HasValue)
                {
                    if (categoryDto.ParentCategoryId.Value == categoryId)
                    {
                        return (false, "Category cannot be its own parent.");
                    }

                    var parentCategory = await _unitOfWork.CategoryRepository.GetByIdAsync(categoryDto.ParentCategoryId.Value);
                    if (parentCategory == null || parentCategory.IsDeleted)
                    {
                        return (false, "Parent category not found.");
                    }

                    category.ParentCategoryId = categoryDto.ParentCategoryId;
                }

                if (categoryDto.IsActive.HasValue)
                    category.IsActive = categoryDto.IsActive.Value;

                category.UpdatedAt = DateTime.UtcNow;

                var updated = await _unitOfWork.CategoryRepository.UpdateAsync(category);
                if (updated)
                {
                    await _unitOfWork.SaveChangesAsync();
                    return (true, "Category updated successfully.");
                }

                return (false, "Failed to update category.");
            }
            catch (Exception ex)
            {
                return (false, $"Update failed: {ex.Message}");
            }
        }

        public async Task<(bool Success, string Message)> DeleteCategoryAsync(int categoryId)
        {
            try
            {
                var canDelete = await CanDeleteCategoryAsync(categoryId);

                if (!canDelete)
                {
                    return (false, "Cannot delete category. This category is being used by news articles or has subcategories.");
                }

                var category = await _unitOfWork.CategoryRepository.GetByIdAsync(categoryId);

                if (category == null || category.IsDeleted)
                {
                    return (false, "Category not found.");
                }

                category.IsDeleted = true;
                category.DeletedAt = DateTime.UtcNow;

                var deleted = await _unitOfWork.CategoryRepository.UpdateAsync(category);
                if (deleted)
                {
                    await _unitOfWork.SaveChangesAsync();
                    return (true, "Category deleted successfully.");
                }

                return (false, "Failed to delete category.");
            }
            catch (Exception ex)
            {
                return (false, $"Delete failed: {ex.Message}");
            }
        }

        public async Task<List<CategoryResponseDTO>> SearchCategoriesAsync(string searchTerm)
        {
            try
            {
                var categories = await _unitOfWork.CategoryRepository
                    .GetAllAsQueryable()
                    .Where(c => !c.IsDeleted && 
                           (c.CategoryName.Contains(searchTerm) || 
                            (c.CategoryDescription != null && c.CategoryDescription.Contains(searchTerm))))
                    .ToListAsync();

                return categories.Select(c => new CategoryResponseDTO
                {
                    CategoryId = c.CategoryId,
                    CategoryName = c.CategoryName,
                    CategoryDescription = c.CategoryDescription,
                    ParentCategoryId = c.ParentCategoryId,
                    IsActive = c.IsActive,
                    CreatedAt = c.CreatedAt,
                    UpdatedAt = c.UpdatedAt
                }).ToList();
            }
            catch
            {
                return new List<CategoryResponseDTO>();
            }
        }

        public async Task<bool> CanDeleteCategoryAsync(int categoryId)
        {
            try
            {
                var hasNewsArticles = await _unitOfWork.NewsArticleRepository
                    .GetAllAsQueryable()
                    .AnyAsync(n => n.CategoryId == categoryId && !n.IsDeleted);

                var hasSubCategories = await _unitOfWork.CategoryRepository
                    .GetAllAsQueryable()
                    .AnyAsync(c => c.ParentCategoryId == categoryId && !c.IsDeleted);

                return !hasNewsArticles && !hasSubCategories;
            }
            catch
            {
                return false;
            }
        }

        public async Task<List<CategoryResponseDTO>> GetSubCategoriesAsync(int parentCategoryId)
        {
            try
            {
                var subCategories = await _unitOfWork.CategoryRepository
                    .GetAllAsQueryable()
                    .Where(c => c.ParentCategoryId == parentCategoryId && !c.IsDeleted)
                    .ToListAsync();

                return subCategories.Select(c => new CategoryResponseDTO
                {
                    CategoryId = c.CategoryId,
                    CategoryName = c.CategoryName,
                    CategoryDescription = c.CategoryDescription,
                    ParentCategoryId = c.ParentCategoryId,
                    IsActive = c.IsActive,
                    CreatedAt = c.CreatedAt,
                    UpdatedAt = c.UpdatedAt
                }).ToList();
            }
            catch
            {
                return new List<CategoryResponseDTO>();
            }
        }

        public async Task<List<CategoryResponseDTO>> GetActiveCategoriesAsync()
        {
            try
            {
                var categories = await _unitOfWork.CategoryRepository
                    .GetAllAsQueryable()
                    .Where(c => c.IsActive && !c.IsDeleted)
                    .ToListAsync();

                return categories.Select(c => new CategoryResponseDTO
                {
                    CategoryId = c.CategoryId,
                    CategoryName = c.CategoryName,
                    CategoryDescription = c.CategoryDescription,
                    ParentCategoryId = c.ParentCategoryId,
                    IsActive = c.IsActive,
                    CreatedAt = c.CreatedAt,
                    UpdatedAt = c.UpdatedAt
                }).ToList();
            }
            catch
            {
                return new List<CategoryResponseDTO>();
            }
        }
    }
}

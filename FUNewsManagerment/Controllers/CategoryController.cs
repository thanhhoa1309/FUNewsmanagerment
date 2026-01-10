using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Repository.DTOs.Category;

namespace FUNewsManagerment.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;
        private readonly ILogger<CategoryController> _logger;

        public CategoryController(ICategoryService categoryService, ILogger<CategoryController> logger)
        {
            _categoryService = categoryService;
            _logger = logger;
        }

        /// <summary>
        /// Get all categories
        /// </summary>
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAllCategories()
        {
            try
            {
                var categories = await _categoryService.GetAllCategoriesAsync();
                return Ok(categories);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all categories");
                return StatusCode(500, new { message = "An error occurred while getting categories" });
            }
        }

        /// <summary>
        /// Get active categories only
        /// </summary>
        [HttpGet("active")]
        [AllowAnonymous]
        public async Task<IActionResult> GetActiveCategories()
        {
            try
            {
                var categories = await _categoryService.GetActiveCategoriesAsync();
                return Ok(categories);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting active categories");
                return StatusCode(500, new { message = "An error occurred while getting active categories" });
            }
        }

        /// <summary>
        /// Get category by ID
        /// </summary>
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetCategoryById(int id)
        {
            try
            {
                var category = await _categoryService.GetCategoryByIdAsync(id);

                if (category == null)
                {
                    return NotFound(new { message = "Category not found" });
                }

                return Ok(category);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting category by ID");
                return StatusCode(500, new { message = "An error occurred while getting category" });
            }
        }

        /// <summary>
        /// Get subcategories of a parent category
        /// </summary>
        [HttpGet("{id}/subcategories")]
        [AllowAnonymous]
        public async Task<IActionResult> GetSubCategories(int id)
        {
            try
            {
                var subcategories = await _categoryService.GetSubCategoriesAsync(id);
                return Ok(subcategories);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting subcategories");
                return StatusCode(500, new { message = "An error occurred while getting subcategories" });
            }
        }

        /// <summary>
        /// Create new category (Admin, Staff only)
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "Admin,Staff")]
        public async Task<IActionResult> CreateCategory([FromBody] CategoryCreateDTO categoryDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var result = await _categoryService.CreateCategoryAsync(categoryDto);

                if (!result.Success)
                {
                    return BadRequest(new { message = result.Message });
                }

                return Ok(new { message = result.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating category");
                return StatusCode(500, new { message = "An error occurred while creating category" });
            }
        }

        /// <summary>
        /// Update category (Admin, Staff only)
        /// </summary>
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Staff")]
        public async Task<IActionResult> UpdateCategory(int id, [FromBody] CategoryUpdateDTO categoryDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var result = await _categoryService.UpdateCategoryAsync(id, categoryDto);

                if (!result.Success)
                {
                    return BadRequest(new { message = result.Message });
                }

                return Ok(new { message = result.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating category");
                return StatusCode(500, new { message = "An error occurred while updating category" });
            }
        }

        /// <summary>
        /// Delete category (Admin only)
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            try
            {
                var canDelete = await _categoryService.CanDeleteCategoryAsync(id);
                if (!canDelete)
                {
                    return BadRequest(new { message = "Cannot delete category with existing news articles or subcategories" });
                }

                var result = await _categoryService.DeleteCategoryAsync(id);

                if (!result.Success)
                {
                    return BadRequest(new { message = result.Message });
                }

                return Ok(new { message = result.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting category");
                return StatusCode(500, new { message = "An error occurred while deleting category" });
            }
        }

        /// <summary>
        /// Search categories
        /// </summary>
        [HttpGet("search")]
        [AllowAnonymous]
        public async Task<IActionResult> SearchCategories([FromQuery] string searchTerm)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(searchTerm))
                {
                    return BadRequest(new { message = "Search term is required" });
                }

                var categories = await _categoryService.SearchCategoriesAsync(searchTerm);
                return Ok(categories);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching categories");
                return StatusCode(500, new { message = "An error occurred while searching categories" });
            }
        }

        /// <summary>
        /// Check if category can be deleted
        /// </summary>
        [HttpGet("{id}/can-delete")]
        [Authorize(Roles = "Admin,Staff")]
        public async Task<IActionResult> CanDeleteCategory(int id)
        {
            try
            {
                var canDelete = await _categoryService.CanDeleteCategoryAsync(id);
                return Ok(new { canDelete });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking if category can be deleted");
                return StatusCode(500, new { message = "An error occurred while checking category" });
            }
        }
    }
}

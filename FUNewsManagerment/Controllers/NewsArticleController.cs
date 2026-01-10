using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Repository.DTOs.NewsArtiticle;
using System.Security.Claims;

namespace FUNewsManagerment.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class NewsArticleController : ControllerBase
    {
        private readonly INewsArticleService _newsArticleService;
        private readonly ILogger<NewsArticleController> _logger;

        public NewsArticleController(INewsArticleService newsArticleService, ILogger<NewsArticleController> logger)
        {
            _newsArticleService = newsArticleService;
            _logger = logger;
        }

        /// <summary>
        /// Get all news articles
        /// </summary>
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAllNewsArticles()
        {
            try
            {
                var articles = await _newsArticleService.GetAllNewsArticlesAsync();
                return Ok(articles);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all news articles");
                return StatusCode(500, new { message = "An error occurred while getting news articles" });
            }
        }

        /// <summary>
        /// Get active news articles only
        /// </summary>
        [HttpGet("active")]
        [AllowAnonymous]
        public async Task<IActionResult> GetActiveNewsArticles()
        {
            try
            {
                var articles = await _newsArticleService.GetActiveNewsArticlesAsync();
                return Ok(articles);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting active news articles");
                return StatusCode(500, new { message = "An error occurred while getting active news articles" });
            }
        }

        /// <summary>
        /// Get news article by ID
        /// </summary>
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetNewsArticleById(int id)
        {
            try
            {
                var article = await _newsArticleService.GetNewsArticleByIdAsync(id);

                if (article == null)
                {
                    return NotFound(new { message = "News article not found" });
                }

                return Ok(article);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting news article by ID");
                return StatusCode(500, new { message = "An error occurred while getting news article" });
            }
        }

        /// <summary>
        /// Get news articles by category
        /// </summary>
        [HttpGet("category/{categoryId}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetNewsArticlesByCategory(int categoryId)
        {
            try
            {
                var articles = await _newsArticleService.GetNewsArticlesByCategoryAsync(categoryId);
                return Ok(articles);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting news articles by category");
                return StatusCode(500, new { message = "An error occurred while getting news articles" });
            }
        }

        /// <summary>
        /// Get news articles by tag
        /// </summary>
        [HttpGet("tag/{tagId}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetNewsArticlesByTag(int tagId)
        {
            try
            {
                var articles = await _newsArticleService.GetNewsArticlesByTagAsync(tagId);
                return Ok(articles);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting news articles by tag");
                return StatusCode(500, new { message = "An error occurred while getting news articles" });
            }
        }

        /// <summary>
        /// Get news articles by staff
        /// </summary>
        [HttpGet("staff/{accountId}")]
        [Authorize(Roles = "Admin,Staff")]
        public async Task<IActionResult> GetNewsArticlesByStaff(int accountId)
        {
            try
            {
                var articles = await _newsArticleService.GetNewsArticlesByStaffAsync(accountId);
                return Ok(articles);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting news articles by staff");
                return StatusCode(500, new { message = "An error occurred while getting news articles" });
            }
        }

        /// <summary>
        /// Get my news articles
        /// </summary>
        [HttpGet("my-articles")]
        [Authorize(Roles = "Staff")]
        public async Task<IActionResult> GetMyNewsArticles()
        {
            try
            {
                var accountIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                
                if (string.IsNullOrEmpty(accountIdClaim) || !int.TryParse(accountIdClaim, out int accountId))
                {
                    return Unauthorized(new { message = "Invalid token" });
                }

                var articles = await _newsArticleService.GetNewsArticlesByStaffAsync(accountId);
                return Ok(articles);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting my news articles");
                return StatusCode(500, new { message = "An error occurred while getting news articles" });
            }
        }

        /// <summary>
        /// Get news articles by date range
        /// </summary>
        [HttpGet("date-range")]
        [AllowAnonymous]
        public async Task<IActionResult> GetNewsArticlesByDateRange([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
        {
            try
            {
                if (startDate > endDate)
                {
                    return BadRequest(new { message = "Start date must be before end date" });
                }

                var articles = await _newsArticleService.GetNewsArticlesByDateRangeAsync(startDate, endDate);
                return Ok(articles);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting news articles by date range");
                return StatusCode(500, new { message = "An error occurred while getting news articles" });
            }
        }

        /// <summary>
        /// Create new news article (Staff only)
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "Staff")]
        public async Task<IActionResult> CreateNewsArticle([FromBody] NewsArticleCreateDTO articleDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var accountIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                
                if (string.IsNullOrEmpty(accountIdClaim) || !int.TryParse(accountIdClaim, out int accountId))
                {
                    return Unauthorized(new { message = "Invalid token" });
                }

                var result = await _newsArticleService.CreateNewsArticleAsync(articleDto, accountId);

                if (!result.Success)
                {
                    return BadRequest(new { message = result.Message });
                }

                return Ok(new { message = result.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating news article");
                return StatusCode(500, new { message = "An error occurred while creating news article" });
            }
        }

        /// <summary>
        /// Update news article (Staff can update own, Admin can update all)
        /// </summary>
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Staff")]
        public async Task<IActionResult> UpdateNewsArticle(int id, [FromBody] NewsArticleUpdateDTO articleDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var accountIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var role = User.FindFirst(ClaimTypes.Role)?.Value;
                
                if (string.IsNullOrEmpty(accountIdClaim) || !int.TryParse(accountIdClaim, out int accountId))
                {
                    return Unauthorized(new { message = "Invalid token" });
                }

                // Check if staff is updating their own article
                if (role == "Staff")
                {
                    var article = await _newsArticleService.GetNewsArticleByIdAsync(id);
                    if (article == null)
                    {
                        return NotFound(new { message = "News article not found" });
                    }

                    if (article.CreatedBy != accountId)
                    {
                        return Forbid();
                    }
                }

                var result = await _newsArticleService.UpdateNewsArticleAsync(id, articleDto);

                if (!result.Success)
                {
                    return BadRequest(new { message = result.Message });
                }

                return Ok(new { message = result.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating news article");
                return StatusCode(500, new { message = "An error occurred while updating news article" });
            }
        }

        /// <summary>
        /// Delete news article (Admin only)
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteNewsArticle(int id)
        {
            try
            {
                var result = await _newsArticleService.DeleteNewsArticleAsync(id);

                if (!result.Success)
                {
                    return BadRequest(new { message = result.Message });
                }

                return Ok(new { message = result.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting news article");
                return StatusCode(500, new { message = "An error occurred while deleting news article" });
            }
        }

        /// <summary>
        /// Search news articles
        /// </summary>
        [HttpGet("search")]
        [AllowAnonymous]
        public async Task<IActionResult> SearchNewsArticles([FromQuery] string searchTerm)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(searchTerm))
                {
                    return BadRequest(new { message = "Search term is required" });
                }

                var articles = await _newsArticleService.SearchNewsArticlesAsync(searchTerm);
                return Ok(articles);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching news articles");
                return StatusCode(500, new { message = "An error occurred while searching news articles" });
            }
        }
    }
}

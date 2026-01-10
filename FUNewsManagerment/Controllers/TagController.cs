using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Repository.DTOs.Tag;

namespace FUNewsManagerment.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class TagController : ControllerBase
    {
        private readonly ITagService _tagService;
        private readonly ILogger<TagController> _logger;

        public TagController(ITagService tagService, ILogger<TagController> logger)
        {
            _tagService = tagService;
            _logger = logger;
        }

        /// <summary>
        /// Get all tags
        /// </summary>
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAllTags()
        {
            try
            {
                var tags = await _tagService.GetAllTagsAsync();
                return Ok(tags);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all tags");
                return StatusCode(500, new { message = "An error occurred while getting tags" });
            }
        }

        /// <summary>
        /// Get tag by ID
        /// </summary>
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetTagById(int id)
        {
            try
            {
                var tag = await _tagService.GetTagByIdAsync(id);

                if (tag == null)
                {
                    return NotFound(new { message = "Tag not found" });
                }

                return Ok(tag);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting tag by ID");
                return StatusCode(500, new { message = "An error occurred while getting tag" });
            }
        }

        /// <summary>
        /// Create new tag (Admin, Staff only)
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "Admin,Staff")]
        public async Task<IActionResult> CreateTag([FromBody] TagCreateDTO tagDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var result = await _tagService.CreateTagAsync(tagDto);

                if (!result.Success)
                {
                    return BadRequest(new { message = result.Message });
                }

                return Ok(new { message = result.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating tag");
                return StatusCode(500, new { message = "An error occurred while creating tag" });
            }
        }

        /// <summary>
        /// Update tag (Admin, Staff only)
        /// </summary>
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Staff")]
        public async Task<IActionResult> UpdateTag(int id, [FromBody] TagUpdateDTO tagDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var result = await _tagService.UpdateTagAsync(id, tagDto);

                if (!result.Success)
                {
                    return BadRequest(new { message = result.Message });
                }

                return Ok(new { message = result.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating tag");
                return StatusCode(500, new { message = "An error occurred while updating tag" });
            }
        }

        /// <summary>
        /// Delete tag (Admin only)
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteTag(int id)
        {
            try
            {
                var result = await _tagService.DeleteTagAsync(id);

                if (!result.Success)
                {
                    return BadRequest(new { message = result.Message });
                }

                return Ok(new { message = result.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting tag");
                return StatusCode(500, new { message = "An error occurred while deleting tag" });
            }
        }

        /// <summary>
        /// Search tags
        /// </summary>
        [HttpGet("search")]
        [AllowAnonymous]
        public async Task<IActionResult> SearchTags([FromQuery] string searchTerm)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(searchTerm))
                {
                    return BadRequest(new { message = "Search term is required" });
                }

                var tags = await _tagService.SearchTagsAsync(searchTerm);
                return Ok(tags);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching tags");
                return StatusCode(500, new { message = "An error occurred while searching tags" });
            }
        }
    }
}

using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FUNewsManagerment.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class ReportController : ControllerBase
    {
        private readonly IReportService _reportService;
        private readonly ILogger<ReportController> _logger;

        public ReportController(IReportService reportService, ILogger<ReportController> logger)
        {
            _reportService = reportService;
            _logger = logger;
        }

        /// <summary>
        /// Get news statistics by date range (Admin only)
        /// </summary>
        [HttpGet("statistics")]
        public async Task<IActionResult> GetNewsStatistics([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
        {
            try
            {
                if (startDate > endDate)
                {
                    return BadRequest(new { message = "Start date must be before end date" });
                }

                var statistics = await _reportService.GetNewsStatisticsByDateRangeAsync(startDate, endDate);
                return Ok(statistics);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting news statistics");
                return StatusCode(500, new { message = "An error occurred while getting statistics" });
            }
        }

        /// <summary>
        /// Get news created by staff report (Admin only)
        /// </summary>
        [HttpGet("news-by-staff")]
        public async Task<IActionResult> GetNewsCreatedByStaffReport([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
        {
            try
            {
                if (startDate > endDate)
                {
                    return BadRequest(new { message = "Start date must be before end date" });
                }

                var report = await _reportService.GetNewsCreatedByStaffReportAsync(startDate, endDate);
                return Ok(report);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting news by staff report");
                return StatusCode(500, new { message = "An error occurred while getting report" });
            }
        }

        /// <summary>
        /// Get news by category report (Admin only)
        /// </summary>
        [HttpGet("news-by-category")]
        public async Task<IActionResult> GetNewsByCategoryReport([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
        {
            try
            {
                if (startDate > endDate)
                {
                    return BadRequest(new { message = "Start date must be before end date" });
                }

                var report = await _reportService.GetNewsByCategoryReportAsync(startDate, endDate);
                return Ok(report);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting news by category report");
                return StatusCode(500, new { message = "An error occurred while getting report" });
            }
        }

        /// <summary>
        /// Get top authors report (Admin only)
        /// </summary>
        [HttpGet("top-authors")]
        public async Task<IActionResult> GetTopAuthorsReport([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
        {
            try
            {
                if (startDate > endDate)
                {
                    return BadRequest(new { message = "Start date must be before end date" });
                }

                var report = await _reportService.GetTopAuthorsReportAsync(startDate, endDate);
                return Ok(report);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting top authors report");
                return StatusCode(500, new { message = "An error occurred while getting report" });
            }
        }
    }
}

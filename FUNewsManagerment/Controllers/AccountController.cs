using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Repository.DTOs.SystemAccount;

namespace FUNewsManagerment.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService _accountService;
        private readonly ILogger<AccountController> _logger;

        public AccountController(IAccountService accountService, ILogger<AccountController> logger)
        {
            _accountService = accountService;
            _logger = logger;
        }

        /// <summary>
        /// Get all accounts (Admin only)
        /// </summary>
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllAccounts()
        {
            try
            {
                var accounts = await _accountService.GetAllAccountsAsync();
                return Ok(accounts);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all accounts");
                return StatusCode(500, new { message = "An error occurred while getting accounts" });
            }
        }

        /// <summary>
        /// Get account by ID (Admin only)
        /// </summary>
        [HttpGet("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAccountById(int id)
        {
            try
            {
                var account = await _accountService.GetAccountByIdAsync(id);

                if (account == null)
                {
                    return NotFound(new { message = "Account not found" });
                }

                return Ok(account);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting account by ID");
                return StatusCode(500, new { message = "An error occurred while getting account" });
            }
        }

        /// <summary>
        /// Create new account (Admin only)
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateAccount([FromBody] SystemAccountCreateDTO accountDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var result = await _accountService.CreateAccountAsync(accountDto);

                if (!result.Success)
                {
                    return BadRequest(new { message = result.Message });
                }

                return Ok(new { message = result.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating account");
                return StatusCode(500, new { message = "An error occurred while creating account" });
            }
        }

        /// <summary>
        /// Update account (Admin only)
        /// </summary>
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateAccount(int id, [FromBody] SystemAccountUpdateDTO accountDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var result = await _accountService.UpdateAccountAsync(id, accountDto);

                if (!result.Success)
                {
                    return BadRequest(new { message = result.Message });
                }

                return Ok(new { message = result.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating account");
                return StatusCode(500, new { message = "An error occurred while updating account" });
            }
        }

        /// <summary>
        /// Delete account (Admin only)
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteAccount(int id)
        {
            try
            {
                var canDelete = await _accountService.CanDeleteAccountAsync(id);
                if (!canDelete)
                {
                    return BadRequest(new { message = "Cannot delete account with existing news articles" });
                }

                var result = await _accountService.DeleteAccountAsync(id);

                if (!result.Success)
                {
                    return BadRequest(new { message = result.Message });
                }

                return Ok(new { message = result.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting account");
                return StatusCode(500, new { message = "An error occurred while deleting account" });
            }
        }

        /// <summary>
        /// Search accounts (Admin only)
        /// </summary>
        [HttpGet("search")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> SearchAccounts([FromQuery] string searchTerm)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(searchTerm))
                {
                    return BadRequest(new { message = "Search term is required" });
                }

                var accounts = await _accountService.SearchAccountsAsync(searchTerm);
                return Ok(accounts);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching accounts");
                return StatusCode(500, new { message = "An error occurred while searching accounts" });
            }
        }

        /// <summary>
        /// Check if account can be deleted (Admin only)
        /// </summary>
        [HttpGet("{id}/can-delete")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CanDeleteAccount(int id)
        {
            try
            {
                var canDelete = await _accountService.CanDeleteAccountAsync(id);
                return Ok(new { canDelete });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking if account can be deleted");
                return StatusCode(500, new { message = "An error occurred while checking account" });
            }
        }
    }
}

using Application.Helpers;
using Application.Interfaces;
using Microsoft.EntityFrameworkCore;
using Repository.DTOs.SystemAccount;
using Repository.Entities;
using Repository.Interfaces;

namespace Application.Services
{
    public class AccountService : IAccountService
    {
        private readonly IUnitOfWork _unitOfWork;

        public AccountService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<SystemAccountResponseDTO>> GetAllAccountsAsync()
        {
            try
            {
                var accounts = await _unitOfWork.AccountRepository
                    .GetAllAsQueryable()
                    .Where(a => !a.IsDeleted)
                    .ToListAsync();

                return accounts.Select(a => new SystemAccountResponseDTO
                {
                    AccountId = a.AccountId,
                    AccountName = a.AccountName,
                    AccountEmail = a.AccountEmail,
                    AccountRole = a.AccountRole,
                    CreatedAt = a.CreatedAt,
                    UpdatedAt = a.UpdatedAt
                }).ToList();
            }
            catch
            {
                return new List<SystemAccountResponseDTO>();
            }
        }

        public async Task<SystemAccountResponseDTO?> GetAccountByIdAsync(int accountId)
        {
            try
            {
                var account = await _unitOfWork.AccountRepository.GetByIdAsync(accountId);

                if (account == null || account.IsDeleted)
                {
                    return null;
                }

                return new SystemAccountResponseDTO
                {
                    AccountId = account.AccountId,
                    AccountName = account.AccountName,
                    AccountEmail = account.AccountEmail,
                    AccountRole = account.AccountRole,
                    CreatedAt = account.CreatedAt,
                    UpdatedAt = account.UpdatedAt
                };
            }
            catch
            {
                return null;
            }
        }

        public async Task<(bool Success, string Message)> CreateAccountAsync(SystemAccountCreateDTO accountDto)
        {
            try
            {
                var existingAccount = await _unitOfWork.AccountRepository.FirstOrDefaultAsync(
                    a => a.AccountEmail == accountDto.AccountEmail
                );

                if (existingAccount != null)
                {
                    return (false, "Email already exists.");
                }

                var account = new SystemAccount
                {
                    AccountName = accountDto.AccountName,
                    AccountEmail = accountDto.AccountEmail,
                    AccountRole = accountDto.AccountRole,
                    AccountPassword = PasswordHelper.HashPassword(accountDto.AccountPassword),
                    CreatedAt = DateTime.UtcNow,
                    IsDeleted = false
                };

                var created = await _unitOfWork.AccountRepository.CreateAsync(account);
                if (created)
                {
                    await _unitOfWork.SaveChangesAsync();
                    return (true, "Account created successfully.");
                }

                return (false, "Failed to create account.");
            }
            catch (Exception ex)
            {
                return (false, $"Create failed: {ex.Message}");
            }
        }

        public async Task<(bool Success, string Message)> UpdateAccountAsync(int accountId, SystemAccountUpdateDTO accountDto)
        {
            try
            {
                var account = await _unitOfWork.AccountRepository.GetByIdAsync(accountId);

                if (account == null || account.IsDeleted)
                {
                    return (false, "Account not found.");
                }

                if (!string.IsNullOrEmpty(accountDto.AccountName))
                    account.AccountName = accountDto.AccountName;

                if (!string.IsNullOrEmpty(accountDto.AccountEmail))
                {
                    var existingAccount = await _unitOfWork.AccountRepository.FirstOrDefaultAsync(
                        a => a.AccountEmail == accountDto.AccountEmail && a.AccountId != accountId
                    );

                    if (existingAccount != null)
                    {
                        return (false, "Email already exists.");
                    }

                    account.AccountEmail = accountDto.AccountEmail;
                }

                if (!string.IsNullOrEmpty(accountDto.AccountRole))
                    account.AccountRole = accountDto.AccountRole;

                if (!string.IsNullOrEmpty(accountDto.AccountPassword))
                {
                    account.AccountPassword = PasswordHelper.HashPassword(accountDto.AccountPassword);
                }

                account.UpdatedAt = DateTime.UtcNow;

                var updated = await _unitOfWork.AccountRepository.UpdateAsync(account);
                if (updated)
                {
                    await _unitOfWork.SaveChangesAsync();
                    return (true, "Account updated successfully.");
                }

                return (false, "Failed to update account.");
            }
            catch (Exception ex)
            {
                return (false, $"Update failed: {ex.Message}");
            }
        }

        public async Task<(bool Success, string Message)> DeleteAccountAsync(int accountId)
        {
            try
            {
                var canDelete = await CanDeleteAccountAsync(accountId);

                if (!canDelete)
                {
                    return (false, "Cannot delete account. This account has created news articles.");
                }

                var account = await _unitOfWork.AccountRepository.GetByIdAsync(accountId);

                if (account == null || account.IsDeleted)
                {
                    return (false, "Account not found.");
                }

                account.IsDeleted = true;
                account.DeletedAt = DateTime.UtcNow;

                var deleted = await _unitOfWork.AccountRepository.UpdateAsync(account);
                if (deleted)
                {
                    await _unitOfWork.SaveChangesAsync();
                    return (true, "Account deleted successfully.");
                }

                return (false, "Failed to delete account.");
            }
            catch (Exception ex)
            {
                return (false, $"Delete failed: {ex.Message}");
            }
        }

        public async Task<List<SystemAccountResponseDTO>> SearchAccountsAsync(string searchTerm)
        {
            try
            {
                var accounts = await _unitOfWork.AccountRepository
                    .GetAllAsQueryable()
                    .Where(a => !a.IsDeleted && 
                           (a.AccountName.Contains(searchTerm) || 
                            a.AccountEmail.Contains(searchTerm) ||
                            a.AccountRole.Contains(searchTerm)))
                    .ToListAsync();

                return accounts.Select(a => new SystemAccountResponseDTO
                {
                    AccountId = a.AccountId,
                    AccountName = a.AccountName,
                    AccountEmail = a.AccountEmail,
                    AccountRole = a.AccountRole,
                    CreatedAt = a.CreatedAt,
                    UpdatedAt = a.UpdatedAt
                }).ToList();
            }
            catch
            {
                return new List<SystemAccountResponseDTO>();
            }
        }

        public async Task<bool> CanDeleteAccountAsync(int accountId)
        {
            try
            {
                // Check if account has created any news articles
                var hasNewsArticles = await _unitOfWork.NewsArticleRepository
                    .GetAllAsQueryable()
                    .AnyAsync(n => !n.IsDeleted);

                return !hasNewsArticles;
            }
            catch
            {
                return false;
            }
        }
    }
}

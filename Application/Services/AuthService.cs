using Application.Helpers;
using Application.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Repository.DTOs.SystemAccount;
using Repository.Entities;
using Repository.Interfaces;

namespace Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly JwtHelper _jwtHelper;

        public AuthService(IUnitOfWork unitOfWork, IConfiguration configuration)
        {
            _unitOfWork = unitOfWork;
            _jwtHelper = new JwtHelper(configuration);
        }

        public async Task<(bool Success, string Token, string Message)> LoginAsync(string email, string password)
        {
            try
            {
                var account = await _unitOfWork.AccountRepository.FirstOrDefaultAsync(
                    a => a.AccountEmail == email && !a.IsDeleted
                );

                if (account == null)
                {
                    return (false, string.Empty, "Invalid email or password.");
                }

                if (!PasswordHelper.VerifyPassword(password, account.AccountPassword))
                {
                    return (false, string.Empty, "Invalid email or password.");
                }

                var token = _jwtHelper.GenerateToken(account.AccountId, account.AccountEmail, account.AccountRole);

                return (true, token, "Login successful.");
            }
            catch (Exception ex)
            {
                return (false, string.Empty, $"Login failed: {ex.Message}");
            }
        }

        public async Task<(bool Success, string Message)> RegisterAsync(SystemAccountCreateDTO accountDto)
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
                    return (true, "Account registered successfully.");
                }

                return (false, "Failed to register account.");
            }
            catch (Exception ex)
            {
                return (false, $"Registration failed: {ex.Message}");
            }
        }

        public async Task<SystemAccountResponseDTO?> GetProfileAsync(int accountId)
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

        public async Task<(bool Success, string Message)> UpdateProfileAsync(int accountId, SystemAccountUpdateDTO accountDto)
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

                if (!string.IsNullOrEmpty(accountDto.AccountPassword))
                {
                    account.AccountPassword = PasswordHelper.HashPassword(accountDto.AccountPassword);
                }

                account.UpdatedAt = DateTime.UtcNow;

                var updated = await _unitOfWork.AccountRepository.UpdateAsync(account);
                if (updated)
                {
                    await _unitOfWork.SaveChangesAsync();
                    return (true, "Profile updated successfully.");
                }

                return (false, "Failed to update profile.");
            }
            catch (Exception ex)
            {
                return (false, $"Update failed: {ex.Message}");
            }
        }
    }
}

using Repository.DTOs.SystemAccount;

namespace Application.Interfaces
{
    public interface IAuthService
    {
        Task<(bool Success, string Token, string Message)> LoginAsync(string email, string password);
        Task<(bool Success, string Message)> RegisterAsync(SystemAccountCreateDTO accountDto);
        Task<SystemAccountResponseDTO?> GetProfileAsync(int accountId);
        Task<(bool Success, string Message)> UpdateProfileAsync(int accountId, SystemAccountUpdateDTO accountDto);
    }
}

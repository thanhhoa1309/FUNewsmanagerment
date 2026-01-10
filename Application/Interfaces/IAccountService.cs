using Repository.DTOs.SystemAccount;

namespace Application.Interfaces
{
    public interface IAccountService
    {
        Task<List<SystemAccountResponseDTO>> GetAllAccountsAsync();
        Task<SystemAccountResponseDTO?> GetAccountByIdAsync(int accountId);
        Task<(bool Success, string Message)> CreateAccountAsync(SystemAccountCreateDTO accountDto);
        Task<(bool Success, string Message)> UpdateAccountAsync(int accountId, SystemAccountUpdateDTO accountDto);
        Task<(bool Success, string Message)> DeleteAccountAsync(int accountId);
        Task<List<SystemAccountResponseDTO>> SearchAccountsAsync(string searchTerm);
        Task<bool> CanDeleteAccountAsync(int accountId);
    }
}

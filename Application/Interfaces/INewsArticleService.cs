using Repository.DTOs.NewsArtiticle;

namespace Application.Interfaces
{
    public interface INewsArticleService
    {
        Task<List<NewsArticleResponseDTO>> GetAllNewsArticlesAsync();
        Task<List<NewsArticleResponseDTO>> GetActiveNewsArticlesAsync();
        Task<NewsArticleResponseDTO?> GetNewsArticleByIdAsync(int newsArticleId);
        Task<(bool Success, string Message)> CreateNewsArticleAsync(NewsArticleCreateDTO newsArticleDto, int createdByAccountId);
        Task<(bool Success, string Message)> UpdateNewsArticleAsync(int newsArticleId, NewsArticleUpdateDTO newsArticleDto);
        Task<(bool Success, string Message)> DeleteNewsArticleAsync(int newsArticleId);
        Task<List<NewsArticleResponseDTO>> SearchNewsArticlesAsync(string searchTerm);
        Task<List<NewsArticleResponseDTO>> GetNewsArticlesByCategoryAsync(int categoryId);
        Task<List<NewsArticleResponseDTO>> GetNewsArticlesByTagAsync(int tagId);
        Task<List<NewsArticleResponseDTO>> GetNewsArticlesByStaffAsync(int accountId);
        Task<List<NewsArticleResponseDTO>> GetNewsArticlesByDateRangeAsync(DateTime startDate, DateTime endDate);
    }
}

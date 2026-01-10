using Application.Interfaces;
using Microsoft.EntityFrameworkCore;
using Repository.DTOs.NewsArtiticle;
using Repository.DTOs.Tag;
using Repository.Entities;
using Repository.Interfaces;

namespace Application.Services
{
    public class NewsArticleService : INewsArticleService
    {
        private readonly IUnitOfWork _unitOfWork;

        public NewsArticleService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<NewsArticleResponseDTO>> GetAllNewsArticlesAsync()
        {
            try
            {
                var newsArticles = await _unitOfWork.NewsArticleRepository
                    .GetAllAsQueryable()
                    .Include(n => n.Category)
                    .Include(n => n.NewsTags)
                        .ThenInclude(nt => nt.Tag)
                    .Where(n => !n.IsDeleted)
                    .ToListAsync();

                return newsArticles.Select(n => MapToResponseDTO(n)).ToList();
            }
            catch
            {
                return new List<NewsArticleResponseDTO>();
            }
        }

        public async Task<List<NewsArticleResponseDTO>> GetActiveNewsArticlesAsync()
        {
            try
            {
                var newsArticles = await _unitOfWork.NewsArticleRepository
                    .GetAllAsQueryable()
                    .Include(n => n.Category)
                    .Include(n => n.NewsTags)
                        .ThenInclude(nt => nt.Tag)
                    .Where(n => !n.IsDeleted && n.NewsStatus.ToLower() == "active")
                    .ToListAsync();

                return newsArticles.Select(n => MapToResponseDTO(n)).ToList();
            }
            catch
            {
                return new List<NewsArticleResponseDTO>();
            }
        }

        public async Task<NewsArticleResponseDTO?> GetNewsArticleByIdAsync(int newsArticleId)
        {
            try
            {
                var newsArticle = await _unitOfWork.NewsArticleRepository
                    .GetAllAsQueryable()
                    .Include(n => n.Category)
                    .Include(n => n.NewsTags)
                        .ThenInclude(nt => nt.Tag)
                    .FirstOrDefaultAsync(n => n.NewsArticleId == newsArticleId && !n.IsDeleted);

                if (newsArticle == null)
                {
                    return null;
                }

                return MapToResponseDTO(newsArticle);
            }
            catch
            {
                return null;
            }
        }

        public async Task<(bool Success, string Message)> CreateNewsArticleAsync(NewsArticleCreateDTO newsArticleDto, int createdByAccountId)
        {
            try
            {
                var category = await _unitOfWork.CategoryRepository.GetByIdAsync(newsArticleDto.CategoryId);
                if (category == null || category.IsDeleted)
                {
                    return (false, "Category not found.");
                }

                var newsArticle = new NewsArticle
                {
                    NewsTitle = newsArticleDto.NewsTitle,
                    Headline = newsArticleDto.Headline,
                    NewsContent = newsArticleDto.NewsContent,
                    NewsSource = newsArticleDto.NewsSource,
                    NewsStatus = newsArticleDto.NewsStatus,
                    CategoryId = newsArticleDto.CategoryId,
                    CreatedAt = DateTime.UtcNow,
                    IsDeleted = false
                };

                var created = await _unitOfWork.NewsArticleRepository.CreateAsync(newsArticle);
                if (!created)
                {
                    return (false, "Failed to create news article.");
                }

                await _unitOfWork.SaveChangesAsync();

                // Add tags if provided
                if (newsArticleDto.TagIds != null && newsArticleDto.TagIds.Any())
                {
                    foreach (var tagId in newsArticleDto.TagIds)
                    {
                        var tag = await _unitOfWork.TagRepository.GetByIdAsync(tagId);
                        if (tag != null && !tag.IsDeleted)
                        {
                            var newsTag = new NewsTag
                            {
                                NewsArticleId = newsArticle.NewsArticleId,
                                TagId = tagId
                            };

                            await _unitOfWork.NewsTagRepository.CreateAsync(newsTag);
                        }
                    }

                    await _unitOfWork.SaveChangesAsync();
                }

                return (true, "News article created successfully.");
            }
            catch (Exception ex)
            {
                return (false, $"Create failed: {ex.Message}");
            }
        }

        public async Task<(bool Success, string Message)> UpdateNewsArticleAsync(int newsArticleId, NewsArticleUpdateDTO newsArticleDto)
        {
            try
            {
                var newsArticle = await _unitOfWork.NewsArticleRepository
                    .GetAllAsQueryable()
                    .Include(n => n.NewsTags)
                    .FirstOrDefaultAsync(n => n.NewsArticleId == newsArticleId && !n.IsDeleted);

                if (newsArticle == null)
                {
                    return (false, "News article not found.");
                }

                if (!string.IsNullOrEmpty(newsArticleDto.NewsTitle))
                    newsArticle.NewsTitle = newsArticleDto.NewsTitle;

                if (newsArticleDto.Headline != null)
                    newsArticle.Headline = newsArticleDto.Headline;

                if (!string.IsNullOrEmpty(newsArticleDto.NewsContent))
                    newsArticle.NewsContent = newsArticleDto.NewsContent;

                if (newsArticleDto.NewsSource != null)
                    newsArticle.NewsSource = newsArticleDto.NewsSource;

                if (!string.IsNullOrEmpty(newsArticleDto.NewsStatus))
                    newsArticle.NewsStatus = newsArticleDto.NewsStatus;

                if (newsArticleDto.CategoryId.HasValue)
                {
                    var category = await _unitOfWork.CategoryRepository.GetByIdAsync(newsArticleDto.CategoryId.Value);
                    if (category == null || category.IsDeleted)
                    {
                        return (false, "Category not found.");
                    }

                    newsArticle.CategoryId = newsArticleDto.CategoryId.Value;
                }

                newsArticle.UpdatedAt = DateTime.UtcNow;

                var updated = await _unitOfWork.NewsArticleRepository.UpdateAsync(newsArticle);
                if (!updated)
                {
                    return (false, "Failed to update news article.");
                }

                // Update tags if provided
                if (newsArticleDto.TagIds != null)
                {
                    // Remove existing tags
                    var existingNewsTags = newsArticle.NewsTags.ToList();
                    foreach (var newsTag in existingNewsTags)
                    {
                        await _unitOfWork.NewsTagRepository.RemoveAsync(newsTag);
                    }

                    // Add new tags
                    foreach (var tagId in newsArticleDto.TagIds)
                    {
                        var tag = await _unitOfWork.TagRepository.GetByIdAsync(tagId);
                        if (tag != null && !tag.IsDeleted)
                        {
                            var newsTag = new NewsTag
                            {
                                NewsArticleId = newsArticleId,
                                TagId = tagId
                            };

                            await _unitOfWork.NewsTagRepository.CreateAsync(newsTag);
                        }
                    }
                }

                await _unitOfWork.SaveChangesAsync();

                return (true, "News article updated successfully.");
            }
            catch (Exception ex)
            {
                return (false, $"Update failed: {ex.Message}");
            }
        }

        public async Task<(bool Success, string Message)> DeleteNewsArticleAsync(int newsArticleId)
        {
            try
            {
                var newsArticle = await _unitOfWork.NewsArticleRepository.GetByIdAsync(newsArticleId);

                if (newsArticle == null || newsArticle.IsDeleted)
                {
                    return (false, "News article not found.");
                }

                newsArticle.IsDeleted = true;
                newsArticle.DeletedAt = DateTime.UtcNow;

                var deleted = await _unitOfWork.NewsArticleRepository.UpdateAsync(newsArticle);
                if (deleted)
                {
                    await _unitOfWork.SaveChangesAsync();
                    return (true, "News article deleted successfully.");
                }

                return (false, "Failed to delete news article.");
            }
            catch (Exception ex)
            {
                return (false, $"Delete failed: {ex.Message}");
            }
        }

        public async Task<List<NewsArticleResponseDTO>> SearchNewsArticlesAsync(string searchTerm)
        {
            try
            {
                var newsArticles = await _unitOfWork.NewsArticleRepository
                    .GetAllAsQueryable()
                    .Include(n => n.Category)
                    .Include(n => n.NewsTags)
                        .ThenInclude(nt => nt.Tag)
                    .Where(n => !n.IsDeleted &&
                           (n.NewsTitle.Contains(searchTerm) ||
                            (n.Headline != null && n.Headline.Contains(searchTerm)) ||
                            n.NewsContent.Contains(searchTerm) ||
                            (n.NewsSource != null && n.NewsSource.Contains(searchTerm))))
                    .ToListAsync();

                return newsArticles.Select(n => MapToResponseDTO(n)).ToList();
            }
            catch
            {
                return new List<NewsArticleResponseDTO>();
            }
        }

        public async Task<List<NewsArticleResponseDTO>> GetNewsArticlesByCategoryAsync(int categoryId)
        {
            try
            {
                var newsArticles = await _unitOfWork.NewsArticleRepository
                    .GetAllAsQueryable()
                    .Include(n => n.Category)
                    .Include(n => n.NewsTags)
                        .ThenInclude(nt => nt.Tag)
                    .Where(n => n.CategoryId == categoryId && !n.IsDeleted)
                    .ToListAsync();

                return newsArticles.Select(n => MapToResponseDTO(n)).ToList();
            }
            catch
            {
                return new List<NewsArticleResponseDTO>();
            }
        }

        public async Task<List<NewsArticleResponseDTO>> GetNewsArticlesByTagAsync(int tagId)
        {
            try
            {
                var newsArticles = await _unitOfWork.NewsArticleRepository
                    .GetAllAsQueryable()
                    .Include(n => n.Category)
                    .Include(n => n.NewsTags)
                        .ThenInclude(nt => nt.Tag)
                    .Where(n => n.NewsTags.Any(nt => nt.TagId == tagId) && !n.IsDeleted)
                    .ToListAsync();

                return newsArticles.Select(n => MapToResponseDTO(n)).ToList();
            }
            catch
            {
                return new List<NewsArticleResponseDTO>();
            }
        }

        public async Task<List<NewsArticleResponseDTO>> GetNewsArticlesByStaffAsync(int accountId)
        {
            try
            {
                var newsArticles = await _unitOfWork.NewsArticleRepository
                    .GetAllAsQueryable()
                    .Include(n => n.Category)
                    .Include(n => n.NewsTags)
                        .ThenInclude(nt => nt.Tag)
                    .Where(n => !n.IsDeleted)
                    .ToListAsync();

                return newsArticles.Select(n => MapToResponseDTO(n)).ToList();
            }
            catch
            {
                return new List<NewsArticleResponseDTO>();
            }
        }

        public async Task<List<NewsArticleResponseDTO>> GetNewsArticlesByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            try
            {
                var newsArticles = await _unitOfWork.NewsArticleRepository
                    .GetAllAsQueryable()
                    .Include(n => n.Category)
                    .Include(n => n.NewsTags)
                        .ThenInclude(nt => nt.Tag)
                    .Where(n => !n.IsDeleted && 
                           n.CreatedAt >= startDate && 
                           n.CreatedAt <= endDate)
                    .ToListAsync();

                return newsArticles.Select(n => MapToResponseDTO(n)).ToList();
            }
            catch
            {
                return new List<NewsArticleResponseDTO>();
            }
        }

        private NewsArticleResponseDTO MapToResponseDTO(NewsArticle newsArticle)
        {
            return new NewsArticleResponseDTO
            {
                NewsArticleId = newsArticle.NewsArticleId,
                NewsTitle = newsArticle.NewsTitle,
                Headline = newsArticle.Headline,
                NewsContent = newsArticle.NewsContent,
                NewsSource = newsArticle.NewsSource,
                NewsStatus = newsArticle.NewsStatus,
                CategoryId = newsArticle.CategoryId,
                CategoryName = newsArticle.Category?.CategoryName,
                CreatedAt = newsArticle.CreatedAt,
                UpdatedAt = newsArticle.UpdatedAt,
                Tags = newsArticle.NewsTags?.Select(nt => new TagResponseDTO
                {
                    TagId = nt.Tag.TagId,
                    TagName = nt.Tag.TagName,
                    Note = nt.Tag.Note,
                    CreatedAt = nt.Tag.CreatedAt,
                    UpdatedAt = nt.Tag.UpdatedAt
                }).ToList()
            };
        }
    }
}

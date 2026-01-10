using Application.Interfaces;
using Microsoft.EntityFrameworkCore;
using Repository.Interfaces;

namespace Application.Services
{
    public class ReportService : IReportService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ReportService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<object> GetNewsStatisticsByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            try
            {
                var newsArticles = await _unitOfWork.NewsArticleRepository
                    .GetAllAsQueryable()
                    .Where(n => !n.IsDeleted && 
                           n.CreatedAt >= startDate && 
                           n.CreatedAt <= endDate)
                    .ToListAsync();

                var totalNews = newsArticles.Count;
                var activeNews = newsArticles.Count(n => n.NewsStatus.ToLower() == "active");
                var inactiveNews = newsArticles.Count(n => n.NewsStatus.ToLower() != "active");

                var newsByDate = newsArticles
                    .GroupBy(n => n.CreatedAt.Date)
                    .Select(g => new
                    {
                        Date = g.Key,
                        Count = g.Count()
                    })
                    .OrderByDescending(x => x.Date)
                    .ToList();

                return new
                {
                    StartDate = startDate,
                    EndDate = endDate,
                    TotalNews = totalNews,
                    ActiveNews = activeNews,
                    InactiveNews = inactiveNews,
                    NewsByDate = newsByDate
                };
            }
            catch
            {
                return new { };
            }
        }

        public async Task<List<object>> GetNewsCreatedByStaffReportAsync(DateTime startDate, DateTime endDate)
        {
            try
            {
                var newsArticles = await _unitOfWork.NewsArticleRepository
                    .GetAllAsQueryable()
                    .Where(n => !n.IsDeleted && 
                           n.CreatedAt >= startDate && 
                           n.CreatedAt <= endDate)
                    .ToListAsync();

                var report = newsArticles
                    .GroupBy(n => new { n.CreatedAt.Date })
                    .Select(g => new
                    {
                        Date = g.Key.Date,
                        Count = g.Count(),
                        NewsArticles = g.Select(n => new
                        {
                            n.NewsArticleId,
                            n.NewsTitle,
                            n.NewsStatus,
                            n.CreatedAt
                        }).ToList()
                    })
                    .OrderByDescending(x => x.Date)
                    .ToList<object>();

                return report;
            }
            catch
            {
                return new List<object>();
            }
        }

        public async Task<List<object>> GetNewsByCategoryReportAsync(DateTime startDate, DateTime endDate)
        {
            try
            {
                var newsArticles = await _unitOfWork.NewsArticleRepository
                    .GetAllAsQueryable()
                    .Include(n => n.Category)
                    .Where(n => !n.IsDeleted && 
                           n.CreatedAt >= startDate && 
                           n.CreatedAt <= endDate)
                    .ToListAsync();

                var report = newsArticles
                    .GroupBy(n => new { n.CategoryId, n.Category.CategoryName })
                    .Select(g => new
                    {
                        CategoryId = g.Key.CategoryId,
                        CategoryName = g.Key.CategoryName,
                        Count = g.Count(),
                        ActiveCount = g.Count(n => n.NewsStatus.ToLower() == "active"),
                        InactiveCount = g.Count(n => n.NewsStatus.ToLower() != "active")
                    })
                    .OrderByDescending(x => x.Count)
                    .ToList<object>();

                return report;
            }
            catch
            {
                return new List<object>();
            }
        }

        public async Task<List<object>> GetTopAuthorsReportAsync(DateTime startDate, DateTime endDate)
        {
            try
            {
                var newsArticles = await _unitOfWork.NewsArticleRepository
                    .GetAllAsQueryable()
                    .Where(n => !n.IsDeleted && 
                           n.CreatedAt >= startDate && 
                           n.CreatedAt <= endDate)
                    .ToListAsync();

                var report = newsArticles
                    .GroupBy(n => n.CreatedAt.Date)
                    .Select(g => new
                    {
                        Date = g.Key,
                        TotalNews = g.Count(),
                        ActiveNews = g.Count(n => n.NewsStatus.ToLower() == "active")
                    })
                    .OrderByDescending(x => x.TotalNews)
                    .ToList<object>();

                return report;
            }
            catch
            {
                return new List<object>();
            }
        }
    }
}

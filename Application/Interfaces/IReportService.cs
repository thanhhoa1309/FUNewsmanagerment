namespace Application.Interfaces
{
    public interface IReportService
    {
        Task<object> GetNewsStatisticsByDateRangeAsync(DateTime startDate, DateTime endDate);
        Task<List<object>> GetNewsCreatedByStaffReportAsync(DateTime startDate, DateTime endDate);
        Task<List<object>> GetNewsByCategoryReportAsync(DateTime startDate, DateTime endDate);
        Task<List<object>> GetTopAuthorsReportAsync(DateTime startDate, DateTime endDate);
    }
}

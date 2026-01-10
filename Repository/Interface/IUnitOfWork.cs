using Repository.Entities;

namespace Repository.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IGenericRepository<SystemAccount> AccountRepository { get; }
        IGenericRepository<NewsArticle> NewsArticleRepository { get; }
        IGenericRepository<Category> CategoryRepository { get; }
        IGenericRepository<Tag> TagRepository { get; }
        IGenericRepository<NewsTag> NewsTagRepository { get; }
        
        Task<int> SaveChangesAsync();
    }
}

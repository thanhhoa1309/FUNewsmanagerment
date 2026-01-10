using Repository;
using Repository.Entities;
using Repository.Interfaces;

namespace Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly NewsManagermentDbContext _dbContext;

        public UnitOfWork(NewsManagermentDbContext dbContext,
            IGenericRepository<SystemAccount> accountRepository,
            IGenericRepository<NewsArticle> newsArticleRepository,
            IGenericRepository<Category> categoryRepository,
            IGenericRepository<Tag> tagRepository,
            IGenericRepository<NewsTag> newsTagRepository)
        {
            _dbContext = dbContext;
            AccountRepository = accountRepository;
            NewsArticleRepository = newsArticleRepository;
            CategoryRepository = categoryRepository;
            TagRepository = tagRepository;
            NewsTagRepository = newsTagRepository;
        }

        public IGenericRepository<SystemAccount> AccountRepository { get; }
        public IGenericRepository<NewsArticle> NewsArticleRepository { get; }
        public IGenericRepository<Category> CategoryRepository { get; }
        public IGenericRepository<Tag> TagRepository { get; }
        public IGenericRepository<NewsTag> NewsTagRepository { get; }

        public async Task<int> SaveChangesAsync()
        {
            return await _dbContext.SaveChangesAsync();
        }

        public void Dispose()
        {
            _dbContext?.Dispose();
        }
    }
}

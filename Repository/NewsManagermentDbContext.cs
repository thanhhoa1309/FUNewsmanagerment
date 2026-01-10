using Microsoft.EntityFrameworkCore;
using Repository.Entities;

namespace Repository
{
    public class NewsManagermentDbContext : DbContext
    {
        public NewsManagermentDbContext(DbContextOptions<NewsManagermentDbContext> options) : base(options)
        {
        }

        public DbSet<SystemAccount> SystemAccounts { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<NewsArticle> NewsArticles { get; set; }
        public DbSet<NewsTag> NewsTags { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // SystemAccount Configuration
            modelBuilder.Entity<SystemAccount>(entity =>
            {
                entity.ToTable("SystemAccounts");
                entity.HasKey(e => e.AccountId);
                entity.Property(e => e.AccountName).IsRequired().HasMaxLength(100);
                entity.Property(e => e.AccountEmail).IsRequired().HasMaxLength(150);
                entity.Property(e => e.AccountRole).IsRequired().HasMaxLength(50);
                entity.Property(e => e.AccountPassword).IsRequired().HasMaxLength(255);
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
                entity.Property(e => e.IsDeleted).HasDefaultValue(false);
                
                entity.HasIndex(e => e.AccountEmail).IsUnique();
                entity.HasQueryFilter(e => !e.IsDeleted);
            });

            // Category Configuration
            modelBuilder.Entity<Category>(entity =>
            {
                entity.ToTable("Categories");
                entity.HasKey(e => e.CategoryId);
                entity.Property(e => e.CategoryName).IsRequired().HasMaxLength(200);
                entity.Property(e => e.CategoryDescription).HasMaxLength(500);
                entity.Property(e => e.IsActive).HasDefaultValue(true);
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
                entity.Property(e => e.IsDeleted).HasDefaultValue(false);

                entity.HasOne(e => e.ParentCategory)
                    .WithMany(e => e.SubCategories)
                    .HasForeignKey(e => e.ParentCategoryId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasQueryFilter(e => !e.IsDeleted);
            });

            // Tag Configuration
            modelBuilder.Entity<Tag>(entity =>
            {
                entity.ToTable("Tags");
                entity.HasKey(e => e.TagId);
                entity.Property(e => e.TagName).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Note).HasMaxLength(500);
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
                entity.Property(e => e.IsDeleted).HasDefaultValue(false);

                entity.HasIndex(e => e.TagName).IsUnique();
                entity.HasQueryFilter(e => !e.IsDeleted);
            });

            // NewsArticle Configuration
            modelBuilder.Entity<NewsArticle>(entity =>
            {
                entity.ToTable("NewsArticles");
                entity.HasKey(e => e.NewsArticleId);
                entity.Property(e => e.NewsTitle).IsRequired().HasMaxLength(500);
                entity.Property(e => e.Headline).HasMaxLength(1000);
                entity.Property(e => e.NewsContent).IsRequired();
                entity.Property(e => e.NewsSource).HasMaxLength(200);
                entity.Property(e => e.NewsStatus).IsRequired().HasMaxLength(50);
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
                entity.Property(e => e.IsDeleted).HasDefaultValue(false);

                entity.HasOne(e => e.Category)
                    .WithMany(c => c.NewsArticles)
                    .HasForeignKey(e => e.CategoryId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.CreatedByAccount)
                    .WithMany()
                    .HasForeignKey(e => e.CreatedBy)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasQueryFilter(e => !e.IsDeleted);
            });

            // NewsTag Configuration (Many-to-Many)
            modelBuilder.Entity<NewsTag>(entity =>
            {
                entity.ToTable("NewsTags");
                entity.HasKey(e => new { e.NewsArticleId, e.TagId });

                entity.HasOne(e => e.NewsArticle)
                    .WithMany(n => n.NewsTags)
                    .HasForeignKey(e => e.NewsArticleId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.Tag)
                    .WithMany(t => t.NewsTags)
                    .HasForeignKey(e => e.TagId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var entries = ChangeTracker.Entries()
                .Where(e => e.Entity is SystemAccount || e.Entity is Category || 
                           e.Entity is Tag || e.Entity is NewsArticle);

            foreach (var entry in entries)
            {
                if (entry.State == EntityState.Added)
                {
                    entry.Property("CreatedAt").CurrentValue = DateTime.UtcNow;
                    entry.Property("IsDeleted").CurrentValue = false;
                }
                else if (entry.State == EntityState.Modified)
                {
                    entry.Property("UpdatedAt").CurrentValue = DateTime.UtcNow;
                }
            }

            return base.SaveChangesAsync(cancellationToken);
        }
    }
}

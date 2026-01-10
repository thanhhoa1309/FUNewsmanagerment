using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Entities
{
    public class NewsArticle
    {
        public int NewsArticleId { get; set; }
        public string NewsTitle { get; set; } = null!;
        public string? Headline { get; set; }
        public string NewsContent { get; set; } = null!;
        public string? NewsSource { get; set; }
        public string NewsStatus { get; set; } = null!;

        public int CategoryId { get; set; }
        public int CreatedBy { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }
        public bool IsDeleted { get; set; }

        // Navigation properties
        public Category Category { get; set; } = null!;
        public SystemAccount CreatedByAccount { get; set; } = null!;
        public ICollection<NewsTag> NewsTags { get; set; } = new List<NewsTag>();
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Entities
{
    public class NewsTag
    {
        public int NewsArticleId { get; set; }
        public int TagId { get; set; }

        public NewsArticle NewsArticle { get; set; } = null!;
        public Tag Tag { get; set; } = null!;
    }
}

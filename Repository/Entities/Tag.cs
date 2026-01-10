using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Entities
{
    public class Tag
    {
        public int TagId { get; set; }
        public string TagName { get; set; } = null!;
        public string? Note { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }
        public bool IsDeleted { get; set; }

        // Navigation properties
        public ICollection<NewsTag> NewsTags { get; set; } = new List<NewsTag>();
    }
}

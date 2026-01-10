using Repository.DTOs.Tag;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.DTOs.NewsArtiticle
{
    public class NewsArticleResponseDTO
    {
        public int NewsArticleId { get; set; }
        public string NewsTitle { get; set; } = null!;
        public string? Headline { get; set; }
        public string NewsContent { get; set; } = null!;
        public string? NewsSource { get; set; }
        public string NewsStatus { get; set; } = null!;
        public int CategoryId { get; set; }
        public string? CategoryName { get; set; }
        public int CreatedBy { get; set; }
        public string? CreatedByName { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public List<TagResponseDTO>? Tags { get; set; }
    }
}

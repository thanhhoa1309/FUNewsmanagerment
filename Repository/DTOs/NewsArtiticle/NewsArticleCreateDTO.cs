using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.DTOs.NewsArtiticle
{
    public class NewsArticleCreateDTO
    {
        [Required]
        [StringLength(500)]
        public string NewsTitle { get; set; } = null!;

        [StringLength(1000)]
        public string? Headline { get; set; }

        [Required]
        public string NewsContent { get; set; } = null!;

        [StringLength(200)]
        public string? NewsSource { get; set; }

        [Required]
        [StringLength(50)]
        public string NewsStatus { get; set; } = null!;

        [Required]
        public int CategoryId { get; set; }

        public List<int>? TagIds { get; set; }
    }
}

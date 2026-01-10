using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.DTOs.NewsArtiticle
{
    public class NewsArticleUpdateDTO
    {
        [StringLength(500)]
        public string? NewsTitle { get; set; }

        [StringLength(1000)]
        public string? Headline { get; set; }

        public string? NewsContent { get; set; }

        [StringLength(200)]
        public string? NewsSource { get; set; }

        [StringLength(50)]
        public string? NewsStatus { get; set; }

        public int? CategoryId { get; set; }

        public List<int>? TagIds { get; set; }
    }
}

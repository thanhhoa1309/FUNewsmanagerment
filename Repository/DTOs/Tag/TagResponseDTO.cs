using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.DTOs.Tag
{
    public class TagResponseDTO
    {
        public int TagId { get; set; }
        public string TagName { get; set; } = null!;
        public string? Note { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}

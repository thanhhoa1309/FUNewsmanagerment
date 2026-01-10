using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.DTOs.Category
{
    public class CategoryCreateDTO
    {
        [Required]
        [StringLength(200)]
        public string CategoryName { get; set; } = null!;

        [StringLength(500)]
        public string? CategoryDescription { get; set; }

        public int? ParentCategoryId { get; set; }

        public bool IsActive { get; set; } = true;
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.DTOs.Tag
{
    public class TagUpdateDTO
    {
        [StringLength(100)]
        public string? TagName { get; set; }

        [StringLength(500)]
        public string? Note { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.DTOs.SystemAccount
{
    public class SystemAccountUpdateDTO
    {
        [StringLength(100)]
        public string? AccountName { get; set; }

        [EmailAddress]
        [StringLength(150)]
        public string? AccountEmail { get; set; }

        [StringLength(50)]
        public string? AccountRole { get; set; }

        [StringLength(255)]
        public string? AccountPassword { get; set; }
    }
}

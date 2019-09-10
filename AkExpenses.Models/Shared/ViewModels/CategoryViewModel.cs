using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace AkExpenses.Models.Shared.ViewModels
{
    public class CategoryViewModel
    {
        [Required]
        [StringLength(50)]
        public string Name { get; set; }

        [StringLength(256)]
        public string Description { get; set; }

        public string CategoryId { get; set; }

    }
}

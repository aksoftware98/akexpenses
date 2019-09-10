using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace AkExpenses.Models.Shared.ViewModels
{
    public class ProviderViewModel
    {
        [Required]
        [StringLength(50)]
        public string Name { get; set; }

        public string ProviderId { get; set; }
    }
}

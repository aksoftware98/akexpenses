using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace AkExpenses.Models.Shared.ViewModels
{
    public class DebtViewModel
    {

        public string Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Title { get; set; }

        [StringLength(256)]
        public string Description { get; set; }

        [Required]
        public DateTime DebtDate { get; set; }

        [Required]
        public bool IsPaid { get; set; }

        [Required]
        public decimal Amount { get; set; }

        [StringLength(256)]
        public string Provider { get; set; }

    }
}

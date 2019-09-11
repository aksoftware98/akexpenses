using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace AkExpenses.Models.Shared.ViewModels
{
    public class PayViewModel
    {
        public string Id { get; set; }

        [Required]
        [StringLength(75)]
        public string Title { get; set; }

        [StringLength(256)]
        public string Description { get; set; }

        public decimal Amount { get; set; }

        public DateTime PayDate { get; set; }

        public string MoneyTypeId { get; set; }

        public string CategoryId { get; set; }

    }
}

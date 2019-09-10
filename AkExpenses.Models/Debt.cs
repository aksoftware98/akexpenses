using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AkExpenses.Models
{
    public class Debt
    {
        [Key]
        public string Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Title { get; set; }

        [StringLength(256)]
        public string Description { get; set; }

        public DateTime DebtDate { get; set; }

        public DateTime CreatedDate { get; set; }

        public decimal Amount { get; set; }

        public bool IsPaid { get; set; } = false; 

        [StringLength(50)]
        public string Provider { get; set; }

        public ICollection<DebtPayment> DebtPayments { get; set; }


        [Required]
        public Account Account { get; set; }

        public string AccountId { get; set; }

    }

}

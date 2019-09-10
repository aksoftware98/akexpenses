using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AkExpenses.Models
{
    public class DebtPayment
    {
        [Key]
        public string Id { get; set; }
        
        [Required]
        public Debt Debt { get; set; }

        public decimal Amount { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime PaymentDate { get; set; }

        [ForeignKey(nameof(Debt))]
        public string DebtId { get; set; }

    }

}

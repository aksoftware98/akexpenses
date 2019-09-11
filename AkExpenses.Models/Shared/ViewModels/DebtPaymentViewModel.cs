using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace AkExpenses.Models.Shared.ViewModels
{
    public class DebtPaymentViewModel
    {
        
        public string Id { get; set; }

        [Required]
        [StringLength(50)]
        public string DebtId { get; set; }
        
        [Required]
        public decimal Amount { get; set; }

        [Required]
        public DateTime PaymentDate { get; set; }

    }
}

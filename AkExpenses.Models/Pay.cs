using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AkExpenses.Models
{
    public class Pay
    {
        public string Id { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public decimal Amount { get; set; }

        public DateTime PayDate { get; set; }

        public MoneyType MoneyType { get; set; }

        [ForeignKey(nameof(MoneyType))]
        public string MoneyTypeId { get; set; }

        public Category Category { get; set; }

        public string CategoryId { get; set; }

        public DateTime CreatedDate { get; set; }


        [Required]
        public Account Account { get; set; }

        public string AccountId { get; set; }

    }

}

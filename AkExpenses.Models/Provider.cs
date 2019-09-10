using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AkExpenses.Models
{
    public class Provider
    {

        [Key]
        public string Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; }

        public DateTime CreatedDate { get; set; }

        public ICollection<Income> Incomes { get; set; }


        [Required]
        public Account Account { get; set; }

        public string AccountId { get; set; }

    }

}

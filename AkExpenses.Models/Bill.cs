using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AkExpenses.Models
{
    public class Bill
    {
        [Key]
        public string Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Number { get; set; }

        [StringLength(256)]
        public string Description { get; set; }

        [StringLength(256)]
        public string Provider { get; set; }

        public DateTime BillDate { get; set; }

        public DateTime CreatedDate { get; set; }

        public ICollection<Outcome> Outcomes { get; set; }


        [Required]
        public Account Account { get; set; }

        public string AccountId { get; set; }

    }

}

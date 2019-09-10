using System.Collections;
using System.ComponentModel.DataAnnotations;

namespace AkExpenses.Models
{
    public class Account
    {
        [Key]
        public string Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; }

        [StringLength(500)]
        public string Description { get; set; }
    }

}

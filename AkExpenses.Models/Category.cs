using System.ComponentModel.DataAnnotations;

namespace AkExpenses.Models
{
    public class Category
    {
        [Key]
        public string Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; }
        
        [StringLength(256)]
        public string Description { get; set; }

        [Required]
        [StringLength(256)]
        public string IconPath { get; set; }

        [Required]
        public Account Account { get; set; }

        public string AccountId { get; set; }
    }

}

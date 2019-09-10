using System.ComponentModel.DataAnnotations.Schema;

namespace AkExpenses.Models
{
    public class Income : Pay
    {

        public Provider Provider { get; set; }
        
        [ForeignKey("Provided")]
        public string ProvidedId { get; set; }

    }

}

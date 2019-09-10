using System.ComponentModel.DataAnnotations.Schema;

namespace AkExpenses.Models
{
    public class Outcome : Pay
    {
        public Bill Bill { get; set; }

        [ForeignKey(nameof(Bill))]
        public string BillId { get; set; }
    }

}

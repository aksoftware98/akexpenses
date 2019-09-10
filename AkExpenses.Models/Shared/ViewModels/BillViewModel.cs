using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace AkExpenses.Models.Shared.ViewModels
{
    public class BillViewModel
    {
        public string Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Number { get; set; }

        [StringLength(256)]
        public string Description { get; set; }

        [Required]
        public DateTime BillDate { get; set; }

        [StringLength(256)]
        public string Provider { get; set; }

        [FromForm(Name = "Attachment")]
        public IFormFile Attachment { get; set; }
    }
}

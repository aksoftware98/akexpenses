﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AkExpenses.Models.Shared.ViewModels
{
    public class MoneyTypeViewModel
    {
        [Required]
        [StringLength(50)]
        public string Name { get; set; }

        [StringLength(265)]
        public string Description { get; set; }

        public string MoneyTypeId { get; set; }

    }
}

using Pustok.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Pustok.ViewModels
{
    public class OrderTrackCodeViewModel
    {
        [Required]
        [StringLength(maximumLength: 20)]
        public string Code { get; set; }
        public Order Order { get; set; }
    }
}

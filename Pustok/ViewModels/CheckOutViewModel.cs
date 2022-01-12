using Pustok.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pustok.ViewModels
{
    public class CheckOutViewModel
    {
        public List<CheckoutItemViewModel> CheckoutItems { get; set; }
        public Order Order { get; set; }
    }
}

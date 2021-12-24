﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pustok.ViewModels
{
    public class BasketViewModel
    {
        public decimal TotalAmount { get; set; }
        public List<BasketItemViewModel> BasketItems { get; set; }
        public class BasketItemViewModel
        {
            public int Count { get; set; }
            public int BookId { get; set; }
            public decimal Price { get; set; }
            public string Name { get; set; }
            public string PosterImage { get; set; }
            public decimal TotalPrice { get; set; }
        }
    }
}
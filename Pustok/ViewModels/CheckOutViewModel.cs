﻿using Pustok.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pustok.ViewModels
{
    public class CheckOutViewModel
    {
        public Book Book { get; set; }
        public int Count { get; set; }
        public decimal TotalAmount { get; set; }
    }
}
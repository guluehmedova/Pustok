﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Pustok.Models
{
    public class Genre:BaseEntity
    {
        [StringLength(maximumLength:50)]
        public string Name { get; set; }
        public List<Book> Books { get; set; }
    }
}

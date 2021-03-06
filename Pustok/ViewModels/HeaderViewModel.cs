using Pustok.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pustok.ViewModels
{
    public class HeaderViewModel
    {
        public List<Setting> Settings { get; set; }
        public List<Genre> Genres { get; set; }
        public BasketViewModel Basket { get; set; }
        public List<Book> Books { get; set; }
    }
}

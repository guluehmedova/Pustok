using Pustok.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pustok.ViewModels
{
    public class BookViewModel
    {
        public List<Book> Books { get; set; }
        public List<Genre> Genres { get; set; }
        public PagenatedList<Book> PagenatedBookLists { get; set; }
    }
}

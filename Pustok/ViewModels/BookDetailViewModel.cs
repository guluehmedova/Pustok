using Pustok.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pustok.ViewModels
{
    public class BookDetailViewModel
    {
        public List<Book> RelatedBooks { get; set; }
        public Book Book { get; set; }
    }
}

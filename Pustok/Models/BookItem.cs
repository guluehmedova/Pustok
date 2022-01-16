using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pustok.Models
{
    public class BookItem:BaseEntity
    {
        public int BookId { get; set; }
        public string AppUserId { get; set; }
        public bool Status { get; set; }
        public AppUser User { get; set; }
        public Book Book { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Pustok.Models
{
    public class Tag:BaseEntity
    {
        [StringLength(maximumLength: 25)]
        public string Name { get; set; }
        public List<BookTag> BookTags { get; set; }
    }
}

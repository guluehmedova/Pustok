using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pustok.Models
{
    public class SocialMedia:BaseEntity
    {
        public string Icon { get; set; }
        public string Bgcolor { get; set; }
        public string Name { get; set; }
        public string RedirectUrl { get; set; }

    }
}

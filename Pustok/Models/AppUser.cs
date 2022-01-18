using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Pustok.Models
{
    public class AppUser:IdentityUser
    {
        [StringLength(maximumLength:50)]
        public string Fullname { get; set; }
        public bool IsAdmin { get; set; }
        public DateTime LastConnectDate { get; set; }
        [StringLength(maximumLength: 50)]
        public string ConnectionId { get; set; }
        //public List<BookComment> BookComments { get; set; }
        //public List<BookComment> Comments { get; set; }
        //public List<BasketItem> BasketItems { get; set; }
        //public List<Order> Orders { get; set; }
    }
}

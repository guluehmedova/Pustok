using Pustok.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pustok.ViewModels
{
    public class ProfileViewModel
    {
        public MemberProfilViewModel  Member{ get; set; }
        public List<Order> Orders { get; set; }
    }
}

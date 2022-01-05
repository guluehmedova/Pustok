using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Pustok.Areas.Manage.ViewModels
{
    public class AdminLoginViewModel
    {
        [StringLength(maximumLength:25, MinimumLength =3)]
        public string Username { get; set; }
        [StringLength(maximumLength:25,MinimumLength =8)]
        public string Password { get; set; }
    }
}

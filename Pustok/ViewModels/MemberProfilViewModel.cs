using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Pustok.ViewModels
{
    public class MemberProfilViewModel
    {
        [StringLength(maximumLength:20)]
        public string UserName { get; set; }
        [StringLength(maximumLength: 20)]
        public string FullName { get; set; }
        [StringLength(maximumLength: 20)]
        public string Email { get; set; }
        [DataType(DataType.Password)]
        [StringLength(maximumLength:25,MinimumLength =8)]
        public string NewPassword { get; set; }
        [DataType(DataType.Password),Compare(nameof(NewPassword))]
        [StringLength(maximumLength: 25, MinimumLength = 8)]
        public string ConfirmNewPassword { get; set; }
        [DataType(DataType.Password)]
        [StringLength(maximumLength: 25, MinimumLength = 8)]
        public string CurrentPassword { get; set; }
    }
}

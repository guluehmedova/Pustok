using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Pustok.Models
{
    public class Slider:BaseEntity
    {
        [StringLength(maximumLength:150)]
        public string Title1 { get; set; }
        [StringLength(maximumLength: 150)]
        public string Title2 { get; set; }
        [StringLength(maximumLength: 500)]
        public string Desc { get; set; }
        [StringLength(maximumLength: 20)]
        public string BtnText { get; set; }
        [StringLength(maximumLength: 250)]
        public string RedirectUrl { get; set; }
        [StringLength(maximumLength:100)]
        public string Image { get; set; }
        [Required]
        public int Order { get; set; }
        [NotMapped]
        public IFormFile ImageFile { get; set; }
    }
}

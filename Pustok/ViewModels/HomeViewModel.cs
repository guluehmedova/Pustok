using Pustok.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pustok.ViewModels
{
    public class HomeViewModel
    {
        public List<Slider> Sliders { get; set; }
        public List<Feature> Features { get; set; }
        public List<Book> FeaturedBooks { get; set; }
        public List<Book> NewBooks { get; set; }
        public List<Book> DiscountedBooks { get; set; }
        public List<PromotionOne> PromotionOnes { get; set; }
        public List<Image> Images { get; set; }
        public List<BookTag> BookTags { get; set; }
        public List<Genre> Genres { get; set; }
        public List<NewBookImage>  newBookImages { get; set; }
    }
}

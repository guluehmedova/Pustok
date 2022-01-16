using Pustok.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pustok.Areas.Manage.ViewModels
{
    public class HomeViewModelArea
    {
        public List<Author> Authors { get; set; }
        public List<Book> Books { get; set; }
        public List<Genre> Genres { get; set; }
        public List<Feature> Features { get; set; }
        public List<Tag> Tags { get; set; }
        public List<Slider> Slider { get; set; }
        public List<Setting> Settings { get; set; }
        public PagenatedList<Author> PagenatedAuthors { get; set; }
        public PagenatedList<Book> PagenatedBooks { get; set; }
        public PagenatedList<Feature> PagenatedFeatures { get; set; }
        public PagenatedList<Genre> PagenatedGenres { get; set; }
        public PagenatedList<Setting> PagenatedSettings { get; set; }
        public PagenatedList<Slider> PagenatedSliders { get; set; }
        public PagenatedList<Tag> PagenatedTags { get; set; }
    }
}

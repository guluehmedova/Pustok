using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Pustok.ViewModels.BasketViewModel;

namespace Pustok.Models
{
    public class PustokContext:IdentityDbContext
    {
        public PustokContext(DbContextOptions<PustokContext> options):base(options)
        {

        }
        public DbSet<Slider> Sliders { get; set; }
        public DbSet<Feature> Features { get; set; }
        public DbSet<Author> Authors { get; set; }
        public DbSet<Genre> Genres { get; set; }
        public DbSet<Book> Books { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<BookTag> BookTags { get; set; }
        public DbSet<PromotionOne> PromotionOnes { get; set; }
        public DbSet<Image> Images { get; set; }
        public DbSet<Setting> Settings { get; set; }
        public DbSet<SocialMedia> SocialMedias { get; set; }
        public DbSet<NewBookImage> NewBookImages { get; set; }
        public DbSet<AppUser> appUsers { get; set; }
        public DbSet<BookComment> BookComments { get; set; }
        public DbSet<BasketItem> BasketItems { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<BookItem> BookItems { get; set; }
    }
}

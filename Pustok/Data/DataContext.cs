using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Pustok.Models;

namespace Pustok.Data
{
    public class DataContext:IdentityDbContext
    {
        public DataContext(DbContextOptions dbContextOptions):base(dbContextOptions)
        {

        }
        public DbSet<Slider> Sliders { get; set; }
        public DbSet<Feature> Features { get; set; }
        public DbSet<Book> Books { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Author> Authors { get; set; }
        public DbSet<BookImage> BookImages { get; set; }
        public DbSet<AppUser> Users { get; set; }
        public DbSet<BasketItem> BasketItems { get; set; }
        public DbSet<Order> Orders { get; set; }    
        public DbSet<OrderItem> OrderItems { get; set; }
    }
}

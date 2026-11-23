using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Cairo_Cinema.API.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
        {
        }
        public DbSet<Movie> Movies { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Actor> Actors { get; set; }
        public DbSet<Cinema> Cinemas { get; set; }
        public DbSet<MovieSubImage> MovieSubImages { get; set; }
        public DbSet<ApplicationUserOtp> ApplicationUserOtps { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<Coupon> Coupons { get; set; }

        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{
        //    base.OnConfiguring(optionsBuilder);
        //    optionsBuilder.UseSqlServer("Data Source=.;Initial Catalog=Cairo_Cinema;Integrated Security=True;Connect Timeout=30;Encrypt=True;Trust Server Certificate=true;");
        //}



    }
}

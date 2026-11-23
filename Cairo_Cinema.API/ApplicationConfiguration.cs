using Cairo_Cinema.API.Utilities;
using Cairo_Cinema.API.Utilities.DbInitializer;
using Cairo_Cinema.API.Utilities.DbSeeding;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Stripe;

namespace Cairo_Cinema.API
{
    public static class ApplicationConfiguration
    {
    public static void RegisterConfig(this IServiceCollection services)
        {
            // Life time
            // AddTransient, AddScoped, AddSingleton
            services.AddScoped<IRepository<Movie>, Repository<Movie>>();
            services.AddScoped<IRepository<Category>, Repository<Category>>();
            services.AddScoped<IRepository<Cinema>, Repository<Cinema>>();
            services.AddScoped<IRepository<Actor>, Repository<Actor>>();
            services.AddScoped<IRepository<ApplicationUserOtp>, Repository<ApplicationUserOtp>>();
            services.AddScoped<IMovieSubImageRepository, MovieSubImageRepository>();
            services.AddScoped<IRepository<Cart>, Repository<Cart>>();
            services.AddTransient<IEmailSender, EmailSender>();
            services.AddScoped<IDbInitializer, DbInitializer>();
            services.AddScoped<IRepository<Models.Coupon>, Repository<Models.Coupon>>();
          
        }
    }
}

using Cairo_Cinema.API.Utilities.DbSeeding;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using Stripe;
namespace Cairo_Cinema.API
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();

            // Add services to the container.
            builder.Services.AddControllersWithViews();
            //__________________________________________________________________
            //Data
            var connectionString =
            builder.Configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string"
            + "'DefaultConnection' not found.");

            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(connectionString));
            //__________________________________________________________________
            // Life time
            // AddTransient, AddScoped, AddSingleton
            // ApplicationConfiguration.RegisterConfig(builder.Services);  
            builder.Services.RegisterConfig();
            //__________________________________________________________________
            //Identity
            builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                options.User.RequireUniqueEmail = true;
                options.SignIn.RequireConfirmedEmail = true;
                options.Lockout.MaxFailedAccessAttempts = 10;
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(1);
                options.Lockout.AllowedForNewUsers = true;
                //options.SignIn.RequireConfirmedEmail = true;
            })
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();
            //__________________________________________________________________
            builder.Services.ConfigureApplicationCookie(options =>
            {
                // Redirect paths
                options.LoginPath = "/Identity/Account/Login"; // Where to redirect unauthenticated users
                options.AccessDeniedPath = "/Identity/Account/AccessDenied"; // Where to redirect forbidden users
            });
            //__________________________________________________________________

            StripeConfiguration.ApiKey = builder.Configuration["Stripe:SecretKey"];

            builder.Services.AddSession();

            var app = builder.Build();
            
            using (var scope = app.Services.CreateScope())
            {
                var dbInitializer = scope.ServiceProvider.GetRequiredService<IDbInitializer>();
                await dbInitializer.InitializeAsync();
            }

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
                app.MapScalarApiReference();
            }

            app.UseHttpsRedirection();
            app.UseRouting();

            app.UseSession();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}

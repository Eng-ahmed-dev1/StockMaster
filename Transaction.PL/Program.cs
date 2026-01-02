using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Transaction.BLL;
using TransactionsTask.Data;
using TransactionsTask.Repos.ProductRepo;
using TransactionsTask.Repos.SupplierRepo;
using TransactionsTask.Repos.TransactionRepo;
using AutoMapper;
using Transaction.DAL;
using Microsoft.AspNetCore.Identity;
using NuGet.Packaging;

namespace TransactionsTask
{
    public class Program
    {
        public static async Task Main(string[] args) 
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllersWithViews();

            builder.Services.AddDbContext<InventoryDB>(options =>
            options.UseSqlServer(
                builder.Configuration.GetConnectionString("DevConn")
            )
            );


            // Identity
            builder.Services.AddIdentity<SystemUsers, IdentityRole>(options =>
            {
                // Password Settings
                options.Password.RequiredLength = 8;
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequiredUniqueChars = 2;

                // User Settings
                options.User.RequireUniqueEmail = true;

                // Lockout Settings
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(30); 
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.AllowedForNewUsers = true;

                // Sign In Settings
                options.SignIn.RequireConfirmedEmail = false;
            })
            .AddEntityFrameworkStores<InventoryDB>()
            .AddDefaultTokenProviders();


            builder.Services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = "/Account/Login";
                options.AccessDeniedPath = "/Account/AccessDenied";
                options.LogoutPath = "/Account/Logout";
                options.ExpireTimeSpan = TimeSpan.FromDays(7); 
                options.SlidingExpiration = true;
            });
            // PDF Services
            QuestPDF.Settings.License = QuestPDF.Infrastructure.LicenseType.Community;

            // Repositories (DAL)
            builder.Services.AddScoped<IProductRepo, ProductRepo>();
            builder.Services.AddScoped<ITransactionRepo, TransactionRepo>();
            builder.Services.AddScoped<ISupplierRepo, SupplierRepo>();

            // Services (BLL)
            builder.Services.AddScoped<IProductService, ProductServies>();
            builder.Services.AddScoped<ITransactionServices, TransactionServices>();
            builder.Services.AddScoped<ISupplierServices, SupplierServices>();
            builder.Services.AddScoped<IAccountServices, AccountServices>(); 

            // AutoMapper
            builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

            var app = builder.Build();

            await SeedDatabaseAsync(app); 

            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthentication(); 
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Account}/{action=Login}/{id?}");

            await app.RunAsync();
        }

        private static async Task SeedDatabaseAsync(WebApplication app)
        {
            using var scope = app.Services.CreateScope();
            var services = scope.ServiceProvider;

            try
            {
                var userManager = services.GetRequiredService<UserManager<SystemUsers>>();
                var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

                var seeded = await IdentityContextSeedData.SeedData(userManager, roleManager);

                if (seeded)
                {
                    Console.WriteLine("Database seeded successfully!");
                }
                else
                {
                    Console.WriteLine("Database already contains data, skipping seed.");
                }
            }
            catch (Exception ex)
            {
                var logger = services.GetRequiredService<ILogger<Program>>();
                logger.LogError(ex, "An error occurred while seeding the database.");
            }
        }
    }
}
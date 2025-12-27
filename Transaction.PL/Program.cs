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
namespace TransactionsTask
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllersWithViews();
            // Repos in DAL
            builder.Services.AddScoped<IProductRepo, ProductRepo>();
            builder.Services.AddScoped<ITransactionRepo, TransactionRepo>();
            builder.Services.AddScoped<ISupplierRepo, SupplierRepo>();
            //Services in BLL
            builder.Services.AddScoped<IProductService, ProductServies>();
            builder.Services.AddScoped<ITransactionServices, TransactionServices>();
            builder.Services.AddScoped<ISupplierServices, SupplierServices>();
            builder.Services.AddDbContext<InventoryDB>(o => o.UseSqlServer(builder.Configuration.GetConnectionString("DevConn")));
            //Services of Mapper
            builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
            //Add Identity 
            builder.Services.AddIdentity<SystemUsers,IdentityRole>(options=>
            {
                // Password Settings 
                options.Password.RequiredUniqueChars = 2;
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = true;
                options.Password.RequiredLength = 8;
                options.Password.RequireNonAlphanumeric = true;
               

                // User Settings 
                options.User.RequireUniqueEmail = true;

                // Lockout Settings 
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromHours(5);
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.AllowedForNewUsers = true;
                // Sign In Settings 
                options.SignIn.RequireConfirmedEmail = false;

            }).AddEntityFrameworkStores<InventoryDB>()
            .AddDefaultTokenProviders();
           

            var app = builder.Build();
            // To seed Data
            using(var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;

                var UserManager = services.GetRequiredService<UserManager<SystemUsers>>();
                var UserRoles = services.GetRequiredService<RoleManager<IdentityRole>>();

                IdentityContextSeedData
                    .SeedData(UserManager, UserRoles)
                    .GetAwaiter()
                    .GetResult();
            }
            
            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
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

            app.Run();
        }
    }
}
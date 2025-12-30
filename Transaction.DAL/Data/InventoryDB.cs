using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Transaction.DAL;
using TransactionsTask.Models;
namespace TransactionsTask.Data
{
    public class InventoryDB : IdentityDbContext<SystemUsers>
    {
        public InventoryDB(DbContextOptions options) : base(options) { }

        public DbSet<Products> Products { get; set; }
        public DbSet<Transactions> Transactions { get; set; }
        public DbSet<Suppliers> Suppliers { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // This is important for the Identity
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Suppliers>(
                o => o.HasIndex(x => x.SupplierEmail).IsUnique()
            );
            modelBuilder.Entity<Transactions>(
                o => o.HasOne(x => x.Supplier)
                .WithMany(x => x.Transactions)
                .HasForeignKey(x => x.SupplierId)
                .OnDelete(DeleteBehavior.SetNull)
            );
            modelBuilder.Entity<Transactions>(
                o => o.HasOne(x => x.Product)
                .WithMany(x => x.Transactions)
                .HasForeignKey(x => x.ProductId)
                .OnDelete(DeleteBehavior.SetNull)
            );
            modelBuilder.Entity<Transactions>(
                o => o.Property(x => x.TransactionType).HasConversion<string>()
            );
        }
    }
}
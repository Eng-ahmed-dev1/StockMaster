using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Transaction.DAL;
using TransactionsTask.Models;

namespace TransactionsTask.Data
{
    public class InventoryDB : IdentityDbContext<SystemUsers>
    {
        public InventoryDB(DbContextOptions<InventoryDB> options) : base(options) { }

        public DbSet<Products> Products { get; set; }
        public DbSet<Transactions> Transactions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Transactions>()
                .HasOne(t => t.CreatedBy)
                .WithMany(u => u.CreatedTransactions)
                .HasForeignKey(t => t.CreatedByUserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Transactions>()
                .HasOne(t => t.Supplier)
                .WithMany(u => u.SuppliedTransactions)
                .HasForeignKey(t => t.SupplierId)
                .OnDelete(DeleteBehavior.SetNull); 

            modelBuilder.Entity<Transactions>()
                .HasOne(t => t.Product)
                .WithMany(p => p.Transactions)
                .HasForeignKey(t => t.ProductId)
                .OnDelete(DeleteBehavior.Cascade); 

            // TransactionType enum as string
            modelBuilder.Entity<Transactions>()
                .Property(t => t.TransactionType)
                .HasConversion<string>();

            // Optional: Indexes for performance
            modelBuilder.Entity<Transactions>()
                .HasIndex(t => t.TransactionDate);

            modelBuilder.Entity<Transactions>()
                .HasIndex(t => t.SupplierId);

            modelBuilder.Entity<Transactions>()
                .HasIndex(t => t.ProductId);
        }
    }
}
using Microsoft.EntityFrameworkCore;
using Transaction.DAL;
using TransactionsTask.Data;
using TransactionsTask.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TransactionsTask.Repos.SupplierRepo
{
    public class SupplierRepo : ISupplierRepo
    {
        private readonly InventoryDB db;
        public SupplierRepo(InventoryDB dB) => db = dB;

        public async Task AddSupplier(SystemUsers supplier)
        {
            await db.Users.AddAsync(supplier);
            await db.SaveChangesAsync();
        }

        public async Task DeleteSupplier(SystemUsers supplier)
        {
            db.Users.Remove(supplier);
            await db.SaveChangesAsync();
        }

        public async Task<bool> FindDuplicateEmail(string Email)
        {
            return await db.Users.AnyAsync(sup => sup.Email == Email);
        }

        public async Task<SystemUsers?> GetSupplierId(string id)
        {
            return await db.Users
            .FirstOrDefaultAsync(sup => sup.Id == id);
        }

        public async Task<SystemUsers?> GetSupplierIdWithDetails(string id)
        {
            return await db.Users
                .Include(x => x.SuppliedTransactions)
                .ThenInclude(x => x.Product)
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<IEnumerable<SystemUsers>> GetSuppliers()
        {
            return await db.Users.AsNoTracking().ToListAsync();
        }

        public async Task UpdateSupplier(SystemUsers supplier)
        {
            db.Users.Update(supplier);
            await db.SaveChangesAsync();
        }
    }
}

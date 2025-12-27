using Microsoft.EntityFrameworkCore;
using TransactionsTask.Data;
using TransactionsTask.Models;

namespace TransactionsTask.Repos.SupplierRepo
{
    public class SupplierRepo : ISupplierRepo
    {
        private readonly InventoryDB db;
        public SupplierRepo(InventoryDB dB) => db = dB;

        public async Task AddSupplier(Suppliers supplier)
        {
            await db.Suppliers.AddAsync(supplier);
            await db.SaveChangesAsync();

        }

        public async Task DeleteSupplier(Suppliers supplier)
        {
            db.Suppliers.Remove(supplier);
            await db.SaveChangesAsync();
        }

        public async Task<bool> FindDuplicateEmail(string Email)
        {
            return await db.Suppliers.AnyAsync(sup => sup.SupplierEmail == Email);
        }

        public async Task<Suppliers?> GetSupplierId(int id)
        {
            return await db.Suppliers
                .FirstOrDefaultAsync(sup => sup.SupplierId == id);
        }

        public async Task<Suppliers?> GetSupplierIdWithDetails(int id)
        {
            return await db.Suppliers
                .Include(x => x.Transactions)
                .ThenInclude(x=>x.Product)
                .FirstOrDefaultAsync(x=>x.SupplierId == id );
        }

        public async Task<IEnumerable<Suppliers>> GetSuppliers()
        {
            return await db.Suppliers.AsNoTracking().ToListAsync();
        }

        public async Task UpdateSupplier(Suppliers supplier)
        {
            db.Suppliers.Update(supplier);
            await db.SaveChangesAsync();
        }
    }
}


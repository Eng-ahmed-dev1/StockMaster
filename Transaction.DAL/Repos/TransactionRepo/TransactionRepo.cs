using Microsoft.EntityFrameworkCore;
using TransactionsTask.Data;
using TransactionsTask.Models;

namespace TransactionsTask.Repos.TransactionRepo
{
    public class TransactionRepo : ITransactionRepo
    {
        private readonly InventoryDB db;
        public TransactionRepo(InventoryDB dB) => db = dB;

        public async Task AddTransaction(Transactions transaction)
        {
            await db.Transactions.AddAsync(transaction);
            await db.SaveChangesAsync();
        }

        public async Task DeleteTransaction(Transactions transaction)
        {
            db.Transactions.Remove(transaction);
            await db.SaveChangesAsync();
        }

        public async Task<IEnumerable<Transactions>> GetAllTransactions()
        {
            return await db.Transactions
                .Include(t => t.Product)
                .Include(t => t.Supplier)
                .Include(t => t.CreatedBy)
                .OrderByDescending(t => t.TransactionDate)
                .AsNoTracking()
                .ToListAsync();
        }
        public async Task<Transactions?> GetTransactionById(int id)
        {
            return await db.Transactions
           .Include(t => t.Product)      
           .Include(t => t.Supplier)       
           .Include(t => t.CreatedBy)    
           .FirstOrDefaultAsync(tran => tran.TransactionId == id);
        }

        public async Task<IEnumerable<Transactions>> GetTransactions()
        {
            return await db.Transactions.AsNoTracking().ToListAsync();
        }

        public async Task<IEnumerable<Transactions>> GetTransactionsBySupplierId(string userId)
        {
            return await db.Transactions
           .Include(t => t.Product)
           .Include(t => t.Supplier)
           .Include(t => t.CreatedBy)
           .Where(t => t.SupplierId == userId) 
           .OrderByDescending(t => t.TransactionDate) 
           .ToListAsync();
        }

        public async Task<IEnumerable<Transactions>> GetTransactionWithDetails()
        {
            return await db.Transactions
                .Include(x => x.Product)
                .Include(x => x.Supplier)
                .ToListAsync();
        }

        public async Task UpdateTransaction(Transactions transaction)
        {
            db.Transactions.Update(transaction);
            await db.SaveChangesAsync();
        }
    }
}
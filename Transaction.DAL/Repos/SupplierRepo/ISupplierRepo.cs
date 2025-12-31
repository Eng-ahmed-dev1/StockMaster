using Transaction.DAL;
using TransactionsTask.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TransactionsTask.Repos.SupplierRepo
{
    public interface ISupplierRepo
    {
        Task<SystemUsers?> GetSupplierId(string id);
        Task<SystemUsers?> GetSupplierIdWithDetails(string id);
        Task<IEnumerable<SystemUsers>> GetSuppliers();
        Task DeleteSupplier(SystemUsers supplier);
        Task UpdateSupplier(SystemUsers supplier);
        Task AddSupplier(SystemUsers supplier);
        Task<bool> FindDuplicateEmail(string Email);
    }
}

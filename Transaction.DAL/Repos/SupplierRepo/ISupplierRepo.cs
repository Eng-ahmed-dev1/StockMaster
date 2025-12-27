using TransactionsTask.Models;

namespace TransactionsTask.Repos.SupplierRepo
{
    public interface ISupplierRepo
    {
        Task<Suppliers?> GetSupplierId(int id);
        Task<Suppliers?> GetSupplierIdWithDetails(int id);
        Task<IEnumerable<Suppliers>> GetSuppliers();
        Task DeleteSupplier(Suppliers supplier);
        Task UpdateSupplier(Suppliers supplier);
        Task AddSupplier(Suppliers supplier);
        Task <bool> FindDuplicateEmail (string Email);
    }
}

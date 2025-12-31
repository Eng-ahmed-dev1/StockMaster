using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using TransactionsTask.Models;

namespace Transaction.BLL
{
    public interface ISupplierServices
    {
        Task<SupplierReadViewModel?> GetSupplierId(string id);
        Task<SupplierReadViewModel?> GetSupplierIdWithDetails(string id);
        Task<IEnumerable<SupplierReadViewModel>> GetSuppliers();
        Task<bool> DeleteSupplier(string id);
        Task<bool> UpdateSupplier(SupplierEditViewModel supplierEdit);
        Task<int> AddSupplier(SupplierCreateViewModel supplierCreate);
        Task<bool> FindDuplicateEmail(string Email);
    }
}
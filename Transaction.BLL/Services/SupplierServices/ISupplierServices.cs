using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using TransactionsTask.Models;

namespace Transaction.BLL
{
    public interface ISupplierServices
    {
        Task<UserViewModel?> GetSupplierId(string id);
        Task<UserViewModel?> GetSupplierIdWithDetails(string id);
        Task<IEnumerable<UserViewModel>> GetSuppliers();


        Task<bool> DeleteSupplier(string id);
        Task<bool> UpdateSupplier(EditUser supplierEdit);
        Task<int> AddSupplier(RegisterViewModel supplierCreate);
        Task<bool> FindDuplicateEmail(string Email);
    }
}
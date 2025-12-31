using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using TransactionsTask.Models;
using TransactionsTask.Repos.ProductRepo;
using TransactionsTask.Repos.SupplierRepo;

namespace Transaction.BLL
{

    public class SupplierServices : ISupplierServices
    {
        private readonly ISupplierRepo _db;
        private readonly IMapper _mapper;
        public SupplierServices(ISupplierRepo repo , IMapper mapper)
        {
            _db = repo;
            _mapper = mapper;
        }
        public async Task<int> AddSupplier(SupplierCreateViewModel supplierCreate)
        {

            var Supplier = _mapper.Map<SystemUsers>(supplierCreate);
            if (Supplier is null)
                throw new ArgumentNullException(nameof(Supplier));
            await _db.AddSupplier(Supplier);
            return int.Parse(Supplier.Id);
        }
        public async Task<bool> DeleteSupplier(string id)
        {
            var supplier = await _db.GetSupplierId(id);
            if (supplier is null)
                return false;
            await _db.DeleteSupplier(supplier);
            return true;
        }

        public async Task<bool> FindDuplicateEmail(string Email)
        {
            var IsDuplicted = _db.FindDuplicateEmail(Email);
            return await IsDuplicted;
        }

        public async Task<SupplierReadViewModel?> GetSupplierId(string id)
        {
            var supplier = await _db.GetSupplierId(id);
            if (supplier is null)
                throw new ArgumentNullException(nameof(supplier));
            return _mapper.Map<SupplierReadViewModel>(supplier);
        }

        public async Task<SupplierReadViewModel?> GetSupplierIdWithDetails(string id)
        {
            var supplier = await _db.GetSupplierIdWithDetails(id);
            if (supplier is null)
                throw new ArgumentNullException(nameof(supplier));
            return _mapper.Map<SupplierReadViewModel>(supplier);
        }

        public async Task<IEnumerable<SupplierReadViewModel>> GetSuppliers()
        {
            var suppliers = await _db.GetSuppliers();
            return _mapper.Map<IEnumerable<SupplierReadViewModel>>(suppliers);
        }

        public async Task<bool> UpdateSupplier(SupplierEditViewModel supplierEdit)
        {
            var supplier = await _db.GetSupplierId(supplierEdit.SupplierId);
            if (supplier is null)
                return false;
            _mapper.Map(supplierEdit, supplier);
            await _db.UpdateSupplier(supplier);
            return true;
        }

    }
}

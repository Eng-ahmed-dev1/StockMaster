using System;
using System.Collections.Generic;
using System.Text;
using TransactionsTask.Repos.ProductRepo;

namespace Transaction.BLL
{
    public interface IProductService
    {
        Task<IEnumerable<ProductReadViewModel>> GetAllProducts();
        Task<int> AddProduct(ProductCreateViewModel productCreate);
        Task<bool> UpdateProduct(ProductEditViewModel productEdit);
        Task<bool> DeleteProduct(int id);
        Task<ProductReadViewModel> GetProductById(int id);
        Task<ProductReadViewModel> GetProductDetailsById(int id);
        Task<bool> UpdateStockLevel(int productId, int quantityChange);

    }
}

using TransactionsTask.Models;

namespace TransactionsTask.Repos.ProductRepo
{
    public interface IProductRepo
    {
        Task<Products?> GetProductById(int id);
        Task<Products?> GetProductDetailsById(int id);
        Task<IEnumerable<Products>> GetProducts();
        Task DeleteProduct(Products product);
        Task UpdateProduct(Products product);
        Task AddProduct(Products product);
    }
}
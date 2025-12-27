using Microsoft.EntityFrameworkCore;
using TransactionsTask.Data;
using TransactionsTask.Models;

namespace TransactionsTask.Repos.ProductRepo
{
    public class ProductRepo : IProductRepo
    {
        private readonly InventoryDB _inventoryDB;
        public ProductRepo(InventoryDB dB) => _inventoryDB = dB;
        public async Task AddProduct(Products product)
        {
            await _inventoryDB.AddAsync(product);
           await _inventoryDB.SaveChangesAsync();
        }

        public async Task DeleteProduct(Products product)
        {
            _inventoryDB.Products.Remove(product);
            await _inventoryDB.SaveChangesAsync();
        }
        public async Task<Products?> GetProductById(int id)
        {
            return await _inventoryDB.Products
                .FirstOrDefaultAsync(product => product.ProductId == id);
        }

        public async Task<Products?> GetProductDetailsById(int id)
        {
            return await
                _inventoryDB.Products
                .Include(x => x.Transactions)
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.ProductId == id);
        }

        public async Task<IEnumerable<Products>> GetProducts()
        {
            return await _inventoryDB.Products.AsNoTracking().ToListAsync();
        }

        public async Task UpdateProduct(Products product)
        {
            _inventoryDB.Products.Update(product);
            await _inventoryDB.SaveChangesAsync();
        }
    }
}
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using TransactionsTask.Models;
using TransactionsTask.Repos.ProductRepo;

namespace Transaction.BLL
{
    public class ProductServies : IProductService
    {
        private readonly IProductRepo _db;
        private readonly IMapper _mapper;
        public ProductServies(IProductRepo db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }
        public async Task<int> AddProduct(ProductCreateViewModel productCreate)
        {

            var product = _mapper.Map<Products>(productCreate);
            if (product is null)
                throw new ArgumentNullException(nameof(product));
            await _db.AddProduct(product);

            return product.ProductId;
        }

        public async Task<bool> DeleteProduct(int id)
        {
            var product = await _db.GetProductById(id);
            if (product is null)
                return false;
            await _db.DeleteProduct(product);
            return true;
        }

        public async Task<IEnumerable<ProductReadViewModel>> GetAllProducts()
        {
            var products = await _db.GetProducts();
            return _mapper.Map<IEnumerable<ProductReadViewModel>>(products);
        }

        public async Task<ProductReadViewModel> GetProductById(int id)
        {
            var product = await _db.GetProductById(id);
            if (product is null)
                throw new ArgumentNullException(nameof(product));
            return _mapper.Map<ProductReadViewModel>(product);
        }

        public async Task<ProductReadViewModel> GetProductDetailsById(int id)
        {
            var product = await _db.GetProductDetailsById(id);
            if (product is null)
                throw new ArgumentNullException(nameof(product));
            return _mapper.Map<ProductReadViewModel>(product);
        }

        public async Task<bool> UpdateProduct(ProductEditViewModel productEdit)
        {
            var product = await _db.GetProductById(productEdit.ProductId);
            if (product is null)
                return false;
            _mapper.Map(productEdit, product);
            await _db.UpdateProduct(product);
            return true;
        }

        public async Task<bool> UpdateStockLevel(int productId, int quantityChange)
        {
            var Product = await _db.GetProductById(productId);

            if (Product is null)
                return false;

            Product.StockLevel += quantityChange;

            if(Product.StockLevel < 0)
                return false;

            await _db.UpdateProduct(Product);
            return true;

        }
    }
}

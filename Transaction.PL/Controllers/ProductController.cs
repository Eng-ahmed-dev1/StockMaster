using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Transaction.BLL;
using TransactionsTask.Models;
using TransactionsTask.Repos.ProductRepo;

namespace TransactionsTask.Controllers
{
    public class ProductController : Controller
    {
        private readonly IProductService _db;
        private readonly IMapper _mapper;
        public ProductController(IProductService db, IMapper mapper) { _db = db; _mapper = mapper; }

        [HttpGet]
        public async Task<IActionResult> Show()
        {
            var Products = await _db.GetAllProducts();
            return View(Products);
        }
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(ProductCreateViewModel productCreate)
        {
            if (!ModelState.IsValid)
            {
                return View(productCreate);
            }
            var Product = await _db.AddProduct(productCreate);
            return RedirectToAction(nameof(Show));
        }
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var Pro = await _db.GetProductById(id);
            if (Pro == null)
                return NotFound();
            var productViewModel = _mapper.Map<ProductEditViewModel>(Pro);
            return View(productViewModel);
        }
        [HttpPost]
        public async Task<IActionResult> Edit(int id, ProductEditViewModel productEdit)
        {
            if (!ModelState.IsValid)
            {
                return View(productEdit);
            }

            var Pro = await _db.GetProductById(id);
            if (Pro == null)
                return NotFound();
            await _db.UpdateProduct(productEdit);
            return RedirectToAction(nameof(Show));
        }
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var Pro = await _db.GetProductById(id);
            if (Pro == null) return NotFound();
            return View(Pro);
        }
        [HttpPost, ActionName(nameof(Delete))]
        public async Task<IActionResult> ConfirmedDelete(int id)
        {
            if (id.Equals(0))
            {
                return BadRequest();
            }
            await _db.DeleteProduct(id);
            return RedirectToAction(nameof(Show));
        }
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var product = await _db.GetProductDetailsById(id);
            if (product == null) return NotFound();
            return View(product);
        }
    }
}
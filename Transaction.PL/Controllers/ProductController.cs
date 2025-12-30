using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Transaction.BLL;
using TransactionsTask.Repos.ProductRepo;

namespace TransactionsTask.Controllers
{
    [Authorize(Roles = "Admin")] 
    public class ProductController : Controller
    {
        private readonly IProductService _db;
        private readonly IMapper _mapper;

        public ProductController(IProductService db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }

        [HttpGet]
        [AllowAnonymous] 
        public async Task<IActionResult> Show()
        {
            var products = await _db.GetAllProducts();
            return View(products);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken] 
        public async Task<IActionResult> Create(ProductCreateViewModel productCreate)
        {
            if (!ModelState.IsValid)
            {
                return View(productCreate);
            }

            await _db.AddProduct(productCreate);
            TempData["Success"] = "Product created successfully!"; 
            return RedirectToAction(nameof(Show));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var pro = await _db.GetProductById(id);
            if (pro == null)
                return NotFound();

            var productViewModel = _mapper.Map<ProductEditViewModel>(pro);
            return View(productViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken] 
        public async Task<IActionResult> Edit(int id, ProductEditViewModel productEdit)
        {
            if (id != productEdit.ProductId) 
                return BadRequest();

            if (!ModelState.IsValid)
            {
                return View(productEdit);
            }

            var pro = await _db.GetProductById(id);
            if (pro == null)
                return NotFound();

            await _db.UpdateProduct(productEdit);
            TempData["Success"] = "Product updated successfully!"; 
            return RedirectToAction(nameof(Show));
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var pro = await _db.GetProductById(id);
            if (pro == null)
                return NotFound();

            return View(pro);
        }

        [HttpPost, ActionName(nameof(Delete))]
        [ValidateAntiForgeryToken] 
        public async Task<IActionResult> ConfirmedDelete(int id)
        {
            if (id <= 0) 
                return BadRequest();

            var pro = await _db.GetProductById(id); 
            if (pro == null)
                return NotFound();

            await _db.DeleteProduct(id);
            TempData["Success"] = "Product deleted successfully!"; 
            return RedirectToAction(nameof(Show));
        }

        [HttpGet]
        [AllowAnonymous] 
        public async Task<IActionResult> Details(int id)
        {
            var product = await _db.GetProductDetailsById(id);
            if (product == null)
                return NotFound();

            return View(product);
        }
    }
}
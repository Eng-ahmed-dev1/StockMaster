using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Transaction.BLL;
using TransactionsTask.Repos.SupplierRepo;

namespace TransactionsTask.Controllers
{
    [Authorize(Roles = "Admin")]
    public class SupplierController : Controller
    {
        private readonly ISupplierServices _db;
        private readonly IMapper _mapper;

        public SupplierController(ISupplierServices db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> ShowSuppliers()
        {
            var suppliers = await _db.GetSuppliers();
            return View(suppliers);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SupplierCreateViewModel suppliersCreate)
        {
            if (!ModelState.IsValid)
                return View(suppliersCreate);

            if (await _db.FindDuplicateEmail(suppliersCreate.SupplierEmail))
            {
                ModelState.AddModelError("SupplierEmail", "This email already exists");
                return View(suppliersCreate);
            }

            await _db.AddSupplier(suppliersCreate);
            TempData["Success"] = "Supplier created successfully!";
            return RedirectToAction(nameof(ShowSuppliers));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            var sup = await _db.GetSupplierId(id);
            if (sup == null)
                return NotFound();

            var newSup = _mapper.Map<SupplierEditViewModel>(sup);
            return View(newSup);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, SupplierEditViewModel suppliersEdit)
        {
            if (id != suppliersEdit.SupplierId)
                return BadRequest();

            if (!ModelState.IsValid)
                return View(suppliersEdit);

            var sup = await _db.GetSupplierId(id);
            if (sup == null)
                return NotFound();

            if (sup.SupplierEmail != suppliersEdit.SupplierEmail)
            {
                if (await _db.FindDuplicateEmail(suppliersEdit.SupplierEmail))
                {
                    ModelState.AddModelError("SupplierEmail", "This email already exists");
                    return View(suppliersEdit);
                }
            }

            await _db.UpdateSupplier(suppliersEdit);
            TempData["Success"] = "Supplier updated successfully!";
            return RedirectToAction(nameof(ShowSuppliers));
        }

        [HttpGet]
        public async Task<IActionResult> Delete(string id)
        {
            var sup = await _db.GetSupplierId(id);
            if (sup == null)
                return NotFound();

            return View(sup);
        }

        [HttpPost, ActionName(nameof(Delete))]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var sup = await _db.GetSupplierId(id);
            if (sup == null)
                return NotFound();

            await _db.DeleteSupplier(id);
            TempData["Success"] = "Supplier deleted successfully!";
            return RedirectToAction(nameof(ShowSuppliers));
        }

        [HttpGet]
        public async Task<IActionResult> Details(string id)
        {
            var sup = await _db.GetSupplierIdWithDetails(id);
            if (sup == null)
                return NotFound();

            return View(sup);
        }
    }
}

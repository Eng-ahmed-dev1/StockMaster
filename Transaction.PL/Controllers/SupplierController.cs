using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Transaction.BLL;
using TransactionsTask.Models;
using TransactionsTask.Repos.SupplierRepo;
//Created By Dev Ahmed
namespace TransactionsTask.Controllers
{
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
            var Suppliers = await _db.GetSuppliers();
            return View(Suppliers);
        }
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(SupplierCreateViewModel suppliersCreate)
        {
            if (!ModelState.IsValid)
            {
                return View(suppliersCreate);
            }
            if (await _db.FindDuplicateEmail(suppliersCreate.SupplierEmail))
            {
                ModelState.AddModelError("", "This Email is already exist write another ");
            }
            else
            {
                await _db.AddSupplier(suppliersCreate);
                return RedirectToAction(nameof(ShowSuppliers));
            }
            return View(suppliersCreate);
        }
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var sup = await _db.GetSupplierId(id);
            if (sup == null)
                return NotFound();
            var newSup = _mapper.Map<SupplierEditViewModel>(sup);
            return View(newSup);
        }
        [HttpPost]
        public async Task<IActionResult> Edit(int id, SupplierEditViewModel suppliersEdit)
        {
            if (!ModelState.IsValid)
                return View(suppliersEdit);

            if (id != suppliersEdit.SupplierId)
                return BadRequest();

            var sup = await _db.GetSupplierId(id);

            if (sup == null)
                return NotFound();

            await _db.UpdateSupplier(suppliersEdit);
            return RedirectToAction(nameof(ShowSuppliers));

        }
        [HttpGet]
        public async Task<IActionResult>Delete(int id)
        {
            var sup = await _db.GetSupplierId(id);
            if (sup == null)
                return NotFound();
            return View(sup);
        }
        [HttpPost, ActionName(nameof(Delete))]
        public async Task<IActionResult>DeleteConfirmed(int id)
        {
            var Sup = await _db.GetSupplierId(id);
            if (Sup != null)
            {
                await _db.DeleteSupplier(id);
                return RedirectToAction(nameof(ShowSuppliers));
            }
            return View(Sup);
        }
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {

            var Sup = await _db.GetSupplierIdWithDetails(id);
            if (Sup == null)
                NotFound();
            return View(Sup);
        }
    }
}
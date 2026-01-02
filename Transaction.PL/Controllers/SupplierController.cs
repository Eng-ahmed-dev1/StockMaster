    using Microsoft.AspNetCore.Mvc;
    using Transaction.BLL;
    using AutoMapper;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Authorization;

    namespace TransactionsTask.Controllers
    {
        [Authorize]
        public class SuppliersController : Controller
        {
            private readonly ISupplierServices _supplierServices;
            private readonly IMapper _mapper;
            private readonly IAccountServices account;

            public SuppliersController(ISupplierServices supplierServices, IMapper mapper , IAccountServices services)
            {
                _supplierServices = supplierServices;
                _mapper = mapper;
                account = services;
            }

            public async Task<IActionResult> ShowAllSuppliers()
            {
                var suppliers = await _supplierServices.GetSuppliers();
                return View(suppliers);
            }

            public async Task<IActionResult> Details(string id)
            {
                if (string.IsNullOrEmpty(id))
                    return NotFound();

                var supplier = await _supplierServices.GetSupplierIdWithDetails(id);
                if (supplier == null)
                    return NotFound();

                return View(supplier);
            }

            public IActionResult Create()
            {
                return View();
            }

            [HttpPost]
            [ValidateAntiForgeryToken]
            public async Task<IActionResult> Create(RegisterViewModel model)
            {
                if (!ModelState.IsValid)
                    return View(model);

                if (await _supplierServices.FindDuplicateEmail(model.Email))
                {
                    ModelState.AddModelError("Email", "This email is already registered.");
                    return View(model);
                }
                if (await account.IsUserNameExistsAsync(model.UserName))
                {
                    ModelState.AddModelError("Username", "This UserName is already registered.");
                    return View(model);
                }

                try
                {
                    await account.RegisterAsync(model);
                    TempData["SuccessMessage"] = "Supplier created successfully!";
                    return RedirectToAction(nameof(ShowAllSuppliers));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"Error creating supplier: {ex.Message}");
                    return View(model);
                }
            }

            public async Task<IActionResult> Edit(string id)
            {
                if (string.IsNullOrEmpty(id))
                    return NotFound();

                var supplier = await _supplierServices.GetSupplierId(id);
                if (supplier == null)
                    return NotFound();

                var editModel = _mapper.Map<EditUser>(supplier);

                return View(editModel);
            }

            [HttpPost]
            [ValidateAntiForgeryToken]
            public async Task<IActionResult> Edit(string id, EditUser model)
            {
                if (id != model.SupplierId)
                    return NotFound();

                if (!ModelState.IsValid)
                    return View(model);

                try
                {
                    var result = await _supplierServices.UpdateSupplier(model);
                    if (!result)
                        return NotFound();

                    TempData["SuccessMessage"] = "Supplier updated successfully!";
                    return RedirectToAction(nameof(ShowAllSuppliers));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"Error updating supplier: {ex.Message}");
                    return View(model);
                }
            }
            [HttpGet]
            public async Task<IActionResult> Delete(string id)
            {
                if (string.IsNullOrEmpty(id))
                    return NotFound();

                var supplier = await _supplierServices.GetSupplierId(id);
                if (supplier == null)
                    return NotFound();

                return View(supplier);
            }

            [HttpPost, ActionName("Delete")]
            [ValidateAntiForgeryToken]
            public async Task<IActionResult> DeleteConfirmed(string id)
            {
                try
                {
                    var result = await _supplierServices.DeleteSupplier(id);
                    if (!result)
                        return NotFound();

                    TempData["SuccessMessage"] = "Supplier deleted successfully!";
                    return RedirectToAction(nameof(ShowAllSuppliers));
                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = $"Error deleting supplier: {ex.Message}";
                    return RedirectToAction(nameof(ShowAllSuppliers));
                }
            }
        }
    }
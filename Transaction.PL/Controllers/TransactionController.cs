using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Claims;
using Transaction.BLL;
using TransactionsTask.Models;

namespace TransactionsTask.Controllers
{
    [Authorize]
    public class TransactionController : Controller
    {
        private readonly ITransactionServices _db;
        private readonly ISupplierServices _supServices;
        private readonly IProductService _proService;
        private readonly IMapper _mapper;

        public TransactionController(ITransactionServices services, IProductService productService, ISupplierServices supplierServices, IMapper mapper)
        {
            _db = services;
            _mapper = mapper;
            _supServices = supplierServices;
            _proService = productService;
        }

        [HttpGet]
        [HttpGet]
        public async Task<IActionResult> ShowAllTransactions()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var isAdmin = User.IsInRole("Admin");

            IEnumerable<TransactionReadProSupViewModels> transactions;

            if (isAdmin)
            {
                transactions = await _db.GetAllTransactions();
            }
            else
            {
                transactions = await _db.GetTransactionsByUserId(userId!);
            }

            return View(transactions);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            await PopulateDropdowns();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TransactionCreateViewModel transactionCreate)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            transactionCreate.CreatedByUserId = userId!;
            ModelState.Remove(nameof(transactionCreate.CreatedByUserId));

            if (!ModelState.IsValid)
            {
                await PopulateDropdowns();
                return View(transactionCreate);
            }

            if (transactionCreate.TransactionType is TransactionType.Inbound)
            {
                await _proService.UpdateStockLevel(transactionCreate.ProductId, transactionCreate.Quantity);
            }
            else if (transactionCreate.TransactionType is TransactionType.Outbound)
            {
                var success = await _proService.UpdateStockLevel(transactionCreate.ProductId, -transactionCreate.Quantity);
                if (!success)
                {
                    ModelState.AddModelError("", "Not enough stock available");
                    await PopulateDropdowns();
                    return View(transactionCreate);
                }
            }

            await _db.AddTransaction(transactionCreate);
            return RedirectToAction(nameof(ShowAllTransactions));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var transaction = await GetAuthorizedTransaction(id);
            if (transaction == null)
                return NotFound();

            await PopulateDropdowns();
            var newEdit = _mapper.Map<TransactionEditViewModel>(transaction);
            return View(newEdit);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, TransactionEditViewModel transactionUpdate)
        {
            var oldTransaction = await GetAuthorizedTransaction(id);
            if (oldTransaction == null)
                return NotFound();

            ModelState.Remove(nameof(transactionUpdate.CreatedByUserId));

            if (!ModelState.IsValid)
            {
                await PopulateDropdowns();
                return View(transactionUpdate);
            }

            transactionUpdate.CreatedByUserId = oldTransaction.CreatedByUserId;

            if (oldTransaction.TransactionType == TransactionType.Inbound)
            {
                await _proService.UpdateStockLevel(oldTransaction.ProductId, -oldTransaction.Quantity);
            }
            else if (oldTransaction.TransactionType == TransactionType.Outbound)
            {
                await _proService.UpdateStockLevel(oldTransaction.ProductId, oldTransaction.Quantity);
            }

            if (transactionUpdate.TransactionType is TransactionType.Inbound)
            {
                await _proService.UpdateStockLevel(transactionUpdate.ProductId, transactionUpdate.Quantity);
            }
            else if (transactionUpdate.TransactionType is TransactionType.Outbound)
            {
                var success = await _proService.UpdateStockLevel(transactionUpdate.ProductId, -transactionUpdate.Quantity);
                if (!success)
                {
                    if (oldTransaction.TransactionType == TransactionType.Inbound)
                    {
                        await _proService.UpdateStockLevel(oldTransaction.ProductId, oldTransaction.Quantity);
                    }
                    else if (oldTransaction.TransactionType == TransactionType.Outbound)
                    {
                        await _proService.UpdateStockLevel(oldTransaction.ProductId, -oldTransaction.Quantity);
                    }
                    ModelState.AddModelError("", "Not enough stock available");
                    await PopulateDropdowns();
                    return View(transactionUpdate);
                }
            }

            await _db.UpdateTransaction(transactionUpdate);
            return RedirectToAction(nameof(ShowAllTransactions));
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var transaction = await GetAuthorizedTransaction(id);
            if (transaction == null)
                return NotFound();

            return View(transaction);
        }

        [HttpPost, ActionName(nameof(Delete))]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var transaction = await GetAuthorizedTransaction(id);
            if (transaction == null)
                return NotFound();

            if (transaction.TransactionType == TransactionType.Inbound)
            {
                await _proService.UpdateStockLevel(transaction.ProductId, -transaction.Quantity);
            }
            else if (transaction.TransactionType == TransactionType.Outbound)
            {
                await _proService.UpdateStockLevel(transaction.ProductId, transaction.Quantity);
            }

            await _db.DeleteTransaction(transaction.TransactionId);
            return RedirectToAction(nameof(ShowAllTransactions));
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var transaction = await GetAuthorizedTransaction(id);
            if (transaction is null)
                return NotFound();

            return View(transaction);
        }

        #region Helper Methods
        private async Task PopulateDropdowns()
        {
            var suppliers = await _supServices.GetSuppliers();
            ViewBag.Suppliers = new SelectList(suppliers, "Id", "UserName");

            var products = await _proService.GetAllProducts();
            ViewBag.Products = new SelectList(products, "ProductId", "ProductName");
        }

        private async Task<TransactionReadProSupViewModels?> GetAuthorizedTransaction(int id)
        {
            var isAdmin = User.IsInRole("Admin");

            if (isAdmin)
            {
                return await _db.GetTransactionId(id);
            }
            else
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var userTransactions = await _db.GetTransactionsByUserId(userId!);
                return userTransactions.FirstOrDefault(t => t.TransactionId == id);
            }
        }
        #endregion
    }
}
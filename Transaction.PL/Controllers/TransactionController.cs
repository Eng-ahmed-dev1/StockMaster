using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Claims;
using System.Threading.Tasks;
using Transaction.BLL;
using TransactionsTask.Models;
using TransactionsTask.Repos.ProductRepo;
using TransactionsTask.Repos.SupplierRepo;

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
        public async Task<IActionResult> ShowAllTransactions()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var transactions = await _db.GetTransactionsByUserId(userId!); 
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
            if (!ModelState.IsValid)
            {
                await PopulateDropdowns();
                return View(transactionCreate);
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            transactionCreate.CreatedByUserId = userId!;

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
            var transaction = await _db.GetTransactionId(id);
            if (transaction == null)
                return NotFound();

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (transaction.CreatedByUserId != userId)
                return Forbid(); 

            await PopulateDropdowns();
            var newEdit = _mapper.Map<TransactionEditViewModel>(transaction);
            return View(newEdit);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, TransactionEditViewModel transactionUpdate)
        {
            var oldTransaction = await _db.GetTransactionId(id);
            if (oldTransaction == null)
                return NotFound();

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (oldTransaction.CreatedByUserId != userId)
                return Forbid();

            if (!ModelState.IsValid)
            {
                await PopulateDropdowns();
                return View(transactionUpdate);
            }

            // 👇 حافظ على الـ CreatedByUserId الأصلي
            transactionUpdate.CreatedByUserId = oldTransaction.CreatedByUserId;

            // عكس الـ transaction القديم
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
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var transactions = await _db.GetTransactionsByUserId(userId!); 
            var transaction = transactions.FirstOrDefault(x => x.TransactionId == id);

            if (transaction == null)
                return NotFound();

            return View(transaction);
        }

        [HttpPost, ActionName(nameof(Delete))]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var transactions = await _db.GetTransactionsByUserId(userId!); 
            var transaction = transactions.FirstOrDefault(x => x.TransactionId == id);

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
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var transactions = await _db.GetTransactionsByUserId(userId!); 
            var transaction = transactions.FirstOrDefault(x => x.TransactionId == id);

            if (transaction is null)
                return NotFound();

            return View(transaction);
        }

        #region HelperMethod
        private async Task PopulateDropdowns()
        {
            ViewBag.Suppliers = new SelectList((await _supServices.GetSuppliers())
              .Select(sup => new
              {
                  Id = sup.SupplierId,
                  Text = sup.SupplierName
              }), "Id", "Text");

            ViewBag.Products = new SelectList((await _proService.GetAllProducts())
            .Select(pro => new
            {
                Id = pro.ProductId,
                Name = pro.ProductName,
            }), "Id", "Name");
        }
        #endregion
    }
}
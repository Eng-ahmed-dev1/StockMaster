using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Threading.Tasks;
using Transaction.BLL;
using TransactionsTask.Models;
using TransactionsTask.Repos.ProductRepo;
using TransactionsTask.Repos.SupplierRepo;
using TransactionsTask.Repos.TransactionRepo;

namespace TransactionsTask.Controllers
{
    public class TransactionController : Controller
    {
        private readonly ITransactionServices _db;
        private readonly ISupplierServices _supServices;
        private readonly IProductService _proService;
        private readonly IMapper _mapper;
        public TransactionController(ITransactionServices services , IProductService productService , ISupplierServices supplierServices, IMapper mapper) 
        {
            _db = services; 
            _mapper = mapper;
            _supServices = supplierServices;
            _proService = productService;
        }

        [HttpGet]
        public async Task<IActionResult> ShowAllTransactions()
        {
            var transations = await _db.GetTransactionWithDetails();
            return View(transations);
        }
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            await PopulateDropdowns();
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(TransactionCreateViewModel transactionCreate)
        {
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
        public async Task<IActionResult>Edit(int id)
        {
            var Transaction = await _db.GetTransactionId(id);
            if (Transaction == null)
                return NotFound();
            await PopulateDropdowns();

            var newEdit = _mapper.Map<TransactionEditViewModel>(Transaction);
            return View(newEdit);
        }
        [HttpPost]
        public async Task<IActionResult> Edit(int id, TransactionEditViewModel transactionUpdate)
        {
            var oldTransaction = await _db.GetTransactionId(id);
            if (oldTransaction == null)
                return NotFound();

            if (!ModelState.IsValid)
            {
                await PopulateDropdowns();
                return View(transactionUpdate);
            }

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
        public async Task<IActionResult>Delete(int id)
        {
            var transactions = await _db.GetTransactionWithDetails();
            var transation = transactions.FirstOrDefault(x => x.TransactionId == id);
            if (transation == null)
                return NotFound();
            return View(transation);
        }

        [HttpPost, ActionName(nameof(Delete))]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var transactions = await _db.GetTransactionWithDetails();
            var transation = transactions.FirstOrDefault(x => x.TransactionId == id);
            if (transation != null)
            {
                if (transation.TransactionType == TransactionType.Inbound)
                {
                    await _proService.UpdateStockLevel(transation.ProductId, -transation.Quantity);
                }
                else if (transation.TransactionType == TransactionType.Outbound)
                {
                    await _proService.UpdateStockLevel(transation.ProductId, transation.Quantity);
                }
                await _db.DeleteTransaction(transation.TransactionId);
                return RedirectToAction(nameof(ShowAllTransactions));
            }
            return NotFound();
        }
        [HttpGet]
        public async Task<IActionResult>Details(int id)
        {
            var transactions = await _db.GetTransactionWithDetails();
            var transaction = transactions.FirstOrDefault(x => x.TransactionId == id);
            if(transaction is null)
                return NotFound();
            return View(transaction);
        }
        #region HlperMetho
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

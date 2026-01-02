using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Claims;
using Transaction.BLL;
using TransactionsTask.Models;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Drawing;


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
            var isAdmin = User.IsInRole("Admin");

            transactionCreate.CreatedByUserId = userId!;
            ModelState.Remove(nameof(transactionCreate.CreatedByUserId));

            if (!isAdmin)
            {
                if (transactionCreate.SupplierId != userId)
                {
                    ModelState.AddModelError("", "You can only create transactions for yourself");
                    await PopulateDropdowns();
                    return View(transactionCreate);
                }

                if (transactionCreate.TransactionType == TransactionType.Outbound)
                {
                    ModelState.AddModelError("", "Suppliers can only create Inbound transactions");
                    await PopulateDropdowns();
                    return View(transactionCreate);
                }
            }

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

            var isAdmin = User.IsInRole("Admin");
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            ModelState.Remove(nameof(transactionUpdate.CreatedByUserId));

            if (!isAdmin)
            {
                if (transactionUpdate.SupplierId != userId)
                {
                    ModelState.AddModelError("", "You can only create transactions for yourself");
                    await PopulateDropdowns();
                    return View(transactionUpdate);
                }

                if (transactionUpdate.TransactionType == TransactionType.Outbound)
                {
                    ModelState.AddModelError("", "Suppliers can only create Inbound transactions");
                    await PopulateDropdowns();
                    return View(transactionUpdate);
                }
            }

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

        #region Export Methods

        [HttpGet]
        public async Task<IActionResult> ExportAll()
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

            var pdf = GeneratePdf(transactions, "All Transactions Report");
            var fileName = $"Transactions_All_{DateTime.Now:yyyyMMdd_HHmmss}.pdf";

            return File(pdf, "application/pdf", fileName);
        }

        [HttpPost]
        public async Task<IActionResult> ExportSelected([FromBody] List<int> selectedIds)
        {
            if (selectedIds == null || !selectedIds.Any())
            {
                return BadRequest(new { message = "No transactions selected" });
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var isAdmin = User.IsInRole("Admin");

            IEnumerable<TransactionReadProSupViewModels> allTransactions;

            if (isAdmin)
            {
                allTransactions = await _db.GetAllTransactions();
            }
            else
            {
                allTransactions = await _db.GetTransactionsByUserId(userId!);
            }

            var selectedTransactions = allTransactions.Where(t => selectedIds.Contains(t.TransactionId));

            if (!selectedTransactions.Any())
            {
                return BadRequest(new { message = "No valid transactions found" });
            }

            var pdf = GeneratePdf(selectedTransactions, "Selected Transactions Report");
            var fileName = $"Transactions_Selected_{DateTime.Now:yyyyMMdd_HHmmss}.pdf";

            return File(pdf, "application/pdf", fileName);
        }

        [HttpGet]
        public async Task<IActionResult> ExportSingle(int id)
        {
            var transaction = await GetAuthorizedTransaction(id);
            if (transaction == null)
                return NotFound();

            var transactions = new List<TransactionReadProSupViewModels> { transaction };

            var pdf = GeneratePdf(transactions, $"Transaction Report #{id}");
            var fileName = $"Transaction_{id}_{DateTime.Now:yyyyMMdd_HHmmss}.pdf";

            return File(pdf, "application/pdf", fileName);
        }

        private byte[] GeneratePdf(IEnumerable<TransactionReadProSupViewModels> transactions, string reportTitle)
        {
            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4.Landscape());
                    page.Margin(40);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(10).FontColor(Colors.Grey.Darken3));

                    // Header
                    page.Header().Element(ComposeHeader);

                    // Content
                    page.Content().Element(content => ComposeContent(content, transactions, reportTitle));

                    // Footer
                    page.Footer().Element(ComposeFooter);
                });
            });

            return document.GeneratePdf();
        }

        private void ComposeHeader(IContainer container)
        {
            container.Column(column =>
            {
                // Title Section with Gradient Effect
                column.Item().Background(Colors.Blue.Darken3).Padding(20).Row(row =>
                {
                    row.RelativeItem().Column(col =>
                    {
                        col.Item().Text("TRANSACTION MANAGEMENT SYSTEM")
                            .FontSize(22)
                            .Bold()
                            .FontColor(Colors.White);

                        col.Item().PaddingTop(5).Text("Professional Transaction Report")
                            .FontSize(12)
                            .FontColor(Colors.Blue.Lighten3);
                    });

                    row.ConstantItem(80).AlignRight().AlignMiddle().Column(col =>
                    {
                        col.Item().AlignRight().Text(DateTime.Now.ToString("MMM dd"))
                            .FontSize(16)
                            .Bold()
                            .FontColor(Colors.White);

                        col.Item().AlignRight().Text(DateTime.Now.ToString("yyyy"))
                            .FontSize(11)
                            .FontColor(Colors.Blue.Lighten3);
                    });
                });

                // Separator Line
                column.Item().PaddingVertical(2).LineHorizontal(1).LineColor(Colors.Blue.Darken2);
            });
        }

        private void ComposeContent(IContainer container, IEnumerable<TransactionReadProSupViewModels> transactions, string reportTitle)
        {
            container.PaddingTop(15).Column(column =>
            {
                // Report Title with Modern Design
                column.Item().PaddingBottom(15).Row(row =>
                {
                    row.RelativeItem().Column(col =>
                    {
                        col.Item().Text(reportTitle)
                            .FontSize(18)
                            .Bold()
                            .FontColor(Colors.Blue.Darken3);

                        col.Item().PaddingTop(3).Text($"Generated on {DateTime.Now:dddd, MMMM dd, yyyy HH:mm}")
                            .FontSize(9)
                            .FontColor(Colors.Grey.Medium);
                    });
                });

                // Statistics Section with Cards
                var inboundCount = transactions.Count(t => t.TransactionType == TransactionType.Inbound);
                var outboundCount = transactions.Count(t => t.TransactionType == TransactionType.Outbound);
                var totalCount = transactions.Count();

                column.Item().PaddingBottom(20).Row(row =>
                {
                    // Inbound Card
                    row.RelativeItem().Border(1).BorderColor(Colors.Green.Medium)
                        .Background(Colors.Green.Lighten4).Padding(12).Column(col =>
                        {
                            col.Item().Row(r =>
                            {
                                r.RelativeItem().Text("↓ INBOUND")
                                    .FontSize(10)
                                    .Bold()
                                    .FontColor(Colors.Green.Darken2);
                            });
                            col.Item().PaddingTop(5).Text(inboundCount.ToString())
                                .FontSize(24)
                                .Bold()
                                .FontColor(Colors.Green.Darken3);
                            col.Item().Text("Transactions")
                                .FontSize(8)
                                .FontColor(Colors.Green.Darken1);
                        });

                    row.ConstantItem(15);

                    // Outbound Card
                    row.RelativeItem().Border(1).BorderColor(Colors.Red.Medium)
                        .Background(Colors.Red.Lighten4).Padding(12).Column(col =>
                        {
                            col.Item().Row(r =>
                            {
                                r.RelativeItem().Text("↑ OUTBOUND")
                                    .FontSize(10)
                                    .Bold()
                                    .FontColor(Colors.Red.Darken2);
                            });
                            col.Item().PaddingTop(5).Text(outboundCount.ToString())
                                .FontSize(24)
                                .Bold()
                                .FontColor(Colors.Red.Darken3);
                            col.Item().Text("Transactions")
                                .FontSize(8)
                                .FontColor(Colors.Red.Darken1);
                        });

                    row.ConstantItem(15);

                    // Total Card
                    row.RelativeItem().Border(1).BorderColor(Colors.Blue.Medium)
                        .Background(Colors.Blue.Lighten4).Padding(12).Column(col =>
                        {
                            col.Item().Row(r =>
                            {
                                r.RelativeItem().Text("∑ TOTAL")
                                    .FontSize(10)
                                    .Bold()
                                    .FontColor(Colors.Blue.Darken2);
                            });
                            col.Item().PaddingTop(5).Text(totalCount.ToString())
                                .FontSize(24)
                                .Bold()
                                .FontColor(Colors.Blue.Darken3);
                            col.Item().Text("Transactions")
                                .FontSize(8)
                                .FontColor(Colors.Blue.Darken1);
                        });
                });

                // Transactions Table
                column.Item().Table(table =>
                {
                    // Define columns
                    table.ColumnsDefinition(columns =>
                    {
                        columns.ConstantColumn(45);      // ID
                        columns.RelativeColumn(2.5f);    // Product
                        columns.RelativeColumn(2);       // Supplier
                        columns.ConstantColumn(75);      // Quantity
                        columns.RelativeColumn(1.5f);    // Date
                        columns.ConstantColumn(85);      // Type
                    });

                    // Header
                    table.Header(header =>
                    {
                        header.Cell().Background(Colors.Blue.Darken3).Padding(10)
                            .Text("ID").Bold().FontSize(10).FontColor(Colors.White);

                        header.Cell().Background(Colors.Blue.Darken3).Padding(10)
                            .Text("PRODUCT").Bold().FontSize(10).FontColor(Colors.White);

                        header.Cell().Background(Colors.Blue.Darken3).Padding(10)
                            .Text("SUPPLIER").Bold().FontSize(10).FontColor(Colors.White);

                        header.Cell().Background(Colors.Blue.Darken3).Padding(10)
                            .Text("QUANTITY").Bold().FontSize(10).FontColor(Colors.White);

                        header.Cell().Background(Colors.Blue.Darken3).Padding(10)
                            .Text("DATE").Bold().FontSize(10).FontColor(Colors.White);

                        header.Cell().Background(Colors.Blue.Darken3).Padding(10)
                            .Text("TYPE").Bold().FontSize(10).FontColor(Colors.White);
                    });

                    // Rows
                    int index = 0;
                    foreach (var transaction in transactions)
                    {
                        var isEven = index % 2 == 0;
                        var bgColor = isEven ? Colors.Grey.Lighten4 : Colors.White;

                        // ID
                        table.Cell().Background(bgColor).Padding(10)
                            .AlignCenter().AlignMiddle()
                            .Text($"#{transaction.TransactionId}")
                            .FontSize(9)
                            .Bold()
                            .FontColor(Colors.Blue.Darken2);

                        // Product
                        table.Cell().Background(bgColor).Padding(10)
                            .AlignLeft().AlignMiddle()
                            .Text(transaction.ProductName)
                            .FontSize(10)
                            .FontColor(Colors.Grey.Darken3);

                        // Supplier
                        table.Cell().Background(bgColor).Padding(10)
                            .AlignLeft().AlignMiddle()
                            .Text(!string.IsNullOrEmpty(transaction.SupplierName)
                                ? transaction.SupplierName
                                : "User")
                            .FontSize(9)
                            .FontColor(Colors.Grey.Darken2);

                        // Quantity with Color
                        var quantityColor = transaction.TransactionType == TransactionType.Inbound
                            ? Colors.Green.Darken2
                            : Colors.Red.Darken2;

                        var quantityPrefix = transaction.TransactionType == TransactionType.Inbound ? "+" : "-";

                        table.Cell().Background(bgColor).Padding(10)
                            .AlignCenter().AlignMiddle()
                            .Text($"{quantityPrefix}{transaction.Quantity}")
                            .FontSize(11)
                            .Bold()
                            .FontColor(quantityColor);

                        // Date
                        table.Cell().Background(bgColor).Padding(10)
                            .AlignCenter().AlignMiddle()
                            .Text(transaction.TransactionDate.ToString("MMM dd, yyyy"))
                            .FontSize(9)
                            .FontColor(Colors.Grey.Darken2);

                        // Type Badge
                        var typeColor = transaction.TransactionType == TransactionType.Inbound
                            ? Colors.Green.Medium
                            : Colors.Red.Medium;

                        var typeBgColor = transaction.TransactionType == TransactionType.Inbound
                            ? Colors.Green.Lighten3
                            : Colors.Red.Lighten3;

                        table.Cell().Background(bgColor).Padding(10)
                            .AlignCenter().AlignMiddle()
                            .Background(typeBgColor)
                            .Border(1)
                            .BorderColor(typeColor)
                            .Padding(6)
                            .Text(transaction.TransactionType.ToString().ToUpper())
                            .FontSize(9)
                            .Bold()
                            .FontColor(typeColor);

                        index++;
                    }
                });
            });
        }

        private void ComposeFooter(IContainer container)
        {
            container.Column(column =>
            {
                column.Item().PaddingTop(10).LineHorizontal(1).LineColor(Colors.Grey.Lighten2);

                column.Item().PaddingTop(10).Row(row =>
                {
                    row.RelativeItem().AlignLeft().Text(text =>
                    {
                        text.Span("© 2025 Transaction Management System").FontSize(8).FontColor(Colors.Grey.Medium);
                    });

                    row.RelativeItem().AlignCenter().Text(text =>
                    {
                        text.Span("Page ").FontSize(8).FontColor(Colors.Grey.Medium);
                        text.CurrentPageNumber().FontSize(8).Bold().FontColor(Colors.Blue.Darken2);
                        text.Span(" of ").FontSize(8).FontColor(Colors.Grey.Medium);
                        text.TotalPages().FontSize(8).Bold().FontColor(Colors.Blue.Darken2);
                    });

                    row.RelativeItem().AlignRight().Text(text =>
                    {
                        text.Span("Confidential Report").FontSize(8).FontColor(Colors.Grey.Medium);
                    });
                });
            });
        }

        #endregion

        #region Helper Methods
        private async Task PopulateDropdowns()
        {
            var isAdmin = User.IsInRole("Admin");

            if (isAdmin)
            {
                var allSuppliers = await _supServices.GetSuppliers();
                ViewBag.Suppliers = new SelectList(allSuppliers, "Id", "UserName");
            }
            else
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var currentSupplier = await _supServices.GetSupplierId(userId!);

                if (currentSupplier != null)
                {
                    var supplierList = new[] { currentSupplier };
                    ViewBag.Suppliers = new SelectList(supplierList, "Id", "UserName");
                }
                else
                {
                    ViewBag.Suppliers = new SelectList(Array.Empty<UserViewModel>(), "Id", "UserName");
                }
            }

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
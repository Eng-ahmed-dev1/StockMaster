using System;
using TransactionsTask.Models;

namespace Transaction.BLL
{
    public class TransactionReadProSupViewModels
    {
        public int TransactionId { get; set; }
        public string CreatedByUserId { get; set; }  
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string? SupplierId { get; set; }
        public string SupplierName { get; set; }
        public int Quantity { get; set; }
        public DateTime TransactionDate { get; set; }
        public TransactionType TransactionType { get; set; }
    }
}
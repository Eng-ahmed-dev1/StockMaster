using System;
using TransactionsTask.Models;

namespace Transaction.BLL
{
    public class TransactionCreateViewModel
    {
        public string CreatedByUserId { get; set; }  
        public int ProductId { get; set; }
        public int? SupplierId { get; set; }
        public int Quantity { get; set; }
        public DateTime TransactionDate { get; set; } = DateTime.UtcNow;  
        public TransactionType TransactionType { get; set; }
    }
}
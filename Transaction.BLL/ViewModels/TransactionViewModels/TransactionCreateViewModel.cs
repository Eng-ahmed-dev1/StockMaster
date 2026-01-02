using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using TransactionsTask.Models;

namespace Transaction.BLL
{
    public class TransactionCreateViewModel
    {
        [BindNever]
        public string CreatedByUserId { get; set; }  
        public int ProductId { get; set; }
        public string? SupplierId { get; set; }
        public int Quantity { get; set; }
        public DateTime TransactionDate { get; set; } = DateTime.UtcNow;  
        public TransactionType TransactionType { get; set; }
    }
}
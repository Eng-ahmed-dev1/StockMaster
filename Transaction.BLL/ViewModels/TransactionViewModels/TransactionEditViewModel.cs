using System;
using TransactionsTask.Models;

namespace Transaction.BLL
{
    public class TransactionEditViewModel
    {
        public int TransactionId { get; set; }
        public string CreatedByUserId { get; set; }  
        public int ProductId { get; set; }
        public int? SupplierId { get; set; }
        public int Quantity { get; set; }
        public DateTime TransactionDate { get; set; }
        public TransactionType TransactionType { get; set; }
    }
}
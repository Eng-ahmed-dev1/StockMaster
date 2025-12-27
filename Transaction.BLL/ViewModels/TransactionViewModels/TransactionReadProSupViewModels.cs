using System;
using System.Collections.Generic;
using System.Text;
using TransactionsTask.Models;

namespace Transaction.BLL
{
    public class TransactionReadProSupViewModels
    {
        public int TransactionId { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public int? SupplierId { get; set; }
        public string SupplierName { get; set; }
        public int Quantity { get; set; }
        public DateTime TransactionDate { get; set; }
        public TransactionType TransactionType { get; set; }

    }
}

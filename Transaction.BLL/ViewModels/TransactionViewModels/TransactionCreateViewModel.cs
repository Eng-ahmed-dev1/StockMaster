using TransactionsTask.Models;
namespace Transaction.BLL
{
    public class TransactionCreateViewModel
    {

        public int ProductId { get; set; }
        public int? SupplierId { get; set; }
        public int Quantity { get; set; }
        public DateTime TransactionDate { get; set; }
        public TransactionType TransactionType { get; set; }
    }
}

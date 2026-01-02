using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Transaction.BLL
{
    public class ProductReadViewModel
    {
        public int TransactionId { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string SKU { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public int StockLevel { get; set; }
        public IEnumerable<TransactionReadProSupViewModels> Transactions { get; set; } = new HashSet<TransactionReadProSupViewModels>();

    }
}

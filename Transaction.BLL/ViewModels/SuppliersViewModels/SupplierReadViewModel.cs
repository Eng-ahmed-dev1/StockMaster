using System.ComponentModel.DataAnnotations;

namespace Transaction.BLL
{
    public class SupplierReadViewModel
    {
        public int SupplierId { get; set; }
        public string SupplierName { get; set; }
        public string SupplierEmail { get; set; }
        public string PhoneNumber { get; set; }
        public IEnumerable<TransactionReadProSupViewModels> Transactions { get; set; }
    }
}

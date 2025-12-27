using System.ComponentModel.DataAnnotations;
namespace TransactionsTask.Models
{
    public class Suppliers
    {
        [Key]
        public int SupplierId { get; set; }
        [MaxLength(100, ErrorMessage = "The Supplier must have a name")]
        public string SupplierName { get; set; }
        [EmailAddress, MaxLength(100)]
        public string SupplierEmail { get; set; }
        [DataType(DataType.PhoneNumber),MaxLength(20)]
        public string PhoneNumber { get; set; }
        public ICollection<Transactions> Transactions { get; set; } = new HashSet<Transactions>();
    }
}

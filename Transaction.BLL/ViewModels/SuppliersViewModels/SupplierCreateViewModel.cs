using System.ComponentModel.DataAnnotations;

namespace Transaction.BLL
{
    public class SupplierCreateViewModel
    {

        [MaxLength(100, ErrorMessage = "The Supplier must have a name")]
        public string SupplierName { get; set; }
        [EmailAddress, MaxLength(100)]
        public string SupplierEmail { get; set; }
        [DataType(DataType.PhoneNumber), MaxLength(20)]
        public string PhoneNumber { get; set; }

    }
}

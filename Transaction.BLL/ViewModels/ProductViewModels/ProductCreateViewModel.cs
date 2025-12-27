using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Transaction.BLL
{
    public class ProductCreateViewModel
    {
        [MaxLength(100, ErrorMessage = "The Product must have a Name")]
        public string ProductName { get; set; }
        [MaxLength(50, ErrorMessage = "The Prduct Must have a SKU")]
        public string SKU { get; set; }
        [MaxLength(500, ErrorMessage = "The Prduct Must have a SKU")]
        public string Description { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        public decimal Price { get; set; }
        public int StockLevel { get; set; }
    }
}

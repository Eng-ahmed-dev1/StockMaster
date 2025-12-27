using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Transaction.BLL
{
    public class ProductEditViewModel
    {
        public int ProductId { get; set; }
        [Required]
        [MaxLength(500)]
        public string Description { get; set; }
        [Column(TypeName ="decimal(18,2)")]
        public decimal Price { get; set; }

    }
}

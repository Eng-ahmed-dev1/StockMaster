using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TransactionsTask.Models
{
    public class Products
    {
        [Key]
        public int ProductId { get; set; }
        [Required]
        [MaxLength(100)]
        public string ProductName { get; set; }
        [Required]
        [StringLength(50,ErrorMessage ="The SKU must be 50 digit ")]
        public string SKU { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        public decimal Price { get; set; }
        public int StockLevel { get; set; }
        [MaxLength(500)]
        public string Description { get; set; }
        public ICollection<Transactions> Transactions { get; set; } = new HashSet<Transactions>();


    }
}

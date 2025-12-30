using System.ComponentModel.DataAnnotations;
using Transaction.DAL; 

namespace TransactionsTask.Models
{
    public class Transactions
    {
        [Key]
        public int TransactionId { get; set; }

        [Required] 
        public string CreatedByUserId { get; set; }
        public SystemUsers CreatedBy { get; set; } 

        public int? ProductId { get; set; }
        public Products Product { get; set; }

        public int? SupplierId { get; set; }
        public Suppliers Supplier { get; set; }

        [Required]
        public int Quantity { get; set; }

        public DateTime TransactionDate { get; set; } = DateTime.UtcNow;

        [Required]
        public TransactionType TransactionType { get; set; }
    }

    public enum TransactionType
    {
        Inbound,  
        Outbound  
    }
}
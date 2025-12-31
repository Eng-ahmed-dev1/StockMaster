using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Transaction.DAL;

namespace TransactionsTask.Models
{
    public class Transactions
    {
        [Key]
        public int TransactionId { get; set; }

        [Required]
        public string CreatedByUserId { get; set; }

        [ForeignKey(nameof(CreatedByUserId))]
        [InverseProperty(nameof(SystemUsers.CreatedTransactions))]
        public SystemUsers CreatedBy { get; set; }

        public int? ProductId { get; set; }
        public Products Product { get; set; }

        public string? SupplierId { get; set; }

        [ForeignKey(nameof(SupplierId))]
        [InverseProperty(nameof(SystemUsers.SuppliedTransactions))]
        public SystemUsers Supplier { get; set; }

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

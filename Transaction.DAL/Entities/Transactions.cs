using System.ComponentModel.DataAnnotations;
namespace TransactionsTask.Models
{
    public class Transactions
    {
        [Key]
        public int TransactionId {  get; set; }
        public int ?ProductId { get; set; }
        public Products Product { get; set; }    
        public int ?SupplierId { get; set; }
        public Suppliers Supplier { get; set; }
        public int Quantity {  get; set; }
        public DateTime TransactionDate {  get; set; }
        public TransactionType TransactionType { get; set; }
    }
    public enum TransactionType
    {
        Inbound,
        Outbound
    }
}

using TransactionsTask.Models;

namespace TransactionsTask.Repos.TransactionRepo
{
    public interface ITransactionRepo
    {
        Task<Transactions?> GetTransactionById(int id);
        Task<IEnumerable<Transactions>> GetTransactions();
        Task<IEnumerable<Transactions>> GetTransactionWithDetails();
        Task<IEnumerable<Transactions>> GetTransactionsBySupplierId(string userId); 

        Task DeleteTransaction(Transactions transaction);
        Task UpdateTransaction(Transactions transaction);
        Task AddTransaction(Transactions transaction);
        //For Admin
        Task<IEnumerable<Transactions>> GetAllTransactions();


    }
}

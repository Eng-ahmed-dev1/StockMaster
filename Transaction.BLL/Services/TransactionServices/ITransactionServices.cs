using System;
using System.Collections.Generic;
using System.Text;

namespace Transaction.BLL
{
    public interface ITransactionServices
    {
        Task<TransactionReadProSupViewModels?> GetTransactionId(int id);
        Task<IEnumerable<TransactionReadProSupViewModels>> GetTransactions();
        Task<IEnumerable<TransactionReadProSupViewModels>> GetTransactionWithDetails();
        Task<bool> DeleteTransaction(int id);
        Task<bool> UpdateTransaction(TransactionEditViewModel transactionEdit);
        Task<int> AddTransaction(TransactionCreateViewModel transactionCreate);
    }
}

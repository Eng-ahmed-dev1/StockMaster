using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using TransactionsTask.Models;
using TransactionsTask.Repos.TransactionRepo;

namespace Transaction.BLL
{
    public class TransactionServices : ITransactionServices
    {
        private readonly ITransactionRepo _db;
        private readonly IMapper _mapper;
        public TransactionServices(ITransactionRepo repo, IMapper mapper)
        {
            _db = repo;
            _mapper = mapper;
        }
        public async Task<int> AddTransaction(TransactionCreateViewModel transactionCreate)
        {
            var transaction = _mapper.Map<TransactionsTask.Models.Transactions>(transactionCreate);
            if (transaction == null)
                throw new ArgumentNullException(nameof(transaction));
            await _db.AddTransaction(transaction);
            return transaction.TransactionId;
        }

        public async Task<bool> DeleteTransaction(int id)
        {
            var transaction = await _db.GetTransactionById(id);
            if (transaction == null)
                return false;
            await _db.DeleteTransaction(transaction);
            return true;
        }
        public async Task<TransactionReadProSupViewModels?> GetTransactionId(int id)
        {
            var transaction = await _db.GetTransactionById(id);
            if (transaction == null)
                throw new ArgumentNullException(nameof(transaction));
            return _mapper.Map<TransactionReadProSupViewModels>(transaction);
        }

        public async Task<IEnumerable<TransactionReadProSupViewModels>> GetTransactions()
        {
            var transations = await _db.GetTransactions();
            return _mapper.Map<IEnumerable<TransactionReadProSupViewModels>>(transations);
        }

        public async Task<IEnumerable<TransactionReadProSupViewModels>> GetTransactionWithDetails()
        {
            var transations = await _db.GetTransactionWithDetails();
            return _mapper.Map<IEnumerable<TransactionReadProSupViewModels>>(transations);
        }

        public async Task<bool> UpdateTransaction(TransactionEditViewModel transactionEdit)
        {
            var transaction = await _db.GetTransactionById(transactionEdit.TransactionId);
            if (transaction == null)
                return false;
            _mapper.Map(transactionEdit, transaction);
            await _db.UpdateTransaction(transaction);
            return true;
        }
    }
}

using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using System.Transactions;
using TransactionsTask.Models;
namespace Transaction.BLL
{
    public class TransactionProfile : Profile
    {
        public TransactionProfile()
        {
            // Map ViewModel For Create
            CreateMap<TransactionCreateViewModel, TransactionsTask.Models.Transactions>();
            // Map ViewModel For Read 
            // we make the for memeber because we can't use entity in the viewmodel directly 
            //so we use it to make as refrence from the entity and don't use it direclty 
            CreateMap<TransactionsTask.Models.Transactions, TransactionReadProSupViewModels>()
                .ForMember(dest => dest.ProductName, p => p.MapFrom(src => src.Product != null ? src.Product.ProductName : "N/A"))
                .ForMember(dest => dest.SupplierName, p => p.MapFrom(src => src.Supplier != null ? src.Supplier.SupplierName : "N/A"));
            // Map ViewModel For Edit 
            CreateMap<TransactionsTask.Models.Transactions, TransactionEditViewModel>().ReverseMap();
            CreateMap<TransactionReadProSupViewModels,TransactionEditViewModel>();
           
        }
    }
}
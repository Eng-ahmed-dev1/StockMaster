using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using TransactionsTask.Models;


namespace Transaction.BLL
{
    public class SupplierProfile : Profile
    {
        public SupplierProfile()
        {
            // Map ViewModel For Create
            CreateMap<SupplierCreateViewModel, SystemUsers>()
           .ForMember(dest => dest.CreatedTransactions, opt => opt.Ignore())
           .ForMember(dest => dest.SuppliedTransactions, opt => opt.Ignore());

            // Map ViewModel For Read 
            CreateMap<SystemUsers, SupplierReadViewModel>()
             .ForMember(dest => dest.Transactions, opt => opt.MapFrom(src => src.SuppliedTransactions));

            // Map ViewModel For Edit 
            CreateMap<SystemUsers, SupplierEditViewModel>().ReverseMap();
            CreateMap<SupplierReadViewModel, SupplierEditViewModel>();
        }
    }
}

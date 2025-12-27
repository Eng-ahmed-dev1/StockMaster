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
            CreateMap<SupplierCreateViewModel, Suppliers>()
                .ForMember(dest => dest.Transactions , opt => opt.Ignore());
            // Map ViewModel For Read 
            CreateMap<Suppliers, SupplierReadViewModel>()
                .ForMember(dest => dest.Transactions , p => p.MapFrom(src => src.Transactions));
            // Map ViewModel For Edit 
            CreateMap<Suppliers, SupplierEditViewModel>().ReverseMap();
            CreateMap<SupplierReadViewModel, SupplierEditViewModel>();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using AutoMapper;
using TransactionsTask.Models;
namespace Transaction.BLL
{
    public class ProductProfile : Profile
    {
        public ProductProfile()
        {
            // Map ViewModel For Create
            CreateMap<ProductCreateViewModel, Products>();
            // Map ViewModel For Read 
            CreateMap<Products, ProductReadViewModel>()
                .ForMember(dest => dest.Transactions, p => p.MapFrom(src => src.Transactions));

            // Map ViewModel For Edit 
            CreateMap<ProductEditViewModel, Products>()
                .ForMember(dest => dest.ProductName, opt => opt.Ignore())
                .ForMember(dest => dest.SKU, opt => opt.Ignore())
                .ForMember(dest => dest.Transactions, opt => opt.Ignore());
            CreateMap<Products, ProductEditViewModel>();
            CreateMap<ProductReadViewModel, ProductEditViewModel>();

        }
    }
}
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
            CreateMap<RegisterViewModel, SystemUsers>()
               .ForMember(dest => dest.CreatedTransactions, opt => opt.Ignore())
               .ForMember(dest => dest.SuppliedTransactions, opt => opt.Ignore());


            CreateMap<SystemUsers, UserViewModel>()
                 .ForMember(dest => dest.Transactions, opt => opt.MapFrom(src => src.SuppliedTransactions));

            CreateMap<SystemUsers, EditUser>().ReverseMap();
            CreateMap<UserViewModel, EditUser>();
        }
    }
}

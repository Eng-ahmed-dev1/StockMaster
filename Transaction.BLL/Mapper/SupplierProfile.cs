using AutoMapper;
using Transaction.DAL;
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

            CreateMap<EditUser, SystemUsers>()
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.SupplierName))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.SupplierEmail))
                .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.PhoneNumber))
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.SupplierId))
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow));

            CreateMap<SystemUsers, EditUser>()
                .ForMember(dest => dest.SupplierName, opt => opt.MapFrom(src => src.FullName))
                .ForMember(dest => dest.SupplierEmail, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.PhoneNumber))
                .ForMember(dest => dest.SupplierId, opt => opt.MapFrom(src => src.Id));

            CreateMap<UserViewModel, EditUser>()
                .ForMember(dest => dest.SupplierName, opt => opt.MapFrom(src => src.FullName))
                .ForMember(dest => dest.SupplierEmail, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.SupplierId, opt => opt.MapFrom(src => src.Id));
        }
    }
}
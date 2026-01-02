using AutoMapper;
using TransactionsTask.Models;

namespace Transaction.BLL
{
    public class TransactionProfile : Profile
    {
        public TransactionProfile()
        {
            CreateMap<TransactionCreateViewModel, Transactions>();

        
            CreateMap<Transactions, TransactionReadProSupViewModels>()
                .ForMember(dest => dest.ProductName,
                    opt => opt.MapFrom(src => src.Product != null ? src.Product.ProductName : "N/A"))
                .ForMember(dest => dest.SupplierName,
                    opt => opt.MapFrom(src => src.Supplier != null ? src.Supplier.UserName : "N/A"));

            CreateMap<Transactions, TransactionEditViewModel>().ReverseMap();
            CreateMap<TransactionReadProSupViewModels, TransactionEditViewModel>();
        }
    }
}
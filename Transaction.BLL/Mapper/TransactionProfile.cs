using AutoMapper;
using TransactionsTask.Models;

namespace Transaction.BLL
{
    public class TransactionProfile : Profile
    {
        public TransactionProfile()
        {
            // Map ViewModel For Create
            CreateMap<TransactionCreateViewModel, Transactions>();

            // Map ViewModel For Read 
            // we make the ForMember because we can't use entity in the viewmodel directly 
            // so we use it to make as reference from the entity and don't use it directly 
            CreateMap<Transactions, TransactionReadProSupViewModels>()
                .ForMember(dest => dest.ProductName,
                    opt => opt.MapFrom(src => src.Product != null ? src.Product.ProductName : "N/A"))
                .ForMember(dest => dest.SupplierName,
                    opt => opt.MapFrom(src => src.Supplier != null ? src.Supplier.UserName : "N/A"));

            // Map ViewModel For Edit 
            CreateMap<Transactions, TransactionEditViewModel>().ReverseMap();
            CreateMap<TransactionReadProSupViewModels, TransactionEditViewModel>();
        }
    }
}
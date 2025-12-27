namespace Transaction.BLL
{
    public class AuthenticationProfile : Profile    
    {
        public AuthenticationProfile()
        {
            CreateMap<RegisterViewModel, SystemUsers>();
            CreateMap<SystemUsers, UserViewModel>();
        }
    }
}

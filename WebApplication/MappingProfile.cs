using AutoMapper;
using Fotron.DAL.Models;
using Fotron.WebApplication.Models.API.v1.ViewModels;

namespace Fotron.WebApplication
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<AddTokenRequestViewModel, AddTokenRequest>();

            CreateMap<TokenStatistics, TokenStatisticsResponseViewModel>();

            CreateMap<Token, TokenBaseInfoResponseViewModel>();

            CreateMap<Token, TokenFullInfoResponseViewModel>();
        }
    }
}

using AutoMapper;
using GoldenCrown.DTOs.Finance;
using GoldenCrown.Application.DTOs.Finance;

namespace GoldenCrown.Api.DTOs.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            
           CreateMap<TransactionHistoryDto,TransactionHistoryResponse>();
        }
    }
}

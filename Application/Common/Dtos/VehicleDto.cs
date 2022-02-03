using AutoMapper;
using CleanArchitecture.Application.Common.MappingProfile;

namespace CleanArchitecture.Application.Common.Dtos
{
    public class VehicleDto : IMapFrom<VehicleMasterDataDto>
    {
        public string Vin { get; set; }
        public string RyderVehicleNumber { get; set; }
        public string Make { get; set; }
        public string Model { get; set; }
        public string Year { get; set; }
        public string AccountNumber { get; set; }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<VehicleMasterDataDto, VehicleDto>()
                .ForMember(dest => dest.RyderVehicleNumber, src => src.MapFrom(x => x.VehicleNumber))
                .ForMember(dest => dest.Model, src => src.MapFrom(x => x.ModelNumber.ToString()))
                .ForMember(dest => dest.Year, src => src.MapFrom(x => x.Year.ToString()))
                .ForMember(dest => dest.AccountNumber, src => src.MapFrom(x => x.CustomerHqNumber.ToString()))
                ;
        }
    }
}
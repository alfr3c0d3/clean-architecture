using AutoMapper;

namespace CleanArchitecture.Application.Common.MappingProfile
{
    public interface IMapFrom<T>
    {
        void Mapping(Profile profile) => profile.CreateMap(typeof(T), GetType());
    }
}

using System;
using System.Linq;
using System.Reflection;
using AutoMapper;

namespace CleanArchitecture.Application.Common.MappingProfile
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            ApplyMappingsFromAssembly(Assembly.GetExecutingAssembly());

            //Can Use Custom Mappings using Legacy Declarations
            //CreateMap<Activity, ActivityDto>();
            //CreateMap<UserActivity, AttendeeDto>().ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.AppUser.UserName));
        }

        public MappingProfile(Assembly assembly)
        {
            ApplyMappingsFromAssembly(assembly);
        }

        private void ApplyMappingsFromAssembly(Assembly assembly)
        {
            var types = assembly.GetExportedTypes()
                .Where(t => t.GetInterfaces().Any(i =>
                    i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IMapFrom<>)))
                .ToList();

            foreach (var type in types)
            {
                var instance = Activator.CreateInstance(type);

                var methodInfo = type.GetMethod("Mapping")
                                 ?? type.GetInterface("IMapFrom`1")?.GetMethod("Mapping");

                methodInfo?.Invoke(instance, new object[] { this });

            }
        }
    }
}

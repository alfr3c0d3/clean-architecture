using CleanArchitecture.Domain.Entities;

namespace CleanArchitecture.Application.Common.Interfaces
{
    public interface IJwtGenerator
    {
        string CreateToken(AppUser user);
    }
}

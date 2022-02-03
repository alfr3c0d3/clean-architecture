using System.Linq;
using System.Security.Claims;
using CleanArchitecture.Application.Common.Interfaces;
using Microsoft.AspNetCore.Http;

namespace CleanArchitecture.Infrastructure.Services
{
    public class UserAccessor : IUserAccessor
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserAccessor(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public string GetCurrentUsername()
        {
            var userName = _httpContextAccessor.HttpContext.User?.Claims?.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;
            return userName;
        }
    }
}

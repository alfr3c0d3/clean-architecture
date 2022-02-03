using System.Net;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.Application.Common.Exceptions;
using CleanArchitecture.Application.Common.Interfaces;
using CleanArchitecture.Application.User.Dtos;
using CleanArchitecture.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace CleanArchitecture.Application.User
{
    public class CurrentUser
    {
        public class Query : IRequest<UserDto> { }

        public class Handler : IRequestHandler<Query, UserDto>
        {
            private readonly UserManager<AppUser> _userManager;
            private readonly IJwtGenerator _jwtGenerator;
            private readonly IUserAccessor _userAccessor;

            public Handler(UserManager<AppUser> userManager, IJwtGenerator jwtGenerator, IUserAccessor userAccessor)
            {
                _jwtGenerator = jwtGenerator;
                _userAccessor = userAccessor;
                _userManager = userManager;
            }
            public async Task<UserDto> Handle(Query request, CancellationToken cancellationToken)
            {
                var user = await _userManager.FindByNameAsync(_userAccessor.GetCurrentUsername());

                if (user == null)
                    throw new RestException(HttpStatusCode.Unauthorized);

                return new UserDto
                {
                    DisplayName = user.DisplayName,
                    Token = _jwtGenerator.CreateToken(user),
                    UserName = user.UserName
                };
            }
        }
    }
}

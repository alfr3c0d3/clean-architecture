using System.Net;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.Application.Common.Exceptions;
using CleanArchitecture.Application.Common.Interfaces;
using CleanArchitecture.Application.User.Dtos;
using CleanArchitecture.Domain.Entities;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace CleanArchitecture.Application.User
{
    public class Login
    {
        public class Query : IRequest<UserDto>
        {
            public string Email { get; set; }
            public string Password { get; set; }
        }
        public class QueryValidator : AbstractValidator<Query>
        {
            public QueryValidator()
            {
                RuleFor(x => x.Email).NotEmpty();
                RuleFor(x => x.Password).NotEmpty();
            }
        }
        public class Handler : IRequestHandler<Query, UserDto>
        {
            private readonly UserManager<AppUser> _userManager;
            private readonly SignInManager<AppUser> _signInManager;
            private readonly IJwtGenerator _jwtGenerator;

            public Handler(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, IJwtGenerator jwtGenerator)
            {
                _signInManager = signInManager;
                _jwtGenerator = jwtGenerator;
                _userManager = userManager;
            }
            public async Task<UserDto> Handle(Query request, CancellationToken cancellationToken)
            {
                var user = await _userManager.FindByEmailAsync(request.Email);
                if (user == null)
                    throw new RestException(HttpStatusCode.Unauthorized);
                var result = await _signInManager.CheckPasswordSignInAsync(user, request.Password, false);
                if (result.Succeeded)
                {
                    return new UserDto
                    {
                        DisplayName = user.DisplayName,
                        Token = _jwtGenerator.CreateToken(user),
                        UserName = user.UserName
                    };
                }
                throw new RestException(HttpStatusCode.Unauthorized);
            }
        }
    }
}

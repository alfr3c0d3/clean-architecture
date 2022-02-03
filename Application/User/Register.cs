using System;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.Application.Common.Exceptions;
using CleanArchitecture.Application.Common.Interfaces;
using CleanArchitecture.Application.Common.Validators;
using CleanArchitecture.Application.User.Dtos;
using CleanArchitecture.Domain.Entities;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CleanArchitecture.Application.User
{
    public class Register
    {
        public class Command : IRequest<UserDto>
        {
            public string DisplayName { get; set; }
            public string UserName { get; set; }
            public string Email { get; set; }
            public string Password { get; set; }
        }

        public class QueryValidator : AbstractValidator<Command>
        {
            public QueryValidator()
            {
                RuleFor(x => x.DisplayName).NotEmpty();
                RuleFor(x => x.UserName).NotEmpty();
                RuleFor(x => x.Email).NotEmpty().EmailAddress();
                RuleFor(x => x.Password).Password();
            }
        }
        public class Handler : IRequestHandler<Command, UserDto>
        {
            private readonly IApplicationDbContext _context;
            private readonly UserManager<AppUser> _userManager;
            private readonly IJwtGenerator _jwtGenerator;

            public Handler(IApplicationDbContext context, UserManager<AppUser> userManager, IJwtGenerator jwtGenerator)
            {
                _context = context;
                _jwtGenerator = jwtGenerator;
                _userManager = userManager;
            }

            public async Task<UserDto> Handle(Command request, CancellationToken cancellationToken)
            {
                if (await _context.Users.Where(x => x.Email == request.Email).AnyAsync(cancellationToken))
                    throw new RestException(HttpStatusCode.BadRequest, new { Email = "Email already exists" });

                if (await _context.Users.Where(x => x.UserName == request.UserName).AnyAsync(cancellationToken))
                    throw new RestException(HttpStatusCode.BadRequest, new { UserName = "UserName already exists" });

                var user = new AppUser
                {
                    DisplayName = request.DisplayName,
                    UserName = request.UserName,
                    Email = request.Email,
                };

                var result = await _userManager.CreateAsync(user, request.Password);

                if (result.Succeeded)
                {
                    //TODO: Generate Token
                    return new UserDto
                    {
                        DisplayName = user.DisplayName,
                        Token = _jwtGenerator.CreateToken(user),
                        UserName = user.UserName
                    };
                }

                throw new Exception("Problem creating user");
            }
        }
    }
}

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using CleanArchitecture.Application.Activities.Dtos;
using CleanArchitecture.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CleanArchitecture.Application.Activities
{
    public class List
    {
        public class Query : IRequest<ICollection<ActivityDto>>
        {
            public int? Limit { get; set; }
        }

        public class Handler : IRequestHandler<Query, ICollection<ActivityDto>>
        {
            private readonly IApplicationDbContext _context;
            private readonly IMapper _mapper;

            public Handler(IApplicationDbContext context, IMapper mapper)
            {
                _context = context;
                _mapper = mapper;
            }

            public async Task<ICollection<ActivityDto>> Handle(Query request, CancellationToken cancellationToken)
            {
                var queryable = request.Limit > 0 ? _context.Activities.OrderBy(x => x.Date).Take((int)request.Limit) : _context.Activities.OrderBy(x => x.Date);

                var activities = await queryable.ToListAsync(cancellationToken);

                return _mapper.Map<List<ActivityDto>>(activities);
            }
        }
    }
}

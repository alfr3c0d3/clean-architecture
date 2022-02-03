using CleanArchitecture.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Threading;
using System.Threading.Tasks;

namespace CleanArchitecture.Application.Common.Interfaces
{
    public interface IApplicationDbContext
    {
        public DbSet<Activity> Activities { get; set; }
        public DbSet<AppUser> Users { get; set; }

        // Methods 
        public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
        public EntityEntry Remove(object entity);
    }
}
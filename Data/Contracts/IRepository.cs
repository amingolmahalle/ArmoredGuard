using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Entities.BaseEntity;
using Microsoft.EntityFrameworkCore;

namespace Data.Contracts
{
    public interface IRepository<TEntity> where TEntity : class, IEntity
    {
        DbSet<TEntity> Entities { get; }

        IQueryable<TEntity> Table { get; }

        IQueryable<TEntity> TableNoTracking { get; }

        ValueTask<TEntity> GetByIdAsync(CancellationToken cancellationToken, params object[] ids);

        Task UpdateAsync(TEntity entity, CancellationToken cancellationToken, bool saveNow = true);
    }
}
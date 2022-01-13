using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Entities.BaseEntity;
using Microsoft.EntityFrameworkCore;

namespace Data.Contracts;

public interface IRepository<TEntity, in TKey> where TEntity : class, IEntity
{
    DbSet<TEntity> Entities { get; }

    IQueryable<TEntity> Table { get; }

    IQueryable<TEntity> TableNoTracking { get; }

    Task AddAsync(TEntity entity, CancellationToken cancellationToken, bool saveNow = true);

    Task UpdateAsync(TEntity entity, CancellationToken cancellationToken, bool saveNow = true);

    Task DeleteAsync(TEntity entity, CancellationToken cancellationToken, bool saveNow = true);

    Task DeleteRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken, bool saveNow = true);
}
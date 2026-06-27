using Hasin.Model.Entities;
using Hasin.Model.Specifications;
using Hasin.Model.Specifications.Bases;

namespace Hasin.Model.Repositories;

public interface IRepository<TEntity> where TEntity : BaseEntity
{
    Task<TEntity?> GetById(Guid id, CancellationToken cancellationToken = default);
    Task<List<TEntity>> GetAll(CancellationToken cancellationToken = default);
    Task<List<TEntity>> GetList(Specification<TEntity> spec, CancellationToken cancellationToken = default);
    Task Add(TEntity entity, CancellationToken cancellationToken = default);

    public Task Update(TEntity entity, CancellationToken cancellationToken = default);

    public Task Delete(Guid id, CancellationToken cancellationToken = default);
}
using System.Collections.Concurrent;
using Hasin.Model.Entities;
using Hasin.Model.Repositories;
using Hasin.Model.Specifications.Bases;

namespace Hasin.Infrastructure.Repositories;

public class InMemoryRepository<TEntity> : IRepository<TEntity>
    where TEntity : BaseEntity
{
    protected static readonly ConcurrentDictionary<Guid, TEntity> Entities = new();

     

    public Task<TEntity?> GetById(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        Entities.TryGetValue(id, out var entity);
        return Task.FromResult(entity);
    }

    public Task<List<TEntity>> GetAll(
        CancellationToken cancellationToken = default)
    {
        return Task.FromResult(Entities.Values.ToList());
    }

    public Task<List<TEntity>> GetList(
        Specification<TEntity> spec,
        CancellationToken cancellationToken = default)
    {
        var result = Entities.Values
            .AsQueryable()
            .Where(spec.ToExpression())
            .ToList();

        return Task.FromResult(result);
    }

    public Task Add(
        TEntity entity,
        CancellationToken cancellationToken = default)
    {
        if (!Entities.TryAdd(entity.Id, entity))
            throw new InvalidOperationException(
                $"Entity with id '{entity.Id}' already exists.");

        return Task.CompletedTask;
    }

    public Task Update(
        TEntity entity,
        CancellationToken cancellationToken = default)
    {
        Entities[entity.Id] = entity;

        return Task.CompletedTask;
    }

    public Task Delete(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        Entities.Remove(id , out var entity);
        return Task.CompletedTask;
    }
}
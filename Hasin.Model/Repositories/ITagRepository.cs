using Hasin.Model.Entities;

namespace Hasin.Model.Repositories;

public interface ITagRepository : IRepository<Tag>
{
    Task<Tag?> GetByKey(string key, CancellationToken cancellationToken = default);
}
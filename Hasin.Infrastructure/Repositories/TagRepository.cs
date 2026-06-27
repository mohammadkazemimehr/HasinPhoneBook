using Hasin.Model.Entities;
using Hasin.Model.Repositories;

namespace Hasin.Infrastructure.Repositories;

public class TagRepository : InMemoryRepository<Tag>, ITagRepository
{
  
    public async Task<Tag?> GetByKey(string key, CancellationToken cancellationToken = default)
    {
        return await Task.FromResult(Entities.Values.FirstOrDefault(c => c.Key == key));
    }
}
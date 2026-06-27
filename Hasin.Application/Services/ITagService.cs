using Hasin.Application.Models.Tags;
using Hasin.Model.Entities;

namespace Hasin.Application.Services;

public interface ITagService
{
    public Task<Tag> PrepareTag(string key, CancellationToken cancellationToken = default);
}
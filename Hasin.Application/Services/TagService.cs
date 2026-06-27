using Hasin.Application.Models.Tags;
using Hasin.Model.Entities;
using Hasin.Model.Repositories;

namespace Hasin.Application.Services;

public class TagService : ITagService
{
    private readonly ITagRepository _tagRepository;

    public TagService(ITagRepository tagRepository)
    {
        _tagRepository = tagRepository;
    }

    public async Task<Tag> PrepareTag(string key, CancellationToken cancellationToken = default)
    {
        var tag = await _tagRepository.GetByKey(key, cancellationToken);
        if (tag is not null)
            return tag;

        tag = new Tag(key);
        await _tagRepository.Add(tag, cancellationToken);
        return tag;
    }
}
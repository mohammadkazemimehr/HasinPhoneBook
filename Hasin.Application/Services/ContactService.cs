using Hasin.Application.Mappers;
using Hasin.Application.Models.Contacts;
using Hasin.Model.Repositories;
using Hasin.Model.Specifications.Contacts;
using Hasin.Model.ValueObjects;

namespace Hasin.Application.Services;

public sealed class ContactService : IContactService
{
    private readonly IContactRepository _contactContactRepository;
    private readonly ITagService _tagService;

    public ContactService(IContactRepository contactRepository, ITagService tagService)
    {
        _contactContactRepository = contactRepository;
        _tagService = tagService;
    }

    public async Task<Guid> Create(
        CreateContactDto dto,
        CancellationToken cancellationToken = default)
    {
        var tag = await _tagService.PrepareTag(dto.Tag, cancellationToken);
        var contact = dto.ToContact(tag);
        await _contactContactRepository.Add(contact, cancellationToken);
        return contact.Id;
    }

    public async Task Update(
        UpdateContactDto dto,
        CancellationToken cancellationToken = default)
    {
        var contact = await _contactContactRepository.GetById(dto.Id, cancellationToken);

        if (contact is null)
            throw new KeyNotFoundException($"Contact '{dto.Id}' not found.");
        
        var tag = await _tagService.PrepareTag(dto.Tag, cancellationToken);

        contact.Update(dto.FirstName, dto.LastName, dto.PhoneNumbers.Select(x => new PhoneNumber(x)), tag);
        await _contactContactRepository.Update(contact, cancellationToken);
    }

    public async Task Delete(Guid id,
        CancellationToken cancellationToken = default)
    {
        await _contactContactRepository.Delete(id, cancellationToken);
    }

    public async Task<GetContactDto> GetById(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var contact = await _contactContactRepository.GetById(id, cancellationToken);

        if (contact is null)
            throw new KeyNotFoundException($"Contact '{id}' not found.");

        return contact.ToGetContactDto();
    }

    public async Task<List<GetContactDto>> GetByTag(
        string tag,
        CancellationToken cancellationToken = default)
    {
        var contacts = await _contactContactRepository.GetList(
            new ContactsByTagSpecification(tag),
            cancellationToken);

        return contacts.Select(c => c.ToGetContactDto()).ToList();
    }

    public async Task<List<GetContactDto>> GetAll(
        CancellationToken cancellationToken = default)
    {
        var contacts = await _contactContactRepository.GetAll(cancellationToken);
        return contacts.Select(c => c.ToGetContactDto()).ToList();
    }
}
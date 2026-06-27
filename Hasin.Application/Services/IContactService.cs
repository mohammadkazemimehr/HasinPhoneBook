using Hasin.Application.Models.Contacts;

namespace Hasin.Application.Services;

public interface IContactService
{
    Task<Guid> Create(CreateContactDto contactDto, CancellationToken cancellationToken = default);

    Task Update(UpdateContactDto updateContactDto, CancellationToken cancellationToken = default);

    Task Delete(Guid id, CancellationToken cancellationToken = default);

    Task<GetContactDto> GetById(Guid id, CancellationToken cancellationToken = default);

    Task<List<GetContactDto>> GetByTag(string tag, CancellationToken cancellationToken = default);

    Task<List<GetContactDto>> GetAll(CancellationToken cancellationToken = default);
}
using Hasin.Application.Models.Contacts;
using Hasin.Model.Entities;
using Hasin.Model.ValueObjects;

namespace Hasin.Application.Mappers;

public static class ContactMapper
{
    public static Contact ToContact(this CreateContactDto dto, Tag tag)
    {
        return new Contact(
            dto.FirstName,
            dto.LastName,
            new PhoneNumber(dto.PhoneNumber),
            tag);
    }

    public static GetContactDto ToGetContactDto(this Contact contact)
    {
        return new GetContactDto(
            contact.Id,
            contact.FirstName,
            contact.LastName,
            contact.PhoneNumbers
                .Select(x => x.Number)
                .ToList(),
            contact.Tag?.Key ?? string.Empty);
    }
}
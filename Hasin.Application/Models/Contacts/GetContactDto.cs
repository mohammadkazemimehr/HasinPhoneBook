namespace Hasin.Application.Models.Contacts;

public sealed record GetContactDto(
    Guid Id,
    string FirstName,
    string LastName,
    List<string> PhoneNumbers,
    string Tag);
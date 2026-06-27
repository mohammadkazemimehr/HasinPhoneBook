namespace Hasin.Application.Models.Contacts;

public sealed record CreateContactDto(
    string FirstName,
    string LastName,
    string PhoneNumber,
    string Tag);
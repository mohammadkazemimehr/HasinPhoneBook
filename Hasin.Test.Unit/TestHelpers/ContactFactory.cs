using Hasin.Model.Entities;
using Hasin.Model.ValueObjects;

namespace Hasin.Test.Unit.TestHelpers;

internal static class ContactFactory
{
    public static PhoneNumber ValidPhoneNumber(string number = "09129433340")
        => new(number);

    public static Contact ValidContact(
        string firstName = "Mohamamd",
        string lastName = "Kazemi",
        string phoneNumber = "09129433340",
        Tag? tag = null)
        => new(firstName, lastName, new PhoneNumber(phoneNumber), tag);
}

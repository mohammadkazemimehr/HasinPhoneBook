using Hasin.Model.Exceptions;
using Hasin.Model.ValueObjects;

namespace Hasin.Model.Entities;

public sealed class Contact : BaseEntity
{
    private readonly List<PhoneNumber> _phoneNumbers = [];

    public string FirstName { get; private set; } = null!;
    public string LastName { get; private set; } = null!;
    public Tag? Tag { get; private set; }

    public IReadOnlyCollection<PhoneNumber> PhoneNumbers => _phoneNumbers.AsReadOnly();

    public Contact(
        string firstName,
        string lastName,
        PhoneNumber phoneNumber,
        Tag? tag)
    {
        SetPersonalInfo(firstName, lastName, tag);
        _phoneNumbers.Add(phoneNumber);
        Validate();
    }

    public void Update(
        string firstName,
        string lastName,
        IEnumerable<PhoneNumber> phoneNumbers,
        Tag? tag)
    {
        ArgumentNullException.ThrowIfNull(phoneNumbers);

        SetPersonalInfo(firstName, lastName, tag);

        _phoneNumbers.Clear();

        foreach (var phoneNumber in phoneNumbers.Distinct())
        {
            AddPhoneNumber(phoneNumber);
        }

        Validate();
    }

    public void AddPhoneNumber(PhoneNumber phoneNumber)
    {
        if (_phoneNumbers.Contains(phoneNumber))
            throw new DomainValidationException("Phone number already exists.");

        _phoneNumbers.Add(phoneNumber);
    }

    private void SetPersonalInfo(
        string firstName,
        string lastName,
        Tag? tag)
    {
        FirstName = firstName?.Trim() ?? string.Empty;
        LastName = lastName?.Trim() ?? string.Empty;
        Tag = tag;
    }

    private void Validate()
    {
        if (string.IsNullOrWhiteSpace(FirstName))
            throw new DomainValidationException("First name is required.");

        if (string.IsNullOrWhiteSpace(LastName))
            throw new DomainValidationException("Last name is required.");

        if (_phoneNumbers.Count == 0)
            throw new DomainValidationException("At least one phone number is required.");
    }
}
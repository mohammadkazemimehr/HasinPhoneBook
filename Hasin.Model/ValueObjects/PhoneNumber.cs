using System.Text.RegularExpressions;
using Hasin.Model.Exceptions;

namespace Hasin.Model.ValueObjects;

public sealed class PhoneNumber
{
    public string Number { get; }

    public PhoneNumber(string number)
    {
        Number = Validate(number);
    }

    private static string Validate(string number)
    {
        if (string.IsNullOrWhiteSpace(number))
            throw new DomainValidationException("Phone number is required.");

        number = number.Trim();
        
        var regex = new Regex(@"^09\d{9}$", RegexOptions.Compiled);
        if (!regex.IsMatch(number))
            throw new DomainValidationException("Phone number format is invalid.");

        return number;
    }

    public override int GetHashCode()
        => Number.GetHashCode();
}
using Hasin.Model.Exceptions;
using Hasin.Model.ValueObjects;

namespace Hasin.Test.Unit.Model.ValueObjects;

[TestFixture]
public class PhoneNumberTests
{
    [TestCase("09129433340")]
    [TestCase("09351234567")]
    [TestCase("09901112233")]
    public void Constructor_AcceptsValidIranianMobileNumbers(string number)
    {
        var phoneNumber = new PhoneNumber(number);
        Assert.That(phoneNumber.Number, Is.EqualTo(number));
    }

    [Test]
    public void Constructor_TrimsSurroundingWhitespace()
    {
        var phoneNumber = new PhoneNumber("  09123456789  ");
        Assert.That(phoneNumber.Number, Is.EqualTo("09123456789"));
    }

    [TestCase(null)]
    [TestCase("")]
    [TestCase("   ")]
    public void Constructor_Throws_WhenNumberIsNullOrWhitespace(string? number)
    {
        var ex = Assert.Throws<DomainValidationException>(() => new PhoneNumber(number!));
        Assert.That(ex!.message, Is.EqualTo("Phone number is required."));
    }

    [TestCase("0912345678")]   // too short
    [TestCase("091234567890")] // too long
    [TestCase("08123456789")]  // wrong prefix
    [TestCase("9123456789")]   // missing leading 0
    [TestCase("0912345678a")]  // non-digit
    [TestCase("+989123456789")] // format not supported
    public void Constructor_Throws_WhenFormatIsInvalid(string number)
    {
        var ex = Assert.Throws<DomainValidationException>(() => new PhoneNumber(number));
        Assert.That(ex!.message, Is.EqualTo("Phone number format is invalid."));
    }

    [Test]
    public void GetHashCode_IsBasedOnNumber()
    {
        var first = new PhoneNumber("09123456789");
        var second = new PhoneNumber("09123456789");

        Assert.That(first.GetHashCode(), Is.EqualTo(second.GetHashCode()));
    }

    [Test]
    public void Equals_IsNotOverridden_SoEqualNumbersFromDifferentInstancesAreNotEqual()
    {
        var first = new PhoneNumber("09129433340");
        var second = new PhoneNumber("09129433340");
        Assert.Multiple(() =>
        {
            Assert.That(first.Equals(second), Is.False);
            Assert.That(first == second, Is.False);
        });
    }
}

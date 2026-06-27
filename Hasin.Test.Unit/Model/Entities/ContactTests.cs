using Hasin.Model.Entities;
using Hasin.Model.Exceptions;
using Hasin.Model.ValueObjects;
using Hasin.Test.Unit.TestHelpers;

namespace Hasin.Test.Unit.Model.Entities;

[TestFixture]
public class ContactTests
{
    [Test]
    public void Constructor_SetsTrimmedNamesAndInitialPhoneNumber()
    {
        var tag = new Tag("vip");
        var phoneNumber = new PhoneNumber("09123456789");

        var contact = new Contact("  Ali  ", "  Rezaei  ", phoneNumber, tag);
        Assert.Multiple(() =>
        {
            Assert.That(contact.FirstName, Is.EqualTo("Ali"));
            Assert.That(contact.LastName, Is.EqualTo("Rezaei"));
            Assert.That(contact.Tag, Is.SameAs(tag));
            Assert.That(contact.PhoneNumbers, Has.Count.EqualTo(1));
        });
        Assert.That(contact.PhoneNumbers, Has.Member(phoneNumber));
    }

    [Test]
    public void Constructor_AllowsNullTag()
    {
        var contact = ContactFactory.ValidContact(tag: null);

        Assert.That(contact.Tag, Is.Null);
    }

    [TestCase(null)]
    [TestCase("")]
    [TestCase("   ")]
    public void Constructor_Throws_WhenFirstNameIsNullOrWhitespace(string? firstName)
    {
        var ex = Assert.Throws<DomainValidationException>(() =>
            new Contact(firstName!, "Rezaei", ContactFactory.ValidPhoneNumber(), null));

        Assert.That(ex!.message, Is.EqualTo("First name is required."));
    }

    [TestCase(null)]
    [TestCase("")]
    [TestCase("   ")]
    public void Constructor_Throws_WhenLastNameIsNullOrWhitespace(string? lastName)
    {
        var ex = Assert.Throws<DomainValidationException>(() =>
            new Contact("Ali", lastName!, ContactFactory.ValidPhoneNumber(), null));

        Assert.That(ex!.message, Is.EqualTo("Last name is required."));
    }

    [Test]
    public void Update_ReplacesNamesTagAndPhoneNumbers()
    {
        var contact = ContactFactory.ValidContact();
        var newTag = new Tag("family");
        var newNumbers = new[] { new PhoneNumber("09351112233") };

        contact.Update("  Sara  ", "  Ahmadi  ", newNumbers, newTag);
        Assert.Multiple(() =>
        {
            Assert.That(contact.FirstName, Is.EqualTo("Sara"));
            Assert.That(contact.LastName, Is.EqualTo("Ahmadi"));
            Assert.That(contact.Tag, Is.SameAs(newTag));
            Assert.That(contact.PhoneNumbers, Has.Count.EqualTo(1));
        });
        Assert.That(contact.PhoneNumbers, Has.Member(newNumbers[0]));
    }

    [Test]
    public void Update_Throws_WhenPhoneNumbersIsNull()
    {
        var contact = ContactFactory.ValidContact();

        Assert.Throws<ArgumentNullException>(() =>
            contact.Update("Sara", "Ahmadi", null!, null));
    }

    [Test]
    public void Update_Throws_WhenResultingPhoneNumberListIsEmpty()
    {
        var contact = ContactFactory.ValidContact();

        var ex = Assert.Throws<DomainValidationException>(() =>
            contact.Update("Sara", "Ahmadi", Enumerable.Empty<PhoneNumber>(), null));

        Assert.That(ex!.message, Is.EqualTo("At least one phone number is required."));
    }

    [Test]
    public void Update_DoesNotDeduplicate_EqualValuedButDifferentInstancePhoneNumbers()
    {
        // Distinct() relies on default equality, and PhoneNumber does not
        // override Equals, so two different instances with the same number
        // are treated as distinct entries.
        var contact = ContactFactory.ValidContact();
        var numbers = new[] { new PhoneNumber("09123456789"), new PhoneNumber("09123456789") };

        contact.Update("Ali", "Rezaei", numbers, null);

        Assert.That(contact.PhoneNumbers, Has.Count.EqualTo(2));
    }

    [Test]
    public void Update_ThrowsValidation_WhenFirstNameBecomesBlank()
    {
        var contact = ContactFactory.ValidContact();

        Assert.Throws<DomainValidationException>(() =>
            contact.Update("   ", "Ahmadi", [new PhoneNumber("09351112233")], null));
    }

    [Test]
    public void AddPhoneNumber_AppendsNewNumber()
    {
        var contact = ContactFactory.ValidContact(phoneNumber: "09123456789");
        var secondNumber = new PhoneNumber("09351112233");

        contact.AddPhoneNumber(secondNumber);

        Assert.That(contact.PhoneNumbers, Has.Count.EqualTo(2));
        Assert.That(contact.PhoneNumbers, Has.Member(secondNumber));
    }

    [Test]
    public void AddPhoneNumber_Throws_WhenAddingTheExactSameInstanceTwice()
    {
        var phoneNumber = new PhoneNumber("09123456789");
        var contact = new Contact("Ali", "Rezaei", phoneNumber, null);

        var ex = Assert.Throws<DomainValidationException>(() => contact.AddPhoneNumber(phoneNumber));

        Assert.That(ex!.message, Is.EqualTo("Phone number already exists."));
    }

    [Test]
    public void AddPhoneNumber_DoesNotThrow_ForEqualValueDifferentInstance_DueToMissingEqualsOverride()
    {
        var contact = new Contact("Ali", "Rezaei", new PhoneNumber("09123456789"), null);

        Assert.DoesNotThrow(() => contact.AddPhoneNumber(new PhoneNumber("09123456789")));
        Assert.That(contact.PhoneNumbers, Has.Count.EqualTo(2));
    }
}

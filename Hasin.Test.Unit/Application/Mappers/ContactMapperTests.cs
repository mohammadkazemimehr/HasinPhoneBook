using Hasin.Application.Mappers;
using Hasin.Application.Models.Contacts;
using Hasin.Model.Entities;
using Hasin.Test.Unit.TestHelpers;

namespace Hasin.Test.Unit.Application.Mappers;

[TestFixture]
public class ContactMapperTests
{
    [Test]
    public void ToContact_MapsAllFieldsAndAssignsTag()
    {
        var tag = new Tag("vip");
        var dto = new CreateContactDto("Ali", "Rezaei", "09123456789", "vip");

        var contact = dto.ToContact(tag);
        Assert.Multiple(() =>
        {
            Assert.That(contact.FirstName, Is.EqualTo("Ali"));
            Assert.That(contact.LastName, Is.EqualTo("Rezaei"));
            Assert.That(contact.Tag, Is.SameAs(tag));
            Assert.That(contact.PhoneNumbers.Single().Number, Is.EqualTo("09123456789"));
        });
    }

    [Test]
    public void ToGetContactDto_MapsAllFieldsIncludingPhoneNumbersAndTagKey()
    {
        var contact = ContactFactory.ValidContact(
            firstName: "Ali",
            lastName: "Rezaei",
            phoneNumber: "09123456789",
            tag: new Tag("vip"));
        contact.AddPhoneNumber(new Hasin.Model.ValueObjects.PhoneNumber("09351112233"));

        var dto = contact.ToGetContactDto();
        Assert.Multiple(() =>
        {
            Assert.That(dto.Id, Is.EqualTo(contact.Id));
            Assert.That(dto.FirstName, Is.EqualTo("Ali"));
            Assert.That(dto.LastName, Is.EqualTo("Rezaei"));
            Assert.That(dto.PhoneNumbers, Is.EquivalentTo(new[] { "09123456789", "09351112233" }));
            Assert.That(dto.Tag, Is.EqualTo("vip"));
        });
    }

    [Test]
    public void ToGetContactDto_MapsTagToEmptyString_WhenContactHasNoTag()
    {
        var contact = ContactFactory.ValidContact(tag: null);

        var dto = contact.ToGetContactDto();

        Assert.That(dto.Tag, Is.EqualTo(string.Empty));
    }
}

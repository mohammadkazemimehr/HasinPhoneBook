using Hasin.Infrastructure.Repositories;
using Hasin.Model.Entities;
using Hasin.Model.Specifications.Contacts;
using Hasin.Test.Unit.TestHelpers;

namespace Hasin.Test.Unit.Infrastructure;

[TestFixture]
public class ContactRepositoryTests
{
    private ContactRepository _sut = null!;

    [SetUp]
    public void SetUp()
    {
        _sut = new ContactRepository();
    }

    [Test]
    public async Task Add_ThenGetById_ReturnsTheSameContact()
    {
        var contact = ContactFactory.ValidContact();

        await _sut.Add(contact);
        var result = await _sut.GetById(contact.Id);

        Assert.That(result, Is.SameAs(contact));
    }

    [Test]
    public async Task GetAll_ContainsAddedContact()
    {
        var contact = ContactFactory.ValidContact();
        await _sut.Add(contact);

        var all = await _sut.GetAll();

        Assert.That(all, Has.Member(contact));
    }

    [Test]
    public async Task GetList_WithContactsByTagSpecification_ReturnsOnlyMatchingContacts()
    {
        var uniqueTag = $"tag-{Guid.NewGuid()}";
        var matching = ContactFactory.ValidContact(tag: new Tag(uniqueTag));
        var nonMatching = ContactFactory.ValidContact(phoneNumber: "09351112233", tag: new Tag("other"));
        await _sut.Add(matching);
        await _sut.Add(nonMatching);

        var result = await _sut.GetList(new ContactsByTagSpecification(uniqueTag));

        Assert.That(result, Has.Member(matching));
        Assert.That(result, Has.No.Member(nonMatching));
    }

    [Test]
    public async Task Update_PersistsDomainChanges()
    {
        var contact = ContactFactory.ValidContact(firstName: "Before");
        await _sut.Add(contact);

        var existingNumbers = contact.PhoneNumbers.ToList();
        contact.Update("After", contact.LastName, existingNumbers, contact.Tag);
        await _sut.Update(contact);

        var result = await _sut.GetById(contact.Id);
        Assert.That(result!.FirstName, Is.EqualTo("After"));
    }

    [Test]
    public async Task Delete_RemovesContact()
    {
        var contact = ContactFactory.ValidContact();
        await _sut.Add(contact);

        await _sut.Delete(contact.Id);

        Assert.That(await _sut.GetById(contact.Id), Is.Null);
    }
}

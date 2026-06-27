using Hasin.Application.Models.Contacts;
using Hasin.Application.Services;
using Hasin.Model.Entities;
using Hasin.Model.Repositories;
using Hasin.Model.Specifications.Bases;
using Hasin.Model.ValueObjects;
using Hasin.Test.Unit.TestHelpers;

namespace Hasin.Test.Unit.Application.Services;

[TestFixture]
public class ContactServiceTests
{
    private Mock<IContactRepository> _contactRepository = null!;
    private Mock<ITagService> _tagService = null!;
    private ContactService _sut = null!;

    [SetUp]
    public void SetUp()
    {
        _contactRepository = new Mock<IContactRepository>();
        _tagService = new Mock<ITagService>();
        _sut = new ContactService(_contactRepository.Object, _tagService.Object);
    }

    [Test]
    public async Task Create_PreparesTagAndPersistsMappedContact()
    {
        var tag = new Tag("vip");
        _tagService.Setup(x => x.PrepareTag("vip", It.IsAny<CancellationToken>())).ReturnsAsync(tag);

        var dto = new CreateContactDto("Ali", "Rezaei", "09123456789", "vip");

        var id = await _sut.Create(dto);

        _contactRepository.Verify(
            x => x.Add(
                It.Is<Contact>(c =>
                    c.Id == id &&
                    c.FirstName == "Ali" &&
                    c.LastName == "Rezaei" &&
                    c.Tag == tag &&
                    c.PhoneNumbers.Single().Number == "09123456789"),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Test]
    public void Update_Throws_WhenContactDoesNotExist()
    {
        _contactRepository
            .Setup(x => x.GetById(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Contact?)null);

        var dto = new UpdateContactDto(Guid.NewGuid(), "Ali", "Rezaei", ["09123456789"], "vip");

        Assert.ThrowsAsync<KeyNotFoundException>(() => _sut.Update(dto));

        _contactRepository.Verify(
            x => x.Update(It.IsAny<Contact>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Test]
    public async Task Update_AppliesNewValuesAndPersists_WhenContactExists()
    {
        var existing = ContactFactory.ValidContact(firstName: "Old", lastName: "Name");
        var newTag = new Tag("family");

        _contactRepository
            .Setup(x => x.GetById(existing.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existing);
        _tagService
            .Setup(x => x.PrepareTag("family", It.IsAny<CancellationToken>()))
            .ReturnsAsync(newTag);

        var dto = new UpdateContactDto(existing.Id, "New", "Name2", ["09351112233"], "family");

        await _sut.Update(dto);
        Assert.Multiple(() =>
        {
            Assert.That(existing.FirstName, Is.EqualTo("New"));
            Assert.That(existing.LastName, Is.EqualTo("Name2"));
            Assert.That(existing.Tag, Is.SameAs(newTag));
            Assert.That(existing.PhoneNumbers.Single().Number, Is.EqualTo("09351112233"));
        });
        _contactRepository.Verify(
            x => x.Update(existing, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Test]
    public async Task Delete_DelegatesToRepository()
    {
        var id = Guid.NewGuid();

        await _sut.Delete(id);

        _contactRepository.Verify(x => x.Delete(id, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public void GetById_Throws_WhenContactDoesNotExist()
    {
        _contactRepository
            .Setup(x => x.GetById(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Contact?)null);

        Assert.ThrowsAsync<KeyNotFoundException>(() => _sut.GetById(Guid.NewGuid()));
    }

    [Test]
    public async Task GetById_ReturnsMappedDto_WhenContactExists()
    {
        var contact = ContactFactory.ValidContact(tag: new Tag("vip"));

        _contactRepository
            .Setup(x => x.GetById(contact.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(contact);

        var dto = await _sut.GetById(contact.Id);
        Assert.Multiple(() =>
        {
            Assert.That(dto.Id, Is.EqualTo(contact.Id));
            Assert.That(dto.FirstName, Is.EqualTo(contact.FirstName));
            Assert.That(dto.Tag, Is.EqualTo("vip"));
        });
    }

    [Test]
    public async Task GetByTag_ReturnsContactsMatchingSpecification()
    {
        var matching = ContactFactory.ValidContact(firstName: "Ali", tag: new Tag("vip"));

        _contactRepository
            .Setup(x => x.GetList(It.IsAny<Specification<Contact>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Specification<Contact> spec, CancellationToken _) =>
                spec.IsSatisfiedBy(matching) ? [matching] : []);

        var result = await _sut.GetByTag("vip");

        Assert.That(result, Has.Count.EqualTo(1));
        Assert.That(result[0].Id, Is.EqualTo(matching.Id));
    }

    [Test]
    public async Task GetAll_ReturnsAllContactsMapped()
    {
        var first = ContactFactory.ValidContact(firstName: "Ali");
        var second = ContactFactory.ValidContact(firstName: "Sara", phoneNumber: "09351112233");

        _contactRepository
            .Setup(x => x.GetAll(It.IsAny<CancellationToken>()))
            .ReturnsAsync([first, second]);

        var result = await _sut.GetAll();

        Assert.That(result.Select(x => x.Id), Is.EquivalentTo(new[] { first.Id, second.Id }));
    }
}

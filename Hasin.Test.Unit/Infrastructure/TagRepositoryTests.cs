using Hasin.Infrastructure.Repositories;
using Hasin.Model.Entities;

namespace Hasin.Test.Unit.Infrastructure;

[TestFixture]
public class TagRepositoryTests
{
    // TagRepository's backing store is a static dictionary shared by every
    // instance in this test run, so each test uses a Guid-unique key.
    private TagRepository _sut = null!;

    [SetUp]
    public void SetUp()
    {
        _sut = new TagRepository();
    }

    [Test]
    public async Task Add_ThenGetById_ReturnsTheSameTag()
    {
        var tag = new Tag($"key-{Guid.NewGuid()}");

        await _sut.Add(tag);
        var result = await _sut.GetById(tag.Id);

        Assert.That(result, Is.SameAs(tag));
    }

    [Test]
    public async Task GetByKey_ReturnsMatchingTag()
    {
        var key = $"key-{Guid.NewGuid()}";
        var tag = new Tag(key);
        await _sut.Add(tag);

        var result = await _sut.GetByKey(key);

        Assert.That(result, Is.SameAs(tag));
    }

    [Test]
    public async Task GetByKey_ReturnsNull_WhenKeyDoesNotExist()
    {
        var result = await _sut.GetByKey($"missing-{Guid.NewGuid()}");

        Assert.That(result, Is.Null);
    }
}

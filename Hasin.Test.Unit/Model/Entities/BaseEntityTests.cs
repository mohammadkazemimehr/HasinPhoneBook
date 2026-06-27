using Hasin.Model.Entities;

namespace Hasin.Test.Unit.Model.Entities;

[TestFixture]
public class BaseEntityTests
{
    private sealed class SampleEntity : BaseEntity;

    [Test]
    public void Constructor_AssignsNonEmptyUniqueId()
    {
        var first = new SampleEntity();
        var second = new SampleEntity();

        Assert.That(first.Id, Is.Not.EqualTo(Guid.Empty));
        Assert.That(first.Id, Is.Not.EqualTo(second.Id));
    }

    [Test]
    public void Constructor_SetsCreationDateTimeCloseToNow()
    {
        var before = DateTime.Now;
        var entity = new SampleEntity();
        var after = DateTime.Now;

        Assert.That(entity.CreationDateTime, Is.GreaterThanOrEqualTo(before));
        Assert.That(entity.CreationDateTime, Is.LessThanOrEqualTo(after));
    }
}

[TestFixture]
public class TagTests
{
    [Test]
    public void Constructor_SetsKey()
    {
        var tag = new Tag("vip");

        Assert.That(tag.Key, Is.EqualTo("vip"));
    }

    [Test]
    public void Key_IsMutable()
    {
        var tag = new Tag("vip");

        tag.Key = "family";

        Assert.That(tag.Key, Is.EqualTo("family"));
    }
}

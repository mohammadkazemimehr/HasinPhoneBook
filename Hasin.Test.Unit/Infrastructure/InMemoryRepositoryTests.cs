using System.Linq.Expressions;
using Hasin.Infrastructure.Repositories;
using Hasin.Model.Entities;
using Hasin.Model.Specifications.Bases;

namespace Hasin.Test.Unit.Infrastructure;

[TestFixture]
public class InMemoryRepositoryTests
{
    private sealed class TestEntity : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
    }

    private sealed class NameSpecification : Specification<TestEntity>
    {
        private readonly string _name;

        public NameSpecification(string name) => _name = name;

        public override Expression<Func<TestEntity, bool>> ToExpression()
            => entity => entity.Name == _name;
    }

    private InMemoryRepository<TestEntity> _sut = null!;

    [SetUp]
    public void SetUp()
    {
        _sut = new InMemoryRepository<TestEntity>();
    }

    [Test]
    public async Task Add_ThenGetById_ReturnsTheSameEntity()
    {
        var entity = new TestEntity { Name = "first" };

        await _sut.Add(entity);
        var result = await _sut.GetById(entity.Id);

        Assert.That(result, Is.SameAs(entity));
    }

    [Test]
    public async Task Add_Throws_WhenIdAlreadyExists()
    {
        var entity = new TestEntity { Name = "dup" };
        await _sut.Add(entity);

        Assert.ThrowsAsync<InvalidOperationException>(() => _sut.Add(entity));
    }

    [Test]
    public async Task GetById_ReturnsNull_WhenEntityDoesNotExist()
    {
        var result = await _sut.GetById(Guid.NewGuid());

        Assert.That(result, Is.Null);
    }

    [Test]
    public async Task GetAll_ContainsEveryAddedEntity()
    {
        var first = new TestEntity { Name = "a" };
        var second = new TestEntity { Name = "b" };
        await _sut.Add(first);
        await _sut.Add(second);

        var all = await _sut.GetAll();

        Assert.That(all, Has.Member(first));
        Assert.That(all, Has.Member(second));
    }

    [Test]
    public async Task Update_PersistsChanges()
    {
        var entity = new TestEntity { Name = "before" };
        await _sut.Add(entity);

        entity.Name = "after";
        await _sut.Update(entity);

        var result = await _sut.GetById(entity.Id);
        Assert.That(result!.Name, Is.EqualTo("after"));
    }

    [Test]
    public async Task Delete_RemovesEntity()
    {
        var entity = new TestEntity { Name = "to-delete" };
        await _sut.Add(entity);

        await _sut.Delete(entity.Id);

        Assert.That(await _sut.GetById(entity.Id), Is.Null);
    }

    [Test]
    public void Delete_DoesNotThrow_WhenEntityDoesNotExist()
    {
        Assert.DoesNotThrowAsync(() => _sut.Delete(Guid.NewGuid()));
    }

    [Test]
    public async Task GetList_ReturnsOnlyEntitiesMatchingSpecification()
    {
        var uniqueName = $"target-{Guid.NewGuid()}";
        var matching = new TestEntity { Name = uniqueName };
        var nonMatching = new TestEntity { Name = $"other-{Guid.NewGuid()}" };
        await _sut.Add(matching);
        await _sut.Add(nonMatching);

        var result = await _sut.GetList(new NameSpecification(uniqueName));

        Assert.That(result, Has.Member(matching));
        Assert.That(result, Has.No.Member(nonMatching));
    }
}

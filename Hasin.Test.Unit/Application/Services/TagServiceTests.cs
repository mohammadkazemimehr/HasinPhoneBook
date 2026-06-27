using Hasin.Application.Services;
using Hasin.Model.Entities;
using Hasin.Model.Repositories;

namespace Hasin.Test.Unit.Application.Services;

[TestFixture]
public class TagServiceTests
{
    private Mock<ITagRepository> _tagRepository = null!;
    private TagService _sut = null!;

    [SetUp]
    public void SetUp()
    {
        _tagRepository = new Mock<ITagRepository>();
        _sut = new TagService(_tagRepository.Object);
    }

    [Test]
    public async Task PrepareTag_ReturnsExistingTag_WithoutCreatingANewOne()
    {
        var existing = new Tag("vip");
        _tagRepository
            .Setup(x => x.GetByKey("vip", It.IsAny<CancellationToken>()))
            .ReturnsAsync(existing);

        var result = await _sut.PrepareTag("vip");

        Assert.That(result, Is.SameAs(existing));
        _tagRepository.Verify(x => x.Add(It.IsAny<Tag>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Test]
    public async Task PrepareTag_CreatesAndPersistsNewTag_WhenKeyDoesNotExist()
    {
        _tagRepository
            .Setup(x => x.GetByKey("family", It.IsAny<CancellationToken>()))
            .ReturnsAsync((Tag?)null);

        var result = await _sut.PrepareTag("family");

        Assert.That(result.Key, Is.EqualTo("family"));
        _tagRepository.Verify(
            x => x.Add(It.Is<Tag>(t => t.Key == "family"), It.IsAny<CancellationToken>()),
            Times.Once);
    }
}

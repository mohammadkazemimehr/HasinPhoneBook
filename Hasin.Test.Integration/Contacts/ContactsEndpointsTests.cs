using System.Text.Json;
using Hasin.Application.Models.Contacts;

namespace Hasin.Test.Integration.Contacts;

[TestFixture]
public class ContactsEndpointsTests
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    private ApiWebApplicationFactory _factory = null!;
    private HttpClient _client = null!;

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        _factory = new ApiWebApplicationFactory();
        _client = _factory.CreateClient();
    }

    [OneTimeTearDown]
    public void OneTimeTearDown()
    {
        _client.Dispose();
        _factory.Dispose();
    }

    private sealed record ErrorResponse(int StatusCode, string Error);

    private static CreateContactDto NewCreateDto(
        string firstName = "Mohammad",
        string lastName = "Kazemi mehr",
        string phoneNumber = "09129433340",
        string? tag = null)
        => new(firstName, lastName, phoneNumber, tag ?? $"tag-{Guid.NewGuid()}");

    private async Task<Guid> CreateContactAsync(CreateContactDto dto)
    {
        var response = await _client.PostAsJsonAsync("/api/Contacts", dto);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<Guid>();
    }

    [Test]
    public async Task Create_ReturnsOk_WithNewContactId()
    {
        var response = await _client.PostAsJsonAsync("/api/Contacts", NewCreateDto());

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        var id = await response.Content.ReadFromJsonAsync<Guid>();
        Assert.That(id, Is.Not.EqualTo(Guid.Empty));
    }

    [Test]
    public async Task Create_ThenGetById_ReturnsTheCreatedContact()
    {
        var tag = $"tag-{Guid.NewGuid()}";
        var id = await CreateContactAsync(NewCreateDto(firstName: "Sara", lastName: "Ahmadi", tag: tag));

        var response = await _client.GetAsync($"/api/Contacts/{id}");

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        var dto = await response.Content.ReadFromJsonAsync<GetContactDto>(JsonOptions);
        Assert.Multiple(() =>
        {
            Assert.That(dto!.Id, Is.EqualTo(id));
            Assert.That(dto.FirstName, Is.EqualTo("Sara"));
            Assert.That(dto.LastName, Is.EqualTo("Ahmadi"));
            Assert.That(dto.PhoneNumbers, Is.EquivalentTo(new[] { "09129433340" }));
            Assert.That(dto.Tag, Is.EqualTo(tag));
        });
    }

    [Test]
    public async Task GetById_ReturnsBadRequest_WhenContactDoesNotExist()
    {
        var response = await _client.GetAsync($"/api/Contacts/{Guid.NewGuid()}");
        
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
        var error = await response.Content.ReadFromJsonAsync<ErrorResponse>(JsonOptions);
        Assert.That(error!.Error, Does.Contain("not found"));
    }

    [Test]
    public async Task Create_ReturnsInternalServerError_ForInvalidPhoneNumber()
    {
        var response = await _client.PostAsJsonAsync("/api/Contacts", NewCreateDto(phoneNumber: "12345"));

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
    }

    [Test]
    public async Task Create_ReturnsInternalServerError_ForBlankFirstName()
    {
        var response = await _client.PostAsJsonAsync("/api/Contacts", NewCreateDto(firstName: "   "));
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
    }

    [Test]
    public async Task Update_ChangesArePersisted()
    {
        var id = await CreateContactAsync(NewCreateDto(firstName: "Mohammad"));

        var updateDto = new UpdateContactDto(id, "Moha", "Kazemi", ["09129433341"], $"tag-{Guid.NewGuid()}");
        var updateResponse = await _client.PutAsJsonAsync("/api/Contacts", updateDto);
        Assert.That(updateResponse.StatusCode, Is.EqualTo(HttpStatusCode.OK));

        var getResponse = await _client.GetAsync($"/api/Contacts/{id}");
        var dto = await getResponse.Content.ReadFromJsonAsync<GetContactDto>(JsonOptions);
        Assert.That(dto!.FirstName, Is.EqualTo("Moha"));
        Assert.That(dto.PhoneNumbers, Is.EquivalentTo(new[] { "09129433341" }));
    }

    [Test]
    public async Task Update_ReturnsBadRequest_WhenContactDoesNotExist()
    {
        var updateDto = new UpdateContactDto(Guid.NewGuid(), "Mohammad", "Kazemi", ["09129433340"], "vip");

        var response = await _client.PutAsJsonAsync("/api/Contacts", updateDto);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
    }

    [Test]
    public async Task Delete_RemovesContact_SoSubsequentGetReturnsBadRequest()
    {
        var id = await CreateContactAsync(NewCreateDto());

        var deleteResponse = await _client.DeleteAsync($"/api/Contacts/{id}");
        Assert.That(deleteResponse.StatusCode, Is.EqualTo(HttpStatusCode.OK));

        var getResponse = await _client.GetAsync($"/api/Contacts/{id}");
        Assert.That(getResponse.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
    }

    [Test]
    public async Task Delete_ReturnsOk_EvenWhenContactDoesNotExist()
    {
        var response = await _client.DeleteAsync($"/api/Contacts/{Guid.NewGuid()}");
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
    }

    [Test]
    public async Task GetAll_ContainsCreatedContact()
    {
        var id = await CreateContactAsync(NewCreateDto());

        var response = await _client.GetAsync("/api/Contacts");

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        var contacts = await response.Content.ReadFromJsonAsync<List<GetContactDto>>(JsonOptions);
        Assert.That(contacts!.Select(c => c.Id), Has.Member(id));
    }

    [Test]
    public async Task GetByTag_ReturnsOnlyContactsWithThatTag()
    {
        var uniqueTag = $"tag-{Guid.NewGuid()}";
        var id = await CreateContactAsync(NewCreateDto(tag: uniqueTag));

        var response = await _client.GetAsync($"/api/Contacts/by-tag/{uniqueTag}");

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        var contacts = await response.Content.ReadFromJsonAsync<List<GetContactDto>>(JsonOptions);
        Assert.That(contacts, Has.Count.EqualTo(1));
        Assert.That(contacts![0].Id, Is.EqualTo(id));
        Assert.That(contacts[0].Tag, Is.EqualTo(uniqueTag));
    }

    [Test]
    public async Task GetByTag_ReturnsEmptyList_WhenNoContactHasThatTag()
    {
        var response = await _client.GetAsync($"/api/Contacts/by-tag/no-such-tag-{Guid.NewGuid()}");

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        var contacts = await response.Content.ReadFromJsonAsync<List<GetContactDto>>(JsonOptions);
        Assert.That(contacts, Is.Empty);
    }
}

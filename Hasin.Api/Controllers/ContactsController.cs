using Hasin.Application.Models.Contacts;
using Hasin.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Hasin.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class ContactsController : ControllerBase
{
    private readonly IContactService _contactService;

    public ContactsController(IContactService contactService)
    {
        _contactService = contactService;
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateContactDto dto, CancellationToken cancellationToken)
    {
        var id = await _contactService.Create(dto, cancellationToken);

        return Ok(id);
    }

    [HttpPut]
    public async Task<IActionResult> Update([FromBody] UpdateContactDto dto,
        CancellationToken cancellationToken)
    {
        await _contactService.Update(dto, cancellationToken);
        return Ok();
    }
    
    [HttpDelete("{id:Guid}")]
    public async Task<IActionResult> Delete(
        Guid id,
        CancellationToken cancellationToken)
    {
        await _contactService.Delete(id, cancellationToken);
        return Ok();
    }
    
    [HttpGet("{id:Guid}")]
    public async Task<ActionResult<GetContactDto>> GetById(
        Guid id,
        CancellationToken cancellationToken)
    {
        var result = await _contactService.GetById(id, cancellationToken);
        return Ok(result);
    }
    
    [HttpGet]
    public async Task<ActionResult<List<GetContactDto>>> GetAll(
        CancellationToken cancellationToken)
    {
        var result = await _contactService.GetAll(cancellationToken);
        return Ok(result);
    }
    
    [HttpGet("by-tag/{tag}")]
    public async Task<ActionResult<List<GetContactDto>>> GetByTag(string tag, CancellationToken cancellationToken)
    {
        var result = await _contactService.GetByTag(tag, cancellationToken);
        return Ok(result);
    }
}
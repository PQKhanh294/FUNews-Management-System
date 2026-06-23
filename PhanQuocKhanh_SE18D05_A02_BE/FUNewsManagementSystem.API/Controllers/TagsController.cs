using FUNewsManagementSystem.Services.Dtos;
using FUNewsManagementSystem.Services.Services.Interfaces;
using FUNewsManagementSystem.BusinessObjects.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;

namespace FUNewsManagementSystem.API.Controllers;

[Route("odata/Tags")]
public sealed class TagsController : ControllerBase
{
    private readonly ITagService _tagService;

    public TagsController(ITagService tagService)
    {
        _tagService = tagService;
    }

    [EnableQuery]
    [HttpGet]
    public IActionResult Get() => Ok(_tagService.Query());

    [HttpGet("{key}")]
    public async Task<IActionResult> Get(int key)
    {
        var tag = await _tagService.GetByIdAsync(key);
        return tag is null ? NotFound() : Ok(tag);
    }

    [Authorize(Policy = AuthorizationPolicies.AdminOrStaff)]
    [HttpPost]
    public Task<TagDto> Post([FromBody] TagUpsertDto dto) => _tagService.CreateAsync(dto);

    [Authorize(Policy = AuthorizationPolicies.AdminOrStaff)]
    [HttpPut("{key}")]
    public Task<TagDto> Put(int key, [FromBody] TagUpsertDto dto) => _tagService.UpdateAsync(key, dto);

    [Authorize(Policy = AuthorizationPolicies.AdminOrStaff)]
    [HttpDelete("{key}")]
    public async Task<IActionResult> Delete(int key)
    {
        await _tagService.DeleteAsync(key);
        return NoContent();
    }
}


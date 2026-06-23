using FUNewsManagementSystem.Services.Dtos;
using FUNewsManagementSystem.Services.Services.Interfaces;
using FUNewsManagementSystem.BusinessObjects.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;

namespace FUNewsManagementSystem.API.Controllers;

[Route("odata/Categories")]
public sealed class CategoriesController : ControllerBase
{
    private readonly ICategoryService _categoryService;

    public CategoriesController(ICategoryService categoryService)
    {
        _categoryService = categoryService;
    }

    [EnableQuery]
    [HttpGet]
    public IActionResult Get() => Ok(_categoryService.Query());

    [AllowAnonymous]
    [HttpGet("Tree")]
    public Task<List<CategoryTreeDto>> Tree() => _categoryService.GetTreeAsync(User.Identity?.IsAuthenticated != true);

    [AllowAnonymous]
    [HttpGet("Tree/Public")]
    public Task<List<CategoryTreeDto>> PublicTree() => _categoryService.GetTreeAsync(activeOnly: true);

    [HttpGet("{key}")]
    public async Task<IActionResult> Get(short key)
    {
        var category = await _categoryService.GetByIdAsync(key);
        return category is null ? NotFound() : Ok(category);
    }

    [Authorize(Policy = AuthorizationPolicies.StaffOnly)]
    [HttpPost]
    public Task<CategoryDto> Post([FromBody] CategoryUpsertDto dto) => _categoryService.CreateAsync(dto);

    [Authorize(Policy = AuthorizationPolicies.StaffOnly)]
    [HttpPut("{key}")]
    public Task<CategoryDto> Put(short key, [FromBody] CategoryUpsertDto dto) => _categoryService.UpdateAsync(key, dto);

    [Authorize(Policy = AuthorizationPolicies.StaffOnly)]
    [HttpDelete("{key}")]
    public async Task<IActionResult> Delete(short key)
    {
        await _categoryService.DeleteAsync(key);
        return NoContent();
    }
}


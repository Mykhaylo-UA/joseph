using Joseph.WebApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace Joseph.WebApi.Controllers;

public class FilterController: BaseController
{
    readonly IFilterService _filterService;

    public FilterController(IFilterService filterService)
    {
        _filterService = filterService;
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        return Ok(await _filterService.GetFilters());
    }
}
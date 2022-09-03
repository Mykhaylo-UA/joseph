using Joseph.Data.Entities;
using Joseph.WebApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace Joseph.WebApi.Controllers;

public class JobTypeController: BaseController
{
    readonly IJobTypeService _jobTypeService;

    public JobTypeController(IJobTypeService jobTypeService)
    {
        _jobTypeService = jobTypeService;
    }

    [HttpGet]
    [Route("{id}")]
    public async Task<IActionResult> Get(Guid id)
    {
        if (id == Guid.Empty) return BadRequest("Id can't be empty");

        JobType jobType = await _jobTypeService.GetById(id);
        if (jobType is null) return NotFound();

        return Ok(jobType);
    }
    
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        return Ok(await _jobTypeService.GetAll());
    }
    
    [HttpPost]
    public async Task<IActionResult> Post(JobType jobType)
    {
        JobType jobTypeNew = await _jobTypeService.Add(jobType);
        return Ok(jobTypeNew);
    }
}
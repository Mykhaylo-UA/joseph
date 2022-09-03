using System.Web;
using Joseph.Data.Entities;
using Joseph.WebApi.Services;
using Joseph.WebApi.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Joseph.WebApi.Controllers;

public class JobController : BaseController
{
   readonly UserManager<User> _userManager;
   readonly IJobService _jobService;

   public JobController(UserManager<User> userManager, IJobService jobService)
   {
      _userManager = userManager;
      _jobService = jobService;
   }

   [HttpGet]
   [Route("{id}")]
   public async Task<IActionResult> Get(Guid id)
   {
      if (id == Guid.Empty) return BadRequest("Id can't be empty");

      return Ok(await _jobService.GetById(id));
   }

   [Authorize]
   [HttpGet]
   [Route("getMyJobs")]
   public async Task<IActionResult> GetMyAll()
   {
      if (HttpContext.User.Identity is null) return Unauthorized();
      
      User user = await _userManager.FindByEmailAsync(HttpContext.User.Identity.Name);

      List<Job> jobs = await _jobService.GetAll();
      return Ok(jobs.Where(j => j.UserId == user.Id));
   }
   
   [HttpGet]
   public async Task<IActionResult> GetAll([FromQuery]string search = null, [FromQuery]Filter[] filters = null)
   {
      List<Job> jobs = await _jobService.GetAll();
      if (!string.IsNullOrEmpty(search))
      {
         search = HttpUtility.UrlDecode(search);
         jobs = jobs.Where(j => j.Name.Contains(search, StringComparison.InvariantCultureIgnoreCase)).ToList();
      }
      if (filters is not null)
      {
         jobs = FilteringEntities.FilterJobs(jobs, filters);
      }
      return Ok(jobs);
   }
   
   [Authorize]
   [HttpPost]
   public async Task<IActionResult> Post(Job job)
   {
      if (HttpContext.User.Identity is null) return Unauthorized();
      
      User user = await _userManager.FindByEmailAsync(HttpContext.User.Identity.Name);

      job.UserId = user.Id;
      await _jobService.Add(job);
      
      List<Job> jobs = await _jobService.GetAll();
      return Ok(jobs.Where(j => j.UserId == user.Id));
   }
   
   [Authorize]
   [HttpPut]
   public async Task<IActionResult> Put(Job job)
   {
      if (HttpContext.User.Identity is null) return Unauthorized();
      
      User user = await _userManager.FindByEmailAsync(HttpContext.User.Identity.Name);

      await _jobService.Update(job);
      
      List<Job> jobs = await _jobService.GetAll();
      return Ok(jobs.Where(j => j.UserId == user.Id));
   }

   [Authorize]
   [HttpDelete]
   public async Task<IActionResult> Delete(Guid id)
   {
      if (HttpContext.User.Identity is null) return Unauthorized();
      
      User user = await _userManager.FindByEmailAsync(HttpContext.User.Identity.Name);

      Job job = await _jobService.GetById(id);
      if (job is null)
         return NotFound("Job with so Id is not found");
      if (job.UserId != user.Id)
         return BadRequest("You don't have access to this vacancy");

      await _jobService.Remove(id);
      return Ok();
   }
}
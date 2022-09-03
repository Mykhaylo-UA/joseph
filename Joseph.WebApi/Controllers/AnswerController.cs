using Joseph.Data.Entities;
using Joseph.WebApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Joseph.WebApi.Controllers;

[Authorize]
public class AnswerController: BaseController
{
    readonly IAnswerService _answerService;
    readonly UserManager<User> _userManager;

    public AnswerController(IAnswerService answerService, UserManager<User> userManager)
    {
        _answerService = answerService;
        _userManager = userManager;
    }
    
    [HttpGet]
    [Route("getMyAnswers")]
    public async Task<IActionResult> GetMyAnswers()
    {
        if (HttpContext.User.Identity is null) return Unauthorized();
      
        User user = await _userManager.FindByEmailAsync(HttpContext.User.Identity.Name);

        return Ok(await _answerService.GetUserAnswers(user.Id));
    }

    [HttpGet]
    [Route("answersForJob")]
    public async Task<IActionResult> GetAllForJob(Guid jobId)
    {
        if (jobId == Guid.Empty) return BadRequest("Job id can't be empty");

        return Ok(await _answerService.GetAllForJob(jobId));
    }
    
    [HttpGet]
    [Route("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        if (id == Guid.Empty) return BadRequest("Job id can't be jobId");

        return Ok(await _answerService.GetById(id));
    }
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        return Ok(await _answerService.GetAll());
    }

    [HttpPost]
    public async Task<IActionResult> Post(Answer answer)
    {
        if (HttpContext.User.Identity is null) return Unauthorized();
      
        User user = await _userManager.FindByEmailAsync(HttpContext.User.Identity.Name);
        answer.UserId = user.Id;
        
        Answer adAnswer = await _answerService.Add(answer);
        return Ok(adAnswer);
    }

    [HttpDelete]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _answerService.Delete(id);
        return Ok();
    }
}
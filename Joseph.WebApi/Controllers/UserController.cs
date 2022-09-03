using Joseph.Context;
using Joseph.Data.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Joseph.WebApi.Controllers;

[Authorize]
public class UserController: BaseController
{
    readonly UserManager<User> _userManager;
    readonly ApplicationContext _context;

    public UserController(UserManager<User> userManager, ApplicationContext context)
    {
        _userManager = userManager;
        _context = context;
    }
    [HttpGet]
    public async Task<IActionResult> GetMe()
    {
        if (HttpContext.User.Identity is null) return Unauthorized();
        User user = await _userManager.FindByEmailAsync(HttpContext.User.Identity.Name);

        return Ok(user);
    }

    [HttpPost]
    public async Task<IActionResult> Post(User user)
    {
        if (HttpContext.User.Identity is null) return Unauthorized();
        User updateUser = await _userManager.FindByEmailAsync(HttpContext.User.Identity.Name);

        updateUser.Name = user.Name;
        updateUser.Birthday = user.Birthday;
        updateUser.Surname = user.Surname;
        updateUser.Other = user.Other;
        updateUser.PhoneNumber = user.PhoneNumber;
        
        IdentityResult result = await _userManager.UpdateAsync(updateUser);
        if (result.Succeeded)
        {
            return Ok();
        }

        return BadRequest("Error update");
    }

    [Authorize(Roles = "Admin")]
    [HttpGet]
    [Route("getAllUsers")]
    public async Task<IActionResult> GetAllUser()
    {
        return Ok(await _context.Users.ToListAsync());
    }
}
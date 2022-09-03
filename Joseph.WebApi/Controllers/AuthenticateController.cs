using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Joseph.Data.Constants;
using Joseph.Data.Entities;
using Joseph.ViewModel.ResponseModels;
using Joseph.WebApi.RequestModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace Joseph.WebApi.Controllers;

public class AuthenticateController: BaseController
{
    readonly UserManager<User> _userManager;
    readonly RoleManager<IdentityRole> _roleManager;
    readonly IConfiguration _configuration;

    public AuthenticateController(
        UserManager<User> userManager, 
        RoleManager<IdentityRole> roleManager, 
        IConfiguration configuration)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _configuration = configuration;
    }
    
    [HttpPost]
    [Route("login")]
    public async Task<IActionResult> Login([FromBody] LoginModelRequest model)
    {
        User user = await _userManager.FindByEmailAsync(model.Email);
        if (user == null || !await _userManager.CheckPasswordAsync(user, model.Password)) return Unauthorized();
        
        IList<string> userRoles = await _userManager.GetRolesAsync(user);

        List<Claim> authClaims = new()
        {
            new (ClaimTypes.Name, user.Email),
            new (JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        };
            
        authClaims.AddRange(userRoles.Select(userRole => new Claim(ClaimTypes.Role, userRole)));

        JwtSecurityToken token = GetToken(authClaims);

        return Ok(new AuthModelResponse
        {
            Token = new JwtSecurityTokenHandler().WriteToken(token),
            Expires = token.ValidTo
        });
    }
    
    [HttpPost]
    [Route("register")]
    public async Task<IActionResult> Register([FromBody] RegisterModelRequest model)
    {
        User userExists = await _userManager.FindByEmailAsync(model.Email);
        if (userExists != null)
            return BadRequest("User with so email is found.");
        
        User user = new()
        {
            Email = model.Email,
            UserName = model.Email,
            SecurityStamp = Guid.NewGuid().ToString()
        };
        
        
        IdentityResult result = await _userManager.CreateAsync(user, model.Password);
        if (!result.Succeeded)
            return StatusCode(StatusCodes.Status500InternalServerError, 
                "User creation failed! Please check user details and try again.");
        
        if (!await _roleManager.RoleExistsAsync(UserRoles.Admin))
            await _roleManager.CreateAsync(new IdentityRole(UserRoles.Admin));
        if (!await _roleManager.RoleExistsAsync(UserRoles.Employer))
            await _roleManager.CreateAsync(new IdentityRole(UserRoles.Employer));
        if (!await _roleManager.RoleExistsAsync(UserRoles.Candidate))
            await _roleManager.CreateAsync(new IdentityRole(UserRoles.Candidate));
        
        if (model.Role.Contains(UserRoles.Employer, StringComparison.InvariantCultureIgnoreCase))
        {
            await _userManager.AddToRoleAsync(user, UserRoles.Employer);
        }
        if (model.Role.Contains(UserRoles.Candidate, StringComparison.InvariantCultureIgnoreCase))
        {
            await _userManager.AddToRoleAsync(user, UserRoles.Candidate);
        }
        
        user = await _userManager.FindByEmailAsync(model.Email);
        IList<string> userRoles = await _userManager.GetRolesAsync(user);

        List<Claim> authClaims = new()
        {
            new (ClaimTypes.Name, user.Email),
            new (JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        };
            
        authClaims.AddRange(userRoles.Select(userRole => new Claim(ClaimTypes.Role, userRole)));

        JwtSecurityToken token = GetToken(authClaims);

        return Ok(new AuthModelResponse
        {
            Token = new JwtSecurityTokenHandler().WriteToken(token),
            Expires = token.ValidTo
        });
    }

    [Authorize]
    [HttpPost]
    [Route("addNewRole")]
    public async Task<IActionResult> AddNewRole([FromBody] string role, string email)
    {
        User user;
        if (email is not  null)
        {
            user = await _userManager.FindByEmailAsync(email);
        }
        else if(HttpContext.User.Identity is not null)
        {
            user = await _userManager.FindByEmailAsync(HttpContext.User.Identity.Name);
        }
        else
        {
            return Unauthorized();
        }
        if (role.Contains(UserRoles.Employer, StringComparison.InvariantCultureIgnoreCase))
        {
            await _userManager.AddToRoleAsync(user, UserRoles.Employer);
        }
        if (role.Contains(UserRoles.Candidate, StringComparison.InvariantCultureIgnoreCase))
        {
            await _userManager.AddToRoleAsync(user, UserRoles.Candidate);
        }

        return Ok();
    }

    JwtSecurityToken GetToken(List<Claim> authClaims)
    {
        SymmetricSecurityKey authSigningKey = new (Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));

        JwtSecurityToken token = new (
            issuer: _configuration["JWT:ValidIssuer"],
            audience: _configuration["JWT:ValidAudience"],
            expires: DateTime.Now.AddHours(24),
            claims: authClaims,
            signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
        );

        return token;
    }
}
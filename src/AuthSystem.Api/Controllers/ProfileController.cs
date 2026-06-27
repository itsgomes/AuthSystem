using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using AuthSystem.Application.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuthSystem.Api.Controllers;

[Authorize(Policy = Policies.ProfileRead)]
[ApiController]
[Route("api/profile")]
public sealed class ProfileController : ControllerBase
{
  [HttpGet("me")]
  public IActionResult Me()
  {
    var sub = User.FindFirstValue(JwtRegisteredClaimNames.Sub)
      ?? User.FindFirstValue(ClaimTypes.NameIdentifier);
    
    var email = User.FindFirstValue(ClaimTypes.Email)
      ?? User.FindFirstValue("email");

    var name = User.FindFirstValue(ClaimTypes.Name)
      ?? User.FindFirstValue("name");

    return Ok(new
    {
        sub,
        email,
        name
    });
  }
}
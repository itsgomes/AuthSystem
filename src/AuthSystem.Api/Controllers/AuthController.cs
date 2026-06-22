using AuthSystem.Api.Common;
using AuthSystem.Application.Common;
using AuthSystem.Application.UseCases.Users.LoginUser;
using AuthSystem.Application.UseCases.Users.RegisterUser;
using Microsoft.AspNetCore.Mvc;

namespace AuthSystem.Api.Controllers;

[ApiController]
[Route("api/auth")]
public sealed class AuthController(
  RegisterUserUseCase registerUserUseCase, 
  LoginUserUseCase loginUserUseCase) : ControllerBase
{
  [HttpPost("register")]
  public async Task<IActionResult> Register(
    RegisterUserRequest request,
    CancellationToken cancellationToken)
  {
    var result = await registerUserUseCase.ExecuteAsync(request, cancellationToken);

    if (result.IsFailure)
    {
      return ApiResults.FromError(result.Error!);
    }

    return Created(string.Empty, result.Value);
  }

  [HttpPost("login")]
  public async Task<IActionResult> Login(
    LoginUserRequest request,
    CancellationToken cancellationToken)
  {
    var result = await loginUserUseCase.ExecuteAsync(request, cancellationToken);

    if (result.IsFailure)
    {
      return ApiResults.FromError(result.Error!);
    }

    return Ok(result.Value);
  }
}
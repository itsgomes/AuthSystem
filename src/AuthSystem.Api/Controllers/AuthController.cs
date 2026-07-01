using AuthSystem.Api.Common;
using AuthSystem.Application.UseCases.Users.LoginUser;
using AuthSystem.Application.UseCases.Users.LogoutUser;
using AuthSystem.Application.UseCases.Users.RefreshToken;
using AuthSystem.Application.UseCases.Users.RegisterUser;
using Microsoft.AspNetCore.Mvc;

namespace AuthSystem.Api.Controllers;

[ApiController]
[Route("api/auth")]
public sealed class AuthController(
  RefreshTokenUseCase refreshTokenUseCase,
  RegisterUserUseCase registerUserUseCase, 
  LoginUserUseCase loginUserUseCase,
  LogoutUserUseCase logoutUserUseCase) : ControllerBase
{
  [HttpPost("refresh")]
  public async Task<IActionResult> RefreshToken(RefreshTokenRequest request, CancellationToken cancellationToken)
  {
    var result = await refreshTokenUseCase.ExecuteAsync(request, cancellationToken);

    if (result.IsFailure)
    {
      return ApiResults.FromError(result.Error!);
    }

    return Ok(result.Value);
  }

  [HttpPost("register")]
  public async Task<IActionResult> Register(RegisterUserRequest request, CancellationToken cancellationToken)
  {
    var result = await registerUserUseCase.ExecuteAsync(request, cancellationToken);

    if (result.IsFailure)
    {
      return ApiResults.FromError(result.Error!);
    }

    return Created(string.Empty, result.Value);
  }

  [HttpPost("login")]
  public async Task<IActionResult> Login(LoginUserRequest request, CancellationToken cancellationToken)
  {
    var result = await loginUserUseCase.ExecuteAsync(request, cancellationToken);

    if (result.IsFailure)
    {
      return ApiResults.FromError(result.Error!);
    }

    return Ok(result.Value);
  }

  [HttpPost("logout")]
  public async Task<IActionResult> Logout(LogoutUserRequest request, CancellationToken cancellationToken)
  {
    var result = await logoutUserUseCase.ExecuteAsync(request, cancellationToken);

    if (result.IsFailure)
    {
      return ApiResults.FromError(result.Error!);
    }

    return NoContent();
  }
}
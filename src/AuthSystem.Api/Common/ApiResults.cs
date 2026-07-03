using AuthSystem.Application.Common;
using AuthSystem.Application.UseCases.Users.LoginUser;
using AuthSystem.Application.UseCases.Users.RegisterUser;
using Microsoft.AspNetCore.Mvc;
using AuthSystem.Application.UseCases.Users.RefreshToken;
using AuthSystem.Application.UseCases.Users.DeactivateUser;

namespace AuthSystem.Api.Common;

public static class ApiResults
{
  public static IActionResult FromError(Error error)
  {
    return error.Code switch
    {
      var code when code == RegisterUserErrors.EmailAlreadyExists.Code => new ConflictObjectResult(error),
      
      var code when code == DeactivateUserErrors.UserNotFound.Code => new NotFoundObjectResult(error),

      var code when 
        code == LoginUserErrors.InvalidCredentials.Code || 
        code == RefreshTokenErrors.InvalidRefreshToken.Code ||
        code == RefreshTokenErrors.RefreshTokenReused.Code
          => new UnauthorizedObjectResult(error),

      _ => new BadRequestObjectResult(error)
    };
  }
}
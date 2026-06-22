using AuthSystem.Application.Common;
using AuthSystem.Application.UseCases.Users.LoginUser;
using AuthSystem.Application.UseCases.Users.RegisterUser;
using Microsoft.AspNetCore.Mvc;

namespace AuthSystem.Api.Common;

public static class ApiResults
{
  public static IActionResult FromError(Error error)
  {
    if (error.Code == RegisterUserErrors.EmailAlreadyExists.Code)
    {
      return new ConflictObjectResult(error);
    }

    if (error.Code == LoginUserErrors.InvalidCredentials.Code)
    {
      return new UnauthorizedObjectResult(error);
    }

    return new BadRequestObjectResult(error);
  }
}
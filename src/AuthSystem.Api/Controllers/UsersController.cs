using AuthSystem.Api.Common;
using AuthSystem.Application.Authorization;
using AuthSystem.Application.UseCases.Users.DeactivateUser;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuthSystem.Api.Controllers;

[ApiController]
[Route("api/users")]
public sealed class UsersController(
  DeactivateUserUseCase deactivateUserUseCase
) : ControllerBase
{
  [Authorize(Policy = Policies.UsersDeactivate)]
  [HttpPatch("{userId:guid}/deactivate")]
  public async Task<IActionResult> Deactivate(Guid userId, CancellationToken cancellationToken)
  {
    var request = new DeactivateUserRequest(userId);

    var result = await deactivateUserUseCase.ExecuteAsync(request, cancellationToken);

    if (result.IsFailure)
    {
      return ApiResults.FromError(result.Error!);
    }

    return NoContent();
  }
}
using AuthSystem.Application.Abstractions.Persistence;
using AuthSystem.Application.Common;

namespace AuthSystem.Application.UseCases.Users.DeactivateUser;

public sealed class DeactivateUserUseCase(
  IUserRepository userRepository,
  IRefreshTokenRepository refreshTokenRepository,
  IUnitOfWork unitOfWork)
{
  public async Task<Result> ExecuteAsync(DeactivateUserRequest request, CancellationToken cancellationToken = default)
  {
    if (request.UserId == Guid.Empty)
    {
      return Result.Failure(DeactivateUserErrors.InvalidUserId);
    }

    var user = await userRepository.GetByIdAsync(request.UserId, cancellationToken);
    if (user is null)
    {
      return Result.Failure(DeactivateUserErrors.UserNotFound);
    }

    var activeRefreshTokens = await refreshTokenRepository.GetActiveByUserIdAsync(user.Id, cancellationToken);

    user.Deactivate();

    foreach (var refreshToken in activeRefreshTokens)
    {
      refreshToken.RevokeDueToUserDeactivation();
    }

    await unitOfWork.SaveChangesAsync(cancellationToken);

    return Result.Success();
  }
}
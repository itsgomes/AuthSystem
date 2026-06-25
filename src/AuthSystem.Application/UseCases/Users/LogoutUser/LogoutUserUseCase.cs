using AuthSystem.Application.Abstractions.Persistence;
using AuthSystem.Application.Common;

namespace AuthSystem.Application.UseCases.Users.LogoutUser;

public sealed class LogoutUserUseCase(
  IRefreshTokenRepository refreshTokenRepository,
  IUnitOfWork unitOfWork)
{
  public async Task<Result> ExecuteAsync(
    LogoutUserRequest request,
    CancellationToken cancellationToken
  )
  {
    if (string.IsNullOrWhiteSpace(request.RefreshToken))
    {
      return Result.Failure(LogoutUserErrors.RefreshTokenRequired);
    }

    var refreshToken = await refreshTokenRepository.GetByTokenAsync(
      request.RefreshToken,
      cancellationToken);

    if (refreshToken is null || !refreshToken.IsActive)
    {
      return Result.Failure(LogoutUserErrors.InvalidRefreshToken);
    }

    refreshToken.RevokeDueToLogout();

    await unitOfWork.SaveChangesAsync(cancellationToken);
    
    return Result.Success();
  }
}
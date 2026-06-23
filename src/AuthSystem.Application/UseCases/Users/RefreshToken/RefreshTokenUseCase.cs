using AuthSystem.Application.Abstractions.Persistence;
using AuthSystem.Application.Abstractions.Security;
using AuthSystem.Application.Common;
using RefreshTokenEntity = AuthSystem.Domain.Entities.RefreshToken;

namespace AuthSystem.Application.UseCases.Users.RefreshToken;

public sealed class RefreshTokenUseCase(
  IRefreshTokenRepository refreshTokenRepository,
  IAccessTokenGenerator accessTokenGenerator,
  IRefreshTokenGenerator refreshTokenGenerator,
  IUnitOfWork unitOfWork)
{
  public async Task<Result<RefreshTokenResponse>> ExecuteAsync(
    RefreshTokenRequest request,
    CancellationToken cancellationToken = default)
  {
    if (string.IsNullOrWhiteSpace(request.RefreshToken))
    {
      return Result<RefreshTokenResponse>.Failure(
        RefreshTokenErrors.RefreshTokenRequired);
    }

    var existingRefreshToken = await refreshTokenRepository.GetByTokenAsync(
      request.RefreshToken,
      cancellationToken);

    if (existingRefreshToken is null || !existingRefreshToken.IsActive)
    {
      return Result<RefreshTokenResponse>.Failure(
        RefreshTokenErrors.InvalidRefreshToken);
    }
    
    var user = existingRefreshToken.User;
    var accessToken = accessTokenGenerator.Generate(user);
    var refreshTokenValue = refreshTokenGenerator.Generate();

    existingRefreshToken.Revoke();
    
    var newRefreshToken = new RefreshTokenEntity(
      refreshTokenValue,
      DateTime.UtcNow.AddDays(7),
      user.Id);

    await refreshTokenRepository.AddAsync(newRefreshToken, cancellationToken);
    await unitOfWork.SaveChangesAsync(cancellationToken);

    return Result<RefreshTokenResponse>.Success(
      new RefreshTokenResponse(
        accessToken,
        refreshTokenValue));
  }
}
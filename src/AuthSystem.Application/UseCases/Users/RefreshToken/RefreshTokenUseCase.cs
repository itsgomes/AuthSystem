using AuthSystem.Application.Abstractions.Persistence;
using AuthSystem.Application.Abstractions.Security;
using AuthSystem.Application.Common;
using RefreshTokenEntity = AuthSystem.Domain.Entities.RefreshToken;
using Microsoft.Extensions.Logging;
using AuthSystem.Domain.Entities;

namespace AuthSystem.Application.UseCases.Users.RefreshToken;

public sealed class RefreshTokenUseCase(
  IRefreshTokenRepository refreshTokenRepository,
  IPermissionRepository permissionRepository,
  IAccessTokenGenerator accessTokenGenerator,
  IRefreshTokenGenerator refreshTokenGenerator,
  IRefreshTokenHasher refreshTokenHasher,
  ILogger<RefreshTokenUseCase> logger,
  IUnitOfWork unitOfWork)
{
  public async Task<Result<RefreshTokenResponse>> ExecuteAsync(RefreshTokenRequest request, CancellationToken cancellationToken = default)
  {
    if (string.IsNullOrWhiteSpace(request.RefreshToken))
    {
      return Result<RefreshTokenResponse>.Failure(RefreshTokenErrors.RefreshTokenRequired);
    }

    var refreshTokenHash = refreshTokenHasher.Hash(request.RefreshToken);

    var existingRefreshToken = await refreshTokenRepository.GetByTokenHashAsync(refreshTokenHash, cancellationToken);
    if (existingRefreshToken is null)
    {
      return Result<RefreshTokenResponse>.Failure(RefreshTokenErrors.InvalidRefreshToken);
    }

    if (existingRefreshToken.WasRevokedByRotation)
    {
      return await HandleRefreshTokenReuseAsync(existingRefreshToken, cancellationToken);
    }

    if (!existingRefreshToken.IsActive)
    {
      return Result<RefreshTokenResponse>.Failure(RefreshTokenErrors.InvalidRefreshToken);
    }

    if (!existingRefreshToken.User.Active)
    {
      return Result<RefreshTokenResponse>.Failure(RefreshTokenErrors.InvalidRefreshToken);
    }
    
    return await RotateRefreshTokenAsync(existingRefreshToken, cancellationToken);
  }

  private async Task<Result<RefreshTokenResponse>> HandleRefreshTokenReuseAsync(RefreshTokenEntity existingRefreshToken, CancellationToken cancellationToken)
  {
    logger.LogWarning(
      "Refresh token reuse detected. UserId: {UserId}, RefreshTokenId: {RefreshTokenId}, ReplacedByTokenId: {ReplacedByTokenId}",
      existingRefreshToken.UserId,
      existingRefreshToken.Id,
      existingRefreshToken.ReplacedByTokenId);

    var activeRefreshTokens = await refreshTokenRepository.GetActiveByUserIdAsync(existingRefreshToken.UserId, cancellationToken);

    foreach (var refreshToken in activeRefreshTokens)
    {
      refreshToken.RevokeDueToReuseDetected();
    }

    await unitOfWork.SaveChangesAsync(cancellationToken);

    return Result<RefreshTokenResponse>.Failure(RefreshTokenErrors.RefreshTokenReused);
  }

  private async Task<Result<RefreshTokenResponse>> RotateRefreshTokenAsync(RefreshTokenEntity existingRefreshToken, CancellationToken cancellationToken)
  {
    var user = existingRefreshToken.User;

    var accessToken = await GenerateAccessTokenAsync(user, cancellationToken);

    var refreshTokenValue = refreshTokenGenerator.Generate();

    var refreshTokenHash = refreshTokenHasher.Hash(refreshTokenValue);
    
    var newRefreshToken = new RefreshTokenEntity(refreshTokenHash, DateTime.UtcNow.AddDays(7), user.Id);

    existingRefreshToken.Revoke(RefreshTokenEntity.RotatedReason, newRefreshToken.Id);

    await refreshTokenRepository.AddAsync(newRefreshToken, cancellationToken);

    try
    {      
      await unitOfWork.SaveChangesAsync(cancellationToken);
    }
    catch (ConcurrencyException)
    {
      return Result<RefreshTokenResponse>.Failure(RefreshTokenErrors.InvalidRefreshToken);
    }
 
    return Result<RefreshTokenResponse>.Success(
      new RefreshTokenResponse(accessToken, refreshTokenValue));
  }

  private async Task<string> GenerateAccessTokenAsync(User user, CancellationToken cancellationToken)
  {
    var permissions = await permissionRepository.GetByUserIdAsync(user.Id, cancellationToken);

    return accessTokenGenerator.Generate(user, permissions);
  }
}
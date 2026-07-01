using AuthSystem.Application.Abstractions.Persistence;
using AuthSystem.Application.Abstractions.Security;
using AuthSystem.Application.Common;
using AuthSystem.Domain.Entities;
using RefreshTokenEntity = AuthSystem.Domain.Entities.RefreshToken;

namespace AuthSystem.Application.UseCases.Users.LoginUser;

public sealed class LoginUserUseCase(
  IUserRepository userRepository,
  IPasswordHasher passwordHasher,
  IAccessTokenGenerator accessTokenGenerator,
  IRefreshTokenGenerator refreshTokenGenerator,
  IRefreshTokenRepository refreshTokenRepository,
  IPermissionRepository permissionRepository,
  IUnitOfWork unitOfWork)
{
  public async Task<Result<LoginUserResponse>> ExecuteAsync(LoginUserRequest request, CancellationToken cancellationToken = default)
  {
    var validationError = Validate(request);
    if (validationError is not null)
    {
      return Result<LoginUserResponse>.Failure(validationError);
    }

    var user = await GetAuthenticatedUserAsync(request, cancellationToken);
    if (user is null)
    {
      return Result<LoginUserResponse>.Failure(LoginUserErrors.InvalidCredentials);
    }

    var accessToken = await GenerateAccessTokenAsync(user, cancellationToken);
    var refreshToken = CreateRefreshToken(user);

    await refreshTokenRepository.AddAsync(refreshToken, cancellationToken);
    await unitOfWork.SaveChangesAsync(cancellationToken);

    return Result<LoginUserResponse>.Success(
      CreateResponse(user, accessToken, refreshToken.Token));
  }

  private async Task<User?> GetAuthenticatedUserAsync(LoginUserRequest request, CancellationToken cancellationToken)
  {
    var email = NormalizeEmail(request.Email);

    var user = await userRepository.GetByEmailAsync(email, cancellationToken);
    if (user is null)
    {
      return null;
    }

    var passwordIsValid = passwordHasher.Verify(request.Password, user.PasswordHash);

    return passwordIsValid && user.Active
      ? user 
      : null;
  }

  private async Task<string> GenerateAccessTokenAsync(User user, CancellationToken cancellationToken)
  {
    var permissions = await permissionRepository.GetByUserIdAsync(user.Id, cancellationToken);

    return accessTokenGenerator.Generate(user, permissions);
  }

  private RefreshTokenEntity CreateRefreshToken(User user)
  {
    var refreshTokenValue = refreshTokenGenerator.Generate();

    return new RefreshTokenEntity(refreshTokenValue, DateTime.UtcNow.AddDays(7), user.Id);
  }

  private static LoginUserResponse CreateResponse(User user, string accessToken, string refreshToken)
  {
    return new LoginUserResponse(
      user.Id,
      user.Name,
      user.Email,
      accessToken,
      refreshToken);
  }

  private static string NormalizeEmail(string email)
  {
    return email.Trim().ToLowerInvariant();
  }

  private static Error? Validate(LoginUserRequest request)
  {
    if (string.IsNullOrWhiteSpace(request.Email))
      return LoginUserErrors.EmailRequired;

    if (string.IsNullOrWhiteSpace(request.Password))
      return LoginUserErrors.PasswordRequired;

    return null;
  }
}

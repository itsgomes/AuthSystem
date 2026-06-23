using AuthSystem.Application.Abstractions.Persistence;
using AuthSystem.Application.Abstractions.Security;
using AuthSystem.Application.Common;
using RefreshTokenEntity = AuthSystem.Domain.Entities.RefreshToken;

namespace AuthSystem.Application.UseCases.Users.LoginUser;

public sealed class LoginUserUseCase(
  IUserRepository userRepository,
  IPasswordHasher passwordHasher,
  IAccessTokenGenerator accessTokenGenerator,
  IRefreshTokenGenerator refreshTokenGenerator,
  IRefreshTokenRepository refreshTokenRepository,
  IUnitOfWork unitOfWork)
{
  public async Task<Result<LoginUserResponse>> ExecuteAsync(
    LoginUserRequest request, 
    CancellationToken cancellationToken = default)
  {
    var validationError = Validate(request);
    if (validationError is not null)
    {
      return Result<LoginUserResponse>.Failure(validationError);
    }

    var email = request.Email.Trim().ToLowerInvariant();

    var user = await userRepository.GetByEmailAsync(email, cancellationToken);
    if (user is null)
    {
      return Result<LoginUserResponse>.Failure(LoginUserErrors.InvalidCredentials);
    }

    var passwordIsValid = passwordHasher.Verify(request.Password, user.PasswordHash);
    if (!passwordIsValid)
    {
      return Result<LoginUserResponse>.Failure(LoginUserErrors.InvalidCredentials);
    }

    var accessToken = accessTokenGenerator.Generate(user);
    var refreshTokenValue = refreshTokenGenerator.Generate();
    var refreshToken = new RefreshTokenEntity(
      refreshTokenValue,
      DateTime.UtcNow.AddDays(7),
      user.Id);

    await refreshTokenRepository.AddAsync(refreshToken, cancellationToken);
    await unitOfWork.SaveChangesAsync(cancellationToken);

    return Result<LoginUserResponse>.Success(
      new LoginUserResponse(
        user.Id,
        user.Name,
        user.Email,
        accessToken,
        refreshTokenValue));
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
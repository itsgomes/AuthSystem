using AuthSystem.Application.Abstractions.Persistence;
using AuthSystem.Application.Abstractions.Security;
using AuthSystem.Application.Common;
using AuthSystem.Domain.Entities;

namespace AuthSystem.Application.UseCases.Users.RegisterUser;

public sealed class RegisterUserUseCase(
  IUserRepository userRepository,
  IPasswordHasher passwordHasher,
  IUnitOfWork unitOfWork)
{
  public async Task<Result<RegisterUserResponse>> ExecuteAsync(
    RegisterUserRequest request,
    CancellationToken cancellationToken = default)
  {
    var validationError = Validate(request);
    if (validationError is not null)
    {
      return Result<RegisterUserResponse>.Failure(validationError);
    }

    var name = request.Name.Trim();
    var email = request.Email.Trim().ToLowerInvariant();

    var emailAlreadyExists = await userRepository.ExistsByEmailAsync(email, cancellationToken);
    if (emailAlreadyExists)
    {
      return Result<RegisterUserResponse>.Failure(RegisterUserErrors.EmailAlreadyExists);
    }

    var passwordHash = passwordHasher.Hash(request.Password);
    var user = new User(name, email, passwordHash);

    await userRepository.AddAsync(user, cancellationToken);
    await unitOfWork.SaveChangesAsync(cancellationToken);

    return Result<RegisterUserResponse>.Success(
      new RegisterUserResponse(user.Id, user.Name, user.Email)); 
  }

  private static Error? Validate(RegisterUserRequest request)
  {
    if (string.IsNullOrWhiteSpace(request.Name))
      return RegisterUserErrors.NameRequired;

    if (string.IsNullOrWhiteSpace(request.Email))
      return RegisterUserErrors.EmailRequired;

    if (string.IsNullOrWhiteSpace(request.Password))
      return RegisterUserErrors.PasswordRequired;

    if (request.Password.Length < 8)
      return RegisterUserErrors.PasswordTooShort;

    return null;
  }
}

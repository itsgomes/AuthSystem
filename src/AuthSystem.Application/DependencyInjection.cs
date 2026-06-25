using AuthSystem.Application.UseCases.Users.LoginUser;
using AuthSystem.Application.UseCases.Users.LogoutUser;
using AuthSystem.Application.UseCases.Users.RefreshToken;
using AuthSystem.Application.UseCases.Users.RegisterUser;
using Microsoft.Extensions.DependencyInjection;

namespace AuthSystem.Application;

public static class DependencyInjection
{
  public static IServiceCollection AddApplication(this IServiceCollection services)
  {
    services.AddScoped<RegisterUserUseCase>();
    services.AddScoped<LoginUserUseCase>();
    services.AddScoped<RefreshTokenUseCase>();
    services.AddScoped<LogoutUserUseCase>();

    return services;
  }
}

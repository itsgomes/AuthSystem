using AuthSystem.Application.UseCases.Users.LoginUser;
using AuthSystem.Application.UseCases.Users.RegisterUser;
using Microsoft.Extensions.DependencyInjection;

namespace AuthSystem.Application;

public static class DependencyInjection
{
  public static IServiceCollection AddApplication(this IServiceCollection services)
  {
    services.AddScoped<RegisterUserUseCase>();
    services.AddScoped<LoginUserUseCase>();

    return services;
  }
}

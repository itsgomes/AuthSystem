using AuthSystem.Application.Common;

namespace AuthSystem.Application.UseCases.Users.DeactivateUser;

public static class DeactivateUserErrors
{
  public static readonly Error InvalidUserId = new(
    "DeactivateUser.InvalidUserId",
    "User id is invalid");
  
  public static readonly Error UserNotFound = new(
    "DeactivateUser.UserNotFound",
    "User was not found.");
  
}
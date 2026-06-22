namespace AuthSystem.Application.Common;

public sealed record Error(
  string Code,
  string Message);
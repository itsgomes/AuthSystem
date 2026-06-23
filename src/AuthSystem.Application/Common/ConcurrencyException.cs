namespace AuthSystem.Application.Common;

public sealed class ConcurrencyException(string message, Exception innerException) : Exception(message, innerException)
{
}
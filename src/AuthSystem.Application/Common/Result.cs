namespace AuthSystem.Application.Common;

public sealed class Result
{
  private Result(Error? error, bool isSuccess)
  {
    Error = error;
    IsSuccess = isSuccess;
  }  

  public Error? Error { get; }
  public bool IsSuccess { get; }
  public bool IsFailure => !IsSuccess;

  public static Result Success()
  {
    return new Result(null, true);
  }

  public static Result Failure(Error error)
  {
    return new Result(error, false);
  }
}

public sealed class Result<T>
{
  private Result(T? value, Error? error, bool isSuccess)
  {
    Value = value;
    Error = error;
    IsSuccess = isSuccess;
  }

  public T? Value { get; }
  public Error? Error { get; }
  public bool IsSuccess { get; }
  public bool IsFailure => !IsSuccess;

  public static Result<T> Success(T value)
  {
    return new Result<T>(value, null, true);
  }

  public static Result<T> Failure(Error error)
  {
    return new Result<T>(default, error, false);
  }
}
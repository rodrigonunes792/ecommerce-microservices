namespace Catalog.API.Common;

/// <summary>
/// Result pattern for better error handling
/// Implements OCP - extensible error handling without exceptions
/// </summary>
public class Result<T>
{
    public bool IsSuccess { get; }
    public T? Value { get; }
    public string? Error { get; }
    public IEnumerable<string> Errors { get; }

    protected Result(bool isSuccess, T? value, string? error, IEnumerable<string>? errors = null)
    {
        IsSuccess = isSuccess;
        Value = value;
        Error = error;
        Errors = errors ?? Array.Empty<string>();
    }

    public static Result<T> Success(T value) => new(true, value, null);
    public static Result<T> Failure(string error) => new(false, default, error);
    public static Result<T> Failure(IEnumerable<string> errors) => new(false, default, null, errors);
}

public class Result
{
    public bool IsSuccess { get; }
    public string? Error { get; }
    public IEnumerable<string> Errors { get; }

    protected Result(bool isSuccess, string? error, IEnumerable<string>? errors = null)
    {
        IsSuccess = isSuccess;
        Error = error;
        Errors = errors ?? Array.Empty<string>();
    }

    public static Result Success() => new(true, null);
    public static Result Failure(string error) => new(false, error);
    public static Result Failure(IEnumerable<string> errors) => new(false, null, errors);
}

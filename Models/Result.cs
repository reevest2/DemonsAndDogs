namespace Models;

/// <summary>
/// Represents the outcome of an operation that can succeed with a value or fail with an error.
/// </summary>
public sealed class Result<T>
{
    public T? Value { get; }
    public ServiceError? Error { get; }
    public bool IsSuccess => Error is null;

    private Result(T value) => Value = value;
    private Result(ServiceError error) => Error = error;

    public static Result<T> Ok(T value) => new(value);
    public static Result<T> Fail(ServiceError error) => new(error);
    public static Result<T> Fail(string code, string message) => new(new ServiceError(code, message));

    public static Result<T> NotFound(string entity, string id) =>
        new(new ServiceError(ErrorCodes.NotFound, $"{entity} '{id}' not found."));

    public static Result<T> InvalidInput(string message) =>
        new(new ServiceError(ErrorCodes.InvalidInput, message));

    public static Result<T> Unsupported(string message) =>
        new(new ServiceError(ErrorCodes.Unsupported, message));
}

/// <summary>
/// Represents the outcome of an operation that can succeed (with no value) or fail with an error.
/// </summary>
public sealed class Result
{
    public ServiceError? Error { get; }
    public bool IsSuccess => Error is null;

    private Result() { }
    private Result(ServiceError error) => Error = error;

    public static Result Ok() => new();
    public static Result Fail(ServiceError error) => new(error);
    public static Result Fail(string code, string message) => new(new ServiceError(code, message));

    public static Result NotFound(string entity, string id) =>
        new(new ServiceError(ErrorCodes.NotFound, $"{entity} '{id}' not found."));
}

public record ServiceError(string Code, string Message);

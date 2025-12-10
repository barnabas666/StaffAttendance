namespace StaffAtt.Web.Models;

/// <summary>
/// Generic result class to encapsulate success or failure of an operation.
/// Provides information about the operation's outcome, including any resulting value or error message.
/// </summary>
/// <typeparam name="T"></typeparam>
public class Result<T>
{
    public bool IsSuccess { get; }
    public T? Value { get; }
    public string? ErrorMessage { get; }

    private Result(bool isSuccess, T? value, string? errorMessage)
    {
        IsSuccess = isSuccess;
        Value = value;
        ErrorMessage = errorMessage;
    }

    // Factory methods for creating success and failure results
    public static Result<T> Success(T value) => new Result<T>(true, value, null);
    public static Result<T> Failure(string errorMessage) => new Result<T>(false, default, errorMessage);
}


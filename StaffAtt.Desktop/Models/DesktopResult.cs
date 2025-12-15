namespace StaffAtt.Desktop.Models;

/// <summary>
/// Generic result class to encapsulate success or failure of an operation.
/// Provides information about the operation's outcome, including any resulting value or error message.
/// </summary>
/// <typeparam name="T"></typeparam>
public class DesktopResult<T>
{
    public bool IsSuccess { get; set; }
    public T? Value { get; set; }
    public string? ErrorMessage { get; set; }

    public static DesktopResult<T> Fail(string msg)
        => new() { IsSuccess = false, ErrorMessage = msg };

    public static DesktopResult<T> Success(T value)
        => new() { IsSuccess = true, Value = value };
}

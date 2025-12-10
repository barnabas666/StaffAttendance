namespace StaffAtt.Desktop.Models;
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

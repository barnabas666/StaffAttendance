namespace StaffAttApi.Controllers.DTOs;

public class ResetPasswordRequest
{
    public string UserId { get; set; } = "";
    public string Code { get; set; } = "";
    public string NewPassword { get; set; } = "";
}

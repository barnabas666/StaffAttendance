namespace StaffAttApi.DTOs;

/// <summary>
/// Represents a request to update a staff member's department and approval status by an admin.
/// </summary>
public class UpdateStaffByAdminRequest
{
    public int Id { get; set; }
    public int DepartmentId { get; set; }
    public bool IsApproved { get; set; }
}

